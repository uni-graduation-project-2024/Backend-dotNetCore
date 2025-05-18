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
        private readonly ILogger<AdminController> _logger;

        public AdminController(DataContext context, IConfiguration configuration, ILogger<AdminController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }
      
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
            _logger.LogInformation("Entering GenerateJwtToken");

            var secretKey = _configuration["JwtSettings:SecretKey"];
            _logger.LogInformation($"SecretKey length: {secretKey?.Length ?? 0}");

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new InvalidOperationException("JWT Secret Key is missing in configuration.");
            }

            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

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
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = creds,
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
  

        [Authorize(Roles = "Admin")]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword changePassword)
        {
            var admin = await _context.Admin.FirstOrDefaultAsync(a => a.UserId == changePassword.UserId);
            if (admin == null)
                return NotFound(new { message = "Admin not found" });

            if (!VerifyPassword(changePassword.OldPassword, admin.PasswordHash, admin.PasswordSalt))
                return BadRequest(new { message = "Old password is incorrect" });

            if (VerifyPassword(changePassword.NewPassword, admin.PasswordHash, admin.PasswordSalt))
                return BadRequest(new { message = "New password cannot be the same as the old password" });

            if (string.IsNullOrWhiteSpace(changePassword.NewPassword) || changePassword.NewPassword.Length < 8)
                return BadRequest(new { message = "New password must be at least 8 characters long" });

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
        
        [Authorize(Roles = "Admin")]
        [HttpGet("view-users")]
        public async Task<IActionResult> ViewUsers()
        {
            var users = await _context.User.ToListAsync();
            if (users.Count == 0)
            {
                return Ok(new { message = "No users found." });
            }

            return Ok(users);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-user/{userId}")]
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





