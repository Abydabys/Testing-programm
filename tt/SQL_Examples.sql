-- Examples of SQL queries for initializing test database on PostgreSQL

-- Insert test user (password: "password123" hashed with SHA256)
INSERT INTO "Users" ("Username", "PasswordHash", "FullName", "CreatedAt", "IsActive") 
VALUES ('student1', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'Ivan Petrov', NOW(), true)
ON CONFLICT DO NOTHING;

-- Insert sample test
INSERT INTO "Tests" ("Title", "Description", "CreatedAt", "UpdatedAt", "IsPublished", "MaxAttempts")
VALUES ('Basic Mathematics', 'Test on basic mathematical operations', NOW(), NOW(), true, 3)
RETURNING "Id";

-- Example of inserting questions (replace test_id with actual ID from previous query)
-- Question 1: Single choice
INSERT INTO "Questions" ("TestId", "Text", "Type", "Weight", "Order")
VALUES (1, 'What is 2 + 2?', 1, 5, 1)
RETURNING "Id";

-- Answers for question 1
INSERT INTO "Answers" ("QuestionId", "Text", "IsCorrect", "Order")
VALUES 
  (1, '3', false, 1),
  (1, '4', true, 2),
  (1, '5', false, 3),
  (1, '6', false, 4);

-- Question 2: Multiple choice
INSERT INTO "Questions" ("TestId", "Text", "Type", "Weight", "Order")
VALUES (1, 'Which of these numbers are even?', 2, 10, 2)
RETURNING "Id";

-- Answers for question 2
INSERT INTO "Answers" ("QuestionId", "Text", "IsCorrect", "Order")
VALUES 
  (2, '2', true, 1),
  (2, '3', false, 2),
  (2, '4', true, 3),
  (2, '5', false, 4);

-- View all users
SELECT * FROM "Users";

-- View all tests
SELECT * FROM "Tests";

-- View all questions of a test
SELECT * FROM "Questions" WHERE "TestId" = 1;

-- View all answers for a question
SELECT * FROM "Answers" WHERE "QuestionId" = 1;

-- View user test attempts
SELECT * FROM "TestAttempts" WHERE "UserId" = 1;

-- View user answers to a test
SELECT ta.*, ua.*, a.*, q.*
FROM "TestAttempts" ta
LEFT JOIN "UserAnswers" ua ON ta."Id" = ua."TestAttemptId"
LEFT JOIN "Answers" a ON ua."AnswerId" = a."Id"
LEFT JOIN "Questions" q ON ua."QuestionId" = q."Id"
WHERE ta."Id" = 1;

-- Statistics by test results
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
