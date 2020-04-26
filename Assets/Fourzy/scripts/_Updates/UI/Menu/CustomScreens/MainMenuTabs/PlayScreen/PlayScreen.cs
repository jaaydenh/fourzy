//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.UI.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PlayScreen : MenuTab
    {
        public RectTransform body;
        public RectTransform portalsHolder;
        public ButtonExtended fastPuzzleButton;
        public ButtonExtended gauntletGameButton;

        private MatchmakingScreen matchmakingScreen;
        private OnIPhoneX onIPhoneX;

        private bool fastPuzzlesUnlocked;
        private bool gauntletGameUnlocked;

        public override void Open()
        {
            base.Open();

            fastPuzzlesUnlocked = PlayerPrefsWrapper.GetRewardRewarded("unlock_fast_puzzles_mode");
            fastPuzzleButton.GetBadge("locked").badge.SetState(!fastPuzzlesUnlocked);

            gauntletGameUnlocked = PlayerPrefsWrapper.GetRewardRewarded("unlock_gauntlet_mode");
            gauntletGameButton.GetBadge("locked").badge.SetState(!gauntletGameUnlocked);
        }

        public void ContinueStartTurnBasedGame()
        {
            // List<ChallengeData> next = ChallengeManager.Instance.NextChallenges;

            //open game
            // if (next.Count > 0)
            //     GameManager.Instance.StartGame(next[0].GetGameForPreviousMove());
            // else
            //     StartTurnGame();
        }

        public void StartGauntletAIPack()
        {
            //check
            if (!gauntletGameUnlocked)
            {
                menuController.GetOrAddScreen<PromptScreen>().Prompt(LocalizationManager.Value("gauntlet_title"),
                    LocalizationManager.Value("gauntlet_desc"),
                    LocalizationManager.Value("ok"),
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
                    LocalizationManager.Value("ok"),
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
            onIPhoneX = body.GetComponent<OnIPhoneX>();

            //configure body position
            if (GameManager.Instance.Landscape)
            {
                body.pivot = Vector2.one * .5f;
                body.anchorMin = body.anchorMax = Vector2.one * .5f;

                body.anchoredPosition = Vector2.zero;

                //portals
                portalsHolder.gameObject.SetActive(!GameManager.Instance.hidePortalWidgets);
            }
            else
            {
                body.pivot = new Vector2(.5f, 1f);
                body.anchorMin = Vector2.up;
                body.anchorMax = Vector2.one;

                body.offsetMin = Vector2.zero;

                onIPhoneX.CheckPlatform();
            }
        }
    }
}