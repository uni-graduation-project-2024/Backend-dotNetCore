using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Learntendo_backend.Models
{
    public class ChatbotMessage
    {
        [Key]
        public int ChatId { get; set; }
        public int UserId { get; set; }
        public string ChatName { get; set; }
       
        public DateTime ChatDateTime
        {
            get; set;
        }

        public ICollection<Message> Messages { get; set; }
    }
}
