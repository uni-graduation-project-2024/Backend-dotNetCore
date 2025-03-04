namespace Learntendo_backend.Dtos
{
    public class ViewExamDto
    {
        public int ExamId { get; set; }
        public required string ExamName { get; set; }
        public int NumQuestions { get; set; }
        public string DifficultyLevel { get; set; }
        public int? TotalScore { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? SubjectId { get; set; }
    }
}
