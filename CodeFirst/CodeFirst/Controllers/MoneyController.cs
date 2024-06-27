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

        [HttpPost("pay-for-a-contract")]
        public async Task<IActionResult> Pay(PayDTO payment)
        {
            if (!await _moneyService.ContractExists(payment.ContractId))
            {
                return NotFound("No contract found!");
            }

            if (await _moneyService.IsSubscription(payment.ContractId))
            {
                //Handle subs
                
                if (!await _moneyService.IsTimeValid(payment.ContractId))
                {
                    return BadRequest("It's not the time to renew the subscription, or the subscription has been already cancelled");
                }
                
                if (!await _moneyService.IsPriceValid(payment.ContractId, payment.Amount))
                {
                    return BadRequest("Amount of money should be exactly what is required in the contract");
                }

                await _moneyService.PayForSubscriptionContract(payment.ContractId, payment.Amount);
            }
            else
            {
                //Handle one times

                if (!await _moneyService.IsTimeValid(payment.ContractId))
                {
                    return BadRequest("Time to pay for the contract has passed");
                }
                
                if (!await _moneyService.IsPayedOff(payment.ContractId))
                {
                    return BadRequest("This contract is already payed off");
                }
                
                if (!await _moneyService.IsPriceValid(payment.ContractId, payment.Amount))
                {
                    return BadRequest("Amount of money is incorrect");
                }

                await _moneyService.PayForOneTimeContract(payment.ContractId, payment.Amount);
            }

            return Ok("Payment processed");
        }


        [HttpGet("profit")]
        public async Task<IActionResult> GetProfit([FromQuery] ProfitDTO profit)
        {
            try
            {
                await _moneyService.GetNBPRate(profit.Currency);
            }
            catch (HttpRequestException e)
            {
                return NotFound("Currency not found");
            }
            
            if (profit.IdSoftware.HasValue)
            {
                if (!(await _moneyService.SoftwareExists(profit.IdSoftware)))
                {
                    return NotFound("No software found");
                }
            }

            var profitValue = await _moneyService.GetProfit(profit.IdSoftware, profit.Currency);
            
            return Ok("Profit: " + profitValue);
        }
        
        [HttpGet("predicted-profit")]
        public async Task<IActionResult> GetPredictedProfit([FromQuery] PredictedProfitDTO profit)
        {
            
            try
            {
                await _moneyService.GetNBPRate(profit.Currency);
            }
            catch (HttpRequestException e)
            {
                return NotFound("Currency not found");
            }
            
            if (profit.IdSoftware.HasValue)
            {
                if (!(await _moneyService.SoftwareExists(profit.IdSoftware)))
                {
                    return NotFound("No software found");
                }
            }

            var profitValue = await _moneyService.GetPredictedProfit(profit.IdSoftware, profit.Currency, profit.PeriodInMonths);
            
            return Ok("Predicted profit: " + profitValue);
        }
    }
}