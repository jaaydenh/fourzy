//@vadym udod

using Fourzy._Updates._Tutorial;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Toasts;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PlayScreen : MenuTab
    {
        private const string kLobbyScreenOpened = "lobbyScreenOpened";

        [SerializeField]
        private ButtonExtended discordButton;
        [SerializeField]
        private RectTransform body;

        private OnIPhoneX onIPhoneX;
        private InputFieldPrompt inputPromptScreen;

        private bool fastPuzzlesUnlocked;
        private bool gauntletGameUnlocked = false;

        public override void OnBack()
        {
            base.OnBack();

            if (!tabsParent)
            {
                Application.Quit();
                Debug.Log("App close");
            }
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

        public void StartTutorialAdventure() => menuController.GetOrAddScreen<ProgressionMapScreen>().Open(GameContentManager.Instance.progressionMaps[0]);

        public void ResetTutorial() =>
            PersistantMenuController.Instance.GetOrAddScreen<OnboardingScreen>()
                .OpenTutorial(HardcodedTutorials.GetByName((GameManager.Instance.Landscape ?
                    "OnboardingLandscape" :
                    "Onboarding")));

        public void OpenFastPuzzleScreen()
        {
            //check
            if (!fastPuzzlesUnlocked)
            {
                menuController.GetOrAddScreen<PromptScreen>()
                    .Prompt(LocalizationManager.Value("puzzle_Ladder_title"),
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

        public void OpenDiscord()
        {
            discordButton.GetBadge("attention").badge.Hide();
            PlayerPrefs.SetInt("discord_button_pressed", 1);
            GameManager.Instance.OpenDiscordPage();
        }

        public void OpenOnlineLobby()
        {
            //if (PlayerPrefsWrapper.GetBool(kLobbyScreenOpened))
            //{
            //    menuController.GetOrAddScreen<LobbyScreen>().CheckLobby();
            //}
            //else
            //{
            //    menuController.GetOrAddScreen<ChangeNamePromptScreen>()._Prompt();

            //    PlayerPrefsWrapper.SetBool(kLobbyScreenOpened, true);
            //}

            menuController.GetOrAddScreen<TwoOptionsPromptScreen>()
                ._Prompt(LocalizationManager.Value("private_match"), "", LocalizationManager.Value("create_game"), LocalizationManager.Value("join_game"),
                () =>
                {
                    PersistantMenuController.Instance.GetOrAddScreen<CreateRealtimeGamePrompt>().Prompt();
                },
                null,
                () =>
                {
                    inputPromptScreen = menuController.GetOrAddScreen<InputFieldPrompt>();
                    inputPromptScreen
                        ._Prompt((value) =>
                        {
                            FourzyPhotonManager.onJoinRoomFailed += OnJoinRoomFailed;
                            FourzyPhotonManager.onJoinedRoom += OnJoinedRoom;

                            //try join
                            FourzyPhotonManager.JoinRoom(value);

                        }, LocalizationManager.Value("room_code"), "", LocalizationManager.Value("join_game"), LocalizationManager.Value("close"), decline: () =>
                        {
                            FourzyPhotonManager.onJoinRoomFailed -= OnJoinRoomFailed;
                            FourzyPhotonManager.onJoinedRoom -= OnJoinedRoom;
                        })
                        .CloseOnDecline();
                })
                .CloseOnAccept();
        }

        public void StartRealtimeQuickmatch()
        {
            GameManager.Instance.StartRealtimeQuickGame();
        }

        public void ChangeName() =>
            menuController.GetOrAddScreen<ChangeNamePromptScreen>()._Prompt();

        private void OnJoinedRoom(string roomName)
        {
            inputPromptScreen.Decline();
        }

        private void OnJoinRoomFailed(string roomName)
        {
            GamesToastsController.ShowTopToast(LocalizationManager.Value("wrong_room_code"));
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (body)
            {
                onIPhoneX = body.GetComponent<OnIPhoneX>();
            }

            if (onIPhoneX)
            {
                onIPhoneX.CheckPlatform();
            }

            //force open
            if (GameManager.Instance.Landscape && @default)
            {
                Open();
            }

            discordButton
                .GetBadge("attention")
                .badge
                .SetState(PlayerPrefs.GetInt("discord_button_pressed", 0) == 0);
        }
    }
}