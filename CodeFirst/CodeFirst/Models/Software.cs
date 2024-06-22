using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFirst.Models;

[Table("softwares")]
public class Software
{
    [Key]
    public int IdSoftware { get; set; }
    [MaxLength(100)]
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    [Required]
    public string Description { get; set; } = string.Empty;

    [MaxLength(50)]
    [Required]
    public string Version { get; set; } = string.Empty;
    
    [MaxLength(50)]
    [Required]
    public string Type { get; set; } = string.Empty;
}

