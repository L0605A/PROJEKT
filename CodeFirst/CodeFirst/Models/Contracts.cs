using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFirst.Models
{
    [Table("contracts")]
    public class Contract
    {
        [Key]
        public int IdContract { get; set; }

        [ForeignKey("Client")]
        [Required]
        public int IdClient { get; set; }
        public Client Client { get; set; }

        [ForeignKey("Software")]
        [Required]
        public int IdSoftware { get; set; }
        public Software Software { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateOnly DateFrom { get; set; }

        [Required]
        public decimal Price { get; set; }
        
        public OneTimePayment OneTimePayment { get; set; }
        public Subscription Subscription { get; set; }
    }
}