using CodeFirst.Models;
using CodeFirst.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeFirst.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllClients()
        {
            var clients = await _clientService.GetAllClientsAsync();
            return Ok(clients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClientById(int id)
        {
            var client = await _clientService.GetClientByIdAsync(id);
            if (client == null)
            {
                return NotFound("No client with such ID found");
            }
            return Ok(client);
        }

        [HttpPost("personal")]
        public async Task<IActionResult> CreatePersonalClient([FromBody] PersonalClientDTO client)
        {
            await _clientService.AddPersonalClientAsync(client);
            return Created();
        }

        [HttpPost("corporate")]
        public async Task<IActionResult> CreateCorporateClient([FromBody] CorpoClientDTO client)
        {
            await _clientService.AddCorporateClientAsync(client);
            return Created();
        }

        
        
        [HttpPut("personal/{id}")]
        public async Task<IActionResult> UpdatePersonalClient(int id,[FromBody] PersonalEditDTO client)
        {
            if (! await _clientService.ClientExists(id))
            {
                return BadRequest("No client with such ID found");
            }
            
            if (await _clientService.CorpoClientExists(id))
            {
                return BadRequest("Entered client is corporate, not personal");
            }
            
            if (await _clientService.IsClientDeleted(id))
            {
                return NotFound("The client is deleted");
            }

            await _clientService.UpdatePersonalClientAsync(id, client);
            return Ok("Updated personal client");
        }
        
        [HttpPut("corporate/{id}")]
        public async Task<IActionResult> UpdateCorporateClient(int id,[FromBody] CorpoEditDTO client)
        {
            if (!await _clientService.ClientExists(id))
            {
                return BadRequest("No client with such ID found");
            }
            
            if (! await _clientService.CorpoClientExists(id))
            {
                return BadRequest("Entered client is personal, not corporate");
            }
            
            if (await _clientService.IsClientDeleted(id))
            {
                return NotFound("The client is deleted");
            }

            await _clientService.UpdateCorporateClientAsync(id, client);
            return Ok("Updated corporate client");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _clientService.GetClientByIdAsync(id);
            if (client == null)
            {
                return NotFound("No client with such ID found");
            }
            
            if (await _clientService.IsClientDeleted(id))
            {
                return NotFound("The client is deleted");
            }

            if (client.CorporateClientInfo != null)
            {
                return BadRequest("Cannot delete corporate clients.");
            }

            await _clientService.SoftDeleteClientAsync(id);
            return Ok("Removed a client");
        }
    }
}
