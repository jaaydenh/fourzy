//@vadym udod

using Fourzy._Updates.Mechanics.GameplayScene;
using Newtonsoft.Json;
using SkillzSDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fourzy._Updates.Managers
{
    public class SkillzGameController : SkillzMatchDelegate
    {
        public static SkillzGameController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SkillzGameController();
                }

                return instance;
            }
        }
        private static SkillzGameController instance;

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
                        oppHerdId = Constants.SKILLZ_DEFAULT_OPP_HERD_ID };

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
            CurrentMatch = matchInfo;
            LastMatch = matchInfo;
            OngoingMatch = true;
            SubmitRetries = 3;
            InitializeMatchData();

            GameManager.Instance.StartGame(GameTypeLocal.ASYNC_SKILLZ_GAME);
        }

        public void OnProgressionRoomEnter()
        {

        }

        public void OnSkillzWillExit()
        {
            if (CloseGameOnBack)
            {
                CloseGameOnBack = false;
                GamePlayManager.Instance.BackButtonOnClick();
            }
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
    }

    public class SkillzGameResult
    {
        internal bool state;
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

        public string seed;
    }
}
