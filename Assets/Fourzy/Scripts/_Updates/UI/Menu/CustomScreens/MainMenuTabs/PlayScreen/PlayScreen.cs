//@vadym udod

using Fourzy._Updates._Tutorial;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.UI.Helpers;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PlayScreen : MenuTab
    {
        private const string kLobbyScreenOpened = "lobbyScreenOpened";

        public ButtonExtended discordButton;
        public RectTransform body;
        public RectTransform pieceParent;

        public TMP_Text playerNameLabel;
        public TMP_Text pieceNameLabel;
        public TMP_Text ratingLabel;

        public TMP_Text winsLabel;
        public TMP_Text losesLabel;
        public TMP_Text drawsLabel;

        private OnIPhoneX onIPhoneX;
        private GamePieceView currentGamepiece;

        private bool fastPuzzlesUnlocked;
        private bool gauntletGameUnlocked;

        protected override void Start()
        {
            base.Start();

            UserManager.onRatingUpdate += OnRatingUpate;
            UserManager.onDisplayNameChanged += OnUpdateUserInfo;
            UserManager.OnUpdateUserGamePieceID += OnUpdateUserGamePieceID;
            UserManager.onWinsUpdate += OnWinsUpdate;
            UserManager.onLosesUpdate += OnLosesUpdate;
            UserManager.onDrawsUpdate += OnDrawsUpdate;
        }

        protected void OnDestroy()
        {
            UserManager.onRatingUpdate -= OnRatingUpate;
            UserManager.onDisplayNameChanged -= OnUpdateUserInfo;
            UserManager.OnUpdateUserGamePieceID -= OnUpdateUserGamePieceID;
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
                menuController.GetOrAddScreen<LobbyScreen>().CheckLobby();
            //}
            //else
            //{
            //    menuController.GetOrAddScreen<ChangeNamePromptScreen>()._Prompt();

            //    PlayerPrefsWrapper.SetBool(kLobbyScreenOpened, true);
            //}
        }

        public void StartRealtimeQuickmatch() => menuController.GetScreen<MatchmakingScreen>().OpenRealtime();

        public void ChangeName() =>
            menuController.GetOrAddScreen<ChangeNamePromptScreen>()._Prompt();

        private void OnUpdateUserInfo()
        {
            playerNameLabel.text = UserManager.Instance.userName;
        }

        private void OnRatingUpate(int rating)
        {
            if (UserManager.Instance.lastCachedRating == -1)
            {

                ratingLabel.text = $"{LocalizationManager.Value("rating")}: ...";
            }
            else
            {
                ratingLabel.text = $"{LocalizationManager.Value("rating")}: {UserManager.Instance.lastCachedRatingFiltered}";
            }
        }

        private void OnUpdateUserGamePieceID(string gamePieceID)
        {
            if (currentGamepiece)
            {
                Destroy(currentGamepiece.gameObject);
            }

            GamePieceData _data = GameContentManager.Instance.piecesDataHolder.GetGamePieceData(gamePieceID);
            currentGamepiece = Instantiate(_data.player1Prefab, pieceParent);

            currentGamepiece.transform.localPosition = Vector3.zero;
            currentGamepiece.StartBlinking();

            pieceNameLabel.text = _data.name;
        }

        private void OnWinsUpdate(int wins)
        {
            winsLabel.text = wins + "";
        }

        private void OnLosesUpdate(int loses)
        {
            losesLabel.text = loses + "";
        }

        private void OnDrawsUpdate(int draw)
        {
            drawsLabel.text = draw + "";
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

            UserManager user = UserManager.Instance;
            OnUpdateUserGamePieceID(user.gamePieceID);
            OnUpdateUserInfo();

            OnRatingUpate(-1);
            OnWinsUpdate(UserManager.Instance.playfabWinsCount);
            OnLosesUpdate(UserManager.Instance.playfabLosesCount);
            OnDrawsUpdate(UserManager.Instance.playfabDrawsCount);
        }
    }
}