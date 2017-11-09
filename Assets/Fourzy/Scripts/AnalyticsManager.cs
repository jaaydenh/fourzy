using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics.Experimental;
using Fabric.Answers;

namespace Fourzy
{
    public class AnalyticsManager : MonoBehaviour
    {
        public static void LogCustom(string eventName, Dictionary<string, object> customAttributes = null)
        {
            Answers.LogCustom(eventName, customAttributes);
            AnalyticsEvent.Custom(eventName, customAttributes);
        }

        public static void LogError(string eventName, string errorJSON = null)
        {
            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            customAttributes.Add("errorJSON", errorJSON);
            AnalyticsEvent.Custom(eventName, customAttributes);
            Answers.LogCustom(eventName, customAttributes);
        }

        public static void LogLogin(string method = null, bool? success = null) {
            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            customAttributes.Add("success", success);
            AnalyticsEvent.Custom(method + "_login", customAttributes);
            Answers.LogLogin("facebook", success);
        }

        public static void LogPuzzleChallenge(PuzzleChallengeInfo puzzleChallenge, bool success) {
            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            customAttributes.Add("id", puzzleChallenge.ID);
            customAttributes.Add("level", puzzleChallenge.Level);
            customAttributes.Add("success", success);
            Answers.LogCustom("complete_puzzle_challenge", customAttributes);
        }

        public static void LogGameOver(string gameType, Player player, TokenBoard tokenBoard) {
            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            customAttributes.Add("tokenBoardId", tokenBoard.id);
            customAttributes.Add("tokenBoardName", tokenBoard.name);
            customAttributes.Add("winner", (int)player);
            AnalyticsEvent.GameOver("game_over_" + gameType, customAttributes);
            Answers.LogCustom("game_over_" + gameType, customAttributes);
        }
    }
}
