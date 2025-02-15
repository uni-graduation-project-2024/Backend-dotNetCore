using Learntendo_backend.Data;
using Learntendo_backend.Models;

namespace Learntendo_backend.Services
{
    public class DailyResetService
    {
        private readonly DataContext _db;
        


        public DailyResetService(DataContext db)
        {
            _db = db;
        }



        public void ResetDailyChallenges()
        {
            var users = _db.User.ToList();

            foreach (var user in users)
            {
                user.CompleteDailyChallenge = false;
                user.DateCompleteDailyChallenge = null;
                user.DailyXp = 0;
                user.NumQuestionSolToday = 0;
            }

            _db.SaveChanges();
        }
    }
}

    
