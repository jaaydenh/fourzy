using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics.Experimental;
using Fabric.Answers;
using Firebase.Analytics;

namespace Fourzy
{
    public class AnalyticsManager : MonoBehaviour
    {
        public static void LogCustom(string eventName, Dictionary<string, object> customAttributes = null)
        {
            Answers.LogCustom(eventName, customAttributes);
            AnalyticsEvent.Custom(eventName, customAttributes);

            if (customAttributes != null) {
                Parameter[] customParameters = new Parameter[customAttributes.Count];

                int i = 0;
                foreach (KeyValuePair<string, object> entry in customAttributes)
                {
                    customParameters[i] = new Parameter(entry.Key, entry.Value.ToString());
                    i++;
                }
                FirebaseAnalytics.LogEvent(eventName, customParameters);
            } else {
                FirebaseAnalytics.LogEvent(eventName);
            }
        }

        public static void LogError(string eventName, string errorJSON = null)
        {
            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            customAttributes.Add("errorJSON", errorJSON);
            AnalyticsEvent.Custom(eventName, customAttributes);
            Answers.LogCustom(eventName, customAttributes);
            FirebaseAnalytics.LogEvent(eventName, "errorJSON", errorJSON);
        }

        public static void LogLogin(string method = null, bool? success = null) {
            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            customAttributes.Add("success", success);
            AnalyticsEvent.Custom(method + "_login", customAttributes);
            Answers.LogLogin("facebook", success);
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
        }

        public static void LogPuzzleChallenge(PuzzleChallengeInfo puzzleChallenge, bool success) {
            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            customAttributes.Add("id", puzzleChallenge.ID);
            customAttributes.Add("level", puzzleChallenge.Level);
            customAttributes.Add("success", success);
            Answers.LogCustom("complete_puzzle_challenge", customAttributes);

            Parameter[] PuzzleChallengeParameters = {
                new Parameter("id", puzzleChallenge.ID),
                new Parameter("level", puzzleChallenge.Level),
                new Parameter("success", success.ToString())
            };
            FirebaseAnalytics.LogEvent("complete_puzzle_challenge", PuzzleChallengeParameters);
        }

        public static void LogGameOver(string gameType, PlayerEnum player, TokenBoard tokenBoard) {
            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            customAttributes.Add("tokenBoardId", tokenBoard.id);
            customAttributes.Add("tokenBoardName", tokenBoard.name);
            customAttributes.Add("winner", (int)player);
            AnalyticsEvent.GameOver("game_over_" + gameType, customAttributes);
            Answers.LogCustom("game_over_" + gameType, customAttributes);

            Parameter[] GameOverParameters = {
                new Parameter("tokenBoardId", tokenBoard.id),
                new Parameter("tokenBoardName", tokenBoard.name),
                new Parameter("winner", (int)player)
            };
            FirebaseAnalytics.LogEvent("game_over_" + gameType, GameOverParameters);
        }
    }
}
