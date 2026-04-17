// Examples of using the Testing System API

using tt;
using tt.Services;
using tt.Models;

// ============================================================================
// Example 1: User Registration and Login
// ============================================================================

async Task LoginExample()
{
    var services = new ServiceContainer();

    // Register a new user
    bool registered = await services.AuthenticationService.RegisterAsync(
        username: "student1",
        password: "securePassword123",
        fullName: "Ivan Petrov"
    );

    if (!registered)
    {
        Console.WriteLine("Registration error - user already exists");
        return;
    }

    // Login to system
    var user = await services.AuthenticationService.LoginAsync(
        username: "student1",
        password: "securePassword123"
    );

    if (user == null)
    {
        Console.WriteLine("Login error - invalid credentials");
        return;
    }

    Console.WriteLine($"Welcome, {user.FullName}!");
}

// ============================================================================
// Example 2: Creating a Test with Questions
// ============================================================================

async Task CreateTestExample()
{
    var services = new ServiceContainer();

    // Create a test
    var test = new Test
    {
        Title = "Basic Mathematics",
        Description = "Test on basic mathematical operations",
        MaxAttempts = 3,
        IsPublished = false
    };

    await services.TestService.CreateTestAsync(test);
    Console.WriteLine($"Test created with ID: {test.Id}");

    // Add first question (single choice)
    var question1 = new Question
    {
        TestId = test.Id,
        Text = "What is 2 + 2?",
        Type = QuestionType.SingleChoice,
        Weight = 5,
        Order = 1
    };

    await services.QuestionService.CreateQuestionAsync(question1);

    // Add answers to first question
    var answers1 = new List<Answer>
    {
        new Answer { QuestionId = question1.Id, Text = "3", IsCorrect = false, Order = 1 },
        new Answer { QuestionId = question1.Id, Text = "4", IsCorrect = true, Order = 2 },
        new Answer { QuestionId = question1.Id, Text = "5", IsCorrect = false, Order = 3 },
        new Answer { QuestionId = question1.Id, Text = "6", IsCorrect = false, Order = 4 }
    };

    foreach (var answer in answers1)
    {
        // In a real implementation, you need to create a service to add answers
        // await services.QuestionService.AddAnswerAsync(answer);
    }

    // Add second question (multiple choice)
    var question2 = new Question
    {
        TestId = test.Id,
        Text = "Which of these numbers are even?",
        Type = QuestionType.MultipleChoice,
        Weight = 10,
        Order = 2
    };

    await services.QuestionService.CreateQuestionAsync(question2);

    // Publish the test
    await services.TestService.PublishTestAsync(test.Id);
    Console.WriteLine("Test published!");
}

// ============================================================================
// Example 3: User Takes a Test
// ============================================================================

async Task TakeTestExample()
{
    var services = new ServiceContainer();
    var userId = 1;  // ID of logged-in user
    var testId = 1;  // ID of test

    // Check if user can attempt the test
    bool canAttempt = await services.TestAttemptService.CanUserAttemptTestAsync(userId, testId);

    if (!canAttempt)
    {
        Console.WriteLine("You have exhausted the number of attempts for this test");
        return;
    }

    // Start the test
    var attempt = await services.TestAttemptService.StartTestAsync(userId, testId);

    if (attempt == null)
    {
        Console.WriteLine("Error starting test");
        return;
    }

    Console.WriteLine($"Test started. Attempt #{attempt.Id}");

    // Get test questions
    var questions = (await services.QuestionService.GetQuestionsByTestIdAsync(testId)).ToList();

    // Simulate user answers
    // Question 1: SingleChoice - select answer with ID 5 (correct)
    await services.TestAttemptService.SubmitAnswerAsync(attempt.Id, questions[0].Id, 5);

    // Question 2: MultipleChoice - select answers with ID 7 and 9 (correct)
    await services.TestAttemptService.SubmitMultipleAnswersAsync(
        attempt.Id,
        questions[1].Id,
        new List<int> { 7, 9 }
    );

    // Complete test and calculate results
    var completedAttempt = await services.TestAttemptService.CompleteTestAsync(attempt.Id);

    Console.WriteLine($"Test completed!");
    Console.WriteLine($"Points earned: {completedAttempt.Score}/{completedAttempt.MaxScore}");
    Console.WriteLine($"Percentage: {completedAttempt.Percentage:F2}%");
    Console.WriteLine($"Grade: {GetGrade(completedAttempt.Percentage)}");
}

// ============================================================================
// Example 4: View Test Results
// ============================================================================

async Task ViewResultsExample()
{
    var services = new ServiceContainer();
    var userId = 1;

    // Get all user test attempts
    var attempts = (await services.TestAttemptService.GetUserTestAttemptsAsync(userId)).ToList();

    Console.WriteLine($"Test completion results:");
    Console.WriteLine(new string('?', 80));

    foreach (var attempt in attempts)
    {
        Console.WriteLine($"Test: {attempt.Test.Title}");
        Console.WriteLine($"Date: {attempt.StartedAt:dd.MM.yyyy HH:mm}");
        Console.WriteLine($"Status: {(attempt.IsCompleted ? "Completed" : "In Progress")}");

        if (attempt.IsCompleted)
        {
            Console.WriteLine($"Result: {attempt.Score}/{attempt.MaxScore} ({attempt.Percentage:F2}%)");
            Console.WriteLine($"Grade: {GetGrade(attempt.Percentage)}");
        }

        Console.WriteLine(new string('?', 80));
    }
}

// ============================================================================
// Example 5: Upload Image to Question
// ============================================================================

async Task UploadQuestionImageExample()
{
    var services = new ServiceContainer();
    var questionId = 1;

    // Read image from disk
    byte[] imageData = File.ReadAllBytes("path/to/image.png");
    string mimeType = "image/png";

    // Upload image
    bool success = await services.QuestionService.UploadQuestionImageAsync(
        questionId,
        imageData,
        mimeType
    );

    if (success)
    {
        Console.WriteLine("Image uploaded successfully");
    }
}

// ============================================================================
// Example 6: Test Statistics
// ============================================================================

async Task TestStatisticsExample()
{
    var services = new ServiceContainer();
    var testId = 1;

    // Get all test attempts
    var attempts = (await services.TestAttemptService.GetTestAttemptsForTestAsync(testId))
        .Where(a => a.IsCompleted)
        .ToList();

    if (attempts.Count == 0)
    {
        Console.WriteLine("No completed attempts for this test");
        return;
    }

    var avgScore = attempts.Average(a => a.Score);
    var avgPercentage = attempts.Average(a => a.Percentage);
    var maxScore = attempts.Max(a => a.Score);
    var minScore = attempts.Min(a => a.Score);

    Console.WriteLine($"Test statistics:");
    Console.WriteLine($"Total attempts: {attempts.Count}");
    Console.WriteLine($"Average score: {avgScore:F2}");
    Console.WriteLine($"Average percentage: {avgPercentage:F2}%");
    Console.WriteLine($"Maximum score: {maxScore}");
    Console.WriteLine($"Minimum score: {minScore}");
}

// ============================================================================
// HELPER FUNCTIONS
// ============================================================================

string GetGrade(double percentage)
{
    return percentage switch
    {
        >= 90 => "A (Excellent)",
        >= 80 => "B (Good)",
        >= 70 => "C (Satisfactory)",
        >= 60 => "D (Weak)",
        _ => "F (Unsatisfactory)"
    };
}

// ============================================================================
// MAIN METHOD - RUN EXAMPLES
// ============================================================================

async Task Main()
{
    Console.WriteLine("??????????????????????????????????????????????????????");
    Console.WriteLine("?      Examples of Using the Testing System       ?");
    Console.WriteLine("??????????????????????????????????????????????????????\n");

    try
    {
        // Uncomment the needed example:

        // await LoginExample();
        // await CreateTestExample();
        // await TakeTestExample();
        // await ViewResultsExample();
        // await UploadQuestionImageExample();
        // await TestStatisticsExample();

        Console.WriteLine("\nRun the needed example by uncommenting one of the lines in the Main() method");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        Console.WriteLine($"StackTrace: {ex.StackTrace}");
    }
}

await Main();
