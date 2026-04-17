## Инструкция по установке и первому запуску

### Шаг 1: Установка зависимостей

Выполните следующие команды в PowerShell:

```powershell
cd "C:\Users\123\source\repos\tt\tt"
dotnet add package BCrypt.Net-Core
```

### Шаг 2: Подготовка PostgreSQL

1. Установите PostgreSQL (если еще не установлен)
2. Создайте БД или используйте существующую
3. Обновите строку подключения в `Data/TestingDbContext.cs`:

```csharp
optionsBuilder.UseNpgsql("Server=your_server;Port=5432;Database=your_db;User Id=your_user;Password=your_password;");
```

### Шаг 3: Создание миграций

```powershell
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Шаг 4: Запуск приложения

```powershell
dotnet run
```

## Структура проекта

```
tt/
??? Models/                    # Модели данных
?   ??? User.cs
?   ??? Test.cs
?   ??? Question.cs
?   ??? Answer.cs
?   ??? TestAttempt.cs
?   ??? UserAnswer.cs
??? Data/
?   ??? TestingDbContext.cs    # DbContext для EF Core
??? Services/                  # Бизнес-логика
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
??? UI/                        # Формы Windows Forms
?   ??? LoginForm.cs
?   ??? TestSelectionForm.cs
?   ??? TestingForm.cs
?   ??? ResultsForm.cs
??? Utils/                     # Вспомогательные классы
?   ??? ImageHelper.cs
?   ??? ScoreCalculator.cs
??? ServiceContainer.cs        # Контейнер сервисов
??? Program.cs                 # Точка входа
??? tt.csproj                  # Файл проекта
```

## Ключевые особенности

? **Entity Framework Core Code-First** - полная типизированность и контроль над схемой БД
? **Поддержка PostgreSQL** - использует Npgsql провайдер
? **Две типа вопросов** - выбор одного или нескольких вариантов
? **Система весов** - каждый вопрос имеет вес (баллы)
? **Загрузка картинок** - поддержка изображений к вопросам
? **Ограничение попыток** - контроль количества попыток прохождения
? **Расчет результатов** - автоматический расчет баллов и процентов
? **Безопасность паролей** - использует BCrypt для хеширования

## Примеры использования

### Инициализация и вход

```csharp
var services = new ServiceContainer();

// Вход в систему
var user = await services.AuthenticationService.LoginAsync("username", "password");

if (user != null)
{
    // Получить список тестов
    var tests = await services.TestService.GetAllPublishedTestsAsync();
    
    foreach (var test in tests)
    {
        // Начать тест
        var attempt = await services.TestAttemptService.StartTestAsync(user.Id, test.Id);
        
        if (attempt != null)
        {
            // Получить вопросы
            var questions = await services.QuestionService.GetQuestionsByTestIdAsync(test.Id);
            
            // Пользователь отвечает на вопросы...
            
            // Завершить тест и получить результаты
            var completed = await services.TestAttemptService.CompleteTestAsync(attempt.Id);
            
            Console.WriteLine($"Результат: {completed.Score}/{completed.MaxScore}");
            Console.WriteLine($"Процент: {completed.Percentage:F2}%");
        }
    }
}
```

## Следующие шаги

1. ?? **Реализовать UI** - доделать дизайн форм в Designer'е
2. ?? **Установить BCrypt.Net-Core** - для безопасного хеширования паролей
3. ??? **Создать миграции** - подготовить БД
4. ?? **Добавить логирование** - отслеживать действия пользователей
5. ?? **Написать unit-тесты** - покрыть сервисы тестами
6. ? **Оптимизировать запросы** - использовать правильные indices в БД
