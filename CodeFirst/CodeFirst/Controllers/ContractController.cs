using CodeFirst.Models;
using CodeFirst.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeFirst.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;

        public ContractController(IContractService ContractService)
        {
            _contractService = ContractService;
        }
        

        [HttpPost]
        public async Task<IActionResult> AddOneTimePaymentContract(OneTimePaymentDTO contractDto)
        {
            //Check if client exists
            if (!await _contractService.ClientExists(contractDto.IdClient))
            {
                return NotFound(new { Message = "Client not found." });
            }

            //Check if software exists
            if (!await _contractService.SoftwareExists(contractDto.IdSoftware))
            {
                return NotFound(new { Message = "Software not found." });
            }

            //Check if client already has an active contract for this software
            if (await _contractService.ClientHasContract(contractDto))
            {
                return Conflict(new { Message = "Client already has an active contract for this software." });
            }

            //Check if the period is correct
            if (!await _contractService.PeriodCorrect(contractDto.DateFrom, contractDto.DateTo))
            {
                return BadRequest(new { Message = "The contract period must be between 3 and 30 days." });
            }

            //Finally, create the contract
            var result = await _contractService.CreateContractAsync(contractDto);
            return Ok(result);
        }
    }
}