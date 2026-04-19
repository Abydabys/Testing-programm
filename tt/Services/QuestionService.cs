using Microsoft.EntityFrameworkCore;
using tt.Data;
using tt.Models;

namespace tt.Services
{
    // Сервис для управления вопросами и вариантами ответов
    public class QuestionService
    {
        private readonly TestingDbContext _context;

        public QuestionService(TestingDbContext context)
        {
            _context = context;
        }

        // Получить все вопросы конкретного теста (вместе с ответами)
        public async Task<List<Question>> GetQuestionsByTestIdAsync(int testId)
        {
            return await _context.Questions
                .Include(q => q.Answers) // Подгружаем варианты ответов к каждому вопросу
                .Where(q => q.TestId == testId)
                .OrderBy(q => q.OrderIndex) // Сортируем по порядку
                .ToListAsync();
        }

        // Получить один вопрос по ID вместе с его ответами
        public async Task<Question?> GetQuestionWithAnswersAsync(int questionId)
        {
            return await _context.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == questionId);
        }

        // Добавить новый вопрос к тесту
        public async Task<Question> AddQuestionAsync(int testId, string text, int orderIndex = 0)
        {
            var question = new Question
            {
                TestId = testId,
                Text = text,
                OrderIndex = orderIndex
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        // Добавить вариант ответа к вопросу
        // isCorrect = true означает, что этот вариант — правильный
        public async Task<Answer> AddAnswerAsync(int questionId, string text, bool isCorrect)
        {
            var answer = new Answer
            {
                QuestionId = questionId,
                Text = text,
                IsCorrect = isCorrect
            };

            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();
            return answer;
        }

        // Обновить текст вопроса
        public async Task<bool> UpdateQuestionAsync(Question question)
        {
            var existing = await _context.Questions.FindAsync(question.Id);
            if (existing == null)
                return false;

            existing.Text = question.Text;
            existing.OrderIndex = question.OrderIndex;

            await _context.SaveChangesAsync();
            return true;
        }

        // Удалить вопрос (вместе со всеми его ответами — это делает БД автоматически)
        public async Task<bool> DeleteQuestionAsync(int questionId)
        {
            var question = await _context.Questions.FindAsync(questionId);
            if (question == null)
                return false;

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return true;
        }

        // Удалить конкретный вариант ответа
        public async Task<bool> DeleteAnswerAsync(int answerId)
        {
            var answer = await _context.Answers.FindAsync(answerId);
            if (answer == null)
                return false;

            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();
            return true;
        }

        // Получить правильный ответ для вопроса
        public async Task<Answer?> GetCorrectAnswerAsync(int questionId)
        {
            return await _context.Answers
                .FirstOrDefaultAsync(a => a.QuestionId == questionId && a.IsCorrect);
        }
    }
}
