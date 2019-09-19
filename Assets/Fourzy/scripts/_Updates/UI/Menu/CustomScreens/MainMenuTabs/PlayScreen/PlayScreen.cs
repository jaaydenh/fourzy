//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using System.Collections.Generic;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PlayScreen : MenuTab
    {
        public PlayerInfoWidget playerInfoIwdget;
        public ButtonExtended turnPlayButton;
        public ButtonExtended newsButton;

        private MatchmakingScreen matchmakingScreen;

        protected override void Awake()
        {
            base.Awake();

            ChallengeManager.OnChallengesUpdate += OnChallengesUpdate;
            ChallengeManager.OnChallengeUpdate += OnChallengeUpdate;
            ChallengeManager.OnChallengeUpdateLocal += OnChallengeUpdate;

            GameManager.onNewsFetched += OnNewsFetched;
        }

        protected override void Start()
        {
            base.Start();

            matchmakingScreen = menuController.GetScreen<MatchmakingScreen>();

            turnPlayButton.GetBadge("games").badge.SetValue(ChallengeManager.Instance.NextChallenges.Count);
        }

        public void ContinueStartTurnBasedGame()
        {
            List<ChallengeData> next = ChallengeManager.Instance.NextChallenges;

            //open game
            if (next.Count > 0)
                GameManager.Instance.StartGame(next[0].GetGameForPreviousMove());
            else
                StartTurnGame();
        }

        public void StartGauntletAIPack() => menuController.GetScreen<VSGamePrompt>().Prompt(5);

        public void StartFastPuzzleGame() => GameManager.Instance.StartGame(GameContentManager.Instance.GetFastPuzzle());

        public void StartTurnGame() => matchmakingScreen.OpenTurnbased();

        public void OpenNews() => menuController.GetScreen<NewsPromptScreen>()._Prompt();

        private void OnChallengesUpdate(List<ChallengeData> data) => turnPlayButton.GetBadge("games").badge.SetValue(ChallengeManager.Instance.NextChallenges.Count);

        private void OnChallengeUpdate(ChallengeData data) => turnPlayButton.GetBadge("games").badge.SetValue(ChallengeManager.Instance.NextChallenges.Count);

        private void OnNewsFetched()
        {
            int newsCount = GameManager.Instance.latestNews.Count;

            newsButton.GetBadge().badge.SetValue(newsCount);
            newsButton.SetActive(newsCount > 0);
        }
    }
}