namespace tt.Models
{
    public enum QuestionType
    {
        SingleChoice = 1,      // Выбор одного варианта
        MultipleChoice = 2      // Выбор нескольких вариантов
    }

    public class Question
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public string Text { get; set; }
        public QuestionType Type { get; set; }
        public int Weight { get; set; }  // Вес вопроса
        public int Order { get; set; }   // Порядок в тесте
        public byte[] ImageData { get; set; }  // Картинка к вопросу
        public string ImageMimeType { get; set; }

        // Navigation properties
        public Test Test { get; set; }
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}
