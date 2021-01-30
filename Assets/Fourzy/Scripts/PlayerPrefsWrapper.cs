//modded @vadym udod

using Fourzy._Updates;
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
        private const string kPlacementStyle = "placement_style";

        public static string kEventRewarded = "eventRewarded_";

        public static bool InstructionPopupWasDisplayed(int tokenId) =>
            GetBool(kInstructionPopupDisplayed + tokenId);

        public static void SetInstructionPopupDisplayed(int tokenId, bool state) =>
            SetBool(kInstructionPopupDisplayed + tokenId, state);

        public static bool GetPuzzleChallengeComplete(string ID) => GetBool(kPuzzleChallenge + ID);

        public static void SetPuzzleChallengeComplete(string ID, bool state) => 
            SetBool(kPuzzleChallenge + ID, state);

        public static bool PuzzlePackOpened(string ID) => GetBool(kPuzzleChallenge + ID + "_opened");

        public static void SetPuzzlePackOpened(string ID, bool state) =>
            SetBool(kPuzzleChallenge + ID + "_opened", state);

        public static bool PuzzlePackUnlocked(string ID) => GetBool(kPuzzleChallenge + ID + "_unlocked");

        public static void SetPuzzlePackUnlocked(string ID, bool state) => 
            SetBool(kPuzzleChallenge + ID + "_unlocked", state);

        public static void SetCurrentGameTheme(int currentTheme) =>
            PlayerPrefs.SetInt(kCurrentTheme, currentTheme);

        public static int GetCurrentTheme() =>
            PlayerPrefs.GetInt(kCurrentTheme, 0);

        public static bool GetThemeUnlocked(int themeID) => GetBool(kThemeUnlocked + themeID);

        public static void SetThemeUnlocked(int themeID, bool state) => SetBool(kThemeUnlocked + themeID, state);

        public static void SetTutorialState(string name, bool state) => SetBool(kTutorial + name, state);

        public static bool GetTutorialFinished(string name) => GetBool(kTutorial + name);

        public static void SetTutorialOpened(string name, bool state) => SetBool(kTutorialOpened + name, state);

        public static bool GetTutorialOpened(string name) => GetBool(kTutorialOpened + name);

        public static void SetUserName(string userName) =>
            PlayerPrefs.SetString(kUserName, userName);

        public static string GetUserName() =>
            PlayerPrefs.GetString(kUserName);

        public static void GamePieceUpdatePiecesCount(string id, int value) =>
            PlayerPrefs.SetInt(kGamePiecePieces + id, value);

        public static void GamePieceDeleteData(string id)
        {
            PlayerPrefs.DeleteKey(kGamePiecePieces + id);
            PlayerPrefs.DeleteKey(kGamePieceChampions + id);
        }

        public static int GetGamePiecePieces(string id) =>
            PlayerPrefs.GetInt(kGamePiecePieces + id, 0);

        public static bool HaveGamePieceRecord(string id) =>
            PlayerPrefs.HasKey(kGamePiecePieces + id);

        public static void GamePieceUpdateChampionsCount(string id, int value) =>
            PlayerPrefs.SetInt(kGamePieceChampions + id, value);

        public static int GetGamePieceChampions(string id) =>
            PlayerPrefs.GetInt(kGamePieceChampions + id, 0);

        public static string GetSelectedGamePiece() =>
            PlayerPrefs.GetString(kSelectedGamePiece, InternalSettings.Current.DEFAULT_GAME_PIECE);

        public static void SetSelectedGamePiece(string pieceID) =>
            PlayerPrefs.SetString(kSelectedGamePiece, pieceID);

        public static bool GetGameViewed(string gameID) => GetBool(kGameViewed + gameID);

        public static void SetGameViewed(string gameID) =>
            PlayerPrefs.SetInt(kGameViewed + gameID, 1);

        public static bool GetGameRewarded(string gameID) => GetBool(kGameRewarded + gameID);

        public static void SetGameRewarded(string gameID, bool state) => SetBool(kGameRewarded + gameID, state);

        public static bool GetInitialPropertiesSet() =>
            PlayerPrefs.GetInt(kInitialProperties, 0) != 0;

        public static void InitialPropertiesSet(bool value) =>
            PlayerPrefs.SetInt(kInitialProperties, value ? 1 : 0);

        public static string GetRemoteSetting(string key) =>
            PlayerPrefs.GetString(kRemoteSetting + key, "0");

        public static void SetRemoteSetting(string key, string value) =>
            PlayerPrefs.SetString(kRemoteSetting + key, value);

        public static void SetRewardRewarded(string id, bool state) => SetBool(kEventRewarded + id, state);

        public static bool GetRewardRewarded(string id) => GetBool(kEventRewarded + id);

        public static void SetFastPuzzleComplete(string id, bool state) => SetBool(kFastPuzzle + id, state);

        public static bool GetFastPuzzleComplete(string id) => GetBool(kFastPuzzle + id);

        public static void SetAdventureComplete(string id, bool state) => SetBool(kAdventureComplete + id, state);

        public static bool GetAdventureComplete(string id) => GetBool(kAdventureComplete + id);

        public static void SetAdventureUnlocked(string id, bool state) => SetBool(kAdventureUnlocked + id, state);

        public static bool GetAdventureUnlocked(string id) => GetBool(kAdventureUnlocked + id);

        public static void SetAdventureNew(string id, bool state) => SetBool(kAdventureNew + id, state);

        public static bool GetAdventureNew(string id) => GetBool(kAdventureNew + id);

        public static int GetPuzzleHintProgress(string id) =>
            PlayerPrefs.GetInt(kPuzzleHintProgress + id, 0);

        public static void SetPuzzleHintProgress(string id, int value) =>
            PlayerPrefs.SetInt(kPuzzleHintProgress + id, value);

        public static bool GetNewsOpened(string id) => GetBool(kNewsOpened + id);

        public static void SetNewsOpened(string id, bool state) => SetBool(kNewsOpened + id, state);

        public static int GetFastPuzzlesLeaderboardVersion() =>
            PlayerPrefs.GetInt(kFastPuzzlesLeaderboardVersion, -1);

        public static void SetFastPuzzlesLeaderboardVersion(int value) =>
            PlayerPrefs.SetInt(kFastPuzzlesLeaderboardVersion, value);

        public static int GetHintTutorialStage() =>
            PlayerPrefs.GetInt(kHintsTutorialStage, 0);

        public static void SetHintTutorialStage(int value) =>
            PlayerPrefs.SetInt(kHintsTutorialStage, value);

        public static int GetPlacementStyle() =>
#if UNITY_IOS || UNITY_ANDROID
            PlayerPrefs.GetInt(kPlacementStyle, InternalSettings.Current.DEFAULT_PLACEMENT_STYLE_TOUCH);

#elif UNITY_STANDALONE || UNITY_EDITOR
            PlayerPrefs.GetInt(kPlacementStyle, InternalSettings.Current.DEFAULT_PLACEMENT_STYLE_POINTER);
#endif

        public static void SetPlacementStyle(int value) =>
            PlayerPrefs.SetInt(kPlacementStyle, value);

        public static void SetBool(string key, bool value) =>
            PlayerPrefs.SetInt(key, value ? 1 : 0);

        public static bool GetBool(string key, bool defaultValue = false) =>
            PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) != 0;

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
