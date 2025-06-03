using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Learntendo_backend.Models
{
    public class Files
    {
        [Key]
        public int FileId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public int? SubjectId { get; set; }

        public int? NumOfExams { get; set; }
        public DateTime CreatedDate { get; set; }
        [ForeignKey("UserId")]
        public int? UserId { get; set; }

        public User? User { get; set; }
        [NotMapped]
        public IFormFile FileUp { get; set; }
    }
}
