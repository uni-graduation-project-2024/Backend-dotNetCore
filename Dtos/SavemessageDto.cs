namespace Learntendo_backend.Dtos
{
    public class SavemessageDto
    {
        public int ChatId { get; set; }
        public int UserId { get; set; }
        public string UserMessages { get; set; }
        public string AiResponse { get; set; }

        public DateTime ChatDateTime { get; set; }
    }
}
