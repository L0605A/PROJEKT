using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFirst.Models
{
    [Table("subscriptions")]
    public class Subscription
    {
        [Key]
        [ForeignKey("Contract")]
        public int IdContract { get; set; }
        public Contract Contract { get; set; }

        [Required]
        public int RenevalTimeInMonths { get; set; }
    }
}