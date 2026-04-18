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
            // TODO: Store the dbContext parameter in the _dbContext field.
        }

        public async Task<Question> GetQuestionByIdAsync(int id)
        {
            // TODO: Query _dbContext.Questions and include its Answers navigation property.
            // TODO: Return the first question where Id matches the given id, or null if not found.
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Question>> GetQuestionsByTestIdAsync(int testId)
        {
            // TODO: Query _dbContext.Questions filtered by TestId == testId.
            // TODO: Include the Answers navigation property.
            // TODO: Order the results by the Order property ascending.
            // TODO: Return the result as a list.
            throw new NotImplementedException();
        }

        public async Task<bool> CreateQuestionAsync(Question question)
        {
            // TODO: Wrap in a try-catch block.
            // TODO: In the try block, add the question to _dbContext.Questions.
            // TODO: Call SaveChangesAsync to persist the changes.
            // TODO: Return true on success.
            // TODO: In the catch block, return false.
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateQuestionAsync(Question question)
        {
            // TODO: Wrap in a try-catch block.
            // TODO: In the try block, call _dbContext.Questions.Update(question).
            // TODO: Call SaveChangesAsync to persist the changes.
            // TODO: Return true on success.
            // TODO: In the catch block, return false.
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteQuestionAsync(int questionId)
        {
            // TODO: Wrap in a try-catch block.
            // TODO: In the try block, find the question using _dbContext.Questions.FindAsync(questionId).
            // TODO: If the question is null, return false.
            // TODO: Remove the question from _dbContext.Questions.
            // TODO: Call SaveChangesAsync to persist the deletion.
            // TODO: Return true on success.
            // TODO: In the catch block, return false.
            throw new NotImplementedException();
        }

        public async Task<bool> UploadQuestionImageAsync(int questionId, byte[] imageData, string mimeType)
        {
            // TODO: Wrap in a try-catch block.
            // TODO: In the try block, find the question using _dbContext.Questions.FindAsync(questionId).
            // TODO: If the question is null, return false.
            // TODO: Set question.ImageData to the imageData parameter.
            // TODO: Set question.ImageMimeType to the mimeType parameter.
            // TODO: Call SaveChangesAsync to persist the changes.
            // TODO: Return true on success.
            // TODO: In the catch block, return false.
            throw new NotImplementedException();
        }
    }
}
