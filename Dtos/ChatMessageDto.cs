using System.ComponentModel.DataAnnotations;

namespace Learntendo_backend.Dtos
{
    public class ChatMessageDto
    {
        [Required]
        public string SenderId { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
