//modded @vadym udod

using Fourzy._Updates.Serialized;
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
        public static string kHints = "hints_";
        public static string kGameViewed = "gameViewed_";
        public static string kGameRewarded = "gameRewarded_";
        public static string kRemoteSetting = "remoteSetting_";
        public static string kInitialProperties = "initialProperties";
        public static string kFastPuzzle = "fast_puzzle_";
        public static string kAdventureComplete = "adventure_complete_";
        public static string kAdventureUnlocked = "adventure_unlocked_";
        public static string kAdventureNew = "adventure_new_";
        public static string kPuzzleHintProgress = "puzzle_hint_progress_";
        public static string kNewsOpened = "news_opened_";
        public static string kFastPuzzlesLeaderboardVersion = "fast_puzzles_leaderboard_version";
        public static string kLastProjectVersion = "kProjectVersion";

        public static string kEventRewarded = "eventRewarded_";

        public static bool InstructionPopupWasDisplayed(int tokenId) => PlayerPrefs.GetInt(kInstructionPopupDisplayed + tokenId, 0) != 0;

        public static void SetInstructionPopupDisplayed(int tokenId, bool state) => PlayerPrefs.SetInt(kInstructionPopupDisplayed + tokenId, state ? 1 : 0);

        public static bool GetPuzzleChallengeComplete(string ID) => PlayerPrefs.GetInt(kPuzzleChallenge + ID) != 0;

        public static void SetPuzzleChallengeComplete(string ID, bool state) => PlayerPrefs.SetInt(kPuzzleChallenge + ID, state ? 1 : 0);

        public static bool PuzzlePackOpened(string ID) => PlayerPrefs.GetInt(kPuzzleChallenge + ID + "_opened", 0) != 0;

        public static void SetPuzzlePackOpened(string ID, bool value) => PlayerPrefs.SetInt(kPuzzleChallenge + ID + "_opened", value ? 1 : 0);

        public static bool PuzzlePackUnlocked(string ID) => PlayerPrefs.GetInt(kPuzzleChallenge + ID + "_unlocked", 0) != 0;

        public static void SetPuzzlePackUnlocked(string ID, bool value) => PlayerPrefs.SetInt(kPuzzleChallenge + ID + "_unlocked", value ? 1 : 0);

        public static void SetCurrentGameTheme(int currentTheme) => PlayerPrefs.SetInt(kCurrentTheme, currentTheme);

        public static int GetCurrentTheme() => PlayerPrefs.GetInt(kCurrentTheme, 0);

        public static bool GetThemeUnlocked(int themeID) => PlayerPrefs.GetInt(kThemeUnlocked + themeID, 0) != 0;

        public static void SetThemeUnlocked(int themeID, bool state) => PlayerPrefs.SetInt(kThemeUnlocked + themeID, state ? 1 : 0);

        public static void SetTutorialState(OnboardingDataHolder tutorial, bool state) => PlayerPrefs.SetInt(kTutorial + tutorial.tutorialName, state ? 1 : 0);

        public static bool GetTutorialFinished(OnboardingDataHolder tutorial) => PlayerPrefs.GetInt(kTutorial + tutorial.tutorialName, 0) != 0;

        public static void SetTutorialOpened(OnboardingDataHolder tutorial, bool state) => PlayerPrefs.SetInt(kTutorialOpened + tutorial.tutorialName, state ? 1 : 0);

        public static bool GetTutorialOpened(OnboardingDataHolder tutorial) => PlayerPrefs.GetInt(kTutorialOpened + tutorial.tutorialName, 0) != 0;

        public static void SetUsetName(string userName) => PlayerPrefs.SetString(kUserName, userName);

        public static string GetUserName() => PlayerPrefs.GetString(kUserName);

        public static void GamePieceUpdatePiecesCount(string id, int value) => PlayerPrefs.SetInt(kGamePiecePieces + id, value);

        public static void GamePieceDeleteData(string id)
        {
            PlayerPrefs.DeleteKey(kGamePiecePieces + id);
            PlayerPrefs.DeleteKey(kGamePieceChampions + id);
        }

        public static int GetGamePiecePieces(string id) => PlayerPrefs.GetInt(kGamePiecePieces + id, 0);

        public static bool HaveGamePieceRecord(string id) => PlayerPrefs.HasKey(kGamePiecePieces + id);

        public static void GamePieceUpdateChampionsCount(string id, int value) => PlayerPrefs.SetInt(kGamePieceChampions + id, value);

        public static int GetGamePieceChampions(string id) => PlayerPrefs.GetInt(kGamePieceChampions + id, 0);

        public static string GetSelectedGamePiece() => PlayerPrefs.GetString(kSelectedGamePiece, "");

        public static void SetSelectedGamePiece(string pieceID) => PlayerPrefs.SetString(kSelectedGamePiece, pieceID);

        public static bool GetGameViewed(string gameID) => PlayerPrefs.GetInt(kGameViewed + gameID, 0) != 0;

        public static void SetGameViewed(string gameID) => PlayerPrefs.SetInt(kGameViewed + gameID, 1);

        public static bool GetGameRewarded(string gameID) => PlayerPrefs.GetInt(kGameRewarded + gameID, 0) != 0;

        public static void SetGameRewarded(string gameID, bool value) => PlayerPrefs.SetInt(kGameRewarded + gameID, value ? 1 : 0);

        public static bool GetInitialPropertiesSet() => PlayerPrefs.GetInt(kInitialProperties, 0) != 0;

        public static void InitialPropertiesSet() => PlayerPrefs.SetInt(kInitialProperties, 1);

        public static string GetRemoteSetting(string key) => PlayerPrefs.GetString(kRemoteSetting + key, "0");

        public static void SetRemoteSetting(string key, string value) => PlayerPrefs.SetString(kRemoteSetting + key, value);

        public static void SetRewardRewarded(string id, bool state) => PlayerPrefs.SetInt(kEventRewarded + id, state ? 1 : 0);

        public static bool GetRewardRewarded(string id) => PlayerPrefs.GetInt(kEventRewarded + id, 0) != 0;

        public static void SetFastPuzzleComplete(string id, bool state) => PlayerPrefs.SetInt(kFastPuzzle + id, state ? 1 : 0);

        public static bool GetFastPuzzleComplete(string id) => PlayerPrefs.GetInt(kFastPuzzle + id, 0) != 0;

        public static void SetAdventureComplete(string id, bool state) => PlayerPrefs.SetInt(kAdventureComplete + id, state ? 1 : 0);

        public static bool GetAdventureComplete(string id) => PlayerPrefs.GetInt(kAdventureComplete + id, 0) != 0;

        public static void SetAdventureUnlocked(string id, bool state) => PlayerPrefs.SetInt(kAdventureUnlocked + id, state ? 1 : 0);

        public static bool GetAdventureUnlocked(string id) => PlayerPrefs.GetInt(kAdventureUnlocked + id, 0) != 0;

        public static void SetAdventureNew(string id, bool state) => PlayerPrefs.SetInt(kAdventureNew + id, state ? 1 : 0);

        public static bool GetAdventureNew(string id) => PlayerPrefs.GetInt(kAdventureNew + id, 1) != 0;

        public static int GetPuzzleHintProgress(string id) => PlayerPrefs.GetInt(kPuzzleHintProgress + id, 0);

        public static void SetPuzzleHintProgress(string id, int value) => PlayerPrefs.SetInt(kPuzzleHintProgress + id, value);

        public static bool GetNewsOpened(string id) => PlayerPrefs.GetInt(kNewsOpened + id, 0) != 0;

        public static void SetNewsOpened(string id, bool state) => PlayerPrefs.SetInt(kNewsOpened + id, state ? 1 : 0);

        public static int GetFastPuzzlesLeaderboardVersion() => PlayerPrefs.GetInt(kFastPuzzlesLeaderboardVersion, -1);

        public static void SetFastPuzzlesLeaderboardVersion(int value) => PlayerPrefs.SetInt(kFastPuzzlesLeaderboardVersion, value);

        public static string GetLastProjectVersion() => PlayerPrefs.GetString(kLastProjectVersion);

        public static void SetLastProjectVersion(string value) => PlayerPrefs.SetString(kLastProjectVersion, value);

        #region Currencies

        public static int GetCoins() => PlayerPrefs.GetInt(kCoins, 0);

        public static void SetCoins(int quantity) => PlayerPrefs.SetInt(kCoins, quantity);

        public static int GetGems() => PlayerPrefs.GetInt(kGems, 0);

        public static void SetGems(int quantity) => PlayerPrefs.SetInt(kGems, quantity);

        public static int GetPortalPoints() => PlayerPrefs.GetInt(kPortalPoints, 0);

        public static void SetPortalPoints(int quantity) => PlayerPrefs.SetInt(kPortalPoints, quantity);

        public static int GetRarePortalPoints() => PlayerPrefs.GetInt(kRarePortalPoints, 0);

        public static void SetRarePortalPoints(int quantity) => PlayerPrefs.SetInt(kRarePortalPoints, quantity);

        public static int GetTickets() => PlayerPrefs.GetInt(kTickets, 0);

        public static void SetTickets(int quantity) => PlayerPrefs.SetInt(kTickets, quantity);

        public static int GetHints() => PlayerPrefs.GetInt(kHints, 0);

        public static void SetHints(int quantity) => PlayerPrefs.SetInt(kHints, quantity);

        public static int GetXP() => PlayerPrefs.GetInt(kXP, 0);

        public static void SetXP(int quantity) => PlayerPrefs.SetInt(kXP, quantity);
        #endregion
    }
}
