using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Learntendo_backend.Models
{
    public class Exam
    {
        [Key]
        public int ExamId { get; set; }

        public string QuestionType { get; set; }

        public int NumQuestions { get; set; }

        public int? NumCorrectQuestions { get; set; }

        public int? NumWrongQuestions { get; set; }

        public string DifficultyLevel { get; set; }

        public string? McqQuestionsData { get; set; }

        public string? TfQuestionsData { get; set; }

        public TimeOnly? TimeTaken { get; set; }

        public int? TotalScore { get; set; }

        public int XpCollected { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int UserId { get; set; }

        public int SubjectId { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public Subject? Subject { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public User? User { get; set; }
    }
}
