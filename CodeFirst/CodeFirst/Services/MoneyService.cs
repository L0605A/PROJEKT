using CodeFirst.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace CodeFirst.Services
{
    public interface IMoneyService
    {
        Task<bool> ContractExists(int ID);
        
        Task<bool> IsSubscription(int ID);
        
        Task<bool> IsPriceValid(int ID, decimal amount, bool IsSubscription, int idClient = 0);
        
        Task PayForOneTimeContract(int IdContract, Decimal amount);
        
        Task PayForSubscriptionContract(int IdContract, Decimal amount);
        
        Task<Decimal> GetProfit(string currency = "PLN");
        
        Task<Decimal> GetProfitForSoftware(int IdSoftware ,string currency = "PLN");

        Task<double> GetNBPRate(string currencyCode);
    }

    public class MoneyService : IMoneyService
    {

        private readonly ApplicationContext _context;
        private readonly HttpClient _httpClient;

        public MoneyService(ApplicationContext context)
        {
            _context = context;
        }
        
        public async Task<bool> ContractExists(int ID)
        {
            return await _context.Contracts.AnyAsync(c => c.IdContract == ID);
        }
        
        public async Task<bool> IsSubscription(int ID)
        {
            return await _context.Subscriptions.AnyAsync(c => c.IdContract == ID);
        }
        

        public async Task<bool> IsPriceValid(int ID, decimal amount, bool IsSubscription,int idClient = 0)
        {
            var contract = await _context.Contracts.Where(c => c.IdContract == ID).FirstOrDefaultAsync();

            var price = contract.Price;
            
            if (IsSubscription)
            {
                var client = await _context.Clients.Include(c => c.Contracts).FirstOrDefaultAsync(c => c.IdClient == idClient);
                if (client.Contracts.Any())
                {
                    return ((float) price * 0.95).Equals((float) amount);
                }
                return price == amount;
            }

            var pricePaid = await _context.Ledgers.Where(l => l.IdContract == ID).Select(l => l.AmountPaid).SumAsync();

            return pricePaid + amount <= price;

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
                    AmountPaid = amount
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

        public async Task<decimal> GetProfit(string currency = "PLN")
        {
            var ledgers = await _context.Ledgers.Include(l => l.Contract).ToListAsync();
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
                    var oneTimePayment = await _context.OneTimePayments.FirstOrDefaultAsync(otp => otp.IdContract == ledger.IdContract);

                    if (oneTimePayment != null && oneTimePayment.Status == "active")
                    {
                        profit += ledger.AmountPaid;
                    }
                }
            }
            
            return profit * (decimal)exchangeRate;
        }

        public async Task<decimal> GetProfitForSoftware(int IdSoftware, string currency = "PLN")
        {
            var ledgers = await _context.Ledgers
                .Include(l => l.Contract)
                .Where(l => l.Contract.IdSoftware == IdSoftware)
                .ToListAsync();
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
                    var oneTimePayment = await _context.OneTimePayments.FirstOrDefaultAsync(otp => otp.IdContract == ledger.IdContract);

                    if (oneTimePayment != null && oneTimePayment.Status == "active")
                    {
                        profit += ledger.AmountPaid;
                    }
                }
            }
            
            return profit * (decimal)exchangeRate;
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