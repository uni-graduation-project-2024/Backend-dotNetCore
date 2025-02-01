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


namespace Learntendo_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
            private readonly DataContext _context;
            private readonly IAuthRepository _authRepository;
            private readonly IConfiguration _configuration;

            public AdminController(DataContext context, IAuthRepository authRepository, IConfiguration configuration)
            {
                _context = context;
                _authRepository = authRepository;
                _configuration = configuration;
            }

            // Admin login
            [HttpPost("login")]
            public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
            {
                var admin = await _context.Admin.FirstOrDefaultAsync(a => a.Email == loginDto.Email);
                if (admin == null)
                    return Unauthorized(new { message = "Admin not found" });

                if (!VerifyPassword(loginDto.Password, admin.PasswordHash, admin.PasswordSalt))
                    return Unauthorized(new { message = "Invalid password" });

                var token = GenerateJwtToken(admin);
                return Ok(new { token });
            }

            private string GenerateJwtToken(Admin admin)
            {
                var claims = new[]
                {
            new Claim(ClaimTypes.NameIdentifier, admin.AdminId.ToString()),
            new Claim(ClaimTypes.Email, admin.Email),
            new Claim(ClaimTypes.Name, admin.Username),
            new Claim(ClaimTypes.Role, "Admin")
        };

                var key = Encoding.UTF8.GetBytes(_configuration["JwtSecretKey"]); // Use a secret key from the configuration
                var securityKey = new SymmetricSecurityKey(key);
                var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    NotBefore = DateTime.Now,
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = creds
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return tokenHandler.WriteToken(token);
            }

            // Change password
            [HttpPost("change-password")]
            [Authorize(Roles = "Admin")]  // Ensure that only admins can change their password
            public async Task<IActionResult> ChangePassword([FromBody] ChangePassword changePasswordDto)
            {
                var admin = await _context.Admin.FirstOrDefaultAsync(a => a.AdminId == changePasswordDto.UserId);
                if (admin == null)
                    return NotFound(new { message = "Admin not found" });

                if (!VerifyPassword(changePasswordDto.OldPassword, admin.PasswordHash, admin.PasswordSalt))
                    return BadRequest(new { message = "Old password is incorrect" });

                if (string.IsNullOrWhiteSpace(changePasswordDto.NewPassword) || changePasswordDto.NewPassword.Length < 8)
                    return BadRequest(new { message = "New password must be at least 8 characters long" });

                CreatePasswordHash(changePasswordDto.NewPassword, out byte[] newHash, out byte[] newSalt);
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
            [Authorize(Roles = "Admin")]  // Ensure only admins can delete users
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





