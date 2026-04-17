-- Примеры SQL запросов для инициализации тестовой БД PostgreSQL

-- Вставка тестового пользователя (пароль: "password123" хеширован с SHA256)
INSERT INTO "Users" ("Username", "PasswordHash", "FullName", "CreatedAt", "IsActive") 
VALUES ('student1', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'Иван Петров', NOW(), true)
ON CONFLICT DO NOTHING;

-- Вставка тестового теста
INSERT INTO "Tests" ("Title", "Description", "CreatedAt", "UpdatedAt", "IsPublished", "MaxAttempts")
VALUES ('Основы математики', 'Тест по основным математическим операциям', NOW(), NOW(), true, 3)
RETURNING "Id";

-- Пример вставки вопросов (замените test_id на реальный ID из предыдущего запроса)
-- Вопрос 1: Выбор одного ответа
INSERT INTO "Questions" ("TestId", "Text", "Type", "Weight", "Order")
VALUES (1, 'Чему равно 2 + 2?', 1, 5, 1)
RETURNING "Id";

-- Ответы для вопроса 1
INSERT INTO "Answers" ("QuestionId", "Text", "IsCorrect", "Order")
VALUES 
  (1, '3', false, 1),
  (1, '4', true, 2),
  (1, '5', false, 3),
  (1, '6', false, 4);

-- Вопрос 2: Выбор нескольких ответов
INSERT INTO "Questions" ("TestId", "Text", "Type", "Weight", "Order")
VALUES (1, 'Какие из этих чисел четные?', 2, 10, 2)
RETURNING "Id";

-- Ответы для вопроса 2
INSERT INTO "Answers" ("QuestionId", "Text", "IsCorrect", "Order")
VALUES 
  (2, '2', true, 1),
  (2, '3', false, 2),
  (2, '4', true, 3),
  (2, '5', false, 4);

-- Просмотр всех пользователей
SELECT * FROM "Users";

-- Просмотр всех тестов
SELECT * FROM "Tests";

-- Просмотр всех вопросов теста
SELECT * FROM "Questions" WHERE "TestId" = 1;

-- Просмотр всех ответов на вопрос
SELECT * FROM "Answers" WHERE "QuestionId" = 1;

-- Просмотр попыток пользователя
SELECT * FROM "TestAttempts" WHERE "UserId" = 1;

-- Просмотр ответов пользователя на тест
SELECT ta.*, ua.*, a.*, q.*
FROM "TestAttempts" ta
LEFT JOIN "UserAnswers" ua ON ta."Id" = ua."TestAttemptId"
LEFT JOIN "Answers" a ON ua."AnswerId" = a."Id"
LEFT JOIN "Questions" q ON ua."QuestionId" = q."Id"
WHERE ta."Id" = 1;

-- Статистика по результатам теста
SELECT 
  t."Title",
  COUNT(ta."Id") as "TotalAttempts",
  AVG(ta."Percentage") as "AveragePercentage",
  MAX(ta."Score") as "MaxScore",
  MIN(ta."Score") as "MinScore"
FROM "Tests" t
LEFT JOIN "TestAttempts" ta ON t."Id" = ta."TestId"
WHERE t."IsPublished" = true
GROUP BY t."Id", t."Title";
