using Microsoft.EntityFrameworkCore;
using tt.Data;
using tt.Models;

namespace tt.Services
{
    public class TestService : ITestService
    {
        private readonly TestingDbContext _context;

        public TestService(TestingDbContext context)
        {
            _context = context;
        }

        // Получить тест по ID вместе со всеми вопросами и ответами
        public async Task<Test> GetTestByIdAsync(int id)
        {
            return await _context.Tests
                .Include(t => t.Questions)
                    .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        // Получить все опубликованные тесты (видны студентам)
        public async Task<IEnumerable<Test>> GetAllPublishedTestsAsync()
        {
            return await _context.Tests
                .Where(t => t.IsPublished)
                .ToListAsync();
        }

        // Создать новый тест — после сохранения test.Id будет заполнен из БД
        public async Task<bool> CreateTestAsync(Test test)
        {
            test.CreatedAt = DateTime.UtcNow;
            test.UpdatedAt = DateTime.UtcNow;
            _context.Tests.Add(test);
            await _context.SaveChangesAsync();
            return true;
        }

        // Обновить данные теста
        public async Task<bool> UpdateTestAsync(Test test)
        {
            var existing = await _context.Tests.FindAsync(test.Id);
            if (existing == null) return false;

            existing.Title = test.Title;
            existing.Description = test.Description;
            existing.MaxAttempts = test.MaxAttempts;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        // Опубликовать тест — сделать доступным для студентов
        public async Task<bool> PublishTestAsync(int testId)
        {
            var test = await _context.Tests.FindAsync(testId);
            if (test == null) return false;

            test.IsPublished = true;
            test.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // Удалить тест
        public async Task<bool> DeleteTestAsync(int testId)
        {
            var test = await _context.Tests.FindAsync(testId);
            if (test == null) return false;

            _context.Tests.Remove(test);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}