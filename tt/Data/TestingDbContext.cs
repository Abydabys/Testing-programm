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
            // Connection string can be moved to config file or environment variables
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=testing_db;User Id=postgres;Password=password;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.FullName).HasMaxLength(200);
                entity.HasMany(e => e.TestAttempts).WithOne(e => e.User).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            // Test configuration
            modelBuilder.Entity<Test>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Description).HasMaxLength(2000);
                entity.HasMany(e => e.Questions).WithOne(e => e.Test).HasForeignKey(e => e.TestId).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.TestAttempts).WithOne(e => e.Test).HasForeignKey(e => e.TestId).OnDelete(DeleteBehavior.Cascade);
            });

            // Question configuration
            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Text).IsRequired();
                entity.Property(e => e.Type).IsRequired();
                entity.HasMany(e => e.Answers).WithOne(e => e.Question).HasForeignKey(e => e.QuestionId).OnDelete(DeleteBehavior.Cascade);
            });

            // Answer configuration
            modelBuilder.Entity<Answer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Text).IsRequired();
                entity.HasMany(e => e.UserAnswers).WithOne(e => e.Answer).HasForeignKey(e => e.AnswerId).OnDelete(DeleteBehavior.Cascade);
            });

            // TestAttempt configuration
            modelBuilder.Entity<TestAttempt>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasMany(e => e.UserAnswers).WithOne(e => e.TestAttempt).HasForeignKey(e => e.TestAttemptId).OnDelete(DeleteBehavior.Cascade);
            });

            // UserAnswer configuration
            modelBuilder.Entity<UserAnswer>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
        }
    }
}
