//modded @vadym udod

using Fourzy._Updates.Serialized;
using UnityEngine;

namespace Fourzy
{
    public static class PlayerPrefsWrapper
    {
        public const string kTutorial = "tutorial_";
        public const string kTutorialOpened = "tutorialOpened_";

        private const string kInstructionPopupDisplayed = "instructionPopupDisplayed_";
        private const string kPuzzleChallenge = "puzzleChallenge_";
        private const string kCurrentTheme = "currentTheme_";
        private const string kThemeUnlocked = "themeUnlocked_";
        private const string kGamePiecePieces = "gamePiecePieces_";
        private const string kGamePieceChampions = "gamePieceChampions_";
        private const string kSelectedGamePiece = "selectedGamePiece_";
        private const string kUserName = "userName_";
        private const string kCoins = "coins_";
        private const string kGems = "gems_";
        private const string kXP = "xp_";
        private const string kPortalPoints = "portalPoints_";
        private const string kRarePortalPoints = "rarePortalPoints_";
        private const string kTickets = "tickets_";
        private const string kHints = "hints_";
        private const string kHintsTutorialStage = "hintsTutorialStage";
        private const string kGameViewed = "gameViewed_";
        private const string kGameRewarded = "gameRewarded_";
        private const string kRemoteSetting = "remoteSetting_";
        private const string kInitialProperties = "initialProperties";
        private const string kFastPuzzle = "fast_puzzle_";
        private const string kAdventureComplete = "adventure_complete_";
        private const string kAdventureUnlocked = "adventure_unlocked_";
        private const string kAdventureNew = "adventure_new_";
        private const string kPuzzleHintProgress = "puzzle_hint_progress_";
        private const string kNewsOpened = "news_opened_";
        private const string kFastPuzzlesLeaderboardVersion = "fast_puzzles_leaderboard_version";
        private const string kLastProjectVersion = "kProjectVersion";
        private const string kLanguageUpdated = "language_updated";
        private const string kPlacementStyle = "placement_style";

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

        public static void SetTutorialState(string name, bool state) => PlayerPrefs.SetInt(kTutorial + name, state ? 1 : 0);

        public static bool GetTutorialFinished(string name) => PlayerPrefs.GetInt(kTutorial + name, 0) != 0;

        public static void SetTutorialOpened(string name, bool state) => PlayerPrefs.SetInt(kTutorialOpened + name, state ? 1 : 0);

        public static bool GetTutorialOpened(string name) => PlayerPrefs.GetInt(kTutorialOpened + name, 0) != 0;

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

        public static int GetHintTutorialStage() => PlayerPrefs.GetInt(kHintsTutorialStage, 0);

        public static void SetHintTutorialStage(int value) => PlayerPrefs.SetInt(kHintsTutorialStage, value);

        public static bool GetLanguageUpdated() => PlayerPrefs.GetInt(kLanguageUpdated, 0) != 0;

        public static void SetLanguageUpdated(bool value) => PlayerPrefs.SetInt(kLanguageUpdated, value ? 1 : 0);

        public static int GetPlacementStyle() => PlayerPrefs.GetInt(kPlacementStyle, Constants.DEFAULT_PLACEMENT_STYLE);

        public static void SetPlacementStyle(int value) => PlayerPrefs.SetInt(kPlacementStyle, value);

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
