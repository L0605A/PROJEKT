using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Numerics;

namespace CodeFirst.Models
{
    [Table("clients")]
    public class Client
    {
        [Key]
        public int IdClient { get; set; }

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public Decimal PhoneNumber { get; set; }

        public PersonalClient PersonalClient { get; set; }
        public CorporateClient CorporateClient { get; set; }
        public ICollection<Contract> Contracts { get; set; }
        public bool IsDeleted { get; set; }
    }
}
