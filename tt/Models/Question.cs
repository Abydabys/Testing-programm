namespace tt.Models
{
    public enum QuestionType
    {
        SingleChoice = 1,      // Single choice from multiple options
        MultipleChoice = 2      // Multiple choice options
    }

    public class Question
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public string Text { get; set; }
        public QuestionType Type { get; set; }
        public int Weight { get; set; }  // Question weight (points)
        public int Order { get; set; }   // Order in test
        public byte[] ImageData { get; set; }  // Image for question
        public string ImageMimeType { get; set; }

        // Navigation properties
        public Test Test { get; set; }
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}
