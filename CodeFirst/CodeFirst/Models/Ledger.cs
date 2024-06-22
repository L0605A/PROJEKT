using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFirst.Models
{
    [Table("ledgers")]
    public class Ledger
    {
        [Key]
        public int IdPayment { get; set; }

        [ForeignKey("Contract")]
        [Required]
        public int IdContract { get; set; }
        public Contract Contract { get; set; }

        [Required]
        public decimal AmountPaid { get; set; }
    }
}