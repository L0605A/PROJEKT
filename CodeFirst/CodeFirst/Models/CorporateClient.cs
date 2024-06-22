using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace CodeFirst.Models
{
    [Table("corporate_clients")]
    public class CorporateClient
    {
        [Key]
        [ForeignKey("Client")]
        public int IdClient { get; set; }
        public Client Client { get; set; }

        [Required]
        public string CorpoName { get; set; } = string.Empty;

        [Required]
        public BigInteger KRS { get; set; }
    }
}