using Learntendo_backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Learntendo_backend.Models;
using Group = Learntendo_backend.Models.Group;
using Microsoft.Extensions.DependencyInjection;

namespace Learntendo_backend.Services
{
    public class GroupService : BackgroundService    {
        private readonly DataContext _db;
        private readonly IServiceScopeFactory _scopeFactory;
        private int groupNumber;

        public GroupService(IServiceScopeFactory scopeFactory, DataContext db)
        {
            _db = db;
            _scopeFactory = scopeFactory;
        }
      

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var groupService = scope.ServiceProvider.GetRequiredService<GroupService>();
                    await groupService.ResetWeeklyGroups();
                }

                // 🔥 تشغيل إعادة التوزيع كل 7 أيام
                await Task.Delay(TimeSpan.FromDays(7), stoppingToken);
            }
        }
        public async Task ResetWeeklyGroups()
        {
            // حذف المجموعات القديمة
            _db.Group.RemoveRange(_db.Group);
            await _db.SaveChangesAsync();

            // جلب جميع المستخدمين وتصفير النقاط
            var users = await _db.User.ToListAsync();
            foreach (var user in users)
            {
                user.StreakScore = 0;
                user.DailyXp = 0;
                user.GroupId = 0;
            }
            await _db.SaveChangesAsync();

            // توزيع المستخدمين على مجموعات جديدة
            //Random random = new Random();
            //int groupNumber = 1;

            //for (int i = 0; i < users.Count; i += 10)
            //{
            //    var newGroup = new Group { GroupName = $"Group {groupNumber}", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(7) };
            //    _db.Group.Add(newGroup);
            //    await _db.SaveChangesAsync();

            //    foreach (var user in users.Skip(i).Take(10))
            //    {
            //        user.GroupId = newGroup.GroupId;
            //    }
            //    groupNumber++;
            //}

            //await _db.SaveChangesAsync();
            var newGroups = new List<Group>();

            for (int i = 0; i < users.Count; i += 10)
            {
                var newGroup = new Group { GroupName = $"Group {groupNumber}", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(7) };
                newGroups.Add(newGroup);
                groupNumber++;
            }

            _db.Group.AddRange(newGroups); // ✅ إضافة جميع الجروبات دفعة واحدة
            await _db.SaveChangesAsync();

        }
    }


}
