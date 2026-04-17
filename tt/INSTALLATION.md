## Installation and First Launch Instructions

### Step 1: Install Dependencies

Run the following commands in PowerShell:

```powershell
cd "C:\Users\123\source\repos\tt\tt"
dotnet add package BCrypt.Net-Core
```

### Step 2: Prepare PostgreSQL

1. Install PostgreSQL (if not already installed)
2. Create a database or use an existing one
3. Update the connection string in `Data/TestingDbContext.cs`:

```csharp
optionsBuilder.UseNpgsql("Server=your_server;Port=5432;Database=your_db;User Id=your_user;Password=your_password;");
```

### Step 3: Create Migrations

```powershell
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Step 4: Run Application

```powershell
dotnet run
```

## Project Structure

```
tt/
??? Models/                    # Data models
?   ??? User.cs
?   ??? Test.cs
?   ??? Question.cs
?   ??? Answer.cs
?   ??? TestAttempt.cs
?   ??? UserAnswer.cs
??? Data/
?   ??? TestingDbContext.cs    # DbContext for EF Core
??? Services/                  # Business logic
?   ??? IAuthenticationService.cs
?   ??? AuthenticationService.cs
?   ??? IUserService.cs
?   ??? UserService.cs
?   ??? ITestService.cs
?   ??? TestService.cs
?   ??? IQuestionService.cs
?   ??? QuestionService.cs
?   ??? ITestAttemptService.cs
?   ??? TestAttemptService.cs
??? UI/                        # Windows Forms forms
?   ??? LoginForm.cs
?   ??? TestSelectionForm.cs
?   ??? TestingForm.cs
?   ??? ResultsForm.cs
??? Utils/                     # Helper classes
?   ??? ImageHelper.cs
?   ??? ScoreCalculator.cs
??? ServiceContainer.cs        # Service container
??? Program.cs                 # Entry point
??? tt.csproj                  # Project file
```

## Key Features

? **Entity Framework Core Code-First** - full type safety and database control
? **PostgreSQL Support** - uses Npgsql provider
? **Two Question Types** - single or multiple choice
? **Weighted Questions** - each question has weight (points)
? **Image Upload** - support for question images
? **Attempt Limits** - control number of test attempts
? **Automatic Scoring** - automatic score and percentage calculation
? **Password Security** - uses BCrypt for hashing

## Usage Examples

### Initialization and Login

```csharp
var services = new ServiceContainer();

// Login to system
var user = await services.AuthenticationService.LoginAsync("username", "password");

if (user != null)
{
    // Get list of tests
    var tests = await services.TestService.GetAllPublishedTestsAsync();
    
    foreach (var test in tests)
    {
        // Start test
        var attempt = await services.TestAttemptService.StartTestAsync(user.Id, test.Id);
        
        if (attempt != null)
        {
            // Get questions
            var questions = await services.QuestionService.GetQuestionsByTestIdAsync(test.Id);
            
            // User answers questions...
            
            // Complete test and get results
            var completed = await services.TestAttemptService.CompleteTestAsync(attempt.Id);
            
            Console.WriteLine($"Result: {completed.Score}/{completed.MaxScore}");
            Console.WriteLine($"Percentage: {completed.Percentage:F2}%");
        }
    }
}
```

## Next Steps

1. ?? **Implement UI** - complete form design in Designer
2. ?? **Install BCrypt.Net-Core** - for secure password hashing
3. ??? **Create Migrations** - prepare database
4. ?? **Add Logging** - track user actions
5. ?? **Write Unit Tests** - cover services with tests
6. ? **Optimize Queries** - use proper indices in database
