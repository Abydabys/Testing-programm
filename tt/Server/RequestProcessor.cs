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
        private readonly IUserService _userService;

        // ── Constructor ───────────────────────────────────────────────────

        public RequestProcessor(TestingDbContext dbContext)
        {
            // TODO: Create a new UserService, passing dbContext, and store it in _userService.
            // TODO: Create a new AuthenticationService, passing _userService, and store it in _authService.
            // TODO: Create a new TestService, passing dbContext, and store it in _testService.
            // TODO: Create a new QuestionService, passing dbContext, and store it in _questionService.
            // TODO: Create a new TestAttemptService, passing dbContext, and store it in _testAttemptService.
        }

        // ── Dispatcher ────────────────────────────────────────────────────

        /// <summary>
        /// Routes the message to the correct handler method based on MessageType.
        /// Always returns a NetworkMessage — never throws to the caller.
        /// </summary>
        public async Task<NetworkMessage> ProcessAsync(NetworkMessage request)
        {
            // TODO: Wrap the entire method in a try-catch block.
            //       In the catch, return NetworkMessage.CreateError(request.Type, ex.Message).

            // TODO: Inside the try block, use a switch expression on request.Type:
            //   MessageType.Login                → return await HandleLoginAsync(request)
            //   MessageType.Register             → return await HandleRegisterAsync(request)
            //   MessageType.ValidateUser         → return await HandleValidateUserAsync(request)
            //   MessageType.GetTestById          → return await HandleGetTestByIdAsync(request)
            //   MessageType.GetAllPublishedTests → return await HandleGetAllPublishedTestsAsync(request)
            //   MessageType.CreateTest           → return await HandleCreateTestAsync(request)
            //   MessageType.UpdateTest           → return await HandleUpdateTestAsync(request)
            //   MessageType.PublishTest          → return await HandlePublishTestAsync(request)
            //   MessageType.DeleteTest           → return await HandleDeleteTestAsync(request)
            //   MessageType.GetQuestionById      → return await HandleGetQuestionByIdAsync(request)
            //   MessageType.GetQuestionsByTestId → return await HandleGetQuestionsByTestIdAsync(request)
            //   MessageType.CreateQuestion       → return await HandleCreateQuestionAsync(request)
            //   MessageType.UpdateQuestion       → return await HandleUpdateQuestionAsync(request)
            //   MessageType.DeleteQuestion       → return await HandleDeleteQuestionAsync(request)
            //   MessageType.UploadQuestionImage  → return await HandleUploadQuestionImageAsync(request)
            //   MessageType.StartTest            → return await HandleStartTestAsync(request)
            //   MessageType.GetTestAttemptById   → return await HandleGetTestAttemptByIdAsync(request)
            //   MessageType.GetUserTestAttempts  → return await HandleGetUserTestAttemptsAsync(request)
            //   MessageType.GetTestAttemptsForTest → return await HandleGetTestAttemptsForTestAsync(request)
            //   MessageType.SubmitAnswer         → return await HandleSubmitAnswerAsync(request)
            //   MessageType.SubmitMultipleAnswers→ return await HandleSubmitMultipleAnswersAsync(request)
            //   MessageType.CompleteTest         → return await HandleCompleteTestAsync(request)
            //   MessageType.CanUserAttemptTest   → return await HandleCanUserAttemptTestAsync(request)
            //   default                          → return NetworkMessage.CreateError(request.Type, "Unknown message type")
            throw new NotImplementedException();
        }

        // ── Authentication handlers ───────────────────────────────────────

        private async Task<NetworkMessage> HandleLoginAsync(NetworkMessage request)
        {
            // TODO: Call request.GetPayload<LoginRequest>() to extract the username and password.
            //       (LoginRequest is a simple record: record LoginRequest(string Username, string Password))
            // TODO: Call _authService.LoginAsync(payload.Username, payload.Password) using await.
            // TODO: If the returned user is null, return NetworkMessage.CreateError(MessageType.Login, "Invalid credentials").
            // TODO: If the user is not null, return a new NetworkMessage(MessageType.Login, user) with Success = true.
            throw new NotImplementedException();
        }

        private async Task<NetworkMessage> HandleRegisterAsync(NetworkMessage request)
        {
            // TODO: Call request.GetPayload<RegisterRequest>() to extract username, password, and fullName.
            //       (RegisterRequest is a record: record RegisterRequest(string Username, string Password, string FullName))
            // TODO: Call _authService.RegisterAsync(payload.Username, payload.Password, payload.FullName) using await.
            // TODO: If the result is false, return NetworkMessage.CreateError(MessageType.Register, "Username already taken").
            // TODO: If true, return a new NetworkMessage { Type = MessageType.Register, Success = true }.
            throw new NotImplementedException();
        }

        private async Task<NetworkMessage> HandleValidateUserAsync(NetworkMessage request)
        {
            // TODO: Call request.GetPayload<User>() to extract the User object.
            // TODO: Call _authService.ValidateUserAsync(user) using await.
            // TODO: Return a new NetworkMessage with Type = MessageType.ValidateUser, Payload = result serialised as JSON, and Success = true.
            throw new NotImplementedException();
        }

        // ── Test handlers ─────────────────────────────────────────────────

        private async Task<NetworkMessage> HandleGetTestByIdAsync(NetworkMessage request)
        {
            // TODO: Call request.GetPayload<IdRequest>() to extract the id.
            //       (IdRequest is a record: record IdRequest(int Id))
            // TODO: Call _testService.GetTestByIdAsync(payload.Id) using await.
            // TODO: If the result is null, return an error message.
            // TODO: Otherwise return a NetworkMessage with the test as the payload.
            throw new NotImplementedException();
        }

        private async Task<NetworkMessage> HandleGetAllPublishedTestsAsync(NetworkMessage request)
        {
            // TODO: Call _testService.GetAllPublishedTestsAsync() using await.
            // TODO: Return a new NetworkMessage with the list of tests as the payload and Success = true.
            throw new NotImplementedException();
        }

        private async Task<NetworkMessage> HandleCreateTestAsync(NetworkMessage request)
        {
            // TODO: Call request.GetPayload<Test>() to extract the test object.
            // TODO: Call _testService.CreateTestAsync(test) using await and store the result.
            // TODO: If the result is false, return an error.
            // TODO: Return a success NetworkMessage with the created test (now containing its DB-assigned Id) as the payload.
            throw new NotImplementedException();
        }

        private async Task<NetworkMessage> HandleUpdateTestAsync(NetworkMessage request)
        {
            // TODO: Call request.GetPayload<Test>() to extract the test.
            // TODO: Call _testService.UpdateTestAsync(test) using await.
            // TODO: Return success or error based on the result.
            throw new NotImplementedException();
        }

        private async Task<NetworkMessage> HandlePublishTestAsync(NetworkMessage request)
        {
            // TODO: Call request.GetPayload<IdRequest>() to extract the testId.
            // TODO: Call _testService.PublishTestAsync(payload.Id) using await.
            // TODO: Return success or error based on the result.
            throw new NotImplementedException();
        }

        private async Task<NetworkMessage> HandleDeleteTestAsync(NetworkMessage request)
        {
            // TODO: Call request.GetPayload<IdRequest>() to extract the testId.
            // TODO: Call _testService.DeleteTestAsync(payload.Id) using await.
            // TODO: Return success or error based on the result.
            throw new NotImplementedException();
        }

        // ── Question handlers ─────────────────────────────────────────────

        private async Task<NetworkMessage> HandleGetQuestionByIdAsync(NetworkMessage request)
        {
            // TODO: Call request.GetPayload<IdRequest>() to extract the question id.
            // TODO: Call _questionService.GetQuestionByIdAsync(payload.Id) using await.
            // TODO: If the result is null, return an error.
            // TODO: Otherwise return the question as the payload.
            throw new NotImplementedException();
        }

        private async Task<NetworkMessage> HandleGetQuestionsByTestIdAsync(NetworkMessage request)
        {
            // TODO: Call request.GetPayload<IdRequest>() to extract the testId.
            // TODO: Call _questionService.GetQuestionsByTestIdAsync(payload.Id) using await.
            // TODO: Return the list of questions as the payload.
            throw new NotImplementedException();
        }

        private async Task<NetworkMessage> HandleCreateQuestionAsync(NetworkMessage request)
        {
            // TODO: Call request.GetPayload<Question>() to extract the question.
            // TODO: Call _questionService.CreateQuestionAsync(question) using await.
            // TODO: Return the created question (with its DB Id) as the payload on success.
            throw new NotImplementedException();
        }

        private async Task<NetworkMessage> HandleUpdateQuestionAsync(NetworkMessage request)
        {
            // TODO: Call request.GetPayload<Question>() to extract the question.
            // TODO: Call _questionService.UpdateQuestionAsync(question) using await.
            // TODO: Return success or error based on the result.
            throw new NotImplementedException();
        }

        private async Task<NetworkMessage> HandleDeleteQuestionAsync(NetworkMessage request)
        {
            // TODO: Call request.GetPayload<IdRequest>() to extract the questionId.
            // TODO: Call _questionService.DeleteQuestionAsync(payload.Id) using await.
            // TODO: Return success or error based on the result.
            throw new NotImplementedException();
        }

        private async Task<NetworkMessage> HandleUploadQuestionImageAsync(NetworkMessage request)
        {
            // TODO: Call request.GetPayload<UploadImageRequest>() to extract the questionId, imageData (base64 string), and mimeType.
            //       (UploadImageRequest is: record UploadImageRequest(int QuestionId, string ImageDataBase64, string MimeType))
            // TODO: Convert payload.ImageDataBase64 back to a byte array using Convert.FromBase64String.
            // TODO: Call _questionService.UploadQuestionImageAsync(questionId, imageBytes, mimeType) using await.
            // TODO: Return success or error based on the result.
            throw new NotImplementedException();
        }

        // ── Test attempt handlers ─────────────────────────────────────────

        private async Task<NetworkMessage> HandleStartTestAsync(NetworkMessage request)
        {
            // TODO: Call request.GetPayload<UserTestRequest>() to extract userId and testId.
            //       (UserTestRequest is: record UserTestRequest(int UserId, int TestId))
            // TODO: Call _testAttemptService.StartTestAsync(payload.UserId, payload.TestId) using await.
            // TODO: If the result is null, return an error.
            // TODO: Otherwise return the new TestAttempt as the payload.
            throw new NotImplementedException();
        }

        private async Task<NetworkMessage> HandleGetTestAttemptByIdAsync(NetworkMessage request)
        {
            // TODO: Call request.GetPayload<IdRequest>() to extract the id.
            // TODO: Call _testAttemptService.GetTestAttemptByIdAsync(payload.Id) using await.
            // TODO: If null, return an error. Otherwise return the attempt as the payload.
            throw new NotImplementedException();
        }

        private async Task<NetworkMessage> HandleGetUserTestAttemptsAsync(NetworkMessage request)
        {
            // TODO: Call request.GetPayload<IdRequest>() to extract the userId.
            // TODO: Call _testAttemptService.GetUserTestAttemptsAsync(payload.Id) using await.
            // TODO: Return the list as the payload.
            throw new NotImplementedException();
        }

        private async Task<NetworkMessage> HandleGetTestAttemptsForTestAsync(NetworkMessage request)
        {
            // TODO: Call request.GetPayload<IdRequest>() to extract the testId.
            // TODO: Call _testAttemptService.GetTestAttemptsForTestAsync(payload.Id) using await.
            // TODO: Return the list as the payload.
            throw new NotImplementedException();
        }

        private async Task<NetworkMessage> HandleSubmitAnswerAsync(NetworkMessage request)
        {
            // TODO: Call request.GetPayload<SubmitAnswerRequest>() to extract attemptId, questionId, answerId.
            //       (SubmitAnswerRequest is: record SubmitAnswerRequest(int TestAttemptId, int QuestionId, int AnswerId))
            // TODO: Call _testAttemptService.SubmitAnswerAsync(...) using await.
            // TODO: Return success or error based on the result.
            throw new NotImplementedException();
        }

        private async Task<NetworkMessage> HandleSubmitMultipleAnswersAsync(NetworkMessage request)
        {
            // TODO: Call request.GetPayload<SubmitMultipleAnswersRequest>() to extract attemptId, questionId, and the list of answerIds.
            //       (SubmitMultipleAnswersRequest is: record SubmitMultipleAnswersRequest(int TestAttemptId, int QuestionId, List<int> AnswerIds))
            // TODO: Call _testAttemptService.SubmitMultipleAnswersAsync(...) using await.
            // TODO: Return success or error based on the result.
            throw new NotImplementedException();
        }

        private async Task<NetworkMessage> HandleCompleteTestAsync(NetworkMessage request)
        {
            // TODO: Call request.GetPayload<IdRequest>() to extract the testAttemptId.
            // TODO: Call _testAttemptService.CompleteTestAsync(payload.Id) using await.
            // TODO: If null, return an error. Otherwise return the completed TestAttempt as the payload.
            throw new NotImplementedException();
        }

        private async Task<NetworkMessage> HandleCanUserAttemptTestAsync(NetworkMessage request)
        {
            // TODO: Call request.GetPayload<UserTestRequest>() to extract userId and testId.
            // TODO: Call _testAttemptService.CanUserAttemptTestAsync(payload.UserId, payload.TestId) using await.
            // TODO: Return a NetworkMessage with the boolean result serialised as the payload.
            throw new NotImplementedException();
        }
    }

    // ── Payload record types (define in this file or a separate Payloads.cs) ──

    // TODO: Define the following records that are used as request payloads:

    // record LoginRequest(string Username, string Password);
    // record RegisterRequest(string Username, string Password, string FullName);
    // record IdRequest(int Id);
    // record UserTestRequest(int UserId, int TestId);
    // record SubmitAnswerRequest(int TestAttemptId, int QuestionId, int AnswerId);
    // record SubmitMultipleAnswersRequest(int TestAttemptId, int QuestionId, List<int> AnswerIds);
    // record UploadImageRequest(int QuestionId, string ImageDataBase64, string MimeType);
}
