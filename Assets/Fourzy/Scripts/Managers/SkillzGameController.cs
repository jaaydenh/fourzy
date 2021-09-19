//@vadym udod

using Fourzy._Updates.Mechanics.GameplayScene;
using SkillzSDK;

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

        private bool closeGameOnBack = false;

        internal Match CurrentMatch { get; set; }
        internal bool OngoingMatch { get; set; }
        internal float GetGameTimerValue
        {
            get
            {
                if (CurrentMatch.GameParams.ContainsKey(Constants.SKILLZ_GAME_TIMER_KEY))
                {
                    return float.TryParse(CurrentMatch.GameParams[Constants.SKILLZ_GAME_TIMER_KEY], out float _timer) ? _timer : Constants.SKILLZ_DEFAULT_GAME_TIMER;
                }
                else
                {
                    return Constants.SKILLZ_DEFAULT_GAME_TIMER;
                }
            }
        }
        internal int GetGamesCountValue
        {
            get
            {
                if (CurrentMatch.GameParams.ContainsKey(Constants.SKILLZ_GAMES_COUNT_KEY))
                {
                    return int.TryParse(CurrentMatch.GameParams[Constants.SKILLZ_GAMES_COUNT_KEY], out int _gamesCount) ? _gamesCount : Constants.SKILLZ_DEFAULT_GAMES_COUNT;
                }
                else
                {
                    return Constants.SKILLZ_DEFAULT_GAMES_COUNT;
                }
            }
        }

        public void OnMatchWillBegin(Match matchInfo)
        {
            CurrentMatch = matchInfo;
            OngoingMatch = true;

            GameManager.Instance.StartGame(GameTypeLocal.ASYNC_SKILLZ_GAME);
        }

        public void OnProgressionRoomEnter()
        {

        }

        public void OnSkillzWillExit()
        {
            if (closeGameOnBack)
            {
                closeGameOnBack = false;
                GamePlayManager.Instance.BackButtonOnClick();
            }
        }

        public void FinishMatch()
        {
            CurrentMatch = null;
            OngoingMatch = false;
        }

        public void ForfeitMatch()
        {
            FinishMatch();

            closeGameOnBack = true;
            SkillzCrossPlatform.AbortMatch();
        }
    }
}
