using Microsoft.EntityFrameworkCore;
using tt.Data;
using tt.Models;

namespace tt.Services
{
    public interface ITestService
    {
        Task<Test> GetTestByIdAsync(int id);
        Task<IEnumerable<Test>> GetAllPublishedTestsAsync();
        Task<bool> CreateTestAsync(Test test);
        Task<bool> UpdateTestAsync(Test test);
        Task<bool> PublishTestAsync(int testId);
        Task<bool> DeleteTestAsync(int testId);
    }

    public class TestService : ITestService
    {
        private readonly TestingDbContext _dbContext;

        public TestService(TestingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Test> GetTestByIdAsync(int id)
        {
            return await _dbContext.Tests
                .Include(t => t.Questions)
                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Test>> GetAllPublishedTestsAsync()
        {
            return await _dbContext.Tests
                .Where(t => t.IsPublished)
                .OrderByDescending(t => t.UpdatedAt)
                .ToListAsync();
        }

        public async Task<bool> CreateTestAsync(Test test)
        {
            try
            {
                test.CreatedAt = DateTime.UtcNow;
                test.UpdatedAt = DateTime.UtcNow;
                _dbContext.Tests.Add(test);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateTestAsync(Test test)
        {
            try
            {
                test.UpdatedAt = DateTime.UtcNow;
                _dbContext.Tests.Update(test);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> PublishTestAsync(int testId)
        {
            try
            {
                var test = await _dbContext.Tests.FindAsync(testId);
                if (test == null)
                    return false;

                test.IsPublished = true;
                test.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteTestAsync(int testId)
        {
            try
            {
                var test = await _dbContext.Tests.FindAsync(testId);
                if (test == null)
                    return false;

                _dbContext.Tests.Remove(test);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
