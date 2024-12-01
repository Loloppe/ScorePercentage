using HarmonyLib;
using ScorePercentage.Utils;
using System;
using System.Threading;
using TMPro;

namespace ScorePercentage.HarmonyPatches
{
    class LevelStatsViewPatches
    {
        private static BeatmapLevelsModel beatmapLevelsModel = null;
        private static BeatmapDataLoader beatmapDataLoader = null;

        [HarmonyPatch(typeof(StandardLevelDetailView), nameof(StandardLevelDetailView.RefreshContent))]
        static class StandardLevelDetailViewPatch
        {
            static void Prefix(StandardLevelDetailView __instance)
            {
                beatmapLevelsModel = __instance._beatmapLevelsModel;
                beatmapDataLoader = __instance._beatmapDataLoader;
            }
        }

        [HarmonyPatch(typeof(LevelStatsView), nameof(LevelStatsView.ShowStats))]
        static class ShowStatsPatch
        {
            static void Prefix(in BeatmapKey beatmapKey, PlayerData playerData)
            {
                if (PluginConfig.Instance.EnableMenuHighscore)
                {
                    if (playerData != null)
                    {
                        PlayerLevelStatsData playerLevelStatsData = playerData.GetOrCreatePlayerLevelStatsData(beatmapKey);

                        if (playerLevelStatsData.validScore)
                        {
                            ScorePercentageCommon.currentScore = playerLevelStatsData.highScore;
                        }
                        else
                        {
                            ScorePercentageCommon.currentPercentage = 0;
                            ScorePercentageCommon.currentScore = 0;
                        }
                    }
                }


            }
            static async void Postfix(BeatmapKey beatmapKey, PlayerData playerData, TextMeshProUGUI ____highScoreText)
            {
                if (ScorePercentageCommon.currentScore != 0 && beatmapLevelsModel != null && beatmapDataLoader != null)
                {
                    var beatmapData = await beatmapLevelsModel.LoadBeatmapLevelDataAsync(beatmapKey.levelId, BeatmapLevelDataVersion.Original, new CancellationToken());
                    var readonlyBeatmapData = await beatmapDataLoader.LoadBeatmapDataAsync(beatmapData.beatmapLevelData, beatmapKey, 0f, false, null, null, BeatmapLevelDataVersion.Original, playerData.gameplayModifiers, playerData.playerSpecificSettings, false);
                    int currentDifficultyMaxScore = ScoreModel.ComputeMaxMultipliedScoreForBeatmap(readonlyBeatmapData);
                    ScorePercentageCommon.currentPercentage = ScorePercentageCommon.calculatePercentage(currentDifficultyMaxScore, ScorePercentageCommon.currentScore);
                    ____highScoreText.text = ScorePercentageCommon.currentScore.ToString() + " " + "(" + Math.Round(ScorePercentageCommon.currentPercentage, 2).ToString() + "%)";
                }
            }
        }
    }
}