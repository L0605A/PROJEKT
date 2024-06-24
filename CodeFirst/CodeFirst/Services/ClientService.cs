﻿


using CodeFirst.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeFirst.Services
{
    public interface IClientService
    {

        Task<bool> ClientExists(int id);
        Task<IEnumerable<Client>> GetAllClientsAsync();
        Task<Client?> GetClientByIdAsync(int id);
        Task AddPersonalClientAsync(PersonalClientDTO client);
        Task AddCorporateClientAsync(CorpoClientDTO client);
        Task UpdatePersonalClientAsync(int id, PersonalEditDTO client);
        Task UpdateCorporateClientAsync(int id, CorpoEditDTO client);
        Task SoftDeleteClientAsync(int id);
    }

    public class ClientService : IClientService
    {
        private readonly ApplicationContext _context;

        public ClientService(ApplicationContext context)
        {
            _context = context;
        }
        
        public async Task<bool> ClientExists(int id)
        {
            return await _context.Clients.AnyAsync(c => c.IdClient == id);
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            return await _context.Clients
                .Include(c => c.PersonalClient)
                .Include(c => c.CorporateClient)
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<Client?> GetClientByIdAsync(int id)
        {
            return await _context.Clients
                                 .Include(c => c.PersonalClient)
                                 .Include(c => c.CorporateClient)
                                 .FirstOrDefaultAsync(c => c.IdClient == id && !c.IsDeleted);
        }

        public async Task AddCorporateClientAsync(CorpoClientDTO client)
        {
            var clientTemp = new Client()
            {
                Address = client.Address,
                Email = client.Email,
                PhoneNumber = client.PhoneNumber,
                IsDeleted = false
            };
            
            await _context.Clients.AddAsync(clientTemp);
            await _context.SaveChangesAsync();

            var corpoClient = new CorporateClient()
            {
                IdClient = clientTemp.IdClient,
                Client = clientTemp,
                CorpoName = client.CorpoName,
                KRS = client.KRS
            };

            clientTemp.CorporateClient = corpoClient;
            
            await _context.CorporateClients.AddAsync(corpoClient);
            
            await _context.SaveChangesAsync();
        }
        
        public async Task AddPersonalClientAsync(PersonalClientDTO client)
        {
            var clientTemp = new Client()
            {
                Address = client.Address,
                Email = client.Email,
                PhoneNumber = client.PhoneNumber,
                IsDeleted = false
            };
            
            await _context.Clients.AddAsync(clientTemp);
            await _context.SaveChangesAsync();

            var personalClient = new PersonalClient()
            {
                IdClient = clientTemp.IdClient,
                Client = clientTemp,
                Name = client.Name,
                Surname = client.Surname,
                PESEL = client.PESEL
            };

            clientTemp.PersonalClient = personalClient ;
            
            await _context.PersonalClients.AddAsync(personalClient);
            
            await _context.SaveChangesAsync();
        }
        
        public async Task UpdatePersonalClientAsync(int id, PersonalEditDTO client)
        {
            var existingClient = await _context.Clients
                .Include(c => c.PersonalClient)
                .FirstOrDefaultAsync(c => c.IdClient == id && !c.IsDeleted);

            if (existingClient != null)
            {
                existingClient.Address = client.Address;
                existingClient.Email = client.Email;
                existingClient.PhoneNumber = client.PhoneNumber;

                existingClient.PersonalClient.Name = client.Name;
                existingClient.PersonalClient.Surname = client.Surname;

                _context.Clients.Update(existingClient);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateCorporateClientAsync(int id, CorpoEditDTO client)
        {
            var existingClient = await _context.Clients
                .Include(c => c.CorporateClient)
                .FirstOrDefaultAsync(c => c.IdClient == id && !c.IsDeleted);

            if (existingClient != null)
            {
                existingClient.Address = client.Address;
                existingClient.Email = client.Email;
                existingClient.PhoneNumber = client.PhoneNumber;

                existingClient.CorporateClient.CorpoName = client.CorpoName;

                _context.Clients.Update(existingClient);
                await _context.SaveChangesAsync();
            }
        }


        public async Task SoftDeleteClientAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                client.IsDeleted = true;
                _context.Clients.Update(client);
                await _context.SaveChangesAsync();
            }
        }
        
    }
}
