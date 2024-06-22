using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFirst.Models;

public class Subscription
{
    [Key]
    [ForeignKey("Contracts")]
    public int IdContract { get; set; }
    
    [Required]
    public int RenevalTimeInMonths { get; set; }
}