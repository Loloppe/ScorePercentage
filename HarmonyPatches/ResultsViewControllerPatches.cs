using System;
using HarmonyLib;
using ScorePercentage.Utils;
using TMPro;
using UnityEngine;

namespace ScorePercentage.HarmonyPatches
{
    [HarmonyPatch(typeof(ResultsViewController), nameof(ResultsViewController.SetDataToUI))]
    class ResultsViewControllerPatches
    {
        static void Postfix(ref TextMeshProUGUI ____rankText, ref LevelCompletionResults ____levelCompletionResults,
            ref IReadonlyBeatmapData ____transformedBeatmapData, ref GameObject ____newHighScoreText, ref TextMeshProUGUI ____scoreText)
        {
            int maxScore;
            double resultPercentage;
            int resultScore;
            int modifiedScore;
            string rankTextLine2 = "";
            string colorPositive = "#00B300";
            string colorNegative = "#FF0000";
            string positiveIndicator = "";

            if (____levelCompletionResults.levelEndStateType == LevelCompletionResults.LevelEndStateType.Cleared)
            {
                modifiedScore = ____levelCompletionResults.modifiedScore;
                
                maxScore = ScoreModel.ComputeMaxMultipliedScoreForBeatmap(____transformedBeatmapData);

                if (____levelCompletionResults.gameplayModifiers.noFailOn0Energy
                    || (____levelCompletionResults.gameplayModifiers.enabledObstacleType != GameplayModifiers.EnabledObstacleType.All)
                    || ____levelCompletionResults.gameplayModifiers.noArrows
                    || ____levelCompletionResults.gameplayModifiers.noBombs
                    || ____levelCompletionResults.gameplayModifiers.zenMode
                    || ____levelCompletionResults.gameplayModifiers.songSpeed == GameplayModifiers.SongSpeed.Slower
                    )
                {
                    resultScore = modifiedScore;
                }
                else
                {
                    resultScore = ____levelCompletionResults.multipliedScore;
                }

                resultPercentage = ScorePercentageCommon.calculatePercentage(maxScore, resultScore);

                ____rankText.autoSizeTextContainer = false;
                ____rankText.enableWordWrapping = false;

                if (PluginConfig.Instance.EnableLevelEndRank)
                {
                    ____rankText.text = "<line-height=27.5%><size=60%>" + Math.Round(resultPercentage, 2).ToString() + "<size=45%>%";

                    if (PluginConfig.Instance.EnableScorePercentageDifference && ScorePercentageCommon.currentPercentage != 0)
                    {
                        double currentPercentage = ScorePercentageCommon.currentPercentage;
                        double percentageDifference = resultPercentage - currentPercentage;
                        string percentageDifferenceColor;
                        if (percentageDifference >= 0)
                        {
                            percentageDifferenceColor = colorPositive;
                            positiveIndicator = "+";
                        }
                        else
                        {
                            percentageDifferenceColor = colorNegative;
                            positiveIndicator = "";
                            if (Math.Round(percentageDifference, 2) == 0)
                            {
                                positiveIndicator = "-";
                            }
                        }
                        rankTextLine2 = "\n<color=" + percentageDifferenceColor + "><size=40%>" + positiveIndicator + Math.Round(percentageDifference,2).ToString() + "<size=30%>%";
                    }
                    ____newHighScoreText.SetActive(false);
                }
                ____rankText.text = ____rankText.text + rankTextLine2;

                if (PluginConfig.Instance.EnableScoreDifference)
                {
                    string scoreDifference;
                    string scoreDifferenceColor = "";
                    int currentScore = ScorePercentageCommon.currentScore;
                    if (currentScore != 0)
                    {
                        scoreDifference = ScoreFormatter.Format(modifiedScore - currentScore);
                        if ((modifiedScore - currentScore) >= 0)
                        {
                            scoreDifferenceColor = colorPositive;
                            positiveIndicator = "+";
                        }
                        else if ((modifiedScore - currentScore) < 0)
                        {
                            scoreDifferenceColor = colorNegative;
                            positiveIndicator = "";
                        }

                        ____scoreText.text = "<line-height=27.5%><size=60%>" + ScoreFormatter.Format(modifiedScore) + "\n"
                                + "<size=40%><color=" + scoreDifferenceColor + "><size=40%>" + positiveIndicator + scoreDifference;
                    }
                }
            }
            ScorePercentageCommon.currentPercentage = 0;
            ScorePercentageCommon.currentScore = 0;
        }
    }
}