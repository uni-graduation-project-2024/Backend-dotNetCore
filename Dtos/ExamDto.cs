namespace Learntendo_backend.Dtos
{
    namespace Learntendo_backend.DTOs
    {
        public class ExamDto
        {
            public string? QuestionType { get; set; }
            
            public string? ExamName { get; set; }
            public int? NumQuestions { get; set; }
            public int? NumCorrectQuestions { get; set; }
            public int? NumWrongQuestions { get; set; }
            public string? DifficultyLevel { get; set; }

       
            public List<McqQuestionDto>? McqQuestionsData { get; set; }

     
            public List<TfQuestionDto>? TfQuestionsData { get; set; }

            public TimeOnly? TimeTaken { get; set; }
            public int? TotalScore { get; set; }
            public int XpCollected { get; set; }
            public int? UserId { get; set; }
            public int? SubjectId { get; set; }
        }

        public class McqQuestionDto
        {
            public int Id { get; set; }
            public string Text { get; set; }
            public List<string> Options { get; set; }
            public string CorrectAnswer { get; set; }
            public string UserAnswer { get; set; }
            public bool IsCorrect { get; set; }
            public string Explain { get; set; }
        }

        public class TfQuestionDto
        {
            public int Id { get; set; }
            public string Text { get; set; }
            public bool CorrectAnswer { get; set; }
            public bool UserAnswer { get; set; }
            public bool IsCorrect { get; set; }
            public string Explain { get; set; }
        }
    }

}
