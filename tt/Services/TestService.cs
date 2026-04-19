using Microsoft.EntityFrameworkCore;
using tt.Data;
using tt.Models;

namespace tt.Services
{
    // Сервис для работы с тестами: получение, создание, удаление
    public class TestService
    {
        private readonly TestingDbContext _context;

        public TestService(TestingDbContext context)
        {
            _context = context;
        }

        // Получить все доступные тесты
        public async Task<List<Test>> GetAllTestsAsync()
        {
            return await _context.Tests.ToListAsync();
        }

        // Получить один тест по ID, вместе со всеми его вопросами и ответами
        public async Task<Test?> GetTestWithQuestionsAsync(int testId)
        {
            return await _context.Tests
                .Include(t => t.Questions)       // Подгружаем вопросы теста
                    .ThenInclude(q => q.Answers) // Подгружаем варианты ответов к каждому вопросу
                .FirstOrDefaultAsync(t => t.Id == testId);
        }

        // Создать новый тест
        public async Task<Test> CreateTestAsync(string title, string description, int timeLimitMinutes = 30)
        {
            var test = new Test
            {
                Title = title,
                Description = description,
                TimeLimitMinutes = timeLimitMinutes,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tests.Add(test);
            await _context.SaveChangesAsync();
            return test;
        }

        // Обновить данные теста (название, описание, время)
        public async Task<bool> UpdateTestAsync(Test test)
        {
            var existing = await _context.Tests.FindAsync(test.Id);
            if (existing == null)
                return false;

            existing.Title = test.Title;
            existing.Description = test.Description;
            existing.TimeLimitMinutes = test.TimeLimitMinutes;

            await _context.SaveChangesAsync();
            return true;
        }

        // Удалить тест и все связанные с ним данные
        public async Task<bool> DeleteTestAsync(int testId)
        {
            var test = await _context.Tests.FindAsync(testId);
            if (test == null)
                return false;

            _context.Tests.Remove(test);
            await _context.SaveChangesAsync();
            return true;
        }

        // Получить количество вопросов в тесте
        public async Task<int> GetQuestionCountAsync(int testId)
        {
            return await _context.Questions
                .CountAsync(q => q.TestId == testId);
        }

        // Поиск тестов по названию (частичное совпадение)
        public async Task<List<Test>> SearchTestsAsync(string searchTerm)
        {
            return await _context.Tests
                .Where(t => t.Title.Contains(searchTerm))
                .ToListAsync();
        }
    }
}
