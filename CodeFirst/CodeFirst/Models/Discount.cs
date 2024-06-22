using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFirst.Models
{
    [Table("discounts")]
    public class Discount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdDiscount { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Offer { get; set; } = string.Empty;
        
        [Required]
        public int Amt { get; set; }
        
        
        [Required]
        public DateOnly DateFrom { get; set; }
        
        [Required]
        public DateOnly DateTo { get; set; }
        
    }
}