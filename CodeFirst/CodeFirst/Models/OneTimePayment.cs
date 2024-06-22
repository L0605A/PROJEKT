using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFirst.Models
{
    [Table("one_time_payments")]
    public class OneTimePayment
    {
        [Key]
        [ForeignKey("Contract")]
        public int IdContract { get; set; }
        public Contract Contract { get; set; }

        [ForeignKey("Software")]
        [Required]
        public int IdSoftware { get; set; }
        public Software Software { get; set; }

        [Required]
        public string Version { get; set; } = string.Empty;

        [Required]
        public DateOnly DateTo { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        [Required]
        public int UpdatePeriod { get; set; }
    }
}