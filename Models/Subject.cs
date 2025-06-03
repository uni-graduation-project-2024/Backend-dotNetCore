using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Learntendo_backend.Models
{
    public class Subject
    {
        [Key]
        public int? SubjectId { get; set; }

        [Required]
        public required string SubjectName { get; set; }

        public required string SubjectColor { get; set; }

        public int NumExams { get; set; } = 0;

        public int TotalQuestions { get; set; } = 0;
        [ForeignKey(nameof(User))]
        public required int UserId { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();

        [Required]
        [JsonIgnore]
        [IgnoreDataMember]
        public required User User { get; set; }
    }
}
