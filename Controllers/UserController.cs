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
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Learntendo_backend.Migrations;

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
        public async Task<IActionResult> UserNavbarInfo(int userId)
        {
            try
            {
                var user = await _userRepo.GetByIdFun(userId);
                var userNavbarInfo = new
                {
                    user.StreakScore,
                    user.IfStreakActive,
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
        public async Task<IActionResult> UserProfile(int userId)
        {
            try
            {
                var user = await _userRepo.GetByIdFun(userId);

                
                string base64Image = null;
                if (!string.IsNullOrEmpty(user.ProfilePicturePath))  
                {
                   
                    var relativePath = user.ProfilePicturePath.TrimStart('/');
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

                    if (System.IO.File.Exists(fullPath))
                    {
                        var imageBytes = System.IO.File.ReadAllBytes(fullPath);
                        var extension = Path.GetExtension(fullPath).ToLower().Replace(".", ""); // jpg/png
                        base64Image = $"data:image/{extension};base64,{Convert.ToBase64String(imageBytes)}";
                    }
                }

                var userProfileInfo = new
                {
                    username = user.Username,
                    email = user.Email,
                    joinedDate = user.JoinedDate,
                    totalXp = user.TotalXp,
                    totalQuestion = user.TotalQuestion,
                    streakScore = user.StreakScore,
                    level = user.Level,
                    profileImage = base64Image  // صورة مش مجرد مسار
                };

                return Ok(userProfileInfo);
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


        [HttpPost("upload-profile-pic/{userId}")]
        public async Task<IActionResult> UploadProfilePicture(int userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Please upload a valid file." });
            }

            var user = await _context.User.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile_pics");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            user.ProfilePicturePath = $"/profile_pics/{uniqueFileName}";
            await _context.SaveChangesAsync();

            return Ok(new { message = "Profile picture uploaded successfully.", path = user.ProfilePicturePath });
        }

     

        [HttpDelete("delete-account/{userId}")]
        public async Task<IActionResult> DeleteAccount(int userId)
        {
            var user = await _context.User
                .Include(u => u.Subjects)
                    .ThenInclude(s => s.Exams)
                .Include(u => u.Exams) 
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return NotFound(new { message = "User not found." });

            if (user.Exams != null && user.Exams.Any())
            {
                _context.Exam.RemoveRange(user.Exams);
            }

            foreach (var subject in user.Subjects)
            {
                if (subject.Exams != null && subject.Exams.Any())
                {
                    _context.Exam.RemoveRange(subject.Exams);
                }
            }

            _context.Subject.RemoveRange(user.Subjects);

            _context.User.Remove(user);

            await _context.SaveChangesAsync();

            return Ok(new { message = "User and all related data deleted successfully." });
        }


        [HttpPost("ReportProblem/{userId}")]
        public async Task<IActionResult> ReportProblem(int userId, [FromBody] string problemText)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.ProblemReport = problemText;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Problem report sent successfully" });
        }



    }
}
