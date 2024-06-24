using CodeFirst.Models;
using CodeFirst.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeFirst.Tests.Services
{
    [TestClass]
    public class ContractServiceTest
    {
        private DbContextOptions<ApplicationContext> _dbContextOptions;
        private ApplicationContext _context;
        private ContractService _contractService;

        [TestInitialize]
        public void TestInitialize()
        {
            //Make a mock
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationContext(_dbContextOptions);

            //Put some values into the mock
            SeedDatabase();

            //Get the service
            _contractService = new ContractService(_context);
        }

        private void SeedDatabase()
        {
            //Add clients
            _context.Clients.AddRange(new List<Client>
            {
                new Client { IdClient = 1, Address = "123 Main St", Email = "client1@example.com", PhoneNumber = 1234567890, IsDeleted = false },
                new Client { IdClient = 2, Address = "456 Elm St", Email = "client2@example.com", PhoneNumber = 9876543210, IsDeleted = false }
            });

            //Add softwares
            _context.Softwares.AddRange(new List<Software>
            {
                new Software { IdSoftware = 1, Name = "Software1" },
                new Software { IdSoftware = 2, Name = "Software2" }
            });

            //Add contracts
            _context.Contracts.AddRange(new List<Contract>
            {
                new Contract { IdContract = 1, IdClient = 1, IdSoftware = 1, Name = "Contract1", DateFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(-10)), Price = 1000 },
                new Contract { IdContract = 2, IdClient = 1, IdSoftware = 2, Name = "Contract2", DateFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(-5)), Price = 1500 }
            });

            //Add one-time payments
            _context.OneTimePayments.AddRange(new List<OneTimePayment>
            {
                new OneTimePayment { IdContract = 1, DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(10)), Status = "active", UpdatePeriod = 1 }
            });

            //Add subscriptions
            _context.Subscriptions.AddRange(new List<Subscription>
            {
                new Subscription { IdContract = 2, RenevalTimeInMonths = 6 }
            });

            _context.SaveChanges();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task ClientExists_ShouldReturnTrue_WhenClientExists()
        {
            //Act
            var result = await _contractService.ClientExists(1);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ClientExists_ShouldReturnFalse_WhenClientDoesNotExist()
        {
            //Act
            var result = await _contractService.ClientExists(99);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task SoftwareExists_ShouldReturnTrue_WhenSoftwareExists()
        {
            //Act
            var result = await _contractService.SoftwareExists(1);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task SoftwareExists_ShouldReturnFalse_WhenSoftwareDoesNotExist()
        {
            //Act
            var result = await _contractService.SoftwareExists(99);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ClientHasContract_ShouldReturnTrue_WhenClientHasValidContract()
        {
            //Act
            var result = await _contractService.ClientHasContract(1, 1);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ClientHasContract_ShouldReturnFalse_WhenClientDoesNotHaveValidContract()
        {
            //Act
            var result = await _contractService.ClientHasContract(1, 99);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task PeriodCorrect_ShouldReturnTrue_WhenPeriodIsCorrect()
        {
            //Act
            var result = await _contractService.PeriodCorrect(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(10)));

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task PeriodCorrect_ShouldReturnFalse_WhenPeriodIsIncorrect()
        {
            //Act
            var result = await _contractService.PeriodCorrect(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(31)));

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task RenewalPeriodCorrect_ShouldReturnTrue_WhenRenewalPeriodIsCorrect()
        {
            //Act
            var result = await _contractService.RenewalPeriodCorrect(12);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task RenewalPeriodCorrect_ShouldReturnFalse_WhenRenewalPeriodIsIncorrect()
        {
            //Act
            var result = await _contractService.RenewalPeriodCorrect(25);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task CreateContractAsync_ShouldCreateOneTimePaymentContract()
        {
            //Arrange
            var contractDto = new OneTimePaymentDTO
            {
                IdClient = 1,
                IdSoftware = 1,
                Name = "NewContract",
                DateFrom = DateOnly.FromDateTime(DateTime.Now),
                DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(20)),
                Price = 2000,
                Version = "1.0",
                UpdatePeriod = 2
            };

            //Act
            var result = await _contractService.CreateContractAsync(contractDto);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("inactive", result.Status);
            Assert.AreEqual(1, result.oneTimePayment.IdClient);
        }

        [TestMethod]
        public async Task CreateContractAsync_ShouldCreateSubscriptionContract()
        {
            //Arrange
            var subscriptionDto = new SubscriptionDTO
            {
                IdClient = 2,
                IdSoftware = 2,
                Name = "NewSubscription",
                DateFrom = DateOnly.FromDateTime(DateTime.Now),
                Price = 3000,
                RenevalTimeInMonths = 12
            };

            //Act
            var result = await _contractService.CreateContractAsync(subscriptionDto);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Subscription.IdClient);
        }
    }
}
