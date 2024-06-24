using CodeFirst.Models;
using CodeFirst.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeFirst.Tests.Services
{
    [TestClass]
    public class ClientServiceTest
    {
        private DbContextOptions<ApplicationContext> _dbContextOptions;
        private ApplicationContext _context;
        private ClientService _clientService;

        [TestInitialize]
        public void TestInitialize()
        {
            //Make a mock
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationContext(_dbContextOptions);

            //Put some values into the mock
            _context.Clients.AddRange(new List<Client>
            {
                new Client { IdClient = 1, Address = "123 Main St", Email = "client1@example.com", PhoneNumber = 1234567890, IsDeleted = false },
                new Client { IdClient = 2, Address = "456 Elm St", Email = "client2@example.com", PhoneNumber = 9876543210, IsDeleted = false }
            });
            _context.SaveChanges();

            //Get the service
            _clientService = new ClientService(_context);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetAllClientsAsync_ShouldReturnAllClients()
        {
            //Act
            var clients = await _clientService.GetAllClientsAsync();

            //Assert
            Assert.AreEqual(2, clients.Count());
        }

        [TestMethod]
        public async Task GetClientByIdAsync_ShouldReturnClient_WhenClientExists()
        {
            //Act
            var client = await _clientService.GetClientByIdAsync(1);

            //Assert
            Assert.IsNotNull(client);
            Assert.AreEqual(1, client.IdClient);
        }

        [TestMethod]
        public async Task GetClientByIdAsync_ShouldReturnNull_WhenClientDoesNotExist()
        {
            //Act
            var client = await _clientService.GetClientByIdAsync(99);

            //Assert
            Assert.IsNull(client);
        }

        [TestMethod]
        public async Task AddClientAsync_ShouldAddClient()
        {
            //Arrange
            var newClient = new Client { IdClient = 3, Address = "789 Oak St", Email = "client3@example.com", PhoneNumber = 5555555555 };

            //Act
            await _clientService.AddClientAsync(newClient);
            var client = await _context.Clients.FindAsync(3);

            //Assert
            Assert.IsNotNull(client);
            Assert.AreEqual("client3@example.com", client.Email);
        }

        [TestMethod]
        public async Task UpdateClientAsync_ShouldUpdateClient()
        {
            //Arrange
            var clientToUpdate = await _context.Clients.FindAsync(1);
            clientToUpdate.Address = "Updated Address";

            //Act
            await _clientService.UpdateClientAsync(clientToUpdate);
            var updatedClient = await _context.Clients.FindAsync(1);

            //Assert
            Assert.AreEqual("Updated Address", updatedClient.Address);
        }

        [TestMethod]
        public async Task SoftDeleteClientAsync_ShouldSoftDeleteClient()
        {
            //Act
            await _clientService.SoftDeleteClientAsync(1);
            var client = await _context.Clients.FindAsync(1);

            //Assert
            Assert.IsTrue(client.IsDeleted);
        }

        [TestMethod]
        public async Task HardDeleteClientAsync_ShouldDeleteClient()
        {
            //Act
            await _clientService.HardDeleteClientAsync(1);
            var client = await _context.Clients.FindAsync(1);

            //Assert
            Assert.IsNull(client);
        }
    }
}
