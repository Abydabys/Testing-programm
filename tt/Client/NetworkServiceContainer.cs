using tt.Client.Network;
using tt.Models;
using tt.Services;
using tt.Shared;

namespace tt.Client
{
    /// <summary>
    /// Drop-in replacement for the old ServiceContainer on the CLIENT side.
    ///
    /// Instead of creating a TestingDbContext and calling services directly,
    /// this class holds a TcpClientConnection and provides the same
    /// IAuthenticationService / ITestService / IQuestionService / ITestAttemptService
    /// interfaces — but every method sends a TCP request to the server and
    /// returns the server's response.
    ///
    /// The UI forms (LoginForm, TestSelectionForm, etc.) only need to swap
    ///   new ServiceContainer()
    /// for
    ///   new NetworkServiceContainer(serverAddress, serverPort)
    /// and everything else stays the same.
    /// </summary>
    public class NetworkServiceContainer : IDisposable
    {
        // ── Fields ────────────────────────────────────────────────────────

        private readonly TcpClientConnection _connection;

        // ── Service interfaces (same names as the original ServiceContainer) ──

        public IAuthenticationService AuthenticationService { get; }
        public ITestService TestService { get; }
        public IQuestionService QuestionService { get; }
        public ITestAttemptService TestAttemptService { get; }

        // ── Constructor ───────────────────────────────────────────────────

        public NetworkServiceContainer(string serverAddress = "127.0.0.1", int serverPort = 9000)
        {
            // TODO: Create a new TcpClientConnection instance, passing serverAddress and serverPort, and store it in _connection.
            // TODO: Call _connection.ConnectAsync().GetAwaiter().GetResult() to open the TCP socket synchronously.
            //       (Constructor cannot be async.)
            // TODO: Create a new NetworkAuthenticationService, passing _connection, and assign it to AuthenticationService.
            // TODO: Create a new NetworkTestService, passing _connection, and assign it to TestService.
            // TODO: Create a new NetworkQuestionService, passing _connection, and assign it to QuestionService.
            // TODO: Create a new NetworkTestAttemptService, passing _connection, and assign it to TestAttemptService.
        }

        // ── IDisposable ───────────────────────────────────────────────────

        public void Dispose()
        {
            // TODO: Call _connection.Dispose() to close the TCP socket cleanly.
        }
    }

    // =========================================================================
    // Network implementations of the service interfaces
    // Each class implements the same interface as its server-side counterpart
    // but communicates over TCP instead of calling the database directly.
    // =========================================================================

    // ── Authentication ────────────────────────────────────────────────────────

    public class NetworkAuthenticationService : IAuthenticationService
    {
        private readonly TcpClientConnection _connection;

        public NetworkAuthenticationService(TcpClientConnection connection)
        {
            // TODO: Store the connection parameter in the _connection field.
        }

        public async Task<User> LoginAsync(string username, string password)
        {
            // TODO: Create a new NetworkMessage with MessageType.Login and an anonymous object
            //       new { Username = username, Password = password } as the payload.
            // TODO: Call _connection.SendAsync(request) using await and store the response.
            // TODO: If response.Success is false, return null.
            // TODO: If response.Success is true, call response.GetPayload<User>() and return the result.
            throw new NotImplementedException();
        }

        public async Task<bool> RegisterAsync(string username, string password, string fullName)
        {
            // TODO: Create a new NetworkMessage with MessageType.Register and an anonymous object
            //       new { Username = username, Password = password, FullName = fullName } as the payload.
            // TODO: Call _connection.SendAsync(request) using await and store the response.
            // TODO: Return response.Success.
            throw new NotImplementedException();
        }

        public async Task<bool> ValidateUserAsync(User user)
        {
            // TODO: Create a new NetworkMessage with MessageType.ValidateUser and user as the payload.
            // TODO: Call _connection.SendAsync(request) using await and store the response.
            // TODO: Call response.GetPayload<bool>() and return the result.
            throw new NotImplementedException();
        }
    }

    // ── Tests ─────────────────────────────────────────────────────────────────

    public class NetworkTestService : ITestService
    {
        private readonly TcpClientConnection _connection;

        public NetworkTestService(TcpClientConnection connection)
        {
            // TODO: Store the connection parameter in the _connection field.
        }

        public async Task<Test> GetTestByIdAsync(int id)
        {
            // TODO: Call _connection.SendIdRequestAsync(MessageType.GetTestById, id) using await.
            // TODO: If response.Success is false, return null.
            // TODO: Otherwise call response.GetPayload<Test>() and return the result.
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Test>> GetAllPublishedTestsAsync()
        {
            // TODO: Create a new NetworkMessage with MessageType.GetAllPublishedTests and an empty payload.
            // TODO: Call _connection.SendAsync(request) using await.
            // TODO: Call response.GetPayload<List<Test>>() and return the result.
            throw new NotImplementedException();
        }

        public async Task<bool> CreateTestAsync(Test test)
        {
            // TODO: Create a new NetworkMessage with MessageType.CreateTest and test as the payload.
            // TODO: Call _connection.SendAsync(request) using await.
            // TODO: If response.Success, call response.GetPayload<Test>() to get the server's version
            //       (which has the DB-assigned Id) and copy its Id back into the test parameter.
            //       This keeps the caller's test object up to date without changing the method signature.
            // TODO: Return response.Success.
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateTestAsync(Test test)
        {
            // TODO: Create a NetworkMessage with MessageType.UpdateTest and test as the payload.
            // TODO: Send it and return response.Success.
            throw new NotImplementedException();
        }

        public async Task<bool> PublishTestAsync(int testId)
        {
            // TODO: Call _connection.SendIdRequestAsync(MessageType.PublishTest, testId) using await.
            // TODO: Return response.Success.
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteTestAsync(int testId)
        {
            // TODO: Call _connection.SendIdRequestAsync(MessageType.DeleteTest, testId) using await.
            // TODO: Return response.Success.
            throw new NotImplementedException();
        }
    }

    // ── Questions ─────────────────────────────────────────────────────────────

    public class NetworkQuestionService : IQuestionService
    {
        private readonly TcpClientConnection _connection;

        public NetworkQuestionService(TcpClientConnection connection)
        {
            // TODO: Store the connection parameter in the _connection field.
        }

        public async Task<Question> GetQuestionByIdAsync(int id)
        {
            // TODO: Call _connection.SendIdRequestAsync(MessageType.GetQuestionById, id) using await.
            // TODO: If response.Success is false, return null.
            // TODO: Otherwise return response.GetPayload<Question>().
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Question>> GetQuestionsByTestIdAsync(int testId)
        {
            // TODO: Call _connection.SendIdRequestAsync(MessageType.GetQuestionsByTestId, testId) using await.
            // TODO: Return response.GetPayload<List<Question>>().
            throw new NotImplementedException();
        }

        public async Task<bool> CreateQuestionAsync(Question question)
        {
            // TODO: Create a NetworkMessage with MessageType.CreateQuestion and question as the payload.
            // TODO: Send it, copy the server-returned question's Id back into the question parameter, return Success.
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateQuestionAsync(Question question)
        {
            // TODO: Create a NetworkMessage with MessageType.UpdateQuestion and question as the payload.
            // TODO: Send it and return response.Success.
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteQuestionAsync(int questionId)
        {
            // TODO: Call _connection.SendIdRequestAsync(MessageType.DeleteQuestion, questionId) using await.
            // TODO: Return response.Success.
            throw new NotImplementedException();
        }

        public async Task<bool> UploadQuestionImageAsync(int questionId, byte[] imageData, string mimeType)
        {
            // TODO: Convert imageData to a Base64 string using Convert.ToBase64String.
            // TODO: Create a NetworkMessage with MessageType.UploadQuestionImage and an anonymous object
            //       new { QuestionId = questionId, ImageDataBase64 = base64, MimeType = mimeType } as the payload.
            // TODO: Send it and return response.Success.
            throw new NotImplementedException();
        }
    }

    // ── Test Attempts ─────────────────────────────────────────────────────────

    public class NetworkTestAttemptService : ITestAttemptService
    {
        private readonly TcpClientConnection _connection;

        public NetworkTestAttemptService(TcpClientConnection connection)
        {
            // TODO: Store the connection parameter in the _connection field.
        }

        public async Task<TestAttempt> StartTestAsync(int userId, int testId)
        {
            // TODO: Create a NetworkMessage with MessageType.StartTest and
            //       new { UserId = userId, TestId = testId } as the payload.
            // TODO: Send it. If response.Success is false, return null.
            // TODO: Otherwise return response.GetPayload<TestAttempt>().
            throw new NotImplementedException();
        }

        public async Task<TestAttempt> GetTestAttemptByIdAsync(int id)
        {
            // TODO: Call _connection.SendIdRequestAsync(MessageType.GetTestAttemptById, id) using await.
            // TODO: Return response.GetPayload<TestAttempt>() or null if not successful.
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TestAttempt>> GetUserTestAttemptsAsync(int userId)
        {
            // TODO: Call _connection.SendIdRequestAsync(MessageType.GetUserTestAttempts, userId) using await.
            // TODO: Return response.GetPayload<List<TestAttempt>>().
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TestAttempt>> GetTestAttemptsForTestAsync(int testId)
        {
            // TODO: Call _connection.SendIdRequestAsync(MessageType.GetTestAttemptsForTest, testId) using await.
            // TODO: Return response.GetPayload<List<TestAttempt>>().
            throw new NotImplementedException();
        }

        public async Task<bool> SubmitAnswerAsync(int testAttemptId, int questionId, int answerId)
        {
            // TODO: Create a NetworkMessage with MessageType.SubmitAnswer and
            //       new { TestAttemptId = testAttemptId, QuestionId = questionId, AnswerId = answerId } as the payload.
            // TODO: Send it and return response.Success.
            throw new NotImplementedException();
        }

        public async Task<bool> SubmitMultipleAnswersAsync(int testAttemptId, int questionId, List<int> answerIds)
        {
            // TODO: Create a NetworkMessage with MessageType.SubmitMultipleAnswers and
            //       new { TestAttemptId = testAttemptId, QuestionId = questionId, AnswerIds = answerIds } as the payload.
            // TODO: Send it and return response.Success.
            throw new NotImplementedException();
        }

        public async Task<TestAttempt> CompleteTestAsync(int testAttemptId)
        {
            // TODO: Call _connection.SendIdRequestAsync(MessageType.CompleteTest, testAttemptId) using await.
            // TODO: If response.Success is false, return null.
            // TODO: Otherwise return response.GetPayload<TestAttempt>().
            throw new NotImplementedException();
        }

        public async Task<bool> CanUserAttemptTestAsync(int userId, int testId)
        {
            // TODO: Create a NetworkMessage with MessageType.CanUserAttemptTest and
            //       new { UserId = userId, TestId = testId } as the payload.
            // TODO: Send it and return response.GetPayload<bool>().
            throw new NotImplementedException();
        }
    }
}
