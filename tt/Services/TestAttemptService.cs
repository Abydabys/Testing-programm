using Microsoft.EntityFrameworkCore;
using tt.Data;
using tt.Models;

namespace tt.Services
{
    public interface ITestAttemptService
    {
        Task<TestAttempt> StartTestAsync(int userId, int testId);
        Task<TestAttempt> GetTestAttemptByIdAsync(int id);
        Task<IEnumerable<TestAttempt>> GetUserTestAttemptsAsync(int userId);
        Task<IEnumerable<TestAttempt>> GetTestAttemptsForTestAsync(int testId);
        Task<bool> SubmitAnswerAsync(int testAttemptId, int questionId, int answerId);
        Task<bool> SubmitMultipleAnswersAsync(int testAttemptId, int questionId, List<int> answerIds);
        Task<TestAttempt> CompleteTestAsync(int testAttemptId);
        Task<bool> CanUserAttemptTestAsync(int userId, int testId);
    }

    public class TestAttemptService : ITestAttemptService
    {
        private readonly TestingDbContext _dbContext;

        public TestAttemptService(TestingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TestAttempt> StartTestAsync(int userId, int testId)
        {
            try
            {
                var test = await _dbContext.Tests
                    .Include(t => t.Questions)
                    .FirstOrDefaultAsync(t => t.Id == testId);

                if (test == null)
                    return null;

                // Вычисляем максимальный балл
                int maxScore = test.Questions.Sum(q => q.Weight);

                var testAttempt = new TestAttempt
                {
                    UserId = userId,
                    TestId = testId,
                    StartedAt = DateTime.UtcNow,
                    Score = 0,
                    MaxScore = maxScore,
                    IsCompleted = false
                };

                _dbContext.TestAttempts.Add(testAttempt);
                await _dbContext.SaveChangesAsync();

                return testAttempt;
            }
            catch
            {
                return null;
            }
        }

        public async Task<TestAttempt> GetTestAttemptByIdAsync(int id)
        {
            return await _dbContext.TestAttempts
                .Include(ta => ta.User)
                .Include(ta => ta.Test)
                .Include(ta => ta.UserAnswers)
                .FirstOrDefaultAsync(ta => ta.Id == id);
        }

        public async Task<IEnumerable<TestAttempt>> GetUserTestAttemptsAsync(int userId)
        {
            return await _dbContext.TestAttempts
                .Where(ta => ta.UserId == userId)
                .Include(ta => ta.Test)
                .OrderByDescending(ta => ta.StartedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TestAttempt>> GetTestAttemptsForTestAsync(int testId)
        {
            return await _dbContext.TestAttempts
                .Where(ta => ta.TestId == testId)
                .Include(ta => ta.User)
                .OrderByDescending(ta => ta.CompletedAt)
                .ToListAsync();
        }

        public async Task<bool> SubmitAnswerAsync(int testAttemptId, int questionId, int answerId)
        {
            try
            {
                var userAnswer = new UserAnswer
                {
                    TestAttemptId = testAttemptId,
                    QuestionId = questionId,
                    AnswerId = answerId,
                    AnsweredAt = DateTime.UtcNow
                };

                _dbContext.UserAnswers.Add(userAnswer);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SubmitMultipleAnswersAsync(int testAttemptId, int questionId, List<int> answerIds)
        {
            try
            {
                foreach (var answerId in answerIds)
                {
                    var userAnswer = new UserAnswer
                    {
                        TestAttemptId = testAttemptId,
                        QuestionId = questionId,
                        AnswerId = answerId,
                        AnsweredAt = DateTime.UtcNow
                    };
                    _dbContext.UserAnswers.Add(userAnswer);
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<TestAttempt> CompleteTestAsync(int testAttemptId)
        {
            try
            {
                var testAttempt = await _dbContext.TestAttempts
                    .Include(ta => ta.UserAnswers)
                    .ThenInclude(ua => ua.Answer)
                    .Include(ta => ta.Test)
                    .ThenInclude(t => t.Questions)
                    .FirstOrDefaultAsync(ta => ta.Id == testAttemptId);

                if (testAttempt == null)
                    return null;

                // Вычисляем количество правильных ответов
                var userAnswersByQuestion = testAttempt.UserAnswers.GroupBy(ua => ua.Answer.QuestionId);

                int score = 0;
                foreach (var questionAnswers in userAnswersByQuestion)
                {
                    var question = testAttempt.Test.Questions.FirstOrDefault(q => q.Id == questionAnswers.Key);
                    if (question == null) continue;

                    var correctAnswerIds = _dbContext.Answers
                        .Where(a => a.QuestionId == questionAnswers.Key && a.IsCorrect)
                        .Select(a => a.Id)
                        .ToList();

                    var userAnswerIds = questionAnswers.Select(ua => ua.AnswerId).ToList();

                    // Для вопросов с одним ответом
                    if (question.Type == QuestionType.SingleChoice)
                    {
                        if (userAnswerIds.Count == 1 && userAnswerIds[0] == correctAnswerIds[0])
                        {
                            score += question.Weight;
                        }
                    }
                    // Для вопросов с несколькими ответами
                    else if (question.Type == QuestionType.MultipleChoice)
                    {
                        if (userAnswerIds.Count == correctAnswerIds.Count &&
                            userAnswerIds.OrderBy(a => a).SequenceEqual(correctAnswerIds.OrderBy(a => a)))
                        {
                            score += question.Weight;
                        }
                    }
                }

                testAttempt.Score = score;
                testAttempt.Percentage = (double)score / testAttempt.MaxScore * 100;
                testAttempt.CompletedAt = DateTime.UtcNow;
                testAttempt.IsCompleted = true;

                await _dbContext.SaveChangesAsync();

                return testAttempt;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CanUserAttemptTestAsync(int userId, int testId)
        {
            var test = await _dbContext.Tests.FindAsync(testId);
            if (test == null)
                return false;

            var attempts = await _dbContext.TestAttempts
                .Where(ta => ta.UserId == userId && ta.TestId == testId)
                .CountAsync();

            return attempts < test.MaxAttempts;
        }
    }
}
