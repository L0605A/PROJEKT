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
                new Client { IdClient = 1, Address = "123 Main St", Email = "client1@example.com", PhoneNumber = "1234567890", IsDeleted = false },
                new Client { IdClient = 2, Address = "456 Elm St", Email = "client2@example.com", PhoneNumber = "9876543210", IsDeleted = false }
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
        public async Task AddPersonalClientAsync_ShouldAddPersonalClient()
        {
            //Arrange
            var personalClientDTO = new PersonalClientDTO
            {
                Address = "789 Maple St",
                Email = "personalclient@example.com",
                PhoneNumber = "1112223333",
                Name = "John",
                Surname = "Doe",
                PESEL = 12345678901
            };

            //Act
            await _clientService.AddPersonalClientAsync(personalClientDTO);
            var addedClient = await _context.Clients.FirstOrDefaultAsync(c => c.Email == "personalclient@example.com");

            //Assert
            Assert.IsNotNull(addedClient);
            Assert.AreEqual("John", addedClient.PersonalClient.Name);
            Assert.AreEqual("Doe", addedClient.PersonalClient.Surname);
        }

        [TestMethod]
        public async Task UpdatePersonalClientAsync_ShouldUpdatePersonalClient()
        {
            //Arrange
            
            var personalClientDTO = new PersonalClientDTO
            {
                Address = "789 Maple St",
                Email = "personalclient@example.com",
                PhoneNumber = "1112223333",
                Name = "John",
                Surname = "Doe",
                PESEL = 12345678901
            };
            
            await _clientService.AddPersonalClientAsync(personalClientDTO);
            
            
            var personalEditDTO = new PersonalEditDTO()
            {
                Address = "Updated Address",
                Email = "client1@example.com",
                PhoneNumber = "1234567890",
                Name = "UpdatedName",
                Surname = "UpdatedSurname"
            };

            //Act
            await _clientService.UpdatePersonalClientAsync(3, personalEditDTO);
            var updatedClient = await _context.Clients.FirstOrDefaultAsync(c => c.IdClient == 3);

            //Assert
            Assert.IsNotNull(updatedClient);
            Assert.AreEqual("Updated Address", updatedClient.Address);
            Assert.AreEqual("UpdatedName", updatedClient.PersonalClient.Name);
            Assert.AreEqual("UpdatedSurname", updatedClient.PersonalClient.Surname);
            Assert.AreEqual(12345678901, updatedClient.PersonalClient.PESEL);
        }

        [TestMethod]
        public async Task AddCorporateClientAsync_ShouldAddCorporateClient()
        {
            //Arrange
            var corporateClientDTO = new CorpoClientDTO
            {
                Address = "789 Pine St",
                Email = "corporateclient@example.com",
                PhoneNumber = "4445556666",
                CorpoName = "ABC Corporation",
                KRS = 9876543210
            };

            //Act
            await _clientService.AddCorporateClientAsync(corporateClientDTO);
            var addedClient = await _context.Clients.FirstOrDefaultAsync(c => c.Email == "corporateclient@example.com");

            //Assert
            Assert.IsNotNull(addedClient);
            Assert.AreEqual("ABC Corporation", addedClient.CorporateClient.CorpoName);
            Assert.AreEqual(9876543210, addedClient.CorporateClient.KRS);
        }

        [TestMethod]
        public async Task UpdateCorporateClientAsync_ShouldUpdateCorporateClient()
        {
            //Arrange
            var corporateClientDTO = new CorpoClientDTO
            {
                Address = "789 Pine St",
                Email = "corporateclient@example.com",
                PhoneNumber = "4445556666",
                CorpoName = "ABC Corporation",
                KRS = 9876543210
            };
            
            await _clientService.AddCorporateClientAsync(corporateClientDTO);
            
            var corporateEditDTO = new CorpoEditDTO()
            {
                Address = "Updated Address",
                Email = "client2@example.com",
                PhoneNumber = "9876543210",
                CorpoName = "UpdatedCorpoName",
            };

            //Act
            await _clientService.UpdateCorporateClientAsync(3, corporateEditDTO);
            var updatedClient = await _context.Clients.FirstOrDefaultAsync(c => c.IdClient == 3);

            //Assert
            Assert.IsNotNull(updatedClient);
            Assert.AreEqual("Updated Address", updatedClient.Address);
            Assert.AreEqual("UpdatedCorpoName", updatedClient.CorporateClient.CorpoName);
            Assert.AreEqual(9876543210, updatedClient.CorporateClient.KRS);
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

    }
}
