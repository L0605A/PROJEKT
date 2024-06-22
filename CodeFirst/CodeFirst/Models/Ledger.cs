using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace CodeFirst.Models;

public class Ledger
{
    [Key]
    public int IdPayment { get; set; }
    
    [ForeignKey("Contracts")]
    public int IdContract { get; set; }
    
    [Required] 
    public BigInteger AmountPaid { get; set; }
}