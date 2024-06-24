using CodeFirst.Models;
using CodeFirst.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CodeFirst.Tests.Services
{
    [TestClass]
    public class MoneyServiceTest
    {
        private DbContextOptions<ApplicationContext> _dbContextOptions;
        private ApplicationContext _context;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private HttpClient _httpClient;
        private MoneyService _moneyService;

        [TestInitialize]
        public void TestInitialize()
        {
            //Make a mock
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationContext(_dbContextOptions);

            //Fill mock with some values
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            //Make a service
            _moneyService = new MoneyService(_context);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task ContractExists_ShouldReturnTrue_WhenContractExists()
        {
            //Arrange
            var contract = new Contract { IdContract = 1 };
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            //Act
            var result = await _moneyService.ContractExists(1);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ContractExists_ShouldReturnFalse_WhenContractDoesNotExist()
        {
            //Act
            var result = await _moneyService.ContractExists(1);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task SoftwareExists_ShouldReturnTrue_WhenSoftwareExists()
        {
            //Arrange
            var software = new Software { IdSoftware = 1 };
            _context.Softwares.Add(software);
            await _context.SaveChangesAsync();

            //Act
            var result = await _moneyService.SoftwareExists(1);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task SoftwareExists_ShouldReturnFalse_WhenSoftwareDoesNotExist()
        {
            //Act
            var result = await _moneyService.SoftwareExists(1);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsSubscription_ShouldReturnTrue_WhenContractIsSubscription()
        {
            //Arrange
            var subscription = new Subscription { IdContract = 1 };
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            //Act
            var result = await _moneyService.IsSubscription(1);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task IsSubscription_ShouldReturnFalse_WhenContractIsNotSubscription()
        {
            //Act
            var result = await _moneyService.IsSubscription(1);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsPriceValid_ShouldReturnTrue_WhenAmountIsValid()
        {
            //Arrange
            var contract = new Contract { IdContract = 1, Price = 100 };
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            //Act
            var result = await _moneyService.IsPriceValid(1, 100);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task IsPriceValid_ShouldReturnFalse_WhenAmountIsInvalid()
        {
            //Arrange
            var contract = new Contract { IdContract = 1, Price = 100 };
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            //Act
            var result = await _moneyService.IsPriceValid(1, 150);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task PayForOneTimeContract_ShouldAddPayment_WhenCalled()
        {
            //Arrange
            var contract = new Contract { IdContract = 1, Price = 100 };
            var oneTimeContract = new OneTimePayment { IdContract = 1};
            _context.Contracts.Add(contract);
            _context.OneTimePayments.Add(oneTimeContract);
            await _context.SaveChangesAsync();

            //Act
            await _moneyService.PayForOneTimeContract(1, 100);

            //Assert
            var ledger = await _context.Ledgers.FirstOrDefaultAsync(l => l.IdContract == 1);
            Assert.IsNotNull(ledger);
            Assert.AreEqual(100, ledger.AmountPaid);
        }

        [TestMethod]
        public async Task PayForSubscriptionContract_ShouldAddPayment_WhenCalled()
        {
            //Arrange
            var contract = new Contract { IdContract = 1 };
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            //Act
            await _moneyService.PayForSubscriptionContract(1, 100);

            //Assert
            var ledger = await _context.Ledgers.FirstOrDefaultAsync(l => l.IdContract == 1);
            Assert.IsNotNull(ledger);
            Assert.AreEqual(100, ledger.AmountPaid);
        }

        [TestMethod]
        public async Task GetProfit_ShouldReturnCorrectProfit_ForOneTime_PLN()
        {
            //Arrange

            var contract1 = new Contract {IdContract = 1 };
            var contract2 = new Contract {IdContract = 2 };
            var contract3 = new Contract {IdContract = 3 };
            
            var oneTimePayment1 = new OneTimePayment { IdContract = 1, Status = "active"};
            var oneTimePayment2 = new OneTimePayment { IdContract = 2, Status = "active"};
            var oneTimePayment3 = new OneTimePayment { IdContract = 3, Status = "inactive"};
            
            var ledger1 = new Ledger { IdContract = 1, AmountPaid = 100 };
            var ledger2 = new Ledger { IdContract = 2, AmountPaid = 200 };
            var ledger3 = new Ledger { IdContract = 3, AmountPaid = 300 };
            var ledger4 = new Ledger { IdContract = 4, AmountPaid = 300 };
            

            _context.Contracts.AddRange(contract1,contract2,contract3);
            _context.OneTimePayments.AddRange(oneTimePayment1, oneTimePayment2, oneTimePayment3);
            _context.Ledgers.AddRange(ledger1, ledger2, ledger3, ledger4);
            
            await _context.SaveChangesAsync();
            
            //Act
            var result = await _moneyService.GetProfit(null,"PLN");

            //Assert
            Assert.AreEqual(300, result);
        }
        
        [TestMethod]
        public async Task GetProfit_ShouldReturnCorrectProfit_ForOneTime_EUR()
        {
            //Arrange

            var contract1 = new Contract {IdContract = 1 };
            var contract2 = new Contract {IdContract = 2 };
            var contract3 = new Contract {IdContract = 3 };
            var contract4 = new Contract {IdContract = 4 };
            
            var oneTimePayment1 = new OneTimePayment { IdContract = 1, Status = "active"};
            var oneTimePayment2 = new OneTimePayment { IdContract = 2, Status = "active"};
            var oneTimePayment3 = new OneTimePayment { IdContract = 3, Status = "inactive"};
            
            var subscription = new Subscription { IdContract = 4, RenevalTimeInMonths = 6 };
            
            var ledger1 = new Ledger { IdContract = 1, AmountPaid = 100 };
            var ledger2 = new Ledger { IdContract = 2, AmountPaid = 200 };
            var ledger3 = new Ledger { IdContract = 3, AmountPaid = 300 };
            var ledger4 = new Ledger { IdContract = 4, AmountPaid = 300 };
            

            _context.Contracts.AddRange(contract1,contract2,contract3, contract4);
            _context.OneTimePayments.AddRange(oneTimePayment1, oneTimePayment2, oneTimePayment3);
            _context.Subscriptions.AddRange(subscription);
            _context.Ledgers.AddRange(ledger1, ledger2, ledger3, ledger4);
            
            await _context.SaveChangesAsync();
            
            //Act
            var result = await _moneyService.GetProfit(null,"EUR");

            //Assert
            Assert.AreNotEqual(300, result);
        }

        [TestMethod]
        public async Task GetPredictedProfit_ShouldReturnCorrectPredictedProfit_ForPLN()
        {
            //Arrange
            var contract1 = new Contract {IdContract = 1 };
            var contract2 = new Contract {IdContract = 2 };
            var contract3 = new Contract {IdContract = 3 };
            var contract4 = new Contract { IdContract = 4, Price = 300, DateFrom = DateOnly.FromDateTime(DateTime.Now.AddMonths(-6)) };
            
            var oneTimePayment1 = new OneTimePayment { IdContract = 1, Status = "active"};
            var oneTimePayment2 = new OneTimePayment { IdContract = 2, Status = "active"};
            var oneTimePayment3 = new OneTimePayment { IdContract = 3, Status = "inactive"};
            
            var subscription = new Subscription { IdContract = 4, RenevalTimeInMonths = 6 };
            
            var ledger1 = new Ledger { IdContract = 1, AmountPaid = 100 };
            var ledger2 = new Ledger { IdContract = 2, AmountPaid = 200 };
            var ledger3 = new Ledger { IdContract = 3, AmountPaid = 300 };
            var ledger4 = new Ledger { IdContract = 4, AmountPaid = 300, PaidOn = DateOnly.FromDateTime(DateTime.Now.AddMonths(-6))};
            var ledger5 = new Ledger { IdContract = 4, AmountPaid = 300, PaidOn = DateOnly.FromDateTime(DateTime.Now)};
            

            _context.Contracts.AddRange(contract1,contract2,contract3, contract4);
            _context.OneTimePayments.AddRange(oneTimePayment1, oneTimePayment2, oneTimePayment3);
            _context.Subscriptions.AddRange(subscription);
            _context.Ledgers.AddRange(ledger1, ledger2, ledger3, ledger4, ledger5);
            
            await _context.SaveChangesAsync();
            

            //Act
            var result = await _moneyService.GetPredictedProfit(null, "PLN", 8);

            //Assert
            Assert.AreEqual(1800, result);
        }
        
        [TestMethod]
        public async Task GetPredictedProfit_ShouldReturnCorrectPredictedProfit_ForEUR()
        {
            //Arrange
            var contract1 = new Contract {IdContract = 1 };
            var contract2 = new Contract {IdContract = 2 };
            var contract3 = new Contract {IdContract = 3 };
            var contract4 = new Contract { IdContract = 4, Price = 300, DateFrom = DateOnly.FromDateTime(DateTime.Now.AddMonths(-6)) };
            
            var oneTimePayment1 = new OneTimePayment { IdContract = 1, Status = "active"};
            var oneTimePayment2 = new OneTimePayment { IdContract = 2, Status = "active"};
            var oneTimePayment3 = new OneTimePayment { IdContract = 3, Status = "inactive"};
            
            var subscription = new Subscription { IdContract = 4, RenevalTimeInMonths = 5 };
            
            var ledger1 = new Ledger { IdContract = 1, AmountPaid = 100 };
            var ledger2 = new Ledger { IdContract = 2, AmountPaid = 200 };
            var ledger3 = new Ledger { IdContract = 3, AmountPaid = 300 };
            var ledger4 = new Ledger { IdContract = 4, AmountPaid = 300, PaidOn = DateOnly.FromDateTime(DateTime.Now.AddMonths(-6))};
            

            _context.Contracts.AddRange(contract1,contract2,contract3, contract4);
            _context.OneTimePayments.AddRange(oneTimePayment1, oneTimePayment2, oneTimePayment3);
            _context.Subscriptions.AddRange(subscription);
            _context.Ledgers.AddRange(ledger1, ledger2, ledger3, ledger4);
            
            await _context.SaveChangesAsync();
            

            //Act
            var result = await _moneyService.GetPredictedProfit(null, "EUR", 7);

            //Assert
            Assert.AreNotEqual(1800, result);
        }
        
        [TestMethod]
        public async Task GetNBPRate_ShouldReturnOne_ForPLN()
        {
            //Act
            var result = await _moneyService.GetNBPRate("PLN");

            //Assert
            Assert.AreEqual(1.0, result);
        }
    }
}
