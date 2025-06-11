using System.ComponentModel.DataAnnotations;

namespace Learntendo_backend.Dtos
{
    public class ChatMessageDto
    {
        
        public string SenderId { get; set; }

        
        public string ReceiverId { get; set; }

        
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }
}
