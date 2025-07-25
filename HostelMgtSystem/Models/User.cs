


using System.ComponentModel.DataAnnotations; 

namespace HostelMgtSystem.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty; 

        [Required]
        public string Role { get; set; } = string.Empty; 

        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string UniqueId { get; set; } = string.Empty;
    }
}
