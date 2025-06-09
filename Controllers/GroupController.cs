using Learntendo_backend.Data;
using Learntendo_backend.Models;
using Learntendo_backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Learntendo_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly GroupService _groupService;
        private readonly DataContext _db;

        public GroupController(GroupService groupService, DataContext db)
        {
            _groupService = groupService;
            _db = db;
        }

        [HttpGet]
        public IActionResult GetLeaderboard(int userId)
        {

            var userGroup = _db.User.Where(u => u.UserId == userId).Select(u => u.GroupId).FirstOrDefault();

            if (userGroup == null)
            {
                return NotFound("User not found or not assigned to a group.");
            }

            // Get EndDate from the Group table
            var group = _db.Group.FirstOrDefault(g => g.GroupId == userGroup);
            if (group == null)
            {
                return NotFound("Group not found.");
            }

            var users = _db.User
                .Where(u => u.GroupId == userGroup)
                .OrderByDescending(u => u.WeeklyXp)
                .ToList();

            var leaderboard = users.Select(user =>
            {
                string base64Image = null;

                if (!string.IsNullOrEmpty(user.ProfilePicturePath))
                {
                    var relativePath = user.ProfilePicturePath.TrimStart('/');
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

                    if (System.IO.File.Exists(fullPath))
                    {
                        var imageBytes = System.IO.File.ReadAllBytes(fullPath);
                        var extension = Path.GetExtension(fullPath).ToLower().Replace(".", "");
                        base64Image = $"data:image/{extension};base64,{Convert.ToBase64String(imageBytes)}";
                    }
                }

                return new
                {
                    user.UserId,
                    user.Username,
                    user.WeeklyXp,
                    user.Level,
                    ProfileImage = base64Image
                };
            }).ToList();

            return Ok(new
            {
                Leaderboard = leaderboard,
                EndDate = group.EndDate
            });

        }


    }
}







