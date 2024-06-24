using CodeFirst.Models;
using CodeFirst.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CodeFirst.Tests.Services
{
    [TestClass]
    public class DiscountServiceTest
    {
        private DbContextOptions<ApplicationContext> _dbContextOptions;
        private ApplicationContext _context;
        private DiscountService _discountService;

        [TestInitialize]
        public void TestInitialize()
        {
            //Put some values into the mock
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationContext(_dbContextOptions);

            //Make service
            _discountService = new DiscountService(_context);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetAllDiscountsAsync_ShouldReturnAllDiscounts()
        {
            //Arrange
            _context.Discounts.AddRange(new List<Discount>
            {
                new Discount { Name = "Discount1", Offer = "Offer1", Amt = 10, DateFrom = DateOnly.ParseExact("01 01 2024", "dd MM yyyy", null), DateTo = DateOnly.ParseExact("31 12 2024", "dd MM yyyy", null) },
                new Discount { Name = "Discount2", Offer = "Offer2", Amt = 20, DateFrom = DateOnly.ParseExact("01 01 2024", "dd MM yyyy", null), DateTo = DateOnly.ParseExact("31 12 2024", "dd MM yyyy", null) }
            });
            await _context.SaveChangesAsync();

            //Act
            var result = await _discountService.GetAllDiscountsAsync();

            //Assert
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task AddDiscountAsync_ShouldAddDiscount()
        {
            //Arrange
            var discountDTO = new DiscountDTO
            {
                Name = "Discount3",
                Offer = "Offer3",
                Amt = 30,
                DateFrom = "01 01 2024",
                DateTo = "31 12 2024"
            };

            //Act
            var result = await _discountService.AddDiscountAsync(discountDTO);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Discount3", result.Name);
            Assert.AreEqual("Offer3", result.Offer);
            Assert.AreEqual(30, result.Amt);
            Assert.AreEqual(DateOnly.ParseExact("01 01 2024", "dd MM yyyy", null), result.DateFrom);
            Assert.AreEqual(DateOnly.ParseExact("31 12 2024", "dd MM yyyy", null), result.DateTo);
        }

        [TestMethod]
        public async Task AddDiscountAsync_ShouldPersistDiscountInDatabase()
        {
            //Arrange
            var discountDTO = new DiscountDTO
            {
                Name = "Discount4",
                Offer = "Offer4",
                Amt = 40,
                DateFrom = "01 01 2024",
                DateTo = "31 12 2024"
            };

            //Act
            var addedDiscount = await _discountService.AddDiscountAsync(discountDTO);
            var result = await _context.Discounts.FirstOrDefaultAsync(d => d.Name == "Discount4");

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(addedDiscount.IdDiscount, result.IdDiscount);
            Assert.AreEqual("Offer4", result.Offer);
            Assert.AreEqual(40, result.Amt);
            Assert.AreEqual(DateOnly.ParseExact("01 01 2024", "dd MM yyyy", null), result.DateFrom);
            Assert.AreEqual(DateOnly.ParseExact("31 12 2024", "dd MM yyyy", null), result.DateTo);
        }
    }
}
