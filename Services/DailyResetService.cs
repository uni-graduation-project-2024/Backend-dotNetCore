using Learntendo_backend.Data;
using Learntendo_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Learntendo_backend.Services
{
    public class DailyChallengeService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DailyChallengeService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                    var challengeService = new DailyResetService(dbContext);
                    await challengeService.ResetDailyChallenges();
                }

                // انتظار حتى منتصف الليل القادم
                var now = DateTime.UtcNow;
                var nextRun = now.Date.AddDays(1);
                var delay = nextRun - now;
                await Task.Delay(delay, stoppingToken);
            }
        }
    }

    public class DailyResetService
    {
        private readonly DataContext _db;

        public DailyResetService(DataContext db)
        {
            _db = db;
        }

        public async Task ResetDailyChallenges()
        {
            var users = await _db.User.ToListAsync();
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            foreach (var user in users)
            {
                bool hasRecentExam = await _db.Exam.AnyAsync(e =>
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
