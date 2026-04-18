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
            // TODO: Store the dbContext parameter in the _dbContext field.
        }

        public async Task<Test> GetTestByIdAsync(int id)
        {
            // TODO: Query _dbContext.Tests filtered by Id == id.
            // TODO: Include the Questions navigation property.
            // TODO: Then include each Question's Answers navigation property.
            // TODO: Return the first match or null.
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Test>> GetAllPublishedTestsAsync()
        {
            // TODO: Query _dbContext.Tests filtered by IsPublished == true.
            // TODO: Order results by UpdatedAt descending (most recently updated first).
            // TODO: Return as a list.
            throw new NotImplementedException();
        }

        public async Task<bool> CreateTestAsync(Test test)
        {
            // TODO: Wrap in a try-catch block.
            // TODO: In the try block, set test.CreatedAt and test.UpdatedAt to DateTime.UtcNow.
            // TODO: Add the test to _dbContext.Tests.
            // TODO: Call SaveChangesAsync and return true.
            // TODO: In the catch block, return false.
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateTestAsync(Test test)
        {
            // TODO: Wrap in a try-catch block.
            // TODO: In the try block, set test.UpdatedAt to DateTime.UtcNow.
            // TODO: Call _dbContext.Tests.Update(test).
            // TODO: Call SaveChangesAsync and return true.
            // TODO: In the catch block, return false.
            throw new NotImplementedException();
        }

        public async Task<bool> PublishTestAsync(int testId)
        {
            // TODO: Wrap in a try-catch block.
            // TODO: In the try block, find the test using FindAsync(testId). If null, return false.
            // TODO: Set test.IsPublished to true.
            // TODO: Set test.UpdatedAt to DateTime.UtcNow.
            // TODO: Call SaveChangesAsync and return true.
            // TODO: In the catch block, return false.
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteTestAsync(int testId)
        {
            // TODO: Wrap in a try-catch block.
            // TODO: In the try block, find the test using FindAsync(testId). If null, return false.
            // TODO: Remove the test from _dbContext.Tests.
            // TODO: Call SaveChangesAsync and return true.
            // TODO: In the catch block, return false.
            throw new NotImplementedException();
        }
    }
}
