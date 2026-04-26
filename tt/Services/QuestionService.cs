using Microsoft.EntityFrameworkCore;
using tt.Data;
using tt.Models;

namespace tt.Services
{
    public class IQuestionService
    {
        private readonly TestingDbContext _context;

        public IQuestionService(TestingDbContext context)
        {
            _context = context;
        }

        // Получить вопрос по ID вместе с вариантами ответов
        public async Task<Question> GetQuestionByIdAsync(int id)
        {
            return await _context.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        // Получить все вопросы теста, отсортированные по Order
        public async Task<IEnumerable<Question>> GetQuestionsByTestIdAsync(int testId)
        {
            return await _context.Questions
                .Include(q => q.Answers)
                .Where(q => q.TestId == testId)
                .OrderBy(q => q.Order)
                .ToListAsync();
        }

        // Добавить вопрос — после сохранения question.Id будет заполнен из БД
        public async Task<bool> CreateQuestionAsync(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return true;
        }

        // Обновить текст и порядок вопроса
        public async Task<bool> UpdateQuestionAsync(Question question)
        {
            var existing = await _context.Questions.FindAsync(question.Id);
            if (existing == null) return false;

            existing.Text = question.Text;
            existing.Order = question.Order;
            existing.Type = question.Type;
            existing.Weight = question.Weight;

            await _context.SaveChangesAsync();
            return true;
        }

        // Удалить вопрос (ответы удалятся каскадом через PostgreSQL)
        public async Task<bool> DeleteQuestionAsync(int questionId)
        {
            var question = await _context.Questions.FindAsync(questionId);
            if (question == null) return false;

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return true;
        }

        // Загрузить изображение для вопроса — сохраняем байты прямо в БД
        public async Task<bool> UploadQuestionImageAsync(int questionId, byte[] imageData, string mimeType)
        {
            var question = await _context.Questions.FindAsync(questionId);
            if (question == null) return false;

            question.ImageData = imageData;
            question.ImageMimeType = mimeType;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}