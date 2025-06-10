using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
namespace Learntendo_backend.Dtos
{
    public class UserDto
    {
        public int UserId { get; set; }

        [Required]
        public required string Username { get; set; }

        public int TotalQuestion { get; set; } = 0;

        public int NumFilesUploadedToday { get; set; } = 0;

        public int NumQuestionSolToday { get; set; } = 0;

        public int Coins { get; set; } = 0;

        public int StreakScore { get; set; } = 0;

        public int FreezeStreak { get; set; } = 5;

        public int TotalXp { get; set; } = 0;

        public int DailyXp { get; set; } = 0;

        public int WeeklyXp { get; set; } = 0;

        public int MonthlyXp { get; set; } = 0;

        public string? CurrentLeague { get; set; } = null!;

        public bool CompleteDailyChallenge { get; set; } = false;

        public bool CompleteWeeklyChallenge { get; set; } = false;

        public bool CompleteMonthlyChallenge { get; set; } = false;
        public Dictionary<string, string> LeagueHistory { get; set; } = new Dictionary<string, string>();

        public string ProfilePicture { get; set; }


    }
}
