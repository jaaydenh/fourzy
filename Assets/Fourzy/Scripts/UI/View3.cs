using UnityEngine;
using UnityEngine.UI;
using HedgehogTeam.EasyTouch;
using System.Collections.Generic;
using mixpanel;

namespace Fourzy
{
    public class View3 : UIView
    {
        //Instance
        public static View3 instance;
        public GameObject areaSelectButton;

        [SerializeField] Text gamePieceNameLabel;
        [SerializeField] Text ratingEloLabel;
        [SerializeField] Text userNameLabel;
        [SerializeField] Image gamePieceImage;
        [SerializeField] Badge homeScreenPlayBadge;

        void Start()
        {
            instance = this;
            keepHistory = true;
            ViewController.instance.currentActiveView = TotalView.view3;

            if (PlayerPrefs.GetInt("onboardingStage") <= 1)
            {
                ViewController.instance.view3.Hide();
                ViewController.instance.HideTabView();

                GameManager.Instance.isOnboardingActive = true;
                GameManager.Instance.shouldLoadOnboarding = true;
                GameManager.Instance.OpenNewGame(GameType.PASSANDPLAY, null, false, "100");
                // GameManager.Instance.onboardingScreen.StartOnboarding();
            } else if (PlayerPrefs.GetInt("puzzleChallengeLevel") <= 2)
            {
                //ViewController.instance.view3.Hide();
                //ViewController.instance.HideTabView();
                //GameManager.Instance.OpenPuzzleChallengeGame();
            }

            Button btn = areaSelectButton.GetComponent<Button>();
            btn.onClick.AddListener(AreaSelectButtonPress);
        }

        public override void Show()
        {
            base.Show();

            ViewController.instance.headerUI.SetActive(true);

            UserManager.OnUpdateUserInfo += UserManager_OnUpdateUserInfo;
            UserManager.OnUpdateUserGamePieceID += UserManager_OnUpdateUserGamePieceID;
            GameManager.OnUpdateGames += GameManager_OnUpdateGames;

            this.UpdatePlayButtonBadgeCount();

            UpdateUI();
        }

        public override void Hide()
        {
            UserManager.OnUpdateUserInfo -= UserManager_OnUpdateUserInfo;
            UserManager.OnUpdateUserGamePieceID -= UserManager_OnUpdateUserGamePieceID;
            GameManager.OnUpdateGames -= GameManager_OnUpdateGames;

            base.Hide();
        }

        public override void ShowAnimated(AnimationDirection sourceDirection)
        {
            ViewController.instance.SetActiveView(TotalView.view3);
            base.ShowAnimated(sourceDirection);
        }

        public override void HideAnimated(AnimationDirection getAwayDirection)
        {
            base.HideAnimated(getAwayDirection);
        }

        public void TurnPlayButton()
        {
            Game game = GameManager.Instance.GetNextActiveGame();
            if (game != null)
            {
                var props = new Value();
                props["HasGameWithMove"] = "True";
                Mixpanel.Track("Turn Play Button Press", props);
                GameManager.Instance.OpenGame(game);
                Hide();
            }
            else
            {
                var props = new Value();
                props["HasGameWithMove"] = "False";
                Mixpanel.Track("Turn Play Button Press", props);
                ViewMatchMaking.instance.isRealtime = false;
                ViewController.instance.ChangeView(ViewController.instance.viewMatchMaking);
            }

            ViewController.instance.HideTabView();
            ViewController.instance.headerUI.SetActive(false);
        }

        public void FastPlayButton() {
            System.DayOfWeek today = System.DateTime.Now.DayOfWeek;
            int hour = System.DateTime.UtcNow.Hour;
            bool isDLS = System.DateTime.UtcNow.IsDaylightSavingTime();
            // Debug.Log("utc hour: " + hour + " isDLS: "+ isDLS + " today: " + today);

            if (today == System.DayOfWeek.Monday || today == System.DayOfWeek.Wednesday || today == System.DayOfWeek.Friday) {

                if ((!isDLS && hour == 1) || (isDLS && hour == 0)) {
                    var props = new Value();
                    props["Source"] = "Home Screen";
                    Mixpanel.Track("Start Realtime Matchmaking", props);

                    ViewMatchMaking.instance.isRealtime = true;
                    ViewController.instance.ChangeView(ViewController.instance.viewMatchMaking);
                    ViewController.instance.HideTabView();
                    ViewController.instance.headerUI.SetActive(false);
                } else {
                    Mixpanel.Track("Open Realtime Confirmation");
                    PopupManager.Instance.OpenPopup<ConfirmPopup>();
                }
            } else {
                Mixpanel.Track("Open Realtime Confirmation");
                PopupManager.Instance.OpenPopup<ConfirmPopup>();
            }
        }

        public void PlayButtonOld() {
            Game game = GameManager.Instance.GetNextActiveGame();
            if (game != null)
            {
                GameManager.Instance.OpenGame(game);
                Hide();
            }
            else
            {
                ViewController.instance.ChangeView(ViewController.instance.viewMatchMaking);
            }

            ViewController.instance.HideTabView();
            ViewController.instance.headerUI.SetActive(false);
        }

        public void TrainingButton() {
            Mixpanel.Track("Puzzle Button Press");
            ViewController.instance.ChangeView(ViewController.instance.viewPuzzleSelection);
            ViewController.instance.HideTabView();
            ViewController.instance.headerUI.SetActive(false);
        }

        public void SettingsButton() {
            Mixpanel.Track("Settings Button Press");
            ViewController.instance.ChangeView(ViewController.instance.viewSettings);
            ViewController.instance.HideTabView();
        }

        public void AreaSelectButtonPress() {
            Mixpanel.Track("Area Select Button Press");
            ViewController.instance.ChangeView(ViewController.instance.viewAreaSelect);
            ViewController.instance.HideTabView();
        }

        void UpdateUI()
        {
            UserManager user = UserManager.Instance;

            ratingEloLabel.text = user.ratingElo.ToString();
            if (user.userName != string.Empty)
            {
                userNameLabel.text = user.userName;
            }

            UpdateGamePieceIcon(user.gamePieceId);

            gamePieceNameLabel.text = GameContentManager.Instance.GetGamePieceName(user.gamePieceId);
            areaSelectButton.GetComponent<Image>().sprite = GameContentManager.Instance.GetCurrentTheme().Preview;
        }

        void UpdateGamePieceIcon(int gamePieceId)
        {
            gamePieceImage.enabled = false;
            foreach (Transform t in gamePieceImage.transform)
            {
                Destroy(t.gameObject);
            }
            GamePiece prefab = GameContentManager.Instance.GetGamePiecePrefab(gamePieceId);
            GamePiece pieceIcon = Instantiate(prefab, gamePieceImage.transform, false);
            pieceIcon.CachedTransform.localPosition = Vector3.zero;
            pieceIcon.CachedTransform.localScale = new Vector3(100, 100, 100);
            pieceIcon.CachedGO.SetLayerRecursively(gamePieceImage.gameObject.layer);
            pieceIcon.View.StartBlinking();
        }

        void UserManager_OnUpdateUserInfo()
        {
            UserManager user = UserManager.Instance;

            ratingEloLabel.text = user.ratingElo.ToString();
            userNameLabel.text = user.userName;
        }

        void UserManager_OnUpdateUserGamePieceID(int gamePieceId)
        {
            UpdateGamePieceIcon(gamePieceId);
            gamePieceNameLabel.text = GameContentManager.Instance.GetGamePieceName(gamePieceId);
        }

        void GameManager_OnUpdateGames(List<Game> games)
        {
            this.UpdatePlayButtonBadgeCount();
        }

        private void UpdatePlayButtonBadgeCount()
        {
            var games = GameManager.Instance.Games;

            int activeGamesCount = 0;

            for (int i = 0; i < games.Count; i++)
            {
                if (games[i].gameState.isCurrentPlayerTurn == true || (games[i].didViewResult == false && games[i].gameState.IsGameOver == true))
                {
                    activeGamesCount++;
                }
            }

            if (activeGamesCount > 0)
            {
                homeScreenPlayBadge.gameObject.SetActive(true);
            }
            else
            {
                homeScreenPlayBadge.gameObject.SetActive(false);
            }

            homeScreenPlayBadge.SetGameCount(activeGamesCount);
        }
    }
}
