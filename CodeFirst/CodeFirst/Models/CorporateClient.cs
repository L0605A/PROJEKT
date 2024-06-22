using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using Microsoft.EntityFrameworkCore;

namespace CodeFirst.Models;

[Table("corporateClients")]
public class CorporateClient
{
    [Key]
    [ForeignKey("Client")]
    public int IdClient { get; set; }

    [Required] 
    public string CorpoName { get; set; } = string.Empty;
    
    [Required] 
    public BigInteger KRS { get; set; }

}