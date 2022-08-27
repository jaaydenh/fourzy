//@vadym udod

using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.Tools;
using Newtonsoft.Json;
using SkillzSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.Managers
{
    public class SkillzGameController : RoutinesBase, SkillzMatchDelegate
    {
        public static SkillzGameController Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("SKillzGameController");
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<SkillzGameController>();

                    //instance.GetProgressionData();
                }

                return instance;
            }
        }
        private static SkillzGameController instance;

        private static List<string> defaultPlayerDataKeys = new List<string>()
        {
            "games_played",
            "cash_games_played"
        };

        private int random = -1;

        private int gamesToPlay;
        private int movesPerMatch;
        private float timer;
        private int winPoints;
        private int drawPoints;
        private int winAllGamesBonus;
        private int pointsPerSecond;
        private int pointsPerMoveLeftWin;
        private int pointsPerMoveLeftLose;
        private int pointsPerMoveLeftDraw;
        private int matchPausesLeft;
        private float lastGameFinishedAt;
        private int lastGameMovesCount;

        internal Match CurrentMatch { get; private set; }
        internal Match LastMatch { get; private set; }
        internal bool OngoingMatch { get; set; }
        internal float GameInitialTimerValue => random - timer;
        internal int CurrentLevelIndex => GamesPlayed.Count;
        internal int LastPlayedLevelIndex { get; set; }
        internal int WinPoints => random - winPoints;
        internal int DrawPoints => random - drawPoints;
        internal int WinAllGamesBonus => random - winAllGamesBonus;
        internal int PointsPerSecond => random - pointsPerSecond;
        internal int PointsPerMoveLeftWin => random - pointsPerMoveLeftWin;
        internal int PointsPerMoveLeftLose => random - pointsPerMoveLeftLose;
        internal int PointsPerMoveLeftDraw => random - pointsPerMoveLeftDraw;
        internal int GamesToPlay => random - gamesToPlay;
        internal int MovesPerMatch => random - movesPerMatch;
        internal int MatchPausesLeft => random - matchPausesLeft;
        internal List<SkillzGameResult> GamesPlayed { get; } = new List<SkillzGameResult>();
        internal int Points => GamesPlayed.Sum(game => game.Points);
        internal bool HaveNextGame => CurrentLevelIndex < GamesToPlay;
        /// <summary>
        /// If game is opened when we return from skillz to fourzy, exit to main menu
        /// </summary>
        internal bool CloseGameOnBack { get; set; }
        internal bool ReturnToSkillzCalled { get; set; }
        internal List<SkillzLevelParams> LevelsInfo { get; private set; }
        internal int SubmitRetries { get; set; }
        internal int ExplicitSeed => random - GetMatchParamInt("RandomSeed", -1);
        internal Player CurrentPlayer => CurrentMatch?.Players.Find(_player => _player.IsCurrentPlayer);
        internal Dictionary<string, ProgressionValue> LatestDefaultPlayerData { get; private set; }
        internal Action OnDefaultPlayerDataReceived { get; set; }
        internal Action OnDefaultPlayerDataUpdated { get; set; }

        internal int PlayerData_GamesPlayed
        {
            get => int.Parse(LatestDefaultPlayerData["games_played"].Value);
            private set => LatestDefaultPlayerData["games_played"] = new ProgressionValue(value + "", "int", "", "Games Played", null);
        }

        internal int PlayerData_CashGamesPlayed
        {
            get => int.Parse(LatestDefaultPlayerData["cash_games_played"].Value);
            private set => LatestDefaultPlayerData["cash_games_played"] = new ProgressionValue(value + "", "int", "", "Cash Games Played", null);
        }

        public static void StartEditorSkillzUI()
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if (SettingsManager.Get(SettingsManager.KEY_AUDIO) == true)
            {
                SkillzCrossPlatform.setSkillzBackgroundMusic("MenuMusic.mp3");
                SkillzCrossPlatform.setSkillzMusicVolume(1f);
            }
            else
            {
                SkillzCrossPlatform.setSkillzMusicVolume(0);
            }

            SkillzState.SetAsyncDelegate(Instance);
            SkillzCrossPlatform.SetEditorBridgeAPI();
#endif
        }

        public void InitializeMatchData()
        {
            random = UnityEngine.Random.Range(10000, 99999);
            GamesPlayed.Clear();
            ReturnToSkillzCalled = false;
            LastPlayedLevelIndex = 0;

            //assign timer value
            timer = GetMatchParamInt(Constants.SKILLZ_GAME_TIMER_KEY, Constants.SKILLZ_DEFAULT_GAME_TIMER);
            //assign games to play
            gamesToPlay = GetMatchParamInt(Constants.SKILLZ_GAMES_COUNT_KEY, Constants.SKILLZ_DEFAULT_GAMES_COUNT);
            //assign moves per match
            movesPerMatch = GetMatchParamInt(Constants.SKILLZ_MOVES_PER_MATCH_KEY, Constants.SKILLZ_MOVES_PER_MATCH);
            //win points
            winPoints = GetMatchParamInt(Constants.SKILLZ_WIN_POINTS_KEY, Constants.SKILLZ_WIN_POINTS);
            //draw points
            drawPoints = GetMatchParamInt(Constants.SKILLZ_DRAW_POINTS_KEY, Constants.SKILLZ_DRAW_POINTS);
            //win all games bonus points
            winAllGamesBonus = GetMatchParamInt(Constants.SKILLZ_WIN_ALL_GAMES_BONUS_KEY, Constants.SKILLZ_WIN_ALL_GAMES_BONUS);
            //points per second left
            pointsPerSecond = GetMatchParamInt(Constants.SKILLZ_POINTS_PER_SECOND_KEY, Constants.SKILLZ_POINTS_PER_SECOND_REMAINING);
            //points per move left win
            pointsPerMoveLeftWin = GetMatchParamInt(Constants.SKILLZ_POINTS_PER_MOVE_WIN_KEY, Constants.SKILLZ_POINTS_PER_MOVE_LEFT_WIN);
            //points per move left lose
            pointsPerMoveLeftLose = GetMatchParamInt(Constants.SKILLZ_POINTS_PER_MOVE_LOSE_KEY, Constants.SKILLZ_POINTS_PER_MOVE_LEFT_LOSE);
            //points per move left draw
            pointsPerMoveLeftDraw = GetMatchParamInt(Constants.SKILLZ_POINTS_PER_MOVE_DRAW_KEY, Constants.SKILLZ_POINTS_PER_MOVE_LEFT_DRAW);
            //match pauses left
            matchPausesLeft = GetMatchParamInt(Constants.SKILLZ_MATCH_PAUSES_KEY, Constants.SKILLZ_PAUSES_COUNT_PER_MATCH);

            lastGameFinishedAt = GameInitialTimerValue;
            lastGameMovesCount = MovesPerMatch;
            LevelsInfo = new List<SkillzLevelParams>();
            //levels info
            for (int levelIndex = 0; levelIndex < GamesToPlay; levelIndex++)
            {
                SkillzLevelParams info =
                    GetLevelInfo(levelIndex) ??
                    LevelsInfo.Last() ??
                    new SkillzLevelParams() { 
                        areaId = Constants.SKILLZ_DEFAULT_AREA, 
                        complexityLow = Constants.SKILLZ_GAME_COMPLEXITY_LOW, 
                        complexityHigh = Constants.SKILLZ_GAME_COMPLEXITY_HIGH, 
                        oppHerdId = Constants.SKILLZ_DEFAULT_OPP_HERD_ID,
                        craftedBoardPercentage = Constants.SKILLZ_DEFAULT_CRAFTED_BOARD_PERCENTAGE };

                int explicitSeed = ExplicitSeed;
                if (explicitSeed > -1)
                {
                    info.seed = explicitSeed + "";

                    if (levelIndex > 0)
                    {
                        info.seed += "_" + levelIndex;
                    }
                }
                else
                {
                    info.seed = (CurrentMatch?.ID.Value.ToString() ?? "default") + levelIndex;
                }

                LevelsInfo.Add(info);
            }
        }

        public void OnMatchWillBegin(Match matchInfo)
        {
            CancelRoutine("scoreResubmit");

            CurrentMatch = matchInfo;
            LastMatch = matchInfo;
            OngoingMatch = true;
            SubmitRetries = 3;
            InitializeMatchData();

            if (matchInfo.IsCustomSynchronousMatch)
            {
                FourzyPhotonManager.Instance.JoinOrCreateRoom(SkillzCrossPlatform.GetMatchInfo().CustomServerConnectionInfo.MatchId);
            }
            else
            {
                GameManager.Instance.StartGame(GameTypeLocal.ASYNC_SKILLZ_GAME);
            }
        }

        public void OnProgressionRoomEnter()
        {

        }

        public void OnSkillzWillExit()
        {
            if (CloseGameOnBack)
            {
                CloseGameOnBack = false;
                Debug.Log("---------------Close game from returning from Skillz.");
                GamePlayManager.Instance.BackButtonOnClick();
            }
        }

        public void GetProgressionData()
        {
            Debug.Log("---------------Requesting latest player data.");
            SkillzCrossPlatform.GetProgressionUserData(ProgressionNamespace.DEFAULT_PLAYER_DATA, defaultPlayerDataKeys, OnReceivedData, OnReceivedDataFail);
        }

        public void TrySubmitScore()
        {
            CancelRoutine("scoreResubmit");

            SkillzCrossPlatform.SubmitScore(Instance.Points, OnSkillzScoreReported, OnSkillzScoreReportedError);
        }

#if UNITY_EDITOR
        public void Test()
        {
            PlayerData_CashGamesPlayed++;
            PlayerData_GamesPlayed++;

            OnDefaultPlayerDataUpdated?.Invoke();
        }
#endif

        private void OnReceivedData(Dictionary<string, ProgressionValue> values)
        {
            LatestDefaultPlayerData = values;

            string logs = "---------------Player default data received: ";
            foreach (var pair in values)
            {
                logs += $"\n{pair.Key} : {pair.Value.Value}";
            }
            Debug.Log(logs);

            OnDefaultPlayerDataReceived?.Invoke();
        }

        private void OnReceivedDataFail(string error)
        {

        }

        public void FinishGame(bool state, params PointsEntry[] points)
        {
            GamesPlayed.Add(new SkillzGameResult() 
            { 
                state = state, 
                pointsEntries = new List<PointsEntry>(points) 
            });
        }

        public void SetLastGameTimeConsumed(float currentTime)
        {
            float timeConsumed = lastGameFinishedAt - currentTime;
            GamesPlayed.Last().timeConsumed = timeConsumed;

            lastGameFinishedAt = currentTime;
        }

        public void SetLastGameMovesConsumed(int currentMovesCount)
        {
            int movesConsumed = lastGameMovesCount - currentMovesCount;
            GamesPlayed.Last().movesConsumed = movesConsumed;

            lastGameMovesCount = currentMovesCount;
        }

        public void OnMatchFinished()
        {
            CurrentMatch = null;
            OngoingMatch = false;
        }

        public void ForfeitMatch()
        {
            OnMatchFinished();

            CloseGameOnBack = true;
            SkillzCrossPlatform.AbortMatch();
        }

        public void UsePause()
        {
            matchPausesLeft++;
        }

        private int GetMatchParamInt(string key, int defaultValue)
        {
            if (CurrentMatch == null)
            {
                return -1;
            }

            if (CurrentMatch.GameParams.ContainsKey(key))
            {
                return random - (int.TryParse(CurrentMatch.GameParams[key], out int value) ? value : defaultValue);
            }
            else
            {
                return random - defaultValue;
            }
        }

        private string GetMatchParamString(string key, string defaultValue)
        {
            if (CurrentMatch == null)
            {
                return "";
            }

            if (CurrentMatch.GameParams.ContainsKey(key))
            {
                return CurrentMatch.GameParams[key];
            }
            else
            {
                return defaultValue;
            }
        }

        private SkillzLevelParams GetLevelInfo(int levelIndex)
        {
            if (CurrentMatch == null || !CurrentMatch.GameParams.ContainsKey("Level" + levelIndex))
            {
                return null;
            }

            SkillzLevelParams result = null;
            try
            {
                result = JsonConvert.DeserializeObject<SkillzLevelParams>(CurrentMatch.GameParams["Level" + levelIndex]);
            }
            catch (Exception) { }

            return result;
        }

        private void OnSkillzScoreReported()
        {
            Debug.Log("---------------Skillz Score submited. All good.");

/*            //Update last player data.
            if (LastMatch.IsCash ?? false)
            {
                PlayerData_CashGamesPlayed++;
            }
            else
            {
                PlayerData_GamesPlayed++;
            }

            OnDefaultPlayerDataUpdated?.Invoke();*/

            GetProgressionData();
        }

        private void OnSkillzScoreReportedError(string error)
        {
            Debug.Log($"Failed to report Skillz score: {error}");

            StartRoutine("scoreResubmit", SkillzScoreReportHelper());
        }

        /// <summary>
        /// When failed to report score
        /// </summary>
        /// <returns></returns>
        private IEnumerator SkillzScoreReportHelper()
        {
            if (SubmitRetries > 0)
            {
                //wait for 3 seconds for the next try
                yield return new WaitForSeconds(3f);

                SubmitRetries--;
                TrySubmitScore();
            }
            else
            {
                Debug.Log("Failed to report score");
                Debug.Log("Last effort, using DisplayTournamentResultsWithScore");

                if (!ReturnToSkillzCalled)
                {
                    ReturnToSkillzCalled = true;
                    SkillzCrossPlatform.DisplayTournamentResultsWithScore(Points);
                }
            }
        }
    }

    public class SkillzGameResult
    {
        internal bool state;
        internal bool isCraftedBoard;
        internal float timeConsumed;
        internal int movesConsumed;
        internal List<PointsEntry> pointsEntries = new List<PointsEntry>();

        internal int Points => pointsEntries.Sum(_entry => _entry.amount);
    }

    public class PointsEntry
    {
        internal string name;
        internal int amount;

        public PointsEntry(string name, int amount)
        {
            this.name = name;
            this.amount = amount;
        }
    }

    [Serializable]
    public class SkillzLevelParams
    {
        public int complexityLow;
        public int complexityHigh;
        public int areaId;
        public string oppHerdId;
        public int aiProfile;
        public string craftedBoardPercentage;
        public bool isCraftedBoard;
        public string seed;
    }
}
