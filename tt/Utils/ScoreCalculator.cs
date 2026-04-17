using tt.Models;

namespace tt.Utils
{
    /// <summary>
    /// Helper class for calculating test results and scores
    /// </summary>
    public static class ScoreCalculator
    {
        public static int CalculateScore(List<(int questionId, List<int> userAnswerIds)> answers, List<(int questionId, int weight, List<int> correctAnswerIds, QuestionType type)> questions)
        {
            int score = 0;

            foreach (var question in questions)
            {
                var userAnswer = answers.FirstOrDefault(a => a.questionId == question.questionId);
                if (userAnswer == default)
                    continue;

                if (question.type == QuestionType.SingleChoice)
                {
                    if (userAnswer.userAnswerIds.Count == 1 && 
                        question.correctAnswerIds.Count == 1 &&
                        userAnswer.userAnswerIds[0] == question.correctAnswerIds[0])
                    {
                        score += question.weight;
                    }
                }
                else if (question.type == QuestionType.MultipleChoice)
                {
                    if (userAnswer.userAnswerIds.Count == question.correctAnswerIds.Count &&
                        userAnswer.userAnswerIds.OrderBy(a => a).SequenceEqual(question.correctAnswerIds.OrderBy(a => a)))
                    {
                        score += question.weight;
                    }
                }
            }

            return score;
        }

        public static double CalculatePercentage(int score, int maxScore)
        {
            if (maxScore == 0)
                return 0;

            return (double)score / maxScore * 100;
        }

        public static string GetScoreGrade(double percentage)
        {
            return percentage switch
            {
                >= 90 => "A",
                >= 80 => "B",
                >= 70 => "C",
                >= 60 => "D",
                _ => "F"
            };
        }
    }
}
