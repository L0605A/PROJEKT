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

    public class DiscountsController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountsController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDiscounts()
        {
            var discounts = await _discountService.GetAllDiscountsAsync();
            return Ok(discounts);
        }

        [HttpPost]
        public async Task<IActionResult> AddDiscount([FromBody] DiscountDTO discountDTO)
        {
            //Check if price for the contract is valid
            if (!await ( _discountService.DateValid(discountDTO.DateFrom)) || !await ( _discountService.DateValid(discountDTO.DateTo)))
            {
                return BadRequest("Dates should be in format \"dd-MM-yyy\"" );
            }
            
            var discount = await _discountService.AddDiscountAsync(discountDTO);
            return Created();
        }
    }
}