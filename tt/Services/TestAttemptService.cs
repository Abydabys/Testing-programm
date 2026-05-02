using Microsoft.EntityFrameworkCore;
using tt.Data;
using tt.Models;

namespace tt.Services
{
    public class ITestAttemptService
    {
        private readonly TestingDbContext _context;

        public ITestAttemptService(TestingDbContext context)
        {
            _context = context;
        }

        public async Task<TestAttempt> StartTestAsync(int userId, int testId)
        {
            var existing = await _context.TestAttempts
                .FirstOrDefaultAsync(a => a.UserId == userId && a.TestId == testId && a.IsCompleted == false);
            if (existing != null) return existing;

            var attempt = new TestAttempt
            {
                UserId      = userId,
                TestId      = testId,
                StartedAt   = DateTime.UtcNow,
                IsCompleted = false,
                Score       = 0,
                MaxScore    = 0,
                Percentage  = 0
            };
            _context.TestAttempts.Add(attempt);
            await _context.SaveChangesAsync();
            return attempt;
        }

        public async Task<TestAttempt> GetTestAttemptByIdAsync(int id)
        {
            return await _context.TestAttempts
                .Include(a => a.Test)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<TestAttempt>> GetUserTestAttemptsAsync(int userId)
        {
            return await _context.TestAttempts
                .Include(a => a.Test)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.StartedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TestAttempt>> GetTestAttemptsForTestAsync(int testId)
        {
            return await _context.TestAttempts
                .Include(a => a.User)
                .Where(a => a.TestId == testId)
                .OrderByDescending(a => a.StartedAt)
                .ToListAsync();
        }

        public async Task<bool> SubmitAnswerAsync(int testAttemptId, int questionId, int answerId)
        {
            var existing = await _context.UserAnswers
                .FirstOrDefaultAsync(ua => ua.TestAttemptId == testAttemptId
                                        && ua.QuestionId == questionId);

            if (existing != null)
            {
                existing.AnswerId   = answerId;
                existing.AnsweredAt = DateTime.UtcNow;
            }
            else
            {
                _context.UserAnswers.Add(new UserAnswer
                {
                    TestAttemptId = testAttemptId,
                    QuestionId    = questionId,
                    AnswerId      = answerId,
                    AnsweredAt    = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SubmitMultipleAnswersAsync(int testAttemptId, int questionId, List<int> answerIds)
        {
            var old = _context.UserAnswers
                .Where(ua => ua.TestAttemptId == testAttemptId && ua.QuestionId == questionId);
            _context.UserAnswers.RemoveRange(old);

            foreach (var answerId in answerIds)
            {
                _context.UserAnswers.Add(new UserAnswer
                {
                    TestAttemptId = testAttemptId,
                    QuestionId    = questionId,
                    AnswerId      = answerId,
                    AnsweredAt    = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TestAttempt> CompleteTestAsync(int testAttemptId)
        {
            var attempt = await _context.TestAttempts
                .Include(a => a.Test)
                    .ThenInclude(t => t.Questions)
                .Include(a => a.UserAnswers)
                    .ThenInclude(ua => ua.Answer)
                .FirstOrDefaultAsync(a => a.Id == testAttemptId);

            if (attempt == null) return null;

            int score    = 0;
            int maxScore = 0;

            foreach (var question in attempt.Test.Questions)
            {
                maxScore += question.Weight;

                var userAnswer = attempt.UserAnswers
                    .FirstOrDefault(ua => ua.QuestionId == question.Id);

                if (userAnswer?.Answer != null && userAnswer.Answer.IsCorrect)
                    score += question.Weight;
            }

            attempt.Score       = score;
            attempt.MaxScore    = maxScore;
            attempt.Percentage  = maxScore > 0 ? Math.Round((double)score / maxScore * 100, 2) : 0;
            attempt.CompletedAt = DateTime.UtcNow;
            attempt.IsCompleted = true;

            await _context.SaveChangesAsync();
            return attempt;
        }

        public async Task<bool> CanUserAttemptTestAsync(int userId, int testId)
        {
            var test = await _context.Tests.FindAsync(testId);
            if (test == null) return false;

            if (test.MaxAttempts == 0) return true;

            int attemptCount = await _context.TestAttempts
                .CountAsync(a => a.UserId == userId
                              && a.TestId == testId
                              && a.IsCompleted == true);

            return attemptCount < test.MaxAttempts;
        }
    }
}
