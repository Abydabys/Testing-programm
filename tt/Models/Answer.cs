namespace tt.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }  // Правильный ответ?
        public int Order { get; set; }       // Порядок вывода

        // Navigation properties
        public Question Question { get; set; }
        public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
    }
}
