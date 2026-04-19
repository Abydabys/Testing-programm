using Microsoft.EntityFrameworkCore;
using tt.Data;
using tt.Models;

namespace tt.Services
{
    // Сервис для управления попытками прохождения тестов
    // Отвечает за запуск теста, сохранение ответов и подсчёт результата
    public class TestAttemptService
    {
        private readonly TestingDbContext _context;

        public TestAttemptService(TestingDbContext context)
        {
            _context = context;
        }

        // Начать новую попытку прохождения теста
        // Создаёт запись в базе данных и возвращает её
        public async Task<TestAttempt> StartAttemptAsync(int userId, int testId)
        {
            var attempt = new TestAttempt
            {
                UserId = userId,
                TestId = testId,
                StartedAt = DateTime.UtcNow,
                IsCompleted = false
            };

            _context.TestAttempts.Add(attempt);
            await _context.SaveChangesAsync();
            return attempt;
        }

        // Сохранить ответ пользователя на один вопрос
        public async Task SaveUserAnswerAsync(int attemptId, int questionId, int answerId)
        {
            // Проверяем, не ответил ли пользователь на этот вопрос раньше
            var existing = await _context.UserAnswers
                .FirstOrDefaultAsync(ua => ua.AttemptId == attemptId && ua.QuestionId == questionId);

            if (existing != null)
            {
                // Если ответ уже был — обновляем его
                existing.AnswerId = answerId;
            }
            else
            {
                // Если ответа ещё не было — создаём новый
                var userAnswer = new UserAnswer
                {
                    AttemptId = attemptId,
                    QuestionId = questionId,
                    AnswerId = answerId
                };
                _context.UserAnswers.Add(userAnswer);
            }

            await _context.SaveChangesAsync();
        }

        // Завершить тест: подсчитать результат и сохранить
        // Возвращает количество правильных ответов
        public async Task<int> FinishAttemptAsync(int attemptId)
        {
            // Загружаем попытку вместе со всеми ответами пользователя
            var attempt = await _context.TestAttempts
                .Include(a => a.UserAnswers)
                    .ThenInclude(ua => ua.Answer) // Подгружаем данные каждого ответа
                .FirstOrDefaultAsync(a => a.Id == attemptId);

            if (attempt == null)
                return 0;

            // Считаем количество правильных ответов
            int correctCount = attempt.UserAnswers
                .Count(ua => ua.Answer != null && ua.Answer.IsCorrect);

            // Сохраняем результат и время окончания
            attempt.Score = correctCount;
            attempt.FinishedAt = DateTime.UtcNow;
            attempt.IsCompleted = true;

            await _context.SaveChangesAsync();
            return correctCount;
        }

        // Получить все попытки конкретного пользователя
        public async Task<List<TestAttempt>> GetUserAttemptsAsync(int userId)
        {
            return await _context.TestAttempts
                .Include(a => a.Test) // Подгружаем название теста
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.StartedAt) // Сначала последние попытки
                .ToListAsync();
        }

        // Получить результаты конкретной попытки со всеми ответами
        public async Task<TestAttempt?> GetAttemptResultAsync(int attemptId)
        {
            return await _context.TestAttempts
                .Include(a => a.Test)
                .Include(a => a.UserAnswers)
                    .ThenInclude(ua => ua.Question)
                .Include(a => a.UserAnswers)
                    .ThenInclude(ua => ua.Answer)
                .FirstOrDefaultAsync(a => a.Id == attemptId);
        }

        // Подсчитать процент правильных ответов
        // Например: 7 из 10 = 70%
        public async Task<double> GetScorePercentageAsync(int attemptId)
        {
            var attempt = await _context.TestAttempts
                .Include(a => a.Test)
                    .ThenInclude(t => t.Questions)
                .FirstOrDefaultAsync(a => a.Id == attemptId);

            if (attempt == null || attempt.Test.Questions.Count == 0)
                return 0;

            int totalQuestions = attempt.Test.Questions.Count;
            double percentage = (double)attempt.Score / totalQuestions * 100;

            // Округляем до двух знаков после запятой
            return Math.Round(percentage, 2);
        }

        // Получить лучший результат пользователя по конкретному тесту
        public async Task<int> GetBestScoreAsync(int userId, int testId)
        {
            var bestAttempt = await _context.TestAttempts
                .Where(a => a.UserId == userId && a.TestId == testId && a.IsCompleted)
                .OrderByDescending(a => a.Score)
                .FirstOrDefaultAsync();

            return bestAttempt?.Score ?? 0;
        }
    }
}
