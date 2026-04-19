namespace tt.Shared
{
    /// <summary>
    /// Defines every type of request the client can send to the server
    /// and every type of response the server sends back.
    /// One enum is used for both directions — the server always echoes
    /// the same MessageType back so the client knows which reply it received.
    /// </summary>
    public enum MessageType
    {
        // ── Authentication ────────────────────────────────────────────────
        Login,
        Register,
        ValidateUser,

        // ── Tests ─────────────────────────────────────────────────────────
        GetTestById,
        GetAllPublishedTests,
        CreateTest,
        UpdateTest,
        PublishTest,
        DeleteTest,

        // ── Questions ─────────────────────────────────────────────────────
        GetQuestionById,
        GetQuestionsByTestId,
        CreateQuestion,
        UpdateQuestion,
        DeleteQuestion,
        UploadQuestionImage,

        // ── Test Attempts ─────────────────────────────────────────────────
        StartTest,
        GetTestAttemptById,
        GetUserTestAttempts,
        GetTestAttemptsForTest,
        SubmitAnswer,
        SubmitMultipleAnswers,
        CompleteTest,
        CanUserAttemptTest,

        // ── System ────────────────────────────────────────────────────────
        Error,
        Ping,
        Disconnect
    }
}
