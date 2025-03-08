﻿using Learntendo_backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Learntendo_backend.Models;
//using Group = Learntendo_backend.Models.Group;
using Microsoft.Extensions.DependencyInjection;
using Group = Learntendo_backend.Models.Group;

namespace Learntendo_backend.Services
{
    public class GroupService : BackgroundService
    {
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
        {

            using (var scope = _scopeFactory.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var startOfWeek = GetStartOfWeek(DateTime.UtcNow);
                var endOfWeek = startOfWeek.AddDays(6).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                var topUsers = _db.User
                                  .OrderByDescending(u => u.WeeklyXp)
                                  .Take(3)
                                  .ToList();

                if (topUsers.Count > 0) topUsers[0].Coins += 100; 
                if (topUsers.Count > 1) topUsers[1].Coins += 70;  
                if (topUsers.Count > 2) topUsers[2].Coins += 50;  

                _db.SaveChanges();
                _db.User.ToList().ForEach(u => u.GroupId = null);
                _db.SaveChanges();
                var oldGroups = _db.Group.ToList();
                _db.Group.RemoveRange(oldGroups);
                _db.SaveChanges();
                var newWeek = new Group
                {
                    GroupName = $"Week {startOfWeek:yyyy-MM-dd}",
                    StartDate = startOfWeek,
                    EndDate = endOfWeek
                };

                _db.Group.Add(newWeek);
                _db.SaveChanges();

                Console.WriteLine($"تم إنشاء الجروب للأسبوع {newWeek.GroupName}");

                var users = _db.User.ToList();
                var random = new Random();

                users = users.OrderBy(u => random.Next()).ToList();

                foreach (var user in users)
                {
                    user.WeeklyXp = 0;
                }

                _db.SaveChanges();

                
                int groupCount = (int)Math.Ceiling(users.Count / 5.0);
                var newGroups = new List<Group>();

                for (int i = 0; i < groupCount; i++)
                {
                    newGroups.Add(new Group
                    {
                        GroupName = $"Group {i + 1} - {newWeek.GroupName}", 
                        StartDate = newWeek.StartDate,
                        EndDate = newWeek.EndDate
                    });
                }

                _db.Group.AddRange(newGroups);
                _db.SaveChanges();
                var savedGroups = _db.Group.Where(g => g.StartDate == startOfWeek).ToList();
              

                if (savedGroups.Count == 0)
                {
                    
                    return;
                }

               
                for (int i = 0; i < users.Count; i++)
                {
                    users[i].GroupId = savedGroups[i % savedGroups.Count].GroupId;
                }

                _db.SaveChanges();

                
            }

        }



    }
}
