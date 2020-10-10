//@vadym udod

using Fourzy._Updates._Tutorial;
using Fourzy._Updates.ClientModel;
using Fourzy._Updates.UI.Helpers;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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

            fastPuzzlesUnlocked = PlayerPrefsWrapper.GetRewardRewarded("unlock_fast_puzzles_mode") || GameManager.Instance.defaultPuzzlesState;
            fastPuzzleButton.GetBadge("locked").badge.SetState(!fastPuzzlesUnlocked);

            gauntletGameUnlocked = PlayerPrefsWrapper.GetRewardRewarded("unlock_gauntlet_mode") || GameManager.Instance.defaultGauntletState;
            gauntletGameButton.GetBadge("locked").badge.SetState(!gauntletGameUnlocked);
        }

        public override void OnBack()
        {
            base.OnBack();

            if (!tabsParent)
            {
                Application.Quit();
                Debug.Log("App close");
            }
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

        public void ResetTutorial() => 
            PersistantMenuController.instance.GetOrAddScreen<OnboardingScreen>().OpenTutorial(HardcodedTutorials.GetByName((GameManager.Instance.Landscape ? "OnboardingLandscape" : "Onboarding")));

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

        public void OpenDiscord() => Application.OpenURL(/*UnityWebRequest.EscapeURL(*/"https://discord.gg/nMZ3MgE"/*)*/);

        public void OpenOnlineLobby() => PersistantMenuController.instance.GetOrAddScreen<LobbyScreen>().CheckLobby();

        public void StartRealtime() => menuController.GetScreen<MatchmakingScreen>().OpenRealtime();

        protected override void OnInitialized()
        {
            base.OnInitialized();

            matchmakingScreen = menuController.GetOrAddScreen<MatchmakingScreen>();
            if (body) onIPhoneX = body.GetComponent<OnIPhoneX>();
            if (portalsHolder) portalsHolder.gameObject.SetActive(!GameManager.Instance.hidePortalWidgets);
            if (onIPhoneX) onIPhoneX.CheckPlatform();

            //force open
            if (GameManager.Instance.Landscape && @default) Open();
        }
    }
}