using Learntendo_backend.Data;
using Learntendo_backend.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Learntendo_backend.Services
{
    public class DailyResetService 
    {
        private readonly DataContext _db;
        //private readonly IServiceScopeFactory _scopeFactory;


        public DailyResetService(IServiceScopeFactory scopeFactory, DataContext db)
        {
            _db = db;
            //_scopeFactory = scopeFactory;
        }

        //public class DailyChallengeService : BackgroundService
        //{


        //public DailyChallengeService(IServiceScopeFactory scopeFactory)
        //{
        //    _scopeFactory = scopeFactory;
        //}

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        using (var scope = _scopeFactory.CreateScope())
        //        {
        //            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        //            var challengeService = scope.ServiceProvider.GetRequiredService<DailyResetService>();

        //            //await challengeService.UpdateDailyChallengesForAllUsers();
        //        }

                var now = DateTime.UtcNow;
                var nextRun = now.Date.AddDays(1);
                var delay = nextRun - now;
                await Task.Delay(delay, stoppingToken);
            }
        }


        public async Task ResetDailyChallenges()
        {
            var users = _db.User.ToList();
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            foreach (var user in users)
            {
             
                bool hasRecentExam = await _db.Exam
                .AnyAsync(e =>
                    e.UserId == user.UserId &&
                    e.XpCollected > 0 &&
                    (e.CreatedDate.Date == today || e.CreatedDate.Date == yesterday));

                if (!hasRecentExam && user.StreakScore > 0)
                {
                    if (user.FreezeStreak > 0)
                        user.FreezeStreak -= 1;
                    else
                        user.StreakScore = 0;
                }

                user.CompleteDailyChallenge = false;
                user.DateCompleteDailyChallenge = null;
                user.DailyXp = 0;
                user.NumQuestionSolToday = 0;
                user.GenerationPower = 5;
            }

            await _db.SaveChangesAsync();
        }


    }
}

    
