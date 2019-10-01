//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.UI.Widgets;
using System.Collections.Generic;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PlayScreen : MenuTab
    {
        public PlayerInfoWidget playerInfoIwdget;

        private MatchmakingScreen matchmakingScreen;

        protected override void Awake()
        {
            base.Awake();
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

        public void StartTutorialAdventure() => menuController.GetScreen<ProgressionMapScreen>().Open(GameContentManager.Instance.progressionMaps[0]);

        public void OpenNews() => menuController.GetScreen<NewsPromptScreen>()._Prompt();

        protected override void OnInitialized()
        {
            base.OnInitialized();

            matchmakingScreen = menuController.GetScreen<MatchmakingScreen>();
        }
    }
}