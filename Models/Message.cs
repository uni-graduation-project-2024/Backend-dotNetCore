using Learntendo_backend.Migrations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learntendo_backend.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }  

        public int ChatId { get; set; }     

        public string UserMessage { get; set; }

        public string AiResponse { get; set; }

        public DateTime ChatDateTime { get; set; }

        
        [ForeignKey("ChatId")]
        public ChatbotMessage ChatbotMessage { get; set; }

    }
}
