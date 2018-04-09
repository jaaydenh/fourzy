﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics.Experimental;
using Fabric.Answers;
using Firebase.Analytics;
using GameAnalyticsSDK;

namespace Fourzy
{
    public class AnalyticsManager : MonoBehaviour
    {
        public static void LogCustom(string eventName, Dictionary<string, object> customAttributes = null)
        {
            Answers.LogCustom(eventName, customAttributes);
            //AnalyticsEvent.Custom(eventName, customAttributes);

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
            //AnalyticsEvent.Custom(eventName, customAttributes);
            Answers.LogCustom(eventName, customAttributes);
            FirebaseAnalytics.LogEvent(eventName, "errorJSON", errorJSON);
            GameAnalytics.NewErrorEvent(GAErrorSeverity.Error, errorJSON);
        }

        public static void LogLogin(string method = null, bool success = false) {
            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            customAttributes.Add("success", success);
            //AnalyticsEvent.Custom(method + "_login", customAttributes);
            Answers.LogLogin("facebook", success);
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
            float successF = 0;
            if (success) {
                successF = 1;
            }
            GameAnalytics.NewDesignEvent("login:" + method, successF);
            GameAnalytics.NewDesignEvent("login:" + method + ":success:" + success.ToString());
        }

        public static void LogPuzzleChallenge(PuzzleChallengeInfo puzzleChallenge, bool success, int moveCount) {
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

            GAProgressionStatus status = success ? GAProgressionStatus.Complete : GAProgressionStatus.Fail;

            float successF = 0;
            if (success)
            {
                successF = 1;
            }
            GameAnalytics.NewDesignEvent("puzzle:" + puzzleChallenge.ID + ":" + puzzleChallenge.Level + ":success", successF);
            Debug.Log("design event puzzle: " + "puzzle:" + puzzleChallenge.ID + ":" + puzzleChallenge.Level + ":success:" + success.ToString());
            GameAnalytics.NewDesignEvent("puzzle:" + puzzleChallenge.ID + ":" + puzzleChallenge.Level + ":success:" + success.ToString());
            GameAnalytics.NewProgressionEvent(status, "puzzle", puzzleChallenge.ID, moveCount);
        }

        public static void LogGameOver(string gameType, PlayerEnum player, TokenBoard tokenBoard) {
            int winner = (int)player;

            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            customAttributes.Add("tokenBoardId", tokenBoard.id);
            customAttributes.Add("tokenBoardName", tokenBoard.name);
            customAttributes.Add("winner", (int)player);
            AnalyticsEvent.GameOver("game_over_" + gameType, customAttributes);
            Answers.LogCustom("game_over_" + gameType, customAttributes);

            Parameter[] GameOverParameters = {
                new Parameter("tokenBoardId", tokenBoard.id),
                new Parameter("tokenBoardName", tokenBoard.name),
                new Parameter("winner", winner)
            };
            FirebaseAnalytics.LogEvent("game_over_" + gameType, GameOverParameters);

            Debug.Log("design event game over: " + tokenBoard.id + ":" + tokenBoard.name + ":winner:" + winner.ToString());
            GameAnalytics.NewDesignEvent(gameType + ":" + tokenBoard.id + ":" + tokenBoard.name + ":winner", (float)winner);
            GameAnalytics.NewDesignEvent(gameType + ":" + tokenBoard.id + ":winner", (float)winner);
            GameAnalytics.NewDesignEvent(gameType + ":" + tokenBoard.id + ":" + tokenBoard.name + ":winner:" + winner.ToString());
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, gameType, tokenBoard.id, winner);
        }
    }
}
