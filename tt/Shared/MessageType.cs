namespace tt.Shared
{
    public enum MessageType
    {
        // Authentication
        Login,
        Register,
        ValidateUser,

        // Tests
        GetTestById,
        GetAllPublishedTests,
        GetAllTests,          // Host only: returns published + draft
        CreateTest,
        UpdateTest,
        PublishTest,
        DeleteTest,

        // Questions
        GetQuestionById,
        GetQuestionsByTestId,
        CreateQuestion,
        UpdateQuestion,
        DeleteQuestion,
        UploadQuestionImage,

        // Test Attempts
        StartTest,
        GetTestAttemptById,
        GetUserTestAttempts,
        GetTestAttemptsForTest,
        SubmitAnswer,
        SubmitMultipleAnswers,
        CompleteTest,
        CanUserAttemptTest,

        // System
        Error,
        Ping,
        Disconnect
    }
}
