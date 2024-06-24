using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Configuration;
using CodeFirst.Models;
using CodeFirst.Services;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CodeFirst.Tests.Services
{
    [TestClass]
    public class AuthServiceTest
    {
        private Mock<IConfiguration> _configurationMock;
        private DbContextOptions<ApplicationContext> _dbContextOptions;
        private ApplicationContext _context;

        [TestInitialize]
        public void TestInitialize()
        {
            //Make a mock
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationContext(_dbContextOptions);

            //Put some values into the mock
            _context.Users.AddRange(new List<User>
            {
                new User { Id = 1, Username = "admin", PasswordHash = PasswordHasher.HashPassword("admin123"), Role = "Admin" },
                new User { Id = 2, Username = "user", PasswordHash = PasswordHasher.HashPassword("user123"), Role = "User" }
            });
            _context.SaveChanges();

            //Get config
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("very_long_secret_key_here_that_is_at_least_16_characters_long");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task Authenticate_ShouldReturnToken_WhenCredentialsAreValid()
        {
            //Arrange
            var authService = new AuthService(_context, _configurationMock.Object);

            //Act
            var token = await authService.Authenticate("admin", "admin123");

            //Assert
            Assert.IsNotNull(token);
        }

        [TestMethod]
        public async Task Authenticate_ShouldReturnNull_WhenUsernameIsInvalid()
        {
            //Arrange
            var authService = new AuthService(_context, _configurationMock.Object);

            //Act
            var token = await authService.Authenticate("invalidUser", "admin123");

            //Assert
            Assert.IsNull(token);
        }

        [TestMethod]
        public async Task Authenticate_ShouldReturnNull_WhenPasswordIsInvalid()
        {
            //Arrange
            var authService = new AuthService(_context, _configurationMock.Object);

            //Act
            var token = await authService.Authenticate("admin", "invalidPassword");

            //Assert
            Assert.IsNull(token);
        }

        [TestMethod]
        public async Task Authenticate_ShouldReturnNull_WhenPasswordIsNull()
        {
            //Arrange
            var authService = new AuthService(_context, _configurationMock.Object);

            //Act
            var token = await authService.Authenticate("admin", null);

            //Assert
            Assert.IsNull(token);
        }

        [TestMethod]
        public async Task Authenticate_ShouldReturnNull_WhenUsernameIsNull()
        {
            //Arrange
            var authService = new AuthService(_context, _configurationMock.Object);

            //Act
            var token = await authService.Authenticate(null, "admin123");

            //Assert
            Assert.IsNull(token);
        }
    }
}
