using System.Text.Json;
using tt.Data;
using tt.Models;
using tt.Services;
using tt.Shared;

namespace tt.Server
{
    /// <summary>
    /// Receives a NetworkMessage from ClientHandler, identifies the MessageType,
    /// calls the appropriate service method, and returns a NetworkMessage response.
    ///
    /// All database-touching code lives here (via the services).
    /// The client never touches the database directly.
    /// </summary>
    public class RequestProcessor
    {
        // ── Fields ────────────────────────────────────────────────────────

        private readonly IAuthenticationService _authService;
        private readonly ITestService _testService;
        private readonly IQuestionService _questionService;
        private readonly ITestAttemptService _testAttemptService;
        private readonly UserService _userService;

        // ── Constructor ───────────────────────────────────────────────────

        public RequestProcessor(TestingDbContext dbContext)
        {
            _userService = new UserService(dbContext);
            _authService = new AuthenticationService(dbContext);
            _testService = new TestService(dbContext);
            _questionService = new QuestionService(dbContext);
            _testAttemptService = new TestAttemptService(dbContext);
        }

        // ── Dispatcher ────────────────────────────────────────────────────

        /// <summary>
        /// Routes the message to the correct handler method based on MessageType.
        /// Always returns a NetworkMessage — never throws to the caller.
        /// </summary>
        public async Task<NetworkMessage> ProcessAsync(NetworkMessage request)
        {
            try
            {
                return request.Type switch
                {
                    MessageType.Login                    => await HandleLoginAsync(request),
                    MessageType.Register                 => await HandleRegisterAsync(request),
                    MessageType.ValidateUser             => await HandleValidateUserAsync(request),
                    MessageType.GetTestById              => await HandleGetTestByIdAsync(request),
                    MessageType.GetAllPublishedTests     => await HandleGetAllPublishedTestsAsync(request),
                    MessageType.CreateTest               => await HandleCreateTestAsync(request),
                    MessageType.UpdateTest               => await HandleUpdateTestAsync(request),
                    MessageType.PublishTest              => await HandlePublishTestAsync(request),
                    MessageType.DeleteTest               => await HandleDeleteTestAsync(request),
                    MessageType.GetQuestionById          => await HandleGetQuestionByIdAsync(request),
                    MessageType.GetQuestionsByTestId     => await HandleGetQuestionsByTestIdAsync(request),
                    MessageType.CreateQuestion           => await HandleCreateQuestionAsync(request),
                    MessageType.UpdateQuestion           => await HandleUpdateQuestionAsync(request),
                    MessageType.DeleteQuestion           => await HandleDeleteQuestionAsync(request),
                    MessageType.UploadQuestionImage      => await HandleUploadQuestionImageAsync(request),
                    MessageType.StartTest                => await HandleStartTestAsync(request),
                    MessageType.GetTestAttemptById       => await HandleGetTestAttemptByIdAsync(request),
                    MessageType.GetUserTestAttempts      => await HandleGetUserTestAttemptsAsync(request),
                    MessageType.GetTestAttemptsForTest   => await HandleGetTestAttemptsForTestAsync(request),
                    MessageType.SubmitAnswer             => await HandleSubmitAnswerAsync(request),
                    MessageType.SubmitMultipleAnswers    => await HandleSubmitMultipleAnswersAsync(request),
                    MessageType.CompleteTest             => await HandleCompleteTestAsync(request),
                    MessageType.CanUserAttemptTest       => await HandleCanUserAttemptTestAsync(request),
                    _                                    => NetworkMessage.CreateError(request.Type, "Unknown message type")
                };
            }
            catch (Exception ex)
            {
                return NetworkMessage.CreateError(request.Type, ex.Message);
            }
        }

        // ── Authentication handlers ───────────────────────────────────────

        private async Task<NetworkMessage> HandleLoginAsync(NetworkMessage request)
        {
            var payload = request.GetPayload<LoginRequest>();
            var user = await _authService.LoginAsync(payload.Username, payload.Password);
            if (user == null)
                return NetworkMessage.CreateError(MessageType.Login, "Invalid credentials");
            return new NetworkMessage(MessageType.Login, user) { Success = true };
        }

        private async Task<NetworkMessage> HandleRegisterAsync(NetworkMessage request)
        {
            var payload = request.GetPayload<RegisterRequest>();
            var result = await _authService.RegisterAsync(payload.Username, payload.Password, payload.FullName);
            if (!result)
                return NetworkMessage.CreateError(MessageType.Register, "Username already taken");
            return new NetworkMessage { Type = MessageType.Register, Success = true };
        }

        private async Task<NetworkMessage> HandleValidateUserAsync(NetworkMessage request)
        {
            var user = request.GetPayload<User>();
            var result = await _authService.ValidateUserAsync(user);
            return new NetworkMessage(MessageType.ValidateUser, result) { Success = true };
        }

        // ── Test handlers ─────────────────────────────────────────────────

        private async Task<NetworkMessage> HandleGetTestByIdAsync(NetworkMessage request)
        {
            var payload = request.GetPayload<IdRequest>();
            var test = await _testService.GetTestByIdAsync(payload.Id);
            if (test == null)
                return NetworkMessage.CreateError(MessageType.GetTestById, "Test not found");
            return new NetworkMessage(MessageType.GetTestById, test) { Success = true };
        }

        private async Task<NetworkMessage> HandleGetAllPublishedTestsAsync(NetworkMessage request)
        {
            var tests = await _testService.GetAllPublishedTestsAsync();
            return new NetworkMessage(MessageType.GetAllPublishedTests, tests) { Success = true };
        }

        private async Task<NetworkMessage> HandleCreateTestAsync(NetworkMessage request)
        {
            var test = request.GetPayload<Test>();
            var result = await _testService.CreateTestAsync(test);
            if (!result)
                return NetworkMessage.CreateError(MessageType.CreateTest, "Failed to create test");
            return new NetworkMessage(MessageType.CreateTest, test) { Success = true };
        }

        private async Task<NetworkMessage> HandleUpdateTestAsync(NetworkMessage request)
        {
            var test = request.GetPayload<Test>();
            var result = await _testService.UpdateTestAsync(test);
            if (!result)
                return NetworkMessage.CreateError(MessageType.UpdateTest, "Test not found or update failed");
            return new NetworkMessage { Type = MessageType.UpdateTest, Success = true };
        }

        private async Task<NetworkMessage> HandlePublishTestAsync(NetworkMessage request)
        {
            var payload = request.GetPayload<IdRequest>();
            var result = await _testService.PublishTestAsync(payload.Id);
            if (!result)
                return NetworkMessage.CreateError(MessageType.PublishTest, "Test not found or publish failed");
            return new NetworkMessage { Type = MessageType.PublishTest, Success = true };
        }

        private async Task<NetworkMessage> HandleDeleteTestAsync(NetworkMessage request)
        {
            var payload = request.GetPayload<IdRequest>();
            var result = await _testService.DeleteTestAsync(payload.Id);
            if (!result)
                return NetworkMessage.CreateError(MessageType.DeleteTest, "Test not found or delete failed");
            return new NetworkMessage { Type = MessageType.DeleteTest, Success = true };
        }

        // ── Question handlers ─────────────────────────────────────────────

        private async Task<NetworkMessage> HandleGetQuestionByIdAsync(NetworkMessage request)
        {
            var payload = request.GetPayload<IdRequest>();
            var question = await _questionService.GetQuestionByIdAsync(payload.Id);
            if (question == null)
                return NetworkMessage.CreateError(MessageType.GetQuestionById, "Question not found");
            return new NetworkMessage(MessageType.GetQuestionById, question) { Success = true };
        }

        private async Task<NetworkMessage> HandleGetQuestionsByTestIdAsync(NetworkMessage request)
        {
            var payload = request.GetPayload<IdRequest>();
            var questions = await _questionService.GetQuestionsByTestIdAsync(payload.Id);
            return new NetworkMessage(MessageType.GetQuestionsByTestId, questions) { Success = true };
        }

        private async Task<NetworkMessage> HandleCreateQuestionAsync(NetworkMessage request)
        {
            var question = request.GetPayload<Question>();
            var result = await _questionService.CreateQuestionAsync(question);
            if (!result)
                return NetworkMessage.CreateError(MessageType.CreateQuestion, "Failed to create question");
            return new NetworkMessage(MessageType.CreateQuestion, question) { Success = true };
        }

        private async Task<NetworkMessage> HandleUpdateQuestionAsync(NetworkMessage request)
        {
            var question = request.GetPayload<Question>();
            var result = await _questionService.UpdateQuestionAsync(question);
            if (!result)
                return NetworkMessage.CreateError(MessageType.UpdateQuestion, "Question not found or update failed");
            return new NetworkMessage { Type = MessageType.UpdateQuestion, Success = true };
        }

        private async Task<NetworkMessage> HandleDeleteQuestionAsync(NetworkMessage request)
        {
            var payload = request.GetPayload<IdRequest>();
            var result = await _questionService.DeleteQuestionAsync(payload.Id);
            if (!result)
                return NetworkMessage.CreateError(MessageType.DeleteQuestion, "Question not found or delete failed");
            return new NetworkMessage { Type = MessageType.DeleteQuestion, Success = true };
        }

        private async Task<NetworkMessage> HandleUploadQuestionImageAsync(NetworkMessage request)
        {
            var payload = request.GetPayload<UploadImageRequest>();
            var imageBytes = Convert.FromBase64String(payload.ImageDataBase64);
            var result = await _questionService.UploadQuestionImageAsync(payload.QuestionId, imageBytes, payload.MimeType);
            if (!result)
                return NetworkMessage.CreateError(MessageType.UploadQuestionImage, "Question not found or image upload failed");
            return new NetworkMessage { Type = MessageType.UploadQuestionImage, Success = true };
        }

        // ── Test attempt handlers ─────────────────────────────────────────

        private async Task<NetworkMessage> HandleStartTestAsync(NetworkMessage request)
        {
            var payload = request.GetPayload<UserTestRequest>();
            var attempt = await _testAttemptService.StartTestAsync(payload.UserId, payload.TestId);
            if (attempt == null)
                return NetworkMessage.CreateError(MessageType.StartTest, "Failed to start test");
            return new NetworkMessage(MessageType.StartTest, attempt) { Success = true };
        }

        private async Task<NetworkMessage> HandleGetTestAttemptByIdAsync(NetworkMessage request)
        {
            var payload = request.GetPayload<IdRequest>();
            var attempt = await _testAttemptService.GetTestAttemptByIdAsync(payload.Id);
            if (attempt == null)
                return NetworkMessage.CreateError(MessageType.GetTestAttemptById, "Test attempt not found");
            return new NetworkMessage(MessageType.GetTestAttemptById, attempt) { Success = true };
        }

        private async Task<NetworkMessage> HandleGetUserTestAttemptsAsync(NetworkMessage request)
        {
            var payload = request.GetPayload<IdRequest>();
            var attempts = await _testAttemptService.GetUserTestAttemptsAsync(payload.Id);
            return new NetworkMessage(MessageType.GetUserTestAttempts, attempts) { Success = true };
        }

        private async Task<NetworkMessage> HandleGetTestAttemptsForTestAsync(NetworkMessage request)
        {
            var payload = request.GetPayload<IdRequest>();
            var attempts = await _testAttemptService.GetTestAttemptsForTestAsync(payload.Id);
            return new NetworkMessage(MessageType.GetTestAttemptsForTest, attempts) { Success = true };
        }

        private async Task<NetworkMessage> HandleSubmitAnswerAsync(NetworkMessage request)
        {
            var payload = request.GetPayload<SubmitAnswerRequest>();
            var result = await _testAttemptService.SubmitAnswerAsync(
                payload.TestAttemptId, payload.QuestionId, payload.AnswerId);
            if (!result)
                return NetworkMessage.CreateError(MessageType.SubmitAnswer, "Failed to submit answer");
            return new NetworkMessage { Type = MessageType.SubmitAnswer, Success = true };
        }

        private async Task<NetworkMessage> HandleSubmitMultipleAnswersAsync(NetworkMessage request)
        {
            var payload = request.GetPayload<SubmitMultipleAnswersRequest>();
            var result = await _testAttemptService.SubmitMultipleAnswersAsync(
                payload.TestAttemptId, payload.QuestionId, payload.AnswerIds);
            if (!result)
                return NetworkMessage.CreateError(MessageType.SubmitMultipleAnswers, "Failed to submit answers");
            return new NetworkMessage { Type = MessageType.SubmitMultipleAnswers, Success = true };
        }

        private async Task<NetworkMessage> HandleCompleteTestAsync(NetworkMessage request)
        {
            var payload = request.GetPayload<IdRequest>();
            var attempt = await _testAttemptService.CompleteTestAsync(payload.Id);
            if (attempt == null)
                return NetworkMessage.CreateError(MessageType.CompleteTest, "Test attempt not found or already completed");
            return new NetworkMessage(MessageType.CompleteTest, attempt) { Success = true };
        }

        private async Task<NetworkMessage> HandleCanUserAttemptTestAsync(NetworkMessage request)
        {
            var payload = request.GetPayload<UserTestRequest>();
            var result = await _testAttemptService.CanUserAttemptTestAsync(payload.UserId, payload.TestId);
            return new NetworkMessage(MessageType.CanUserAttemptTest, result) { Success = true };
        }
    }

    // ── Payload record types ──────────────────────────────────────────────

    record LoginRequest(string Username, string Password);
    record RegisterRequest(string Username, string Password, string FullName);
    record IdRequest(int Id);
    record UserTestRequest(int UserId, int TestId);
    record SubmitAnswerRequest(int TestAttemptId, int QuestionId, int AnswerId);
    record SubmitMultipleAnswersRequest(int TestAttemptId, int QuestionId, List<int> AnswerIds);
    record UploadImageRequest(int QuestionId, string ImageDataBase64, string MimeType);
}
