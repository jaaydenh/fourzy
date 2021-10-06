//@vadym udod

using Fourzy._Updates.Mechanics.GameplayScene;
using FourzyGameModel.Model;
using SkillzSDK;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        private int pointsPerSecond;
        private int pointsPerMoveLeftWin;
        private int pointsPerMoveLeftLose;
        private int pointsPerMoveLeftDraw;
        private int gameComplexity;
        private int matchPausesLeft;

        internal Match CurrentMatch { get; set; }
        internal bool OngoingMatch { get; set; }
        internal float GameInitialTimerValue => random - timer;
        internal int WinPoints => random - winPoints;
        internal int PointsPerSecond => random - pointsPerSecond;
        internal int PointsPerMoveLeftWin => random - pointsPerMoveLeftWin;
        internal int PointsPerMoveLeftLose => random - pointsPerMoveLeftLose;
        internal int PointsPerMoveLeftDraw => random - pointsPerMoveLeftDraw;
        internal int GameComplexity => random - gameComplexity;
        internal int GamesToPlay => random - gamesToPlay;
        internal int MovesPerMatch => random - movesPerMatch;
        internal int MatchPausesLeft => random - matchPausesLeft;
        internal List<SkillzGameResult> GamesPlayed { get; } = new List<SkillzGameResult>();
        internal int Points => GamesPlayed.Sum(game => game.Points);
        internal bool HaveNextGame => GamesPlayed.Count < GamesToPlay;
        internal bool CloseGameOnBack { get; set; }

        public void InitializeMatchData()
        {
            random = Random.Range(10000, 99999);
            GamesPlayed.Clear();

            //assign timer value
            timer = GetMatchParamInt(Constants.SKILLZ_GAME_TIMER_KEY, Constants.SKILLZ_DEFAULT_GAME_TIMER);
            //assign games to play
            gamesToPlay = GetMatchParamInt(Constants.SKILLZ_GAMES_COUNT_KEY, Constants.SKILLZ_DEFAULT_GAMES_COUNT);
            //assign moves per match
            movesPerMatch = GetMatchParamInt(Constants.SKILLZ_MOVES_PER_MATCH_KEY, Constants.SKILLZ_MOVES_PER_MATCH);
            //win points
            winPoints = GetMatchParamInt(Constants.SKILLZ_WIN_POINTS_KEY, Constants.SKILLZ_WIN_POINTS);
            //points per second left
            pointsPerSecond = GetMatchParamInt(Constants.SKILLZ_POINTS_PER_SECOND_KEY, Constants.SKILLZ_POINTS_PER_SECOND_REMAINING);
            //points per move left win
            pointsPerMoveLeftWin = GetMatchParamInt(Constants.SKILLZ_POINTS_PER_MOVE_WIN_KEY, Constants.SKILLZ_POINTS_PER_MOVE_LEFT_WIN);
            //points per move left lose
            pointsPerMoveLeftLose = GetMatchParamInt(Constants.SKILLZ_POINTS_PER_MOVE_LOSE_KEY, Constants.SKILLZ_POINTS_PER_MOVE_LEFT_LOSE);
            //points per move left draw
            pointsPerMoveLeftDraw = GetMatchParamInt(Constants.SKILLZ_POINTS_PER_MOVE_DRAW_KEY, Constants.SKILLZ_POINTS_PER_MOVE_LEFT_DRAW);
            //game complexity
            gameComplexity = GetMatchParamInt(Constants.SKILLZ_GAME_COMPLEXITY_KEY, Constants.SKILLZ_GAME_COMPLEXITY);
            //match pauses left
            matchPausesLeft = GetMatchParamInt(Constants.SKILLZ_MATCH_PAUSES_KEY, Constants.SKILLZ_PAUSES_COUNT_PER_MATCH);
        }

        public void OnMatchWillBegin(Match matchInfo)
        {
            CurrentMatch = matchInfo;
            OngoingMatch = true;
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
            GamesPlayed.Add(new SkillzGameResult() { state = state, pointsEntries = new List<PointsEntry>(points) });
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
    }

    public class SkillzGameResult
    {
        internal bool state;
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
}
