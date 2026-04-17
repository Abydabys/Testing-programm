namespace tt.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public ICollection<TestAttempt> TestAttempts { get; set; } = new List<TestAttempt>();
    }
}
