using UnityEngine;
using UnityEngine.UI;
using HedgehogTeam.EasyTouch;
using System.Collections.Generic;

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
                GameManager.Instance.OpenGame(game);
                Hide();
            }
            else
            {
                ViewMatchMaking.instance.isRealtime = false;
                ViewController.instance.ChangeView(ViewController.instance.viewMatchMaking);
            }

            ViewController.instance.HideTabView();
            ViewController.instance.headerUI.SetActive(false);
        }

        public void FastPlayButton() {
            ViewMatchMaking.instance.isRealtime = true;
            ViewController.instance.ChangeView(ViewController.instance.viewMatchMaking);
            ViewController.instance.HideTabView();
            ViewController.instance.headerUI.SetActive(false);
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
            ViewController.instance.ChangeView(ViewController.instance.viewPuzzleSelection);
            ViewController.instance.HideTabView();
            ViewController.instance.headerUI.SetActive(false);
        }

        public void SettingsButton() {
            ViewController.instance.ChangeView(ViewController.instance.viewSettings);
            ViewController.instance.HideTabView();
        }

        public void AreaSelectButtonPress() {
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

            gamePieceImage.sprite = GameContentManager.Instance.GetGamePieceSprite(user.gamePieceId);
            gamePieceNameLabel.text = GameContentManager.Instance.GetGamePieceName(user.gamePieceId);
        }

        void UserManager_OnUpdateUserInfo()
        {
            UserManager user = UserManager.Instance;

            ratingEloLabel.text = user.ratingElo.ToString();
            userNameLabel.text = user.userName;
        }

        void UserManager_OnUpdateUserGamePieceID(int gamePieceId)
        {
            gamePieceImage.sprite = GameContentManager.Instance.GetGamePieceSprite(gamePieceId);
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
