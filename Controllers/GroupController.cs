using Learntendo_backend.Data;
//using Learntendo_backend.Hubs;
using Learntendo_backend.Models;
using Learntendo_backend.Hubs;
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
        [HttpGet("GetLeaderboard/{groupId}")]
        public async Task<IActionResult> GetLeaderboard(int groupId)
        {
                return NotFound("User not found or not assigned to a group.");
            }

            
            var leaderboard = _db.User
                .Where(u => u.GroupId == userGroup) 
            var leaderboard = await _db.User
                .Where(u => u.GroupId == groupId)
                .OrderByDescending(u => u.WeeklyXp)
                .Select(u => new
                {
                    u.UserId,
                    u.Username,
                    u.WeeklyXp,

                })
                .ToList();
                .ToListAsync();

            return Ok(leaderboard);
        }
        [HttpPost("UpdateScore")]
        public async Task<IActionResult> UpdateScore(int userId, int points, [FromServices] IHubContext<LeaderboardHub> hubContext)
        {
            var user = await _db.User.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            user.WeeklyXp += points;
            await _db.SaveChangesAsync();

            // إرسال تحديث للـ Leaderboard الخاص بالجروب
            await hubContext.Clients.Group($"group-{user.GroupId}").SendAsync("ReceiveLeaderboardUpdate");

            return Ok(new { message = "Score updated", user.WeeklyXp });
        }

        //await _groupService.ResetWeeklyGroups();
        //return Ok(new { message = "Weekly groups reset successfully!" });
    }
}
    



