using System.Net;
using CodeFirst.Models;
using CodeFirst.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeFirst.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]

    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;

        public ContractController(IContractService ContractService)
        {
            _contractService = ContractService;
        }
        
        

        [HttpPost("create-one-time-payment")]
        public async Task<IActionResult> AddOneTimePaymentContract(OneTimePaymentDTO contractDto)
        {
            //Check if client exists
            if (!await _contractService.ClientExists(contractDto.IdClient))
            {
                return NotFound("Client not found." );
            }

            //Check if software exists
            if (!await _contractService.SoftwareExists(contractDto.IdSoftware))
            {
                return NotFound("Software not found." );
            }
            
            //Check if client already has an active contract for this software
            if (await _contractService.ClientHasContract(contractDto.IdClient, contractDto.IdSoftware))
            {
                return Conflict("Client already has an active contract for this software." );
            }

            //Check if price for the contract is valid
            if (!(contractDto.Price > 0))
            {
                return BadRequest("Price is invalid" );
            }
            
            //Check if price for the contract is valid
            if (!await (_contractService.DateValid(contractDto.DateFrom)) || !await (_contractService.DateValid(contractDto.DateTo)))
            {
                return BadRequest("Dates should be in format \"dd-MM-yyy\"" );
            }
            
            //Check if the period is correct
            if (DateOnly.ParseExact(contractDto.DateFrom , "dd-MM-yyyy", null) > DateOnly.ParseExact(contractDto.DateTo , "dd-MM-yyyy", null))
            {
                return BadRequest("DateFrom should be before DateTo" );
            }
            
            //Check if the period is correct
            if (!await _contractService.PeriodCorrect(DateOnly.ParseExact(contractDto.DateFrom , "dd-MM-yyyy", null), DateOnly.ParseExact(contractDto.DateTo , "dd-MM-yyyy", null)))
            {
                return BadRequest("The contract period must be between 3 and 30 days." );
            }

            if (!await _contractService.UpdatePeriodCorrect(contractDto.UpdatePeriod))
            {
                return BadRequest("The update period must be between 1 and 4 years." );
            }
            
            //Finally, create the contract
            var result = await _contractService.CreateContractAsync(contractDto);
            return Ok(result);
        }
        
        
        [HttpPost("create-subscription")]
        public async Task<IActionResult> AddSubscriptionContract(SubscriptionDTO contractDto)
        {
            //Check if client exists
            if (!await _contractService.ClientExists(contractDto.IdClient))
            {
                return NotFound("Client not found." );
            }

            //Check if software exists
            if (!await _contractService.SoftwareExists(contractDto.IdSoftware))
            {
                return NotFound("Software not found." );
            }

            //Check if client already has an active contract for this software
            if (await _contractService.ClientHasContract(contractDto.IdClient, contractDto.IdSoftware))
            {
                return Conflict("Client already has an active contract for this software." );
            }
            
            //Check if price for the contract is valid
            if (!(contractDto.Price > 0))
            {
                return BadRequest("Price is invalid" );
            }
            
                        
            //Check if price for the contract is valid
            if (!await (_contractService.DateValid(contractDto.DateFrom)))
            {
                return BadRequest("Dates should be in format \"dd-MM-yyy\"" );
            }
           
            if (! ( await  _contractService.RenewalPeriodCorrect(contractDto.RenevalTimeInMonths)))
            {
                return BadRequest("Renewal time must be between 1 and 12 months.");
            }

            //Finally, create the contract
            var result = await _contractService.CreateContractAsync(contractDto);
            return Ok(result);
        }
    }
}