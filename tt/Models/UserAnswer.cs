namespace tt.Models
{
    public class UserAnswer
    {
        public int Id { get; set; }
        public int TestAttemptId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public DateTime AnsweredAt { get; set; }

        // Navigation properties
        public TestAttempt TestAttempt { get; set; }
        public Answer Answer { get; set; }
    }
}
