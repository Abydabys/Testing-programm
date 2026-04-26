using Microsoft.EntityFrameworkCore;
using tt.Data;
using tt.Models;
using tt.Services;

namespace tt.Client
{
    // Тот же класс что и раньше, но теперь работает напрямую с PostgreSQL
    // Формы менять не нужно — интерфейс полностью совпадает
    public class NetworkServiceContainer : IDisposable
    {
        private readonly TestingDbContext _context;

        public IAuthenticationService AuthenticationService { get; }
        public ITestService TestService { get; }
        public IQuestionService QuestionService { get; }
        public ITestAttemptService TestAttemptService { get; }
        public UserService UserService { get; }

        // Параметры оставлены чтобы не менять вызовы в формах, но они больше не используются
        public NetworkServiceContainer(string serverAddress = "127.0.0.1", int serverPort = 9000)
        {
            _context = new TestingDbContext();

            AuthenticationService = new AuthenticationService(_context);
            TestService = new TestService(_context);
            QuestionService = new QuestionService(_context);
            TestAttemptService = new TestAttemptService(_context);
            UserService = new UserService(_context);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}