﻿using System.Globalization;
using CodeFirst.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeFirst.Services
{
    public interface IContractService
    {
        Task<bool> ClientExists(int ID);
        
        Task<bool> SoftwareExists(int ID);
        
        Task<bool> DateValid(string date);

        Task<bool> ClientHasContract(int idClient, int idSotware);
        
        Task<bool> PeriodCorrect(DateOnly dateFrom, DateOnly dateTo);
        
        Task<bool> RenewalPeriodCorrect(int renewalPeriod);
        
        Task<bool> UpdatePeriodCorrect(int updatePeriod);
        
        Task<OneTimePaymentResponseDTO> CreateContractAsync(OneTimePaymentDTO contractDto);
        
        Task<SubscriptionResponseDTO> CreateContractAsync(SubscriptionDTO subscriptionDto);
    }

    public class ContractService : IContractService
    {
        private readonly ApplicationContext _context;

        public ContractService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<bool> ClientExists(int ID)
        {
            return await _context.Clients.AnyAsync(c => c.IdClient == ID && c.IsDeleted == false);
        }
        
        public async Task<bool> DateValid(string date)
        {
            DateTime temp;
            return DateTime.TryParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out temp);
        }

        public async Task<bool> SoftwareExists(int ID)
        {
            return await _context.Softwares.AnyAsync(s => s.IdSoftware == ID);
        }
        
        public async Task<bool> UpdatePeriodCorrect(int updatePeriod)
        {
            return updatePeriod >= 1 && updatePeriod <= 4;
        }

        public async Task<bool> ClientHasContract(int idClient, int idSotware)
        {
            var contracts = await _context.Contracts.Where(c => c.IdClient == idClient && c.IdSoftware == idSotware).Select(c => c.IdContract).ToListAsync();

            var validOneTimes = await _context.OneTimePayments
                .Where(o => o.DateTo >= DateOnly.FromDateTime(DateTime.Now) && contracts.Contains(o.IdContract)).ToListAsync();

            var ledgers = await _context.Ledgers.Where(l => contracts.Contains(l.IdContract)).ToListAsync();

            
            var subs = await _context.Subscriptions
                .Where(o => contracts.Contains(o.IdContract))
                .ToListAsync();

            var currentDate = DateOnly.FromDateTime(DateTime.Now);

            var validSubs = subs
                .Where(o => 
                {
                    var ledger = ledgers.OrderBy(l => l.PaidOn).FirstOrDefault(l => l.IdContract == o.IdContract);
                    return ledger != null && ledger.PaidOn.AddMonths(o.RenevalTimeInMonths) <= currentDate;
                })
                .ToList();
            
            return (validOneTimes.Count > 0 || validSubs.Count > 0);
        }

        public async Task<bool> PeriodCorrect(DateOnly dateFrom, DateOnly dateTo)
        {
            var period = dateTo.DayNumber - dateFrom.DayNumber;
            return period >= 3 && period <= 30;
        }

        public async Task<bool> RenewalPeriodCorrect(int renewalPeriod)
        {
            return renewalPeriod >= 1 && renewalPeriod <= 24;
        }
        
        public async Task<OneTimePaymentResponseDTO> CreateContractAsync(OneTimePaymentDTO contractDto)
        {
            decimal finalPrice = contractDto.Price;

            var today = DateOnly.FromDateTime(DateTime.Now);

            var activeDiscounts = await _context.Discounts
                .Where(d => d.DateFrom <= today && d.DateTo >= today)
                .ToListAsync();

            var discountPercentage = activeDiscounts.OrderByDescending(d => d.Amt).Select(d => d.Amt).FirstOrDefault();
            
            var client = await _context.Clients.Include(c => c.Contracts).FirstOrDefaultAsync(c => c.IdClient == contractDto.IdClient);
            if (client.Contracts.Any())
            {
                discountPercentage += 5; 
            }

            var discountAmount = finalPrice * (discountPercentage / 100m);
            finalPrice -= discountAmount;

            finalPrice += (contractDto.UpdatePeriod - 1) * 1000;
            Console.WriteLine(finalPrice);
            
            var contract = new Contract
            {
                IdClient = contractDto.IdClient,
                Client = await _context.Clients.Where(c => c.IdClient == contractDto.IdClient).FirstOrDefaultAsync(),
                IdSoftware = contractDto.IdSoftware,
                Software = await _context.Softwares.Where(s => s.IdSoftware == contractDto.IdSoftware).FirstOrDefaultAsync(),
                Name = contractDto.Name,
                DateFrom = DateOnly.ParseExact(contractDto.DateFrom, "dd-MM-yyyy", null),
                Price = finalPrice,
            };

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            var OneTimePayment = new OneTimePayment()
            {
                IdContract = contract.IdContract,
                Contract = contract,
                Version = contractDto.Version,
                DateTo = DateOnly.ParseExact(contractDto.DateTo, "dd-MM-yyyy", null),
                Status = "inactive",
                UpdatePeriod = contractDto.UpdatePeriod
            };

            contract.OneTimePayment = OneTimePayment;

            _context.OneTimePayments.Add(OneTimePayment);
            await _context.SaveChangesAsync();

            contractDto.Price = finalPrice;

            OneTimePaymentResponseDTO oneTimePaymentDto = new OneTimePaymentResponseDTO()
            {
                IdContract = contract.IdContract,
                oneTimePayment = contractDto,
                Status = OneTimePayment.Status
            };

            return oneTimePaymentDto;
        }
        
        
         public async Task<SubscriptionResponseDTO> CreateContractAsync(SubscriptionDTO subscriptionDto)
        {
            decimal finalPrice = subscriptionDto.Price;

            var today = DateOnly.FromDateTime(DateTime.Now);

            var activeDiscounts = await _context.Discounts
                .Where(d => d.DateFrom <= today && d.DateTo >= today)
                .ToListAsync();

            var discountPercentage = activeDiscounts.OrderByDescending(d => d.Amt).Select(d => d.Amt).FirstOrDefault();
            
            var client = await _context.Clients.Include(c => c.Contracts).FirstOrDefaultAsync(c => c.IdClient == subscriptionDto.IdClient);
            if (client.Contracts.Any())
            {
                discountPercentage += 5;
                subscriptionDto.Price *= 0.95m;
            }

            var discountAmount = finalPrice * (discountPercentage / 100m);
            finalPrice -= discountAmount;
            
            
            var contract = new Contract
            {
                IdClient = subscriptionDto.IdClient,
                Client = await _context.Clients.Where(c => c.IdClient == subscriptionDto.IdClient).FirstOrDefaultAsync(),
                IdSoftware = subscriptionDto.IdSoftware,
                Software = await _context.Softwares.Where(s => s.IdSoftware == subscriptionDto.IdSoftware).FirstOrDefaultAsync(),
                Name = subscriptionDto.Name,
                DateFrom = DateOnly.ParseExact(subscriptionDto.DateFrom, "dd-MM-yyyy", null),
                Price = subscriptionDto.Price
            };

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            var subscription = new Subscription()
            {
                IdContract = contract.IdContract,
                Contract = contract,
                RenevalTimeInMonths = subscriptionDto.RenevalTimeInMonths
            };

            contract.Subscription = subscription;


            var payment = new Ledger()
            {
                IdContract = contract.IdContract,
                Contract = contract,
                AmountPaid = finalPrice,
                PaidOn =  DateOnly.ParseExact(subscriptionDto.DateFrom, "dd-MM-yyyy", null)
            };
            
            _context.Ledgers.Add(payment);
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            SubscriptionResponseDTO subscriptionResponeDto = new SubscriptionResponseDTO()
            {
                IdContract = contract.IdContract,
                Subscription = subscriptionDto
            };

            return subscriptionResponeDto;
        }
    }
}
