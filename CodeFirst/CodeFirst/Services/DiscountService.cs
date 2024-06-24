using CodeFirst.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeFirst.Services
{
    public interface IDiscountService
    {
        Task<IEnumerable<Discount>> GetAllDiscountsAsync();
        Task<Discount> AddDiscountAsync(DiscountDTO discountDTO);
    }

    public class DiscountService : IDiscountService
    {

        private readonly ApplicationContext _context;

        public DiscountService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Discount>> GetAllDiscountsAsync()
        {
            return await _context.Discounts.ToListAsync();
        }

        public async Task<Discount> AddDiscountAsync(DiscountDTO discountDTO)
        {
            var discount = new Discount()
            {
                Name = discountDTO.Name,
                Offer = discountDTO.Offer,
                Amt = discountDTO.Amt,
                DateFrom = DateOnly.ParseExact(discountDTO.DateFrom, "dd-MM-yyyy", null),
                DateTo = DateOnly.ParseExact(discountDTO.DateTo, "dd-MM-yyyy", null)

            };
            
            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();

            return discount;
        }
    }
}