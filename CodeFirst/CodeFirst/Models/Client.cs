using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CodeFirst.Models;

[Table("clients")]
public class Client
{
    [Key]
    public int IdClient { get; set; }

    [Required] 
    public string Address { get; set; } = string.Empty;

    [Required] 
    public string Email { get; set; } = string.Empty;

    [Required] public int PhoneNumber { get; set; }

}