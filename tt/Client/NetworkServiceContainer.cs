using tt.Models;
using tt.Services;
using tt.Shared;
using tt.Client.Network;

namespace tt.Client
{
    public class NetworkServiceContainer : IDisposable
    {
        private readonly TcpClientConnection _connection;
        public NetworkAuthenticationService AuthenticationService { get; }
        public NetworkTestService TestService { get; }
        public NetworkQuestionService QuestionService { get; }
        public NetworkTestAttemptService TestAttemptService { get; }

        private NetworkServiceContainer(string serverAddress, int serverPort)
        {
            _connection = new TcpClientConnection(serverAddress, serverPort);
            AuthenticationService = new NetworkAuthenticationService(_connection);
            TestService = new NetworkTestService(_connection);
            QuestionService = new NetworkQuestionService(_connection);
            TestAttemptService = new NetworkTestAttemptService(_connection);
        }

        public static async Task<NetworkServiceContainer> CreateAsync(
            string serverAddress = "127.0.0.1", int serverPort = 9000)
        {
            var container = new NetworkServiceContainer(serverAddress, serverPort);
            await container._connection.ConnectAsync();
            return container;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }

    public class NetworkAuthenticationService
    {
        private readonly TcpClientConnection _connection;
        public NetworkAuthenticationService(TcpClientConnection connection) => _connection = connection;

        public async Task<User?> LoginAsync(string username, string password)
        {
            var request = new NetworkMessage(MessageType.Login, new { Username = username, Password = password });
            var response = await _connection.SendAsync(request);
            return response.Success ? response.GetPayload<User>() : null;
        }

        public async Task<bool> RegisterAsync(string username, string password, string fullName)
        {
            var request = new NetworkMessage(MessageType.Register, new { Username = username, Password = password, FullName = fullName });
            var response = await _connection.SendAsync(request);
            return response.Success;
        }
    }

    public class NetworkTestService
    {
        private readonly TcpClientConnection _connection;
        public NetworkTestService(TcpClientConnection connection) => _connection = connection;

        public async Task<IEnumerable<Test>> GetAllPublishedTestsAsync()
        {
            var request = new NetworkMessage(MessageType.GetAllPublishedTests, new { });
            var response = await _connection.SendAsync(request);
            return response.Success
                ? response.GetPayload<List<Test>>() ?? new List<Test>()
                : new List<Test>();
        }
    }

    public class NetworkQuestionService
    {
        private readonly TcpClientConnection _connection;
        public NetworkQuestionService(TcpClientConnection connection) => _connection = connection;

        public async Task<IEnumerable<Question>> GetQuestionsByTestIdAsync(int testId)
        {
            var request = new NetworkMessage(MessageType.GetQuestionsByTestId, new { Id = testId });
            var response = await _connection.SendAsync(request);
            return response.Success
                ? response.GetPayload<List<Question>>() ?? new List<Question>()
                : new List<Question>();
        }
    }

    public class NetworkTestAttemptService
    {
        private readonly TcpClientConnection _connection;
        public NetworkTestAttemptService(TcpClientConnection connection) => _connection = connection;

        public async Task<bool> CanUserAttemptTestAsync(int userId, int testId)
        {
            var request = new NetworkMessage(MessageType.CanUserAttemptTest, new { UserId = userId, TestId = testId });
            var response = await _connection.SendAsync(request);
            return response.Success && response.GetPayload<bool>();
        }

        public async Task<TestAttempt?> StartTestAsync(int userId, int testId)
        {
            var request = new NetworkMessage(MessageType.StartTest, new { UserId = userId, TestId = testId });
            var response = await _connection.SendAsync(request);
            if (!response.Success)
            {
                MessageBox.Show($"Server error: {response.ErrorMessage}");
                return null;
            }
            return response.GetPayload<TestAttempt>();
        }

        public async Task<bool> SubmitAnswerAsync(int testAttemptId, int questionId, int answerId)
        {
            var request = new NetworkMessage(
                MessageType.SubmitAnswer,
                new { TestAttemptId = testAttemptId, QuestionId = questionId, AnswerId = answerId });
            var response = await _connection.SendAsync(request);
            return response.Success;
        }

        public async Task<bool> SubmitMultipleAnswersAsync(int testAttemptId, int questionId, List<int> answerIds)
        {
            var request = new NetworkMessage(
                MessageType.SubmitMultipleAnswers,
                new { TestAttemptId = testAttemptId, QuestionId = questionId, AnswerIds = answerIds });
            var response = await _connection.SendAsync(request);
            return response.Success;
        }

        public async Task<TestAttempt?> CompleteTestAsync(int testAttemptId)
        {
            var request = new NetworkMessage(MessageType.CompleteTest, new { Id = testAttemptId });
            var response = await _connection.SendAsync(request);
            return response.Success ? response.GetPayload<TestAttempt>() : null;
        }
    }
}