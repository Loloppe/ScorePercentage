namespace ScorePercentage.Utils
{
    internal static class ScorePercentageCommon
    {
        public static int currentScore = 0;
        public static double currentPercentage = 0;

        public static double calculatePercentage(int maxScore, int resultScore)
        {
            double resultPercentage = (double)(100 / (double)maxScore * (double)resultScore);
            return resultPercentage;
        }
    }
}
