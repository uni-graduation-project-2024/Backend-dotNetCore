using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Learntendo_backend.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public required string Username { get; set; }

        [Required]
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        public int TotalQuestion { get; set; } = 0;

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

        public DateTime? DateCompleteDailyChallenge { get; set; }

        public bool CompleteWeeklyChallenge { get; set; } = false;

        public DateTime? DateCompleteWeeklyChallenge { get; set; }

        public bool CompleteMonthlyChallenge { get; set; } = false;

        public DateTime? DateCompleteMonthlyChallenge { get; set; }



        [Required]
        public DateTime JoinedDate { get; set; }

        public bool? IfStreakActive { get; set; }

        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();

        public ICollection<Exam> Exams { get; set; } = new List<Exam>();

        


       

      
    }
}
