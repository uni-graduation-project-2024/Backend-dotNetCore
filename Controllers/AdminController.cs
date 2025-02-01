using Learntendo_backend.Data;
using Learntendo_backend.Dtos;
using Learntendo_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Learntendo_backend.configurations;
using Learntendo_backend.Controllers;

namespace Learntendo_backend.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        // private object _logger;
        private readonly ILogger<AdminController> _logger;

        public AdminController(DataContext context, IConfiguration configuration, ILogger<AdminController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        // Admin login
        //[Authorize(Roles = "Admin")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var admin = await _context.Admin.FirstOrDefaultAsync(a => a.Email == loginDto.Email);
            
            if (admin == null || !VerifyPassword(loginDto.Password, admin.PasswordHash, admin.PasswordSalt))
                return Unauthorized(new { message = "Invalid email or password" });


            var token = GenerateJwtToken(admin);
            return Ok(new { token });
        }

        private string GenerateJwtToken(Admin admin)
        {

            _logger.LogInformation("Entering GenerateJwtToken"); // Assuming you have an ILogger instance

            var secretKey = _configuration["JwtSettings:SecretKey"];
            _logger.LogInformation($"SecretKey retrieved (length): {secretKey?.Length ?? 0}"); // Log length, NOT the key itself!

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new InvalidOperationException("JWT Secret Key is missing in configuration."); // This will now be logged
            }
            var keyBytes = new byte[64];

            
            _logger.LogInformation($"keyBytes length: {keyBytes.Length}"); // Should be 64

          
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(keyBytes);
            }

            string base64Key = Convert.ToBase64String(keyBytes);
            Console.WriteLine("Base64 Key (Copy this):");
            Console.WriteLine(base64Key);
          

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new InvalidOperationException("JWT Secret Key is missing in configuration.");
            }

            // 2. Convert the secret key to a byte array.  Important for cryptographic operations.
           // var keyBytes = Encoding.UTF8.GetBytes(secretKey);

            // 3. Create the SymmetricSecurityKey using the key from configuration.
            var securityKey = new SymmetricSecurityKey(keyBytes);

            // 4. Create the signing credentials.  Use the SAME key.
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512); // Use HS512

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, admin.UserId.ToString()),
        new Claim(ClaimTypes.Email, admin.Email),
        new Claim(ClaimTypes.Name, admin.Username),
        new Claim(ClaimTypes.Role, "Admin")
    };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds // Use the credentials we just created
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

   
       [HttpPost("change-password")]
       // [Authorize(Roles = "Admin")]  // Ensure only admins can change their password
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword changePassword)
        {
            var admin = await _context.Admin.FirstOrDefaultAsync(a => a.UserId == changePassword.UserId);
            if (admin == null)
                return NotFound(new { message = "Admin not found" });

            // Verify old password
            if (!VerifyPassword(changePassword.OldPassword, admin.PasswordHash, admin.PasswordSalt))
                return BadRequest(new { message = "Old password is incorrect" });

            // Check if the new password is the same as the old one
            if (VerifyPassword(changePassword.NewPassword, admin.PasswordHash, admin.PasswordSalt))
                return BadRequest(new { message = "New password cannot be the same as the old password" });

            // Ensure the new password is valid
            if (string.IsNullOrWhiteSpace(changePassword.NewPassword) || changePassword.NewPassword.Length < 8)
                return BadRequest(new { message = "New password must be at least 8 characters long" });

            // Create hash and salt for new password
            CreatePasswordHash(changePassword.NewPassword, out byte[] newHash, out byte[] newSalt);
            admin.PasswordHash = newHash;
            admin.PasswordSalt = newSalt;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Password updated successfully!" });
        }


        private bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(storedHash);
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        // View all users - Only admins can access this
        [HttpGet("view-users")]
        [Authorize(Roles = "Admin")]  // Ensure only admins can view users
        public async Task<IActionResult> ViewUsers()
        {
            var users = await _context.User.ToListAsync();
            if (users.Count == 0)
            {
                return Ok(new { message = "No users found." });
            }

            return Ok(users);
        }

        // Delete a user - Only admins can access this
        [HttpDelete("delete-user/{userId}")]
        //[Authorize(Roles = "Admin")]  // Ensure only admins can delete users
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _context.User.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully!" });
        }
    }
}





