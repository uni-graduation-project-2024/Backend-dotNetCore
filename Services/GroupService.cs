﻿using Learntendo_backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Learntendo_backend.Models;
//using Group = Learntendo_backend.Models.Group;
using Microsoft.Extensions.DependencyInjection;
using Group = Learntendo_backend.Models.Group;

namespace Learntendo_backend.Services
{
    public class GroupService
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

        public enum UserLevel
        {
            Newbie = 0,
            Beginner = 1,
            Professional = 2,
            Expert = 3,
            Master = 4
        }
        public void AssignUsersToGroupsTest()
        {

            using (var scope = _scopeFactory.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var startOfWeek = GetStartOfWeek(DateTime.UtcNow);
                var endOfWeek = startOfWeek.AddDays(6).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                var groupedUsers = _db.User
               .Where(u => u.GroupId != null)
               .GroupBy(u => u.GroupId)
               .ToList();

                foreach (var group in groupedUsers)
                {
                    var topUsers = group.OrderByDescending(u => u.WeeklyXp).ToList();

                    if (topUsers.Any())
                    {
                        topUsers[0].Coins += 100;
                        UpgradeLevel(topUsers[0]);
                    }
                    if (topUsers.Count > 1) topUsers[1].Coins += 70;
                    if (topUsers.Count > 2) topUsers[2].Coins += 50;
                    if (topUsers.Any()) DowngradeLevel(topUsers.Last());
                }

                _db.SaveChanges();
                //var oldGroups = _db.Group.ToList();
                //_db.Group.RemoveRange(oldGroups);
                var oldGroups = _db.Group.AsNoTracking().ToList();

                foreach (var group in oldGroups)
                {
                    _db.Group.Attach(group);
                    _db.Group.Remove(group);
                }
                _db.SaveChanges();
                var newWeek = new Group
                {
                    GroupName = $"Week {startOfWeek:yyyy-MM-dd}",
                    StartDate = startOfWeek,
                    EndDate = endOfWeek
                };

                _db.Group.Add(newWeek);
                _db.SaveChanges();

                Console.WriteLine($"The group was created for the week {newWeek.GroupName}");

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

                int groupIndex = 0;
                for (int i = 0; i < users.Count; i++)
                {
                    users[i].GroupId = savedGroups[groupIndex].GroupId;


                    if ((i + 1) % 5 == 0 && groupIndex < savedGroups.Count - 1)
                    {
                        groupIndex++;
                    }
                }

                _db.SaveChanges();


            }

        }
        private void UpgradeLevel(User user)
        {
            if (user.Level < UserLevel.Master)
            {
                user.Level++;
            }
        }

        private void DowngradeLevel(User user)
        {
            if (user.Level > UserLevel.Newbie)
            {
                user.Level--;
            }
        }



    }
}