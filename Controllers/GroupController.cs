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


            var leaderboard = _db.User
                .Where(u => u.GroupId == userGroup)
                .OrderByDescending(u => u.WeeklyXp)
                .Select(u => new
                {
                    u.UserId,
                    u.Username,
                    u.WeeklyXp,
                })
                .ToList();

            return Ok(leaderboard);
        }


    }
}







