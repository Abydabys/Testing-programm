using Microsoft.EntityFrameworkCore;
using tt.Data;
using tt.Models;

namespace tt.Services
{
    public interface IQuestionService
    {
        Task<Question> GetQuestionByIdAsync(int id);
        Task<IEnumerable<Question>> GetQuestionsByTestIdAsync(int testId);
        Task<bool> CreateQuestionAsync(Question question);
        Task<bool> UpdateQuestionAsync(Question question);
        Task<bool> DeleteQuestionAsync(int questionId);
        Task<bool> UploadQuestionImageAsync(int questionId, byte[] imageData, string mimeType);
    }

    public class QuestionService : IQuestionService
    {
        private readonly TestingDbContext _dbContext;

        public QuestionService(TestingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Question> GetQuestionByIdAsync(int id)
        {
            return await _dbContext.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<IEnumerable<Question>> GetQuestionsByTestIdAsync(int testId)
        {
            return await _dbContext.Questions
                .Where(q => q.TestId == testId)
                .Include(q => q.Answers)
                .OrderBy(q => q.Order)
                .ToListAsync();
        }

        public async Task<bool> CreateQuestionAsync(Question question)
        {
            try
            {
                _dbContext.Questions.Add(question);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateQuestionAsync(Question question)
        {
            try
            {
                _dbContext.Questions.Update(question);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteQuestionAsync(int questionId)
        {
            try
            {
                var question = await _dbContext.Questions.FindAsync(questionId);
                if (question == null)
                    return false;

                _dbContext.Questions.Remove(question);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UploadQuestionImageAsync(int questionId, byte[] imageData, string mimeType)
        {
            try
            {
                var question = await _dbContext.Questions.FindAsync(questionId);
                if (question == null)
                    return false;

                question.ImageData = imageData;
                question.ImageMimeType = mimeType;
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
