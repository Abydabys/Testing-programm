using tt.Client.Network;
using tt.Models;
using tt.Services;
using tt.Shared;

namespace tt.Client
{
    public class NetworkServiceContainer : IDisposable
    {

        private readonly TcpClientConnection _connection;


        public IAuthenticationService AuthenticationService { get; }
        public ITestService TestService { get; }
        public IQuestionService QuestionService { get; }
        public ITestAttemptService TestAttemptService { get; }

        //Constructor

        public NetworkServiceContainer(string serverAddress = "127.0.0.1", int serverPort = 9000)
        {
            _connection = new TcpClientConnection(serverAddress, serverPort);
            _connection.ConnectAsync().GetAwaiter().GetResult();
            AuthenticationService = new NetworkAuthenticationService(_connection);
            TestService = new NetworkTestService(_connection);
            QuestionService = new NetworkQuestionService(_connection);
            TestAttemptService = new NetworkTestAttemptService(_connection);
        }

        //IDisposable

        public void Dispose()
        {
            _connection.Dispose();
        }
    }

    public class NetworkAuthenticationService : IAuthenticationService
    {
        private readonly TcpClientConnection _connection;

        public NetworkAuthenticationService(TcpClientConnection connection)
        {
            _connection = connection;
        }

        public async Task<User> LoginAsync(string username, string password)
        {
            var request = new NetworkMessage(
            MessageType.Login,
            new { Username = username, Password = password });

            var response = await _connection.SendAsync(request);

            if (!response.Success) return null;

            return response.GetPayload<User>();
        }

        public async Task<bool> RegisterAsync(string username, string password, string fullName)
        {
            var request = new NetworkMessage(
                MessageType.Register,
                new {Username = username, Password = password, FullName = fullName});
                var response = await _connection.SendAsync(request);
            return response.Success;
        }

        public async Task<bool> ValidateUserAsync(User user)
        {
            var request = new NetworkMessage(MessageType.ValidateUser, user);
            var response = await _connection.SendAsync(request);

            return response.GetPayload<bool>();
        }
    }

    //Tests

    public class NetworkTestService : ITestService
    {
        private readonly TcpClientConnection _connection;

        public NetworkTestService(TcpClientConnection connection)
        {
            _connection=connection;
        }

        public async Task<Test> GetTestByIdAsync(int id)
        {
            var responce = await _connection.SendIdRequestAsync(MessageType.GetTestById, id);
            if (!responce.Success)
            {
                return null;
            }
            else
            {
                return responce.GetPayload<Test>();
            }
        }

        public async Task<IEnumerable<Test>> GetAllPublishedTestsAsync()
        {
            var request = new NetworkMessage(MessageType.GetAllPublishedTests, new { });
            var response = await _connection.SendAsync(request);

            return response.GetPayload<List<Test>>();
        }

        public async Task<bool> CreateTestAsync(Test test)
        {
            var request = new NetworkMessage(MessageType.CreateTest,
                test);
            var response = await _connection.SendAsync(request);
            if (response.Success)
            {
                var serverTest = response.GetPayload<Test>();
                test.Id = serverTest.Id;
            }
            return response.Success;
        }

        public async Task<bool> UpdateTestAsync(Test test)
        {
            var request = new NetworkMessage(MessageType.UpdateTest, test);
            var response = await _connection.SendAsync(request);

            return response.Success;
        }

        public async Task<bool> PublishTestAsync(int testId)
        {
            var response = await _connection.SendAsync(new NetworkMessage(MessageType.PublishTest, testId));
            return response.Success;
        }

        public async Task<bool> DeleteTestAsync(int testId)
        {
            var response = await _connection.SendIdRequestAsync(MessageType.DeleteTest, testId);
            return response.Success;
        }
    }

    //Questions

    public class NetworkQuestionService : IQuestionService
    {
        private readonly TcpClientConnection _connection;

        public NetworkQuestionService(TcpClientConnection connection)
        {
            _connection = connection;
        }

        public async Task<Question> GetQuestionByIdAsync(int id)
        {
            var response = await _connection.SendIdRequestAsync(MessageType.GetQuestionById, id);
            if (!response.Success)
            {
                return null;
            }
            else
            {
                return response.GetPayload<Question>();
            }
        }

        public async Task<IEnumerable<Question>> GetQuestionsByTestIdAsync(int testId)
        {
            var response = await _connection.SendIdRequestAsync(MessageType.GetQuestionsByTestId, testId);
            return response.GetPayload<List<Question>>();
        }

        public async Task<bool> CreateQuestionAsync(Question question)
        {
            var request = new NetworkMessage(MessageType.CreateQuestion, question);
            var response = await _connection.SendAsync(request);

            if (response.Success)
            {
                var serverQuestion = response.GetPayload<Question>();
                question.Id = serverQuestion.Id;
            }

            return response.Success;
        }

        public async Task<bool> UpdateQuestionAsync(Question question)
        {
            var request = new NetworkMessage(MessageType.UpdateQuestion, question);
            var response = await _connection.SendAsync(request);

            return response.Success;
        }

        public async Task<bool> DeleteQuestionAsync(int questionId)
        {
            var response = await _connection.SendIdRequestAsync(MessageType.DeleteQuestion, questionId);
            return response.Success;
        }

        public async Task<bool> UploadQuestionImageAsync(int questionId, byte[] imageData, string mimeType)
        {
            var base64 = Convert.ToBase64String(imageData);

            var request = new NetworkMessage(
                MessageType.UploadQuestionImage,
                new
                {
                    QuestionId = questionId,
                    ImageDataBase64 = base64,
                    MimeType = mimeType
                });

            var response = await _connection.SendAsync(request);
            return response.Success;
        }
    }

    //Test attempts

    public class NetworkTestAttemptService : ITestAttemptService
    {
        private readonly TcpClientConnection _connection;

        public NetworkTestAttemptService(TcpClientConnection connection)
        {
            _connection = connection;
        }

        public async Task<TestAttempt> StartTestAsync(int userId, int testId)
        {
            var request = new NetworkMessage(MessageType.StartTest, new { UserId = userId, TestId = testId });
            var response = await _connection.SendAsync(request);
            if (!response.Success)
            {
                return null;
            }
            else
            {
                return response.GetPayload<TestAttempt>();
            }
        }

        public async Task<TestAttempt> GetTestAttemptByIdAsync(int id)
        {
            var response = await _connection.SendIdRequestAsync(MessageType.GetTestAttemptById, id);
            if (response.Success) { return response.GetPayload<TestAttempt>(); }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<TestAttempt>> GetUserTestAttemptsAsync(int userId)
        {
            var response = await _connection.SendIdRequestAsync(MessageType.GetUserTestAttempts, userId);
            return response.GetPayload<IEnumerable<TestAttempt>>();
        }

        public async Task<IEnumerable<TestAttempt>> GetTestAttemptsForTestAsync(int testId)
        {
            var response = await _connection.SendIdRequestAsync(MessageType.GetTestAttemptsForTest, testId);
            return response.GetPayload<List<TestAttempt>>();
        }

        public async Task<bool> SubmitAnswerAsync(int testAttemptId, int questionId, int answerId)
        {
            var request = new NetworkMessage(MessageType.SubmitAnswer,
                new { TestAttemptId = testAttemptId, QuestionId = questionId, AnswerId = answerId });
            var response = await _connection.SendAsync(request);
            return response.Success;
        }

        public async Task<bool> SubmitMultipleAnswersAsync(int testAttemptId, int questionId, List<int> answerIds)
        {
            var request = new NetworkMessage(MessageType.SubmitMultipleAnswers,
                new { TestAttemptId = testAttemptId, QuestionId = questionId, AnswerIds = answerIds });
            var response = await _connection.SendAsync(request);
            return response.Success;
        }

        public async Task<TestAttempt> CompleteTestAsync(int testAttemptId)
        {
            var response = await _connection.SendIdRequestAsync(MessageType.CompleteTest, testAttemptId);
            if (!response.Success) return null;
            else
            {
                return response.GetPayload<TestAttempt>();
            }
        }

        public async Task<bool> CanUserAttemptTestAsync(int userId, int testId)
        {
            var request = new NetworkMessage(MessageType.CanUserAttemptTest,
                new { UserId = userId, TestId = testId });
            var response = await _connection.SendAsync(request);
            return response.GetPayload<bool>();
        }
    }
}
