using CodeFirst.Models;
using CodeFirst.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeFirst.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]

    public class MoneyController : ControllerBase
    {
        private readonly IMoneyService _moneyService;

        public MoneyController(IMoneyService MoneyService)
        {
            _moneyService = MoneyService;
        }

        [HttpPost]
        public async Task<IActionResult> Pay(int contractId, int amount)
        {
            if (await _moneyService.ContractExists(contractId))
            {
                return NotFound("No contract found!");
            }

            if (await _moneyService.IsSubscription(contractId))
            {
                //Handle subs
                
                if (await _moneyService.IsTimeValid(contractId))
                {
                    return BadRequest("It's not the time to renew the subscription, or the subscription has been already cancelled");
                }
                
                if (await _moneyService.IsPriceValid(contractId, amount))
                {
                    return BadRequest("Amount of money should be exactly what is required in the contract");
                }

                await _moneyService.PayForSubscriptionContract(contractId, amount);
            }
            else
            {
                //Handle one times

                if (await _moneyService.IsTimeValid(contractId))
                {
                    return BadRequest("Time to pay for the contract has passed");
                }
                
                if (await _moneyService.IsPriceValid(contractId, amount))
                {
                    return BadRequest("Amount of money is incorrect");
                }

                await _moneyService.PayForOneTimeContract(contractId, amount);
            }

            return Ok("Payment processed");
        }


        [HttpGet("profit")]
        public async Task<IActionResult> GetProfit(int? IdSoftware ,string currency = "PLN")
        {
            try
            {
                await _moneyService.GetNBPRate(currency);
            }
            catch (HttpRequestException e)
            {
                return NotFound("Currency not found");
            }
            
            if (IdSoftware.HasValue)
            {
                if (!(await _moneyService.SoftwareExists(IdSoftware)))
                {
                    return NotFound("No software found");
                }
            }

            var profit = await _moneyService.GetProfit(IdSoftware, currency);
            
            return Ok(profit);
        }
        
        [HttpGet("predicted-profit")]
        public async Task<IActionResult> GetPredictedProfit(int? IdSoftware, int periodInMonths ,string currency = "PLN")
        {
            
            try
            {
                await _moneyService.GetNBPRate(currency);
            }
            catch (HttpRequestException e)
            {
                return NotFound("Currency not found");
            }
            
            if (IdSoftware.HasValue)
            {
                if (!(await _moneyService.SoftwareExists(IdSoftware)))
                {
                    return NotFound("No software found");
                }
            }

            var profit = await _moneyService.GetPredictedProfit(IdSoftware, currency, periodInMonths);
            
            return Ok(profit);
        }
    }
}