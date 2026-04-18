using tt.Data;
using tt.Models;
using tt.Services;

namespace tt
{
    public class ServiceContainer
    {
        private readonly TestingDbContext _dbContext;

        public IUserService UserService { get; }
        public IAuthenticationService AuthenticationService { get; }
        public ITestService TestService { get; }
        public IQuestionService QuestionService { get; }
        public ITestAttemptService TestAttemptService { get; }

        public ServiceContainer()
        {
            // TODO: Create a new TestingDbContext instance and store it in _dbContext.
            // TODO: Create a new UserService, passing _dbContext, and assign it to UserService.
            // TODO: Create a new TestService, passing _dbContext, and assign it to TestService.
            // TODO: Create a new QuestionService, passing _dbContext, and assign it to QuestionService.
            // TODO: Create a new TestAttemptService, passing _dbContext, and assign it to TestAttemptService.
            // TODO: Create a new AuthenticationService, passing UserService, and assign it to AuthenticationService.
        }
    }
}
