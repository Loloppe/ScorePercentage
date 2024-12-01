namespace ScorePercentage
{
    public class PluginConfig
    {
        public static PluginConfig Instance { get; set; }

        public bool EnableMenuHighscore { get; set; } = true;
        public bool EnableLevelEndRank { get; set; } = true;
        public bool EnableScoreDifference { get; set; } = true;
        public bool EnableScorePercentageDifference { get; set; } = true;
    }
}
