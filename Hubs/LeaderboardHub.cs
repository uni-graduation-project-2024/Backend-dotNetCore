using Learntendo_backend.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Learntendo_backend.Models;
using Learntendo_backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
namespace Learntendo_backend.Hubs
{
   
    public class LeaderboardHub : Hub
    {
 
            private readonly DataContext _db;

            public LeaderboardHub(DataContext db)
            {
                _db = db;
            }

            public async Task UpdateLeaderboard(int groupId)
            {
                // جلب جميع المستخدمين في الجروب وترتيبهم حسب النقاط
                var leaderboard = await _db.User
                    .Where(u => u.GroupId == groupId)
                    .OrderByDescending(u => u.WeeklyXp)
                    .Select(u => new { u.UserId, u.Username, u.WeeklyXp })
                    .ToListAsync();

                // إرسال الترتيب المحدّث فقط لأعضاء الجروب
                await Clients.Group($"group-{groupId}").SendAsync("ReceiveLeaderboardUpdate", leaderboard);
            }
        public override async Task OnConnectedAsync()
        {

            //var userIdStr = Context.GetHttpContext().Request.Query["userId"];
            //if (!int.TryParse(userIdStr, out int userId))
            //{
            //    return; // ✅ لو `userId` غير صالح، ما تكملش التنفيذ
            //}

            //var user = await _db.User.FirstOrDefaultAsync(u => u.UserId == userId);
            //if (user == null) return;

            //await Groups.AddToGroupAsync(Context.ConnectionId, $"group-{user.GroupId}");
            ////var userId = Context.GetHttpContext().Request.Query["userId"];
            ////var user = await _db.User.FirstOrDefaultAsync(u => u.UserId == int.Parse(userId))
            ////if (user != null)
            ////{
            ////    // إضافة المستخدم إلى الجروب الخاص به
            ////    await Groups.AddToGroupAsync(Context.ConnectionId, $"group-{user.GroupId}");
            ////}

            ////await base.OnConnectedAsync();
            
                var userIdStr = Context.GetHttpContext()?.Request.Query["userId"];
                if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                {
                    await base.OnConnectedAsync(); // 🔥 مهم علشان ما يحصلش خطأ في `Hub`
                    return;
                }

                var user = await _db.User.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                {
                    await base.OnConnectedAsync();
                    return;
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, $"group-{user.GroupId}");

                await base.OnConnectedAsync(); // 🔥 مهم علشان ما يحصلش خطأ في SignalR
            }

        }

    }






