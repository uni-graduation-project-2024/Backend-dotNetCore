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
using Learntendo_backend.Services;
using System.Collections.Generic;
using iText.Layout.Element;

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


        [HttpGet("user-navbar-info")]
        public async Task<IActionResult> userNavbarInfo(int userId)
        {
            try
            {
                var user = await _userRepo.GetByIdFun(userId);
                var userNavbarInfo = new
                {
                    user.StreakScore,
                    user.Coins,
                    user.GenerationPower,
                    user.FreezeStreak,

                };

                return Ok(userNavbarInfo);
            }
            catch
            {
                return NotFound($"No user found with this ID: {userId}");
            }
        }
        
        [HttpGet("user-profile")]
        // [Authorize(Roles = "Admin")]  // Ensure only admins can view users
        public async Task<IActionResult> userProfile(int userId)
        {

            try
            {
                var user = await _userRepo.GetByIdFun(userId);
                var userDto = _mapper.Map<UserDto>(user);

                return Ok(userDto);
            }
            catch
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

        [HttpGet("get-challenge-status/{userId}")]
        public async Task<IActionResult> GetChallengeStatus(int userId)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var userDto = _mapper.Map<UserDto>(user); 
            int dailytarget = 50;
            List<int> monthlytarget = [1000, 2000, 3000];
           
            return Ok(new
            {
                userDto.DailyXp,
                userDto.MonthlyXp,
                Targetdaily = dailytarget,
                monthtarget = monthlytarget
            });
        }

        [HttpGet("MonthlyBudge/{userId}")]
        public async Task<IActionResult> MonthlyBudge(int userId)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(new
            {
                userDto.LeagueHistory
            });
        }

        [HttpPost("PurchaseGenerationPower/{userId}")]
        public async Task<IActionResult> PurchaseGenerationPower(int userId)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (user.GenerationPower == 5)
            {
                return BadRequest("Maximum Generation Power reached.");
            }

            if (user.Coins < 100)
            {
                return BadRequest("Not enough coins to purchase Generation Power.");
            }

           
            user.GenerationPower += 1;
            user.Coins -= 100;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Generation Power purchased successfully.",
                NewGenerationPower = user.GenerationPower,
                RemainingCoins = user.Coins
            });
        }

    }
}
