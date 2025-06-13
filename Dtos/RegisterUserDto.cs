using System.ComponentModel.DataAnnotations;

namespace Learntendo_backend.Dtos
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "Password is required")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 30 characters.")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }

        [EmailAddress(ErrorMessage = "Please enter valid email")]
        public required string Email { get; set; }
    }
}
