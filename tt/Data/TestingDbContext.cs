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
            if (!optionsBuilder.IsConfigured)
            {
                // Ideally move this to appsettings.json or environment variables
                optionsBuilder.UseNpgsql(
                    "Host=localhost;Port=5434;Database=testing_db;Username=postgres;Password=1234");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USER
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Username)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.PasswordHash)
                      .IsRequired();

                entity.Property(u => u.FullName)
                      .HasMaxLength(200);

                entity.HasMany(u => u.TestAttempts)
                      .WithOne(ta => ta.User)
                      .HasForeignKey(ta => ta.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // TEST
            modelBuilder.Entity<Test>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Title)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(t => t.Description)
                      .HasMaxLength(2000);

                entity.HasMany(t => t.Questions)
                      .WithOne(q => q.Test)
                      .HasForeignKey(q => q.TestId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(t => t.TestAttempts)
                      .WithOne(ta => ta.Test)
                      .HasForeignKey(ta => ta.TestId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // QUESTION
            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(q => q.Id);

                entity.Property(q => q.Text)
                      .IsRequired();

                entity.Property(q => q.Type)
                      .IsRequired();

                entity.HasMany(q => q.Answers)
                      .WithOne(a => a.Question)
                      .HasForeignKey(a => a.QuestionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ANSWER
            modelBuilder.Entity<Answer>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.Text)
                      .IsRequired();

                entity.HasMany(a => a.UserAnswers)
                      .WithOne(ua => ua.Answer)
                      .HasForeignKey(ua => ua.AnswerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // TEST ATTEMPT
            modelBuilder.Entity<TestAttempt>(entity =>
            {
                entity.HasKey(ta => ta.Id);

                entity.HasMany(ta => ta.UserAnswers)
                      .WithOne(ua => ua.TestAttempt)
                      .HasForeignKey(ua => ua.TestAttemptId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // USER ANSWER
            modelBuilder.Entity<UserAnswer>(entity =>
            {
                entity.HasKey(ua => ua.Id);
            });
        }
    }
}