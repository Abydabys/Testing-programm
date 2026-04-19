using tt.Data;
using tt.Services;

namespace tt
{
    // Контейнер сервисов — создаёт и хранит все сервисы приложения
    // Используется чтобы не создавать сервисы каждый раз заново
    public class ServiceContainer
    {
        // Контекст базы данных — единственный на всё приложение
        public TestingDbContext DbContext { get; }

        // Сервис входа и регистрации пользователей
        public AuthenticationService AuthService { get; }

        // Сервис управления пользователями (CRUD операции)
        public UserService UserService { get; }

        // Сервис управления тестами
        public TestService TestService { get; }

        // Сервис управления вопросами и ответами
        public QuestionService QuestionService { get; }

        // Сервис прохождения тестов и подсчёта результатов
        public TestAttemptService TestAttemptService { get; }

        // При создании контейнера создаём все сервисы и передаём им контекст БД
        public ServiceContainer(TestingDbContext context)
        {
            DbContext = context;

            // Создаём каждый сервис, передавая ему контекст базы данных
            AuthService = new AuthenticationService(context);
            UserService = new UserService(context);
            TestService = new TestService(context);
            QuestionService = new QuestionService(context);
            TestAttemptService = new TestAttemptService(context);
        }
    }
}