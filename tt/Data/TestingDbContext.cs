using Microsoft.EntityFrameworkCore;
using tt.Models;

namespace tt.Data
{
    public class TestingDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<TestAttempt> TestAttempts { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // TODO: Check if the optionsBuilder is already configured using optionsBuilder.IsConfigured.
            // TODO: If not configured, call optionsBuilder.UseNpgsql(...) with the connection string.
            // TODO: The connection string should include Server, Port, Database, User Id, and Password fields.
            // TODO: Consider moving the connection string to a config file or environment variable instead of hardcoding it here.
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TODO: Call base.OnModelCreating(modelBuilder) first.

            // TODO: Configure the User entity:
            //   - Set Id as the primary key.
            //   - Make Username required with a max length of 100.
            //   - Make PasswordHash required.
            //   - Set FullName max length to 200.
            //   - Define a one-to-many relationship: User has many TestAttempts, foreign key is UserId, cascade delete.

            // TODO: Configure the Test entity:
            //   - Set Id as the primary key.
            //   - Make Title required with a max length of 500.
            //   - Set Description max length to 2000.
            //   - Define a one-to-many relationship: Test has many Questions, foreign key is TestId, cascade delete.
            //   - Define a one-to-many relationship: Test has many TestAttempts, foreign key is TestId, cascade delete.

            // TODO: Configure the Question entity:
            //   - Set Id as the primary key.
            //   - Make Text required.
            //   - Make Type required.
            //   - Define a one-to-many relationship: Question has many Answers, foreign key is QuestionId, cascade delete.

            // TODO: Configure the Answer entity:
            //   - Set Id as the primary key.
            //   - Make Text required.
            //   - Define a one-to-many relationship: Answer has many UserAnswers, foreign key is AnswerId, cascade delete.

            // TODO: Configure the TestAttempt entity:
            //   - Set Id as the primary key.
            //   - Define a one-to-many relationship: TestAttempt has many UserAnswers, foreign key is TestAttemptId, cascade delete.

            // TODO: Configure the UserAnswer entity:
            //   - Set Id as the primary key.
        }
    }
}
