//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using System.Collections.Generic;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PlayScreen : MenuTab
    {
        //public PlayerInfoWidget playerInfoIwdget;
        public ButtonExtended fastPuzzleButton;
        public ButtonExtended gauntletGameButton;

        private MatchmakingScreen matchmakingScreen;

        private bool fastPuzzlesUnlocked;
        private bool gauntletGameUnlocked;

        public override void Open()
        {
            base.Open();

            fastPuzzlesUnlocked = PlayerPrefsWrapper.GetRewardRewarded("unlock_fast_puzzles_mode");
            fastPuzzleButton.GetBadge("locked").badge.SetState(!fastPuzzlesUnlocked);

            //if (fastPuzzlesUnlocked)
            //    fastPuzzleButton.SetMaterial(null);
            //else
            //    fastPuzzleButton.SetGreyscaleMaterial();

            gauntletGameUnlocked = PlayerPrefsWrapper.GetRewardRewarded("unlock_gauntlet_mode");
            gauntletGameButton.GetBadge("locked").badge.SetState(!gauntletGameUnlocked);

            //if (gauntletGameUnlocked)
            //    gauntletGameButton.SetMaterial(null);
            //else
            //    gauntletGameButton.SetGreyscaleMaterial();
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

        public void StartGauntletAIPack()
        {
            //check
            if (!gauntletGameUnlocked)
            {
                menuController.GetOrAddScreen<PromptScreen>().Prompt("The Gauntlet Locked",
                    "The gauntlet is where you will play a series of more difficult opponents. Defeat them all to win!",
                    "OK",
                    null,
                    () => menuController.CloseCurrentScreen(),
                    null);
                return;
            }

            menuController.GetOrAddScreen<GauntletIntroScreen>()._Prompt();
        }

        public void StartTurnGame() => matchmakingScreen.OpenTurnbased();

        public void StartTutorialAdventure() => menuController.GetOrAddScreen<ProgressionMapScreen>().Open(GameContentManager.Instance.progressionMaps[0]);

        public void OpenFastPuzzleScreen()
        {
            //check
            if (!fastPuzzlesUnlocked)
            {
                menuController.GetOrAddScreen<PromptScreen>().Prompt(LocalizationManager.Value("puzzle_Ladder_title"),
                    LocalizationManager.Value("puzzle_Ladder_desc"),
                    "OK",
                    null,
                    () => menuController.CloseCurrentScreen(),
                    null);
                return;
            }

            tabsParent.menuController.OpenScreen<FastPuzzlesScreen>();
        }

        public void OpenNews() => menuController.GetOrAddScreen<NewsPromptScreen>()._Prompt();

        protected override void OnInitialized()
        {
            base.OnInitialized();

            matchmakingScreen = menuController.GetOrAddScreen<MatchmakingScreen>();
        }
    }
}