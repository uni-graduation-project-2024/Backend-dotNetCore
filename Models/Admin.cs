using System.ComponentModel.DataAnnotations;

namespace Learntendo_backend.Models
{
    public class Admin
    {
        [Key]
        public int UserId { get; set; }

        [EmailAddress(ErrorMessage = "Please enter valid email")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }
    }
}
