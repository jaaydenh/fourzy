﻿//modded @vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Serialized;
using System;
using UnityEngine;

namespace Fourzy
{
    public static class PlayerPrefsWrapper
    {
        public static string kInstructionPopupDisplayed = "instructionPopupDisplayed_";
        public static string kPuzzleChallenge = "puzzleChallenge_";
        public static string kCurrentTheme = "currentTheme_";
        public static string kThemeUnlocked = "themeUnlocked_";
        public static string kGamePiecePieces = "gamePiecePieces_";
        public static string kGamePieceChampions = "gamePieceChampions_";
        public static string kSelectedGamePiece = "selectedGamePiece_";
        public static string kTutorial = "tutorial_";
        public static string kTutorialOpened = "tutorialOpened_";
        public static string kUserName = "userName_";
        public static string kCoins = "coins_";
        public static string kGems = "gems_";
        public static string kXP = "xp_";
        public static string kPortalPoints = "portalPoints_";
        public static string kRarePortalPoints = "rarePortalPoints_";
        public static string kTickets = "tickets_";
        public static string kGameViewed = "gameViewed_";
        public static string kGameRewarded = "gameRewarded_";
        public static string kRemoteSetting = "remoteSetting_";
        public static string kInitialProperties = "initialProperties";
        public static string kPuzzlePackRewarded = "pazzlePackRewarded_";

        public static bool InstructionPopupWasDisplayed(int tokenId)
        {
            int defaultValue = 0;
            return PlayerPrefs.GetInt(kInstructionPopupDisplayed + tokenId, defaultValue) != defaultValue;
        }

        public static void SetInstructionPopupDisplayed(int tokenId, bool isDisplayed)
        {
            int value = isDisplayed ? 1 : 0;
            PlayerPrefs.SetInt(kInstructionPopupDisplayed + tokenId, value);
        }

        public static bool IsPuzzleChallengeComplete(string ID)
        {
            int defaultValue = 0;
            return PlayerPrefs.GetInt(kPuzzleChallenge + ID) != defaultValue;
        }

        public static void SetPuzzleChallengeComplete(string ID, bool completed)
        {
            int value = completed ? 1 : 0;
            PlayerPrefs.SetInt(kPuzzleChallenge + ID, value);
        }

        public static bool PuzzlePackOpened(string ID)
        {
            int defaultValue = 0;
            return PlayerPrefs.GetInt(kPuzzleChallenge + ID + "_opened", defaultValue) != defaultValue;
        }

        public static void SetPuzzlePackOpened(string ID, bool value)
        {
            PlayerPrefs.SetInt(kPuzzleChallenge + ID + "_opened", value ? 1 : 0);
        }

        public static bool PuzzlePackUnlocked(string ID)
        {
            int defaultValue = 0;
            return PlayerPrefs.GetInt(kPuzzleChallenge + ID + "_unlocked", defaultValue) != defaultValue;
        }

        public static void SetPuzzlePackUnlocked(string ID, bool value)
        {
            PlayerPrefs.SetInt(kPuzzleChallenge + ID + "_unlocked", value ? 1 : 0);
        }

        public static void SetCurrentGameTheme(int currentTheme)
        {
            PlayerPrefs.SetInt(kCurrentTheme, currentTheme);
        }

        public static int GetCurrentTheme()
        {
            return PlayerPrefs.GetInt(kCurrentTheme, 0);
        }

        public static bool GetThemeUnlocked(int themeID)
        {
            int defaultValue = 0;
            return PlayerPrefs.GetInt(kThemeUnlocked + themeID, defaultValue) != defaultValue;
        }

        public static void SetThemeUnlocked(int themeID, bool state)
        {
            PlayerPrefs.SetInt(kThemeUnlocked + themeID, state ? 1 : 0);
        }

        public static void SetTutorialState(OnboardingDataHolder tutorial, bool state)
        {
            int value = state ? 1 : 0;
            PlayerPrefs.SetInt(kTutorial + tutorial.name, value);
        }

        public static bool GetTutorialFinished(OnboardingDataHolder tutorial)
        {
            int defaultValue = 0;
            return PlayerPrefs.GetInt(kTutorial + tutorial.name, defaultValue) != defaultValue;
        }

        public static void SetTutorialOpened(OnboardingDataHolder tutorial, bool state)
        {
            int value = state ? 1 : 0;
            PlayerPrefs.SetInt(kTutorialOpened + tutorial.name, value);
        }

        public static bool GetTutorialOpened(OnboardingDataHolder tutorial)
        {
            int defaultValue = 0;
            return PlayerPrefs.GetInt(kTutorialOpened + tutorial.name, defaultValue) != defaultValue;
        }
        
        public static void SetUsetName(string userName)
        {
            PlayerPrefs.SetString(kUserName, userName);
        }

        public static string GetUserName()
        {
            return PlayerPrefs.GetString(kUserName);
        }
        
        public static void GamePieceUpdatePiecesCount(GamePieceData gamePiece)
        {
            PlayerPrefs.SetInt(kGamePiecePieces + gamePiece.ID, gamePiece.pieces);
        }

        public static int GetGamePiecePieces(GamePieceData gamePiece)
        {
            return PlayerPrefs.GetInt(kGamePiecePieces + gamePiece.ID, 0);
        }

        public static bool HaveGamePieceRecord(GamePieceData gamePiece)
        {
            return PlayerPrefs.HasKey(kGamePiecePieces + gamePiece.ID);
        }

        public static void GamePieceUpdateChampionsCount(GamePieceData gamePiece)
        {
            PlayerPrefs.SetInt(kGamePieceChampions + gamePiece.ID, gamePiece.champions);
        }

        public static int GetGamePieceChampions(GamePieceData gamePiece)
        {
            return PlayerPrefs.GetInt(kGamePieceChampions + gamePiece.ID, 0);
        }

        public static int GetSelectedGamePiece()
        {
            return PlayerPrefs.GetInt(kSelectedGamePiece, -1);
        }

        public static void SetSelectedGamePiece(int pieceID)
        {
            PlayerPrefs.SetInt(kSelectedGamePiece, pieceID);
        }

        public static bool GetGameViewed(string gameID)
        {
            return PlayerPrefs.GetInt(kGameViewed + gameID, 0) != 0;
        }

        public static void SetGameViewed(string gameID)
        {
            PlayerPrefs.SetInt(kGameViewed + gameID, 1);
        }

        public static bool GetGameRewarded(string gameID)
        {
            return PlayerPrefs.GetInt(kGameRewarded + gameID, 0) != 0;
        }

        public static void SetGameRewarded(string gameID, bool value)
        {
            PlayerPrefs.SetInt(kGameRewarded + gameID, value ? 1 : 0);
        }

        public static bool GetInitialPropertiesSet()
        {
            return PlayerPrefs.GetInt(kInitialProperties, 0) != 0;
        }

        public static void InitialPropertiesSet()
        {
            PlayerPrefs.SetInt(kInitialProperties, 1);
        }

        public static string GetRemoteSetting(string key)
        {
            return PlayerPrefs.GetString(kRemoteSetting + key, "0");
        }

        public static void SetRemoteSetting(string key, string value)
        {
            PlayerPrefs.SetString(kRemoteSetting + key, value);
        }

        public static void SetPuzzlePackRewarded(string packID, bool state)
        {
            PlayerPrefs.SetInt(kPuzzlePackRewarded + packID, state ? 1 : 0);
        }

        public static bool GetPuzzlePackRewarded(string packID)
        {
            return PlayerPrefs.GetInt(kPuzzlePackRewarded + packID, 0) != 0;
        }

        #region Currencies

        public static int GetCoins()
        {
            return PlayerPrefs.GetInt(kCoins, 0);
        }

        public static void SetCoins(int quantity)
        {
            PlayerPrefs.SetInt(kCoins, quantity);
        }

        public static int GetGems()
        {
            return PlayerPrefs.GetInt(kGems, 0);
        }

        public static void SetGems(int quantity)
        {
            PlayerPrefs.SetInt(kGems, quantity);
        }

        public static int GetPortalPoints()
        {
            return PlayerPrefs.GetInt(kPortalPoints, 0);
        }

        public static void SetPortalPoints(int quantity)
        {
            PlayerPrefs.SetInt(kPortalPoints, quantity);
        }

        public static int GetRarePortalPoints()
        {
            return PlayerPrefs.GetInt(kRarePortalPoints, 0);
        }

        public static void SetRarePortalPoints(int quantity)
        {
            PlayerPrefs.SetInt(kRarePortalPoints, quantity);
        }

        public static int GetTickets()
        {
            return PlayerPrefs.GetInt(kTickets, 0);
        }

        public static void SetTickets(int quantity)
        {
            PlayerPrefs.SetInt(kTickets, quantity);
        }

        public static int GetXP()
        {
            return PlayerPrefs.GetInt(kXP, 0);
        }

        public static void SetXP(int quantity)
        {
            PlayerPrefs.SetInt(kXP, quantity);
        }
        #endregion
    }
}
