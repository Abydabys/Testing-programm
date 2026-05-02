using Microsoft.EntityFrameworkCore;
using tt.Data;
using tt.Models;

namespace tt.Services
{
    public class ITestService
    {
        private readonly TestingDbContext _context;

        public ITestService(TestingDbContext context)
        {
            _context = context;
        }

        public async Task<Test> GetTestByIdAsync(int id)
        {
            return await _context.Tests
                .Include(t => t.Questions)
                    .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Test>> GetAllPublishedTestsAsync()
        {
            return await _context.Tests
                .Where(t => t.IsPublished)
                .ToListAsync();
        }

        public async Task<IEnumerable<Test>> GetAllTestsAsync()
        {
            return await _context.Tests
                .OrderByDescending(t => t.UpdatedAt)
                .ToListAsync();
        }

        public async Task<bool> CreateTestAsync(Test test)
        {
            test.CreatedAt = DateTime.UtcNow;
            test.UpdatedAt = DateTime.UtcNow;
            _context.Tests.Add(test);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTestAsync(Test test)
        {
            var existing = await _context.Tests.FindAsync(test.Id);
            if (existing == null) return false;

            existing.Title       = test.Title;
            existing.Description = test.Description;
            existing.MaxAttempts = test.MaxAttempts;
            existing.UpdatedAt   = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PublishTestAsync(int testId)
        {
            var test = await _context.Tests.FindAsync(testId);
            if (test == null) return false;

            test.IsPublished = !test.IsPublished;
            test.UpdatedAt   = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

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
