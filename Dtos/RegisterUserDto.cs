using System.ComponentModel.DataAnnotations;

namespace Learntendo_backend.Dtos
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "Password is required")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }

        [EmailAddress(ErrorMessage = "Please enter valid email")]
        public required string Email { get; set; }
    }
}
