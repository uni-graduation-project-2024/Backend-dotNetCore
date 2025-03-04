using Learntendo_backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Learntendo_backend.Models;
//using Group = Learntendo_backend.Models.Group;
using Group = Learntendo_backend.Models.Group;
using Microsoft.Extensions.DependencyInjection;
using Group = Learntendo_backend.Models.Group;

namespace Learntendo_backend.Services
{
    public class GroupService : BackgroundService
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
      
        private DateTime GetStartOfWeek(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Saturday)) % 7;
            return date.AddDays(-diff).Date;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
       
        }


        public void AssignUsersToGroups()
            while (!stoppingToken.IsCancellationRequested)
            {
          
                using (var scope = _scopeFactory.CreateScope())
                {
                var _db = scope.ServiceProvider.GetRequiredService<DataContext>();
                    var groupService = scope.ServiceProvider.GetRequiredService<GroupService>();
                    await groupService.ResetWeeklyGroups();
                }

                var startOfWeek = GetStartOfWeek(DateTime.UtcNow);
                var endOfWeek = startOfWeek.AddDays(6).Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                // تصفير GroupId للمستخدمين قبل حذف الجروبات القديمة
                _db.User.ToList().ForEach(u => u.GroupId = null);
                _db.SaveChanges();

                // حذف الجروبات القديمة
                var oldGroups = _db.Group.ToList();
                _db.Group.RemoveRange(oldGroups);
                _db.SaveChanges();

                // إنشاء الجروب الخاص بالأسبوع الجديد
                var newWeek = new Group
                // 🔥 تشغيل إعادة التوزيع كل 7 أيام
                await Task.Delay(TimeSpan.FromDays(7), stoppingToken);
            }
        }
        public async Task ResetWeeklyGroups()
        {
                    GroupName = $"Week {startOfWeek:yyyy-MM-dd}",
                    StartDate = startOfWeek,
                    EndDate = endOfWeek
                };
            // حذف المجموعات القديمة
            _db.Group.RemoveRange(_db.Group);
            await _db.SaveChangesAsync();

                _db.Group.Add(newWeek);
                _db.SaveChanges();

                Console.WriteLine($"تم إنشاء الجروب للأسبوع {newWeek.GroupName}");

                var users = _db.User.ToList();
                var random = new Random();

                users = users.OrderBy(u => random.Next()).ToList();

            // جلب جميع المستخدمين وتصفير النقاط
            var users = await _db.User.ToListAsync();
            foreach (var user in users)
            {
                    user.WeeklyXp = 0;
                user.StreakScore = 0;
                user.DailyXp = 0;
                user.GroupId = 0;
            }

                _db.SaveChanges();

                // إنشاء الجروبات الفرعية داخل هذا الأسبوع الجديد
                int groupCount = (int)Math.Ceiling(users.Count / 5.0);
                var newGroups = new List<Group>();
            await _db.SaveChangesAsync();

                for (int i = 0; i < groupCount; i++)
                {
                    newGroups.Add(new Group
                    {
                        GroupName = $"Group {i + 1} - {newWeek.GroupName}", // ربط الاسم بالأسبوع
                        StartDate = newWeek.StartDate,
                        EndDate = newWeek.EndDate
                    });
                }
            // توزيع المستخدمين على مجموعات جديدة
            //Random random = new Random();
            //int groupNumber = 1;

                _db.Group.AddRange(newGroups);
                _db.SaveChanges();
            //for (int i = 0; i < users.Count; i += 10)
            //{
            //    var newGroup = new Group { GroupName = $"Group {groupNumber}", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(7) };
            //    _db.Group.Add(newGroup);
            //    await _db.SaveChangesAsync();

                // التأكد من حفظ الجروبات
                var savedGroups = _db.Group.Where(g => g.StartDate == startOfWeek).ToList();
                Console.WriteLine($"تم حفظ {savedGroups.Count} جروبًا في قاعدة البيانات.");
            //    foreach (var user in users.Skip(i).Take(10))
            //    {
            //        user.GroupId = newGroup.GroupId;
            //    }
            //    groupNumber++;
            //}

                if (savedGroups.Count == 0)
                {
                    Console.WriteLine(" لم يتم حفظ الجروبات بشكل صحيح!");
                    return;
                }
            //await _db.SaveChangesAsync();
            var newGroups = new List<Group>();

                // ربط المستخدمين بالجروبات الجديدة
                for (int i = 0; i < users.Count; i++)
            for (int i = 0; i < users.Count; i += 10)
            {
                    users[i].GroupId = savedGroups[i % savedGroups.Count].GroupId;
                var newGroup = new Group { GroupName = $"Group {groupNumber}", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(7) };
                newGroups.Add(newGroup);
                groupNumber++;
            }

                _db.SaveChanges();

                Console.WriteLine("تم توزيع المستخدمين على الجروبات بنجاح!");
            }
            _db.Group.AddRange(newGroups); // ✅ إضافة جميع الجروبات دفعة واحدة
            await _db.SaveChangesAsync();

        }



    }


        
}


