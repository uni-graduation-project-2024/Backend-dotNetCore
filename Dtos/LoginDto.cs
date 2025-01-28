using System.ComponentModel.DataAnnotations;

namespace Learntendo_backend.Dtos
{
    public class LoginDto
    {
        [EmailAddress(ErrorMessage = "Please enter valid email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
