using tt.Models;
using tt.Services;
using tt.Shared;
using tt.Client.Network;

namespace tt.Client
{
    public class NetworkServiceContainer : IDisposable
    {
        private readonly TcpClientConnection _connection;

        public NetworkAuthenticationService  AuthenticationService  { get; }
        public NetworkTestService            TestService            { get; }
        public NetworkQuestionService        QuestionService        { get; }
        public NetworkTestAttemptService     TestAttemptService     { get; }
        public NetworkTestEditorService      TestEditorService      { get; }
        public NetworkQuestionEditorService  QuestionEditorService  { get; }

        private NetworkServiceContainer(string serverAddress, int serverPort)
        {
            _connection           = new TcpClientConnection(serverAddress, serverPort);
            AuthenticationService = new NetworkAuthenticationService(_connection);
            TestService           = new NetworkTestService(_connection);
            QuestionService       = new NetworkQuestionService(_connection);
            TestAttemptService    = new NetworkTestAttemptService(_connection);
            TestEditorService     = new NetworkTestEditorService(_connection);
            QuestionEditorService = new NetworkQuestionEditorService(_connection);
        }

        public static async Task<NetworkServiceContainer> CreateAsync(
            string serverAddress = "127.0.0.1", int serverPort = 9000)
        {
            var container = new NetworkServiceContainer(serverAddress, serverPort);
            await container._connection.ConnectAsync();
            return container;
        }

        public void Dispose() => _connection?.Dispose();
    }

    public class NetworkAuthenticationService
    {
        private readonly TcpClientConnection _c;
        public NetworkAuthenticationService(TcpClientConnection c) => _c = c;

        public async Task<User?> LoginAsync(string username, string password)
        {
            var req = new NetworkMessage(MessageType.Login, new { Username = username, Password = password });
            var res = await _c.SendAsync(req);
            return res.Success ? res.GetPayload<User>() : null;
        }

        public async Task<bool> RegisterAsync(string username, string password, string fullName)
        {
            var req = new NetworkMessage(MessageType.Register,
                new { Username = username, Password = password, FullName = fullName });
            var res = await _c.SendAsync(req);
            return res.Success;
        }
    }

    public class NetworkTestService
    {
        private readonly TcpClientConnection _c;
        public NetworkTestService(TcpClientConnection c) => _c = c;

        public async Task<IEnumerable<Test>> GetAllPublishedTestsAsync()
        {
            var req = new NetworkMessage(MessageType.GetAllPublishedTests, new { });
            var res = await _c.SendAsync(req);
            return res.Success ? res.GetPayload<List<Test>>() ?? new List<Test>() : new List<Test>();
        }
    }

    public class NetworkQuestionService
    {
        private readonly TcpClientConnection _c;
        public NetworkQuestionService(TcpClientConnection c) => _c = c;

        public async Task<IEnumerable<Question>> GetQuestionsByTestIdAsync(int testId)
        {
            var req = new NetworkMessage(MessageType.GetQuestionsByTestId, new { Id = testId });
            var res = await _c.SendAsync(req);
            return res.Success ? res.GetPayload<List<Question>>() ?? new List<Question>() : new List<Question>();
        }
    }

    public class NetworkTestAttemptService
    {
        private readonly TcpClientConnection _c;
        public NetworkTestAttemptService(TcpClientConnection c) => _c = c;

        public async Task<bool> CanUserAttemptTestAsync(int userId, int testId)
        {
            var req = new NetworkMessage(MessageType.CanUserAttemptTest, new { UserId = userId, TestId = testId });
            var res = await _c.SendAsync(req);
            return res.Success && res.GetPayload<bool>();
        }

        public async Task<TestAttempt?> StartTestAsync(int userId, int testId)
        {
            var req = new NetworkMessage(MessageType.StartTest, new { UserId = userId, TestId = testId });
            var res = await _c.SendAsync(req);
            if (!res.Success) { MessageBox.Show($"Server error: {res.ErrorMessage}"); return null; }
            return res.GetPayload<TestAttempt>();
        }

        public async Task<bool> SubmitAnswerAsync(int testAttemptId, int questionId, int answerId)
        {
            var req = new NetworkMessage(MessageType.SubmitAnswer,
                new { TestAttemptId = testAttemptId, QuestionId = questionId, AnswerId = answerId });
            var res = await _c.SendAsync(req);
            return res.Success;
        }

        public async Task<bool> SubmitMultipleAnswersAsync(int testAttemptId, int questionId, List<int> answerIds)
        {
            var req = new NetworkMessage(MessageType.SubmitMultipleAnswers,
                new { TestAttemptId = testAttemptId, QuestionId = questionId, AnswerIds = answerIds });
            var res = await _c.SendAsync(req);
            return res.Success;
        }

        public async Task<TestAttempt?> CompleteTestAsync(int testAttemptId)
        {
            var req = new NetworkMessage(MessageType.CompleteTest, new { Id = testAttemptId });
            var res = await _c.SendAsync(req);
            return res.Success ? res.GetPayload<TestAttempt>() : null;
        }
    }

    public class NetworkTestEditorService
    {
        private readonly TcpClientConnection _c;
        public NetworkTestEditorService(TcpClientConnection c) => _c = c;

        public async Task<List<Test>> GetAllTestsAsync()
        {
            var req = new NetworkMessage(MessageType.GetAllTests, new { });
            var res = await _c.SendAsync(req);
            return res.Success ? res.GetPayload<List<Test>>() ?? new List<Test>() : new List<Test>();
        }

        public async Task<Test?> GetTestByIdAsync(int testId)
        {
            var req = new NetworkMessage(MessageType.GetTestById, new { Id = testId });
            var res = await _c.SendAsync(req);
            return res.Success ? res.GetPayload<Test>() : null;
        }

        public async Task<Test?> CreateTestAsync(Test test)
        {
            var req = new NetworkMessage(MessageType.CreateTest, test);
            var res = await _c.SendAsync(req);
            return res.Success ? res.GetPayload<Test>() : null;
        }

        public async Task<bool> UpdateTestAsync(Test test)
        {
            var req = new NetworkMessage(MessageType.UpdateTest, test);
            var res = await _c.SendAsync(req);
            return res.Success;
        }

        public async Task<bool> DeleteTestAsync(int testId)
        {
            var req = new NetworkMessage(MessageType.DeleteTest, new { Id = testId });
            var res = await _c.SendAsync(req);
            return res.Success;
        }

        public async Task<bool> PublishTestAsync(int testId)
        {
            var req = new NetworkMessage(MessageType.PublishTest, new { Id = testId });
            var res = await _c.SendAsync(req);
            return res.Success;
        }
    }

    public class NetworkQuestionEditorService
    {
        private readonly TcpClientConnection _c;
        public NetworkQuestionEditorService(TcpClientConnection c) => _c = c;

        public async Task<List<Question>> GetQuestionsByTestIdAsync(int testId)
        {
            var req = new NetworkMessage(MessageType.GetQuestionsByTestId, new { Id = testId });
            var res = await _c.SendAsync(req);
            return res.Success ? res.GetPayload<List<Question>>() ?? new List<Question>() : new List<Question>();
        }

        public async Task<Question?> CreateQuestionAsync(Question question)
        {
            var req = new NetworkMessage(MessageType.CreateQuestion, question);
            var res = await _c.SendAsync(req);
            return res.Success ? res.GetPayload<Question>() : null;
        }

        public async Task<bool> UpdateQuestionAsync(Question question)
        {
            var req = new NetworkMessage(MessageType.UpdateQuestion, question);
            var res = await _c.SendAsync(req);
            return res.Success;
        }

        public async Task<bool> DeleteQuestionAsync(int questionId)
        {
            var req = new NetworkMessage(MessageType.DeleteQuestion, new { Id = questionId });
            var res = await _c.SendAsync(req);
            return res.Success;
        }

        public async Task<bool> UploadQuestionImageAsync(int questionId, byte[] imageData, string mimeType)
        {
            string base64 = Convert.ToBase64String(imageData);
            var req = new NetworkMessage(MessageType.UploadQuestionImage,
                new { QuestionId = questionId, ImageDataBase64 = base64, MimeType = mimeType });
            var res = await _c.SendAsync(req);
            return res.Success;
        }
    }
}
