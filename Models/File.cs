using System.ComponentModel.DataAnnotations.Schema;

namespace Learntendo_backend.Models
{
    public class File
    {
        public int FileId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }

        [NotMapped]
        public IFormFile FileUp { get; set; }
    }
}
