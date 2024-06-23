using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace CodeFirst.Models
{
    [Table("personal_clients")]
    public class PersonalClient
    {
        [Key]
        [ForeignKey("Client")]
        public int IdClient { get; set; }
        public Client Client { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Surname { get; set; } = string.Empty;

        [Required]
        public Decimal PESEL { get; set; }
    }
}