using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using Microsoft.EntityFrameworkCore;

namespace CodeFirst.Models;

[Table("personalClients")]
public class PersonalClient
{
    [Key]
    [ForeignKey("Client")]
    public int IdClient { get; set; }

    [Required] 
    public string Name { get; set; } = string.Empty;

    [Required] 
    public string Surname { get; set; } = string.Empty;

    [Required] public BigInteger PESEL { get; set; }

}