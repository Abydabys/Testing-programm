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
            _dbContext = new TestingDbContext();

            UserService = new UserService(_dbContext);
            TestService = new TestService(_dbContext);
            QuestionService = new QuestionService(_dbContext);
            TestAttemptService = new TestAttemptService(_dbContext);
            AuthenticationService = new AuthenticationService(UserService);
        }
    }
}
