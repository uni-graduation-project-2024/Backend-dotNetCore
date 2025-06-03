using System.ComponentModel.DataAnnotations;

namespace Learntendo_backend.Dtos
{
    public class SubjectDto
    {
        [Required]
        public string SubjectName { get; set; }

        public string SubjectColor { get; set; }
    }
}
