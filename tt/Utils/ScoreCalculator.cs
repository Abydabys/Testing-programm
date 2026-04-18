using tt.Models;

namespace tt.Utils
{
    /// <summary>
    /// Helper class for calculating test results and scores
    /// </summary>
    public static class ScoreCalculator
    {
        public static int CalculateScore(
            List<(int questionId, List<int> userAnswerIds)> answers,
            List<(int questionId, int weight, List<int> correctAnswerIds, QuestionType type)> questions)
        {
            // TODO: Create an integer variable called score and initialize it to 0.
            // TODO: Loop through each question in the questions list.
            // TODO: For each question, find the matching user answer entry from the answers list where questionId matches.
            // TODO: If no matching answer is found (default), skip this question and continue to the next.
            // TODO: If the question type is SingleChoice:
            //   - Check that the user submitted exactly 1 answer AND the correct answers list also has exactly 1 answer.
            //   - If the user's single answer ID matches the single correct answer ID, add the question's weight to score.
            // TODO: If the question type is MultipleChoice:
            //   - Check that the count of user answer IDs equals the count of correct answer IDs.
            //   - Sort both lists and compare them with SequenceEqual.
            //   - If they match exactly, add the question's weight to score.
            // TODO: Return the final score after the loop.
            throw new NotImplementedException();
        }

        public static double CalculatePercentage(int score, int maxScore)
        {
            // TODO: If maxScore is 0, return 0 to avoid division by zero.
            // TODO: Calculate and return (score / maxScore) * 100 as a double.
            throw new NotImplementedException();
        }

        public static string GetScoreGrade(double percentage)
        {
            // TODO: Use a switch expression on the percentage value to return the correct grade letter:
            //   - 90 or above → "A"
            //   - 80 or above → "B"
            //   - 70 or above → "C"
            //   - 60 or above → "D"
            //   - Below 60    → "F"
            throw new NotImplementedException();
        }
    }
}
