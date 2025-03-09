namespace Learntendo_backend.Dtos
{
    public class FilesDto
    {
        public IFormFile File { get; set; }
        public int UserId { get; set; }

        public List<string> UploadedFiles { get; set; } = new List<string>();

    }
}
