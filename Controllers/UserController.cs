using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Learntendo_backend.Data;
using Learntendo_backend.Models;
using Learntendo_backend.Dtos;

namespace Learntendo_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IDataRepository<User> _userRepo;
        private readonly IMapper _mapper;
        public UserController(DataContext context, IDataRepository<User> userRepo, IMapper mapper)
        {
            _context = context;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword changePasswordDto)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.UserId == changePasswordDto.UserId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            if (user.PasswordHash == null || user.PasswordSalt == null)
                return BadRequest(new { message = "User password data is invalid" });

            if (!VerifyPassword(changePasswordDto.OldPassword, user.PasswordHash, user.PasswordSalt))
                return Unauthorized(new { message = "Old password is incorrect" });

            if (string.IsNullOrWhiteSpace(changePasswordDto.NewPassword))
                return BadRequest(new { message = "New password cannot be empty" });

            CreatePasswordHash(changePasswordDto.NewPassword, out byte[] newHash, out byte[] newSalt);
            user.PasswordHash = newHash;
            user.PasswordSalt = newSalt;

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

        [HttpGet("user-profile")]
        // [Authorize(Roles = "Admin")]  // Ensure only admins can view users
        public async Task<IActionResult> userProfile(int userId)
        {

            try
            {
                var user = await _userRepo.GetByIdFun(userId);

                return Ok(user);
            }
            catch (Exception ex)
            {
                return NotFound($"No user found with this ID: {userId}");
            }
        }
      
        [HttpPost("buy-freeze-streak/{userId}")]
        public async Task<IActionResult> BuyFreezeStreak(int userId)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return NotFound("User not found");

            var userDto = _mapper.Map<UserDto>(user);

            int freezeStreak = userDto?.FreezeStreak ?? 0;
            int coins = userDto?.Coins ?? 0;


            if (freezeStreak < 5)
            {
                if (coins >= 100)
                {
                    coins -= 100; 
                    freezeStreak += 1; 
                    userDto.Coins = coins;
                    userDto.FreezeStreak = freezeStreak;
                    _mapper.Map(userDto, user);
                    await _context.SaveChangesAsync();

                    return Ok("Freeze Streak has been successfully purchased!");
                }
                else
                {
                    return BadRequest("You don't have enough coins to purchase Freeze Streak");
                }
            }

            return BadRequest("You have reached the maximum limit for Freeze Streak");
        }
    }
}
