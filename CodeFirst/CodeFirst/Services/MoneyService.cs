using CodeFirst.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;

namespace CodeFirst.Services
{
    public interface IMoneyService
    {
        Task<bool> ContractExists(int ID);
        Task<bool> IsPayedOff(int ID);
        
        Task<bool> SoftwareExists(int? ID);
        
        Task<bool> IsSubscription(int ID);

        Task<bool> IsTimeValid(int ID);
        
        Task<bool> IsPriceValid(int ID, decimal amount);
        
        Task PayForOneTimeContract(int IdContract, Decimal amount);
        
        Task PayForSubscriptionContract(int IdContract, Decimal amount);
        
        Task<Double> GetProfit(int? IdSoftware ,string currency);
        
        Task<Double> GetPredictedProfit(int? IdSoftware ,string currency, int periodInMonths);

        Task<double> GetNBPRate(string currencyCode);
    }

    public class MoneyService : IMoneyService
    {

        private readonly ApplicationContext _context;
        public readonly HttpClient _httpClient = new HttpClient();

        public MoneyService(ApplicationContext context)
        {
            _context = context;
        }
        
        public async Task<bool> ContractExists(int ID)
        {
            return await _context.Contracts.AnyAsync(c => c.IdContract == ID);
        }
        
        public async Task<bool> IsPayedOff(int ID)
        {
            var price = await _context.Contracts.Where(c => c.IdContract == ID).Select(c => c.Price).FirstOrDefaultAsync();
            var paid = await _context.Ledgers.Where(l => l.IdContract == ID).SumAsync(l => l.AmountPaid);

            return price == paid;
        }
        
        public async Task<bool> SoftwareExists(int? ID)
        {
            return await _context.Softwares.AnyAsync(c => c.IdSoftware == ID);
        }
        
        public async Task<bool> IsSubscription(int ID)
        {
            return await _context.Subscriptions.AnyAsync(c => c.IdContract == ID);
        }
        

        public async Task<bool> IsPriceValid(int ID, decimal amount)
        {
            if (amount <= 0)
            {
                return false;
            }
            
            var contract = await _context.Contracts.Where(c => c.IdContract == ID).FirstOrDefaultAsync();

            var price = contract.Price;
            
            if (await IsSubscription(ID))
            {
                var client = await _context.Clients.Include(c => c.Contracts).FirstOrDefaultAsync(c => c.IdClient == contract.IdClient);
                if (client.Contracts.Any())
                {
                    return ((float) price * 0.95).Equals((float) amount);
                }
                return price == amount;
            }

            var pricePaid = await _context.Ledgers.Where(l => l.IdContract == ID).Select(l => l.AmountPaid).SumAsync();

            return pricePaid + amount <= price;

        }
        
        public async Task<bool> IsTimeValid(int ID)
        {
            var contract = await _context.Contracts.Where(c => c.IdContract == ID).FirstOrDefaultAsync();
            
            var today = DateOnly.FromDateTime(DateTime.Now);

            var price = contract.Price;
            
            if (await IsSubscription(ID))
            {
                var firstPayedOn = contract.DateFrom;

                var payPeriod = await _context.Subscriptions.Where(s => s.IdContract == ID)
                    .Select(s => s.RenevalTimeInMonths).FirstOrDefaultAsync();

                var subscriptionPayments = await _context.Ledgers.Where(l => l.IdContract == ID).ToListAsync();

                var lastPayment = subscriptionPayments.MinBy(l => l.PaidOn);

                var payedAlready = subscriptionPayments.Count;

                var lastPaymentStart = firstPayedOn.AddMonths(payPeriod * (payedAlready-1));

                var lastPaymentEnd = lastPaymentStart.AddMonths(payPeriod);
                
                var currentPaymentStart = firstPayedOn.AddMonths(payPeriod * payedAlready);

                var currentPaymentEnd = currentPaymentStart.AddMonths(payPeriod);
                
                bool isLastPaymentValid = lastPayment.PaidOn >= lastPaymentStart && lastPayment.PaidOn <= lastPaymentEnd;
                bool isCurrentPaymentValid = today >= currentPaymentStart && today <= currentPaymentEnd;
                
                return isCurrentPaymentValid && isLastPaymentValid;
            }

            var terminationDate = await _context.OneTimePayments.Where(o => o.IdContract == ID).Select(c => c.DateTo).FirstOrDefaultAsync();

            return terminationDate <= today;

        }
        
        public async Task PayForOneTimeContract(int IdContract, decimal amount)
        {
            
                var contract = await _context.Contracts.Where(c => c.IdContract == IdContract).FirstOrDefaultAsync();

                var price = contract.Price;

                var pricePaid = await _context.Ledgers.Where(l => l.IdContract == IdContract).Select(l => l.AmountPaid).SumAsync();

                var payment = new Ledger()
                {
                    IdContract = IdContract,
                    Contract = await _context.Contracts.Where(c => c.IdContract == IdContract).FirstOrDefaultAsync(),
                    AmountPaid = amount,
                    PaidOn = DateOnly.FromDateTime(DateTime.Now)
                };
                
                if (pricePaid + amount == price)
                {
                    var oneTimeContract = await _context.OneTimePayments.Where(c => c.IdContract == IdContract).FirstOrDefaultAsync();
                    oneTimeContract.Status = "active";
                }

                await _context.Ledgers.AddAsync(payment);
                
                await _context.SaveChangesAsync();

        }
        
        public async Task PayForSubscriptionContract(int IdContract, decimal amount)
        {

            var payment = new Ledger()
            {
                IdContract = IdContract,
                Contract = await _context.Contracts.Where(c => c.IdContract == IdContract).FirstOrDefaultAsync(),
                AmountPaid = amount,
                PaidOn = DateOnly.FromDateTime(DateTime.Now)
            };
            

            await _context.Ledgers.AddAsync(payment);
                
            await _context.SaveChangesAsync();

        }

        public async Task<double> GetProfit(int? IdSoftware, string currency)
        {
            var ledgers = await _context.Ledgers.Include(l => l.Contract).ToListAsync();
            
            if (IdSoftware.HasValue)
            {
               ledgers = await _context.Ledgers.Include(l => l.Contract).Where(l => l.Contract.IdSoftware == IdSoftware).ToListAsync();
            }
            
            decimal profit = 0;
            double exchangeRate = await GetNBPRate(currency);
            
            foreach (var ledger in ledgers)
            {
                Console.WriteLine("Checking " + ledger.IdContract);

                if (await IsSubscription(ledger.IdContract))
                {
                    profit += ledger.AmountPaid;
                }
                else
                {
                    var oneTimePayment = await _context.OneTimePayments
                        .FirstOrDefaultAsync(otp => otp.IdContract == ledger.IdContract);

                    if (oneTimePayment != null && oneTimePayment.Status == "active")
                    {
                        profit += ledger.AmountPaid;
                    }

                }
                
            }
        
            return ((double)profit / 100) / exchangeRate;
        }
        
        public async Task<double> GetPredictedProfit(int? IdSoftware, string currency,  int periodInMonths)
        {
            var ledgersQuery = _context.Ledgers
                .Include(l => l.Contract)
                .AsQueryable();

            if (IdSoftware.HasValue)
            {
                ledgersQuery = ledgersQuery.Where(l => l.Contract.IdSoftware == IdSoftware);
            }

            var ledgers = await ledgersQuery.ToListAsync();
            decimal profit = 0;
            double exchangeRate = await GetNBPRate(currency);

            foreach (var ledger in ledgers)
            {
                if (await IsSubscription(ledger.IdContract))
                {
                    profit += ledger.AmountPaid;
                }
                else
                {
                    var oneTimePayment = await _context.OneTimePayments
                        .FirstOrDefaultAsync(otp => otp.IdContract == ledger.IdContract);

                    if (oneTimePayment != null && (oneTimePayment.Status == "active" || oneTimePayment.DateTo <= DateOnly.FromDateTime(DateTime.Today)))
                    {
                        profit += ledger.AmountPaid;
                    }
                }
            }

            var subs = await _context.Subscriptions.ToListAsync();
            
            if (IdSoftware.HasValue)
            {
                var contractsIds = await _context.Contracts
                    .Where(c => c.IdSoftware == IdSoftware)
                    .Select(c => c.IdSoftware)
                    .ToListAsync();

                subs = await _context.Subscriptions
                    .Where(s => contractsIds.Contains(s.IdContract))
                    .ToListAsync();

            }

            foreach (var sub in subs)
            {
                var contract = await _context.Contracts.Where(c => c.IdContract == sub.IdContract).FirstOrDefaultAsync();
                
                var subEnd = DateOnly.FromDateTime(DateTime.Today).AddMonths(periodInMonths);
                
                var subscriptionPayments = await _context.Ledgers.Where(l => l.IdContract == sub.IdContract).ToListAsync();
                
                var payedAlready = subscriptionPayments.Count;
                
                var firstPayedOn = contract.DateFrom;

                var payedTill = firstPayedOn.AddMonths(sub.RenevalTimeInMonths * payedAlready);

                int totalMonths = (subEnd.Year - payedTill.Year) * 12 + subEnd.Month - payedTill.Month;
                
                int periodsBetween = totalMonths % periodInMonths;

                profit += contract.Price * periodsBetween;
            }

            return ((double)profit / 100) / exchangeRate;
        }

        
        
        public async Task<double> GetNBPRate(string currencyCode)
        {
            if (string.IsNullOrEmpty(currencyCode) || currencyCode == "PLN")
            {
                return 1.0;
            }

            string url1 = "http://api.nbp.pl/api/exchangerates/tables/A/?format=json";
            string url2 = "http://api.nbp.pl/api/exchangerates/tables/B/?format=json";

            try
            {
                var json1 = await _httpClient.GetStringAsync(url1);
                var json2 = await _httpClient.GetStringAsync(url2);

                var ratesArray1 = JsonDocument.Parse(json1).RootElement[0].GetProperty("rates").EnumerateArray();
                var ratesArray2 = JsonDocument.Parse(json2).RootElement[0].GetProperty("rates").EnumerateArray();

                var allRates = new List<JsonElement>(ratesArray1);
                allRates.AddRange(ratesArray2);
                
                foreach (var rate in allRates)
                {
                    if (rate.GetProperty("code").GetString() == currencyCode)
                    {
                        return rate.GetProperty("mid").GetDouble();
                    }
                }
            }
            catch (HttpRequestException e)
            {
                throw new Exception("Error fetching exchange rates", e);
            }

            return 1.0;
        }

    }
}