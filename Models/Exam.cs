using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Learntendo_backend.Models
{
    public class Exam
    {
        [Key]
        public int ExamId { get; set; }
        //NEW
        public required string ExamName { get; set; }

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
        public DateTime CreatedDate { get; set; }

        //we must to make the forign key nullable to avoid migrations problem
        [ForeignKey("FileId")]
        public int? FileId { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public Files? File { get; set; }
        public int UserId { get; set; }

        public int? SubjectId { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public Subject? Subject { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public User? User { get; set; }
    }
}
