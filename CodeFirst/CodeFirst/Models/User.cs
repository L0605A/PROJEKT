using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFirst.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Username { get; set; }
        [MaxLength(100)]
        public string PasswordHash { get; set; }

        public string Role { get; set; }
    
    }
}
