namespace tt.Models
{
    public class TestAttempt
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TestId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int Score { get; set; }  // Number of points scored
        public int MaxScore { get; set; }  // Maximum possible score
        public double Percentage { get; set; }  // Percentage of correct answers
        public bool IsCompleted { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Test Test { get; set; }
        public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
    }
}
