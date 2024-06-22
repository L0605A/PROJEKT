using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace CodeFirst.Models;

public class Contracts
{
    [Key]
    public int IdContract { get; set; }
    
    [ForeignKey("Client")]
    [Required]
    public int IdClient { get; set; }
    
    [ForeignKey("Software")]
    [Required]
    public int IdSoftware { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateOnly DateFrom { get; set; }
    
    [Required] 
    public BigInteger Price { get; set; }
}