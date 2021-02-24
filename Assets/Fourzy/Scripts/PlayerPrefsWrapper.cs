//modded @vadym udod

using Fourzy._Updates;
using UnityEngine;

namespace Fourzy
{
    public static class PlayerPrefsWrapper
    {
        public static bool InstructionPopupWasDisplayed(int tokenId) =>
            GetBool("instructionPopupDisplayed_" + tokenId);

        public static void SetInstructionPopupDisplayed(int tokenId, bool state) =>
            SetBool("instructionPopupDisplayed_" + tokenId, state);

        public static bool GetPuzzleChallengeComplete(string ID) => GetBool("puzzleChallenge_" + ID);

        public static void SetPuzzleChallengeComplete(string ID, bool state) => 
            SetBool("puzzleChallenge_" + ID, state);

        public static bool PuzzlePackOpened(string ID) => GetBool("puzzleChallenge_" + ID + "_opened");

        public static void SetPuzzlePackOpened(string ID, bool state) =>
            SetBool("puzzleChallenge_" + ID + "_opened", state);

        public static bool PuzzlePackUnlocked(string ID) => GetBool("puzzleChallenge_" + ID + "_unlocked");

        public static void SetPuzzlePackUnlocked(string ID, bool state) => 
            SetBool("puzzleChallenge_" + ID + "_unlocked", state);

        public static void SetCurrentArea(int area) =>
            PlayerPrefs.SetInt("currentTheme_", area);

        public static int GetCurrentArea() =>
            PlayerPrefs.GetInt("currentTheme_", (int)Constants.DEFAULT_AREA);

        public static bool GetAreaUnlocked(int area) => GetBool("themeUnlocked_" + area);

        public static void SetAreaUnlocked(int area, bool state) => SetBool("themeUnlocked_" + area, state);

        public static void SetTutorialState(string name, bool state) => SetBool("tutorial_" + name, state);

        public static bool GetTutorialFinished(string name) => GetBool("tutorial_" + name);

        public static void SetTutorialOpened(string name, bool state) => SetBool("tutorialOpened_" + name, state);

        public static bool GetTutorialOpened(string name) => GetBool("tutorialOpened_" + name);

        public static void AddPuzzlesFailedTimes() => 
            PlayerPrefs.SetInt("puzzles_failed", GetPuzzleFailedTimes() + 1);

        public static int GetPuzzleFailedTimes() => PlayerPrefs.GetInt("puzzles_failed", 0);

        public static void SetUserName(string userName) =>
            PlayerPrefs.SetString("userName_", userName);

        public static string GetUserName() =>
            PlayerPrefs.GetString("userName_");

        public static void GamePieceUpdatePiecesCount(string id, int value) =>
            PlayerPrefs.SetInt("gamePiecePieces_" + id, value);

        public static void GamePieceDeleteData(string id)
        {
            PlayerPrefs.DeleteKey("gamePiecePieces_" + id);
            PlayerPrefs.DeleteKey("gamePieceChampions_" + id);
        }

        public static int GetGamePiecePieces(string id) =>
            PlayerPrefs.GetInt("gamePiecePieces_" + id, 0);

        public static bool HaveGamePieceRecord(string id) =>
            PlayerPrefs.HasKey("gamePiecePieces_" + id);

        public static void GamePieceUpdateChampionsCount(string id, int value) =>
            PlayerPrefs.SetInt("gamePieceChampions_" + id, value);

        public static int GetGamePieceChampions(string id) =>
            PlayerPrefs.GetInt("gamePieceChampions_" + id, 0);

        public static string GetSelectedGamePiece() =>
            PlayerPrefs.GetString("selectedGamePiece_", InternalSettings.Current.DEFAULT_GAME_PIECE);

        public static void SetSelectedGamePiece(string pieceID) =>
            PlayerPrefs.SetString("selectedGamePiece_", pieceID);

        /// <summary>
        /// Turn-based games related
        /// </summary>
        /// <param name="gameID"></param>
        /// <returns></returns>
        public static bool GetGameViewed(string gameID) => GetBool("gameViewed_" + gameID);

        /// <summary>
        /// Turn-based games related
        /// </summary>
        /// <param name="gameID"></param>
        public static void SetGameViewed(string gameID) =>
            PlayerPrefs.SetInt("gameViewed_" + gameID, 1);

        public static void AddAppOpened() => PlayerPrefs.SetInt("times_game_opened", GetAppOpened() + 1);

        public static int GetAppOpened() => PlayerPrefs.GetInt("times_game_opened", 0);

        public static bool GetGameRewarded(string gameID) => GetBool("gameRewarded_" + gameID);

        public static void SetGameRewarded(string gameID, bool state) => SetBool("gameRewarded_" + gameID, state);

        public static string GetRemoteSetting(string key) =>
            PlayerPrefs.GetString("remoteSetting_" + key, "0");

        public static void SetRemoteSetting(string key, string value) =>
            PlayerPrefs.SetString("remoteSetting_" + key, value);

        public static void SetRewardRewarded(string id, bool state) => SetBool("eventRewarded_" + id, state);

        public static bool GetRewardRewarded(string id) => GetBool("eventRewarded_" + id);

        public static void SetFastPuzzleComplete(string id, bool state) => SetBool("fast_puzzle_" + id, state);

        public static bool GetFastPuzzleComplete(string id) => GetBool("fast_puzzle_" + id);

        public static void SetAdventureComplete(string id, bool state) => SetBool("adventure_complete_" + id, state);

        public static bool GetAdventureComplete(string id) => GetBool("adventure_complete_" + id);

        public static void SetAdventureUnlocked(string id, bool state) => SetBool("adventure_unlocked_" + id, state);

        public static bool GetAdventureUnlocked(string id) => GetBool("adventure_unlocked_" + id);

        public static void SetAdventureNew(string id, bool state) => SetBool("adventure_new_" + id, state);

        public static bool GetAdventureNew(string id) => GetBool("adventure_new_" + id);

        public static int GetPuzzleHintProgress(string id) =>
            PlayerPrefs.GetInt("puzzle_hint_progress_" + id, 0);

        public static void SetPuzzleHintProgress(string id, int value) =>
            PlayerPrefs.SetInt("puzzle_hint_progress_" + id, value);

        public static bool GetNewsOpened(string id) => GetBool("news_opened_" + id);

        public static void SetNewsOpened(string id, bool state) => SetBool("news_opened_" + id, state);

        public static int GetFastPuzzlesLeaderboardVersion() =>
            PlayerPrefs.GetInt("fast_puzzles_leaderboard_version", -1);

        public static void SetFastPuzzlesLeaderboardVersion(int value) =>
            PlayerPrefs.SetInt("fast_puzzles_leaderboard_version", value);

        public static int GetHintTutorialStage() =>
            PlayerPrefs.GetInt("hintsTutorialStage", 0);

        public static void SetHintTutorialStage(int value) =>
            PlayerPrefs.SetInt("hintsTutorialStage", value);

        public static int GetPlacementStyle() =>
            PlayerPrefs.GetInt("placement_style", GameManager.HAS_POINTER ?
                    InternalSettings.Current.DEFAULT_PLACEMENT_STYLE_POINTER :
                    InternalSettings.Current.DEFAULT_PLACEMENT_STYLE_TOUCH);

        public static void SetPlacementStyle(int value) =>
            PlayerPrefs.SetInt("placement_style", value);

        public static void SetBool(string key, bool value) =>
            PlayerPrefs.SetInt(key, value ? 1 : 0);

        public static bool GetBool(string key, bool defaultValue = false) =>
            PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) != 0;

        #region Currencies

        public static int GetCoins() => PlayerPrefs.GetInt("coins_", 0);

        public static void SetCoins(int quantity) => PlayerPrefs.SetInt("coins_", quantity);

        public static int GetGems() => PlayerPrefs.GetInt("gems_", 0);

        public static void SetGems(int quantity) => PlayerPrefs.SetInt("gems_", quantity);

        public static int GetPortalPoints() => PlayerPrefs.GetInt("portalPoints_", 0);

        public static void SetPortalPoints(int quantity) => PlayerPrefs.SetInt("portalPoints_", quantity);

        public static int GetRarePortalPoints() => PlayerPrefs.GetInt("rarePortalPoints_", 0);

        public static void SetRarePortalPoints(int quantity) => PlayerPrefs.SetInt("rarePortalPoints_", quantity);

        public static int GetTickets() => PlayerPrefs.GetInt("tickets_", 0);

        public static void SetTickets(int quantity) => PlayerPrefs.SetInt("tickets_", quantity);

        public static int GetHints() => PlayerPrefs.GetInt("hints_", 0);

        public static void SetHints(int quantity) => PlayerPrefs.SetInt("hints_", quantity);

        public static int GetXP() => PlayerPrefs.GetInt("xp_", 0);

        public static void SetXP(int quantity) => PlayerPrefs.SetInt("xp_", quantity);
        #endregion
    }
}
