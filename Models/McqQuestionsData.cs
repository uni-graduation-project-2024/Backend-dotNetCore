using Newtonsoft.Json;

namespace Learntendo_backend.Models
{
    public class McqQuestionsData
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public List<string> Options { get; set; }
        [JsonProperty("correct_answer")] 
        public string CorrectAnswer { get; set; }

        [JsonProperty("user_answer")] 
        public string UserAnswer { get; set; }

        [JsonProperty("is_correct")]  
        public bool IsCorrect { get; set; }
    }
}
