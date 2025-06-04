using iText.Layout.Properties.Grid;
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

    public class DailyResetService
    {
        private readonly DataContext _db;

        public DailyResetService(DataContext db)
        {
            _db = db;
        }

        public async Task ResetDailyChallenges()
        {
            var users = _db.User.ToList();

            foreach (var user in users)
            { 
                if (user.DailyXp<=0 && user.StreakScore > 0)

                {
                    if (user.FreezeStreak > 0)
                        user.FreezeStreak -= 1;
                    else
                    {
                        user.MaximunStreakScore = Math.Max(user.MaximunStreakScore, user.StreakScore);
                        user.StreakScore = 0;
                    }
                }

                user.CompleteDailyChallenge = false;
                user.DateCompleteDailyChallenge = null;
                user.DailyXp = 0;
                user.NumQuestionSolToday = 0;
                user.NumExamCreatedToday = 0;
                user.GenerationPower = 5;
                user.IfStreakActive = false;
            }

            await _db.SaveChangesAsync();
        }
    }
}
