using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace CodeFirst.Models;

public class OneTimePayment
{
    [Key]
    [ForeignKey("Contracts")]
    public int IdContract { get; set; }
    
    [ForeignKey("Software")]
    [Required]
    public string Version { get; set; } = string.Empty;
        
    [Required]
    public DateOnly DateTo { get; set; }
    
    [Required] 
    public string Status { get; set; } = string.Empty;
    
    [Required]
    public int UpdatePeriod  { get; set; }
    
}