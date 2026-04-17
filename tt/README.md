# Testing System on .NET 8

## Architecture

The project is divided into the following layers:

### 1. Models (Data Models)
- `User` - system users
- `Test` - tests
- `Question` - test questions
- `Answer` - answer options
- `TestAttempt` - test attempts
- `UserAnswer` - user answers to questions

### 2. Data (Data Access)
- `TestingDbContext` - Entity Framework Core context for working with PostgreSQL

### 3. Services (Business Logic)
- `IAuthenticationService` - user authentication
- `IUserService` - user management
- `ITestService` - test management
- `IQuestionService` - question management
- `ITestAttemptService` - test attempt management

### 4. UI (User Interface)
- `LoginForm` - login form
- `TestSelectionForm` - test selection form
- `TestingForm` - test taking form
- `ResultsForm` - results display form

### 5. Utils (Helper Classes)
- `ImageHelper` - image handling
- `ScoreCalculator` - score calculation

## Question Types

### SingleChoice (Single Choice)
User must select exactly one correct answer from the provided options.

### MultipleChoice (Multiple Choice)
User can select one or more correct answers.

## Scoring System

- Each question has a weight (points)
- Maximum score = sum of all question weights
- For correct answer, points equal to question weight are awarded
- For incorrect answer, 0 points are awarded
- Percentage = (Points Scored / Maximum Score) * 100
- Grade: A (90-100%), B (80-89%), C (70-79%), D (60-69%), F (<60%)

## Attempt Restrictions

- Each test has a maximum number of attempts
- After exhausting attempts, user cannot retake the test
- Results of all attempts are saved to the database

## Preparation for Work

### 1. Install Dependencies
```bash
dotnet add package BCrypt.Net-Core
```

### 2. Configure Connection String
Edit the file `Data/TestingDbContext.cs` and set the correct connection string to your PostgreSQL database.

### 3. Create Database

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Usage

### Initialize Services
```csharp
var serviceContainer = new ServiceContainer();
```

### Authentication
```csharp
var user = await serviceContainer.AuthenticationService.LoginAsync(username, password);
if (user != null)
{
    // User successfully logged in
}
```

### Get Available Tests
```csharp
var tests = await serviceContainer.TestService.GetAllPublishedTestsAsync();
```

### Start Test
```csharp
var attempt = await serviceContainer.TestAttemptService.StartTestAsync(userId, testId);
```

### Complete Test
```csharp
var completedAttempt = await serviceContainer.TestAttemptService.CompleteTestAsync(attemptId);
```

## TODO

- [ ] Implement UI for all forms
- [ ] Add input data validation
- [ ] Implement logging
- [ ] Add error handling
- [ ] Implement asynchronous UI operations
- [ ] Add multi-language support
- [ ] Implement database backup
- [ ] Optimize database queries
