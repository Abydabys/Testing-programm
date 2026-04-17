// Примеры использования API тестовой системы

using tt;
using tt.Services;
using tt.Models;

// ============================================================================
// ПРИМЕР 1: Регистрация и вход пользователя
// ============================================================================

async Task LoginExample()
{
    var services = new ServiceContainer();

    // Регистрация нового пользователя
    bool registered = await services.AuthenticationService.RegisterAsync(
        username: "student1",
        password: "securePassword123",
        fullName: "Иван Петров"
    );

    if (!registered)
    {
        Console.WriteLine("Ошибка регистрации - пользователь уже существует");
        return;
    }

    // Вход в систему
    var user = await services.AuthenticationService.LoginAsync(
        username: "student1",
        password: "securePassword123"
    );

    if (user == null)
    {
        Console.WriteLine("Ошибка входа - неверные учетные данные");
        return;
    }

    Console.WriteLine($"Добро пожаловать, {user.FullName}!");
}

// ============================================================================
// ПРИМЕР 2: Создание теста с вопросами
// ============================================================================

async Task CreateTestExample()
{
    var services = new ServiceContainer();

    // Создание теста
    var test = new Test
    {
        Title = "Основы математики",
        Description = "Тест по базовым математическим операциям",
        MaxAttempts = 3,
        IsPublished = false
    };

    await services.TestService.CreateTestAsync(test);
    Console.WriteLine($"Тест создан с ID: {test.Id}");

    // Добавление первого вопроса (выбор одного ответа)
    var question1 = new Question
    {
        TestId = test.Id,
        Text = "Чему равно 2 + 2?",
        Type = QuestionType.SingleChoice,
        Weight = 5,
        Order = 1
    };

    await services.QuestionService.CreateQuestionAsync(question1);

    // Добавление ответов к первому вопросу
    var answers1 = new List<Answer>
    {
        new Answer { QuestionId = question1.Id, Text = "3", IsCorrect = false, Order = 1 },
        new Answer { QuestionId = question1.Id, Text = "4", IsCorrect = true, Order = 2 },
        new Answer { QuestionId = question1.Id, Text = "5", IsCorrect = false, Order = 3 },
        new Answer { QuestionId = question1.Id, Text = "6", IsCorrect = false, Order = 4 }
    };

    foreach (var answer in answers1)
    {
        // В реальной реализации нужно создать сервис для добавления ответов
        // await services.QuestionService.AddAnswerAsync(answer);
    }

    // Добавление второго вопроса (выбор нескольких ответов)
    var question2 = new Question
    {
        TestId = test.Id,
        Text = "Какие из этих чисел четные?",
        Type = QuestionType.MultipleChoice,
        Weight = 10,
        Order = 2
    };

    await services.QuestionService.CreateQuestionAsync(question2);

    // Публикация теста
    await services.TestService.PublishTestAsync(test.Id);
    Console.WriteLine("Тест опубликован!");
}

// ============================================================================
// ПРИМЕР 3: Прохождение теста пользователем
// ============================================================================

async Task TakeTestExample()
{
    var services = new ServiceContainer();
    var userId = 1;  // ID вошедшего пользователя
    var testId = 1;  // ID теста

    // Проверка возможности попытки
    bool canAttempt = await services.TestAttemptService.CanUserAttemptTestAsync(userId, testId);

    if (!canAttempt)
    {
        Console.WriteLine("Вы исчерпали количество попыток для этого теста");
        return;
    }

    // Начало теста
    var attempt = await services.TestAttemptService.StartTestAsync(userId, testId);

    if (attempt == null)
    {
        Console.WriteLine("Ошибка при начале теста");
        return;
    }

    Console.WriteLine($"Тест начат. Попытка #{attempt.Id}");

    // Получение вопросов теста
    var questions = (await services.QuestionService.GetQuestionsByTestIdAsync(testId)).ToList();

    // Имитация ответов пользователя
    // Вопрос 1: SingleChoice - выбираем ответ с ID 5 (правильный)
    await services.TestAttemptService.SubmitAnswerAsync(attempt.Id, questions[0].Id, 5);

    // Вопрос 2: MultipleChoice - выбираем ответы с ID 7 и 9 (правильные)
    await services.TestAttemptService.SubmitMultipleAnswersAsync(
        attempt.Id,
        questions[1].Id,
        new List<int> { 7, 9 }
    );

    // Завершение теста и расчет результатов
    var completedAttempt = await services.TestAttemptService.CompleteTestAsync(attempt.Id);

    Console.WriteLine($"Тест завершен!");
    Console.WriteLine($"Набрано баллов: {completedAttempt.Score}/{completedAttempt.MaxScore}");
    Console.WriteLine($"Процент: {completedAttempt.Percentage:F2}%");
    Console.WriteLine($"Оценка: {GetGrade(completedAttempt.Percentage)}");
}

// ============================================================================
// ПРИМЕР 4: Просмотр результатов
// ============================================================================

async Task ViewResultsExample()
{
    var services = new ServiceContainer();
    var userId = 1;

    // Получение всех попыток пользователя
    var attempts = (await services.TestAttemptService.GetUserTestAttemptsAsync(userId)).ToList();

    Console.WriteLine($"Результаты прохождения тестов:");
    Console.WriteLine(new string('?', 80));

    foreach (var attempt in attempts)
    {
        Console.WriteLine($"Тест: {attempt.Test.Title}");
        Console.WriteLine($"Дата: {attempt.StartedAt:dd.MM.yyyy HH:mm}");
        Console.WriteLine($"Статус: {(attempt.IsCompleted ? "Завершен" : "В процессе")}");

        if (attempt.IsCompleted)
        {
            Console.WriteLine($"Результат: {attempt.Score}/{attempt.MaxScore} ({attempt.Percentage:F2}%)");
            Console.WriteLine($"Оценка: {GetGrade(attempt.Percentage)}");
        }

        Console.WriteLine(new string('?', 80));
    }
}

// ============================================================================
// ПРИМЕР 5: Загрузка картинки к вопросу
// ============================================================================

async Task UploadQuestionImageExample()
{
    var services = new ServiceContainer();
    var questionId = 1;

    // Чтение картинки с диска
    byte[] imageData = File.ReadAllBytes("path/to/image.png");
    string mimeType = "image/png";

    // Загрузка картинки
    bool success = await services.QuestionService.UploadQuestionImageAsync(
        questionId,
        imageData,
        mimeType
    );

    if (success)
    {
        Console.WriteLine("Картинка загружена успешно");
    }
}

// ============================================================================
// ПРИМЕР 6: Статистика по тестам
// ============================================================================

async Task TestStatisticsExample()
{
    var services = new ServiceContainer();
    var testId = 1;

    // Получение всех попыток по тесту
    var attempts = (await services.TestAttemptService.GetTestAttemptsForTestAsync(testId))
        .Where(a => a.IsCompleted)
        .ToList();

    if (attempts.Count == 0)
    {
        Console.WriteLine("Нет завершенных попыток для этого теста");
        return;
    }

    var avgScore = attempts.Average(a => a.Score);
    var avgPercentage = attempts.Average(a => a.Percentage);
    var maxScore = attempts.Max(a => a.Score);
    var minScore = attempts.Min(a => a.Score);

    Console.WriteLine($"Статистика по тесту:");
    Console.WriteLine($"Всего попыток: {attempts.Count}");
    Console.WriteLine($"Средний балл: {avgScore:F2}");
    Console.WriteLine($"Средний процент: {avgPercentage:F2}%");
    Console.WriteLine($"Максимальный балл: {maxScore}");
    Console.WriteLine($"Минимальный балл: {minScore}");
}

// ============================================================================
// ВСПОМОГАТЕЛЬНЫЕ ФУНКЦИИ
// ============================================================================

string GetGrade(double percentage)
{
    return percentage switch
    {
        >= 90 => "A (Отлично)",
        >= 80 => "B (Хорошо)",
        >= 70 => "C (Удовлетворительно)",
        >= 60 => "D (Слабо)",
        _ => "F (Неудовлетворительно)"
    };
}

// ============================================================================
// ГЛАВНЫЙ МЕТОД - ЗАПУСК ПРИМЕРОВ
// ============================================================================

async Task Main()
{
    Console.WriteLine("??????????????????????????????????????????????????????");
    Console.WriteLine("?      Примеры использования тестовой системы      ?");
    Console.WriteLine("??????????????????????????????????????????????????????\n");

    try
    {
        // Раскомментируйте нужный пример:

        // await LoginExample();
        // await CreateTestExample();
        // await TakeTestExample();
        // await ViewResultsExample();
        // await UploadQuestionImageExample();
        // await TestStatisticsExample();

        Console.WriteLine("\nЗапустите нужный пример, раскомментировав одну из строк в методе Main()");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
        Console.WriteLine($"StackTrace: {ex.StackTrace}");
    }
}

await Main();
