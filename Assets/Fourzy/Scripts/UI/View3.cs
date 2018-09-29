using UnityEngine;
using UnityEngine.UI;
using HedgehogTeam.EasyTouch;

namespace Fourzy
{
    public class View3 : UIView
    {
        //Instance
        public static View3 instance;
        public GameObject areaSelectButton;

        // Use this for initialization
        void Start()
        {
            instance = this;
            keepHistory = true;
            ViewController.instance.currentActiveView = TotalView.view3;

            if (PlayerPrefs.GetInt("onboardingStage") <= 1)
            {
                ViewController.instance.view3.Hide();
                ViewController.instance.HideTabView();

                GameManager.instance.isOnboardingActive = true;
                GameManager.instance.shouldLoadOnboarding = true;
                GameManager.instance.OpenNewGame(GameType.PASSANDPLAY, null, false, "100");
                // GameManager.instance.onboardingScreen.StartOnboarding();
            } else if (PlayerPrefs.GetInt("puzzleChallengeLevel") <= 2)
            {
                //ViewController.instance.view3.Hide();
                //ViewController.instance.HideTabView();
                //GameManager.instance.OpenPuzzleChallengeGame();
            }

            Button btn = areaSelectButton.GetComponent<Button>();
            btn.onClick.AddListener(AreaSelectButtonPress);
        }

        public override void Show()
        {
            base.Show();
            GameManager.instance.headerUI.SetActive(true);
        }

        public override void Hide()
        {
            //EasyTouch.On_SwipeStart -= On_SwipeStart;
            //EasyTouch.On_Swipe -= On_Swipe;
            //EasyTouch.On_SwipeEnd -= On_SwipeEnd;
            base.Hide();
        }

        public override void ShowAnimated(AnimationDirection sourceDirection)
        {
            //ViewController.instance.currentActiveView = TotalView.view3;
            ViewController.instance.SetActiveView(TotalView.view3);
            base.ShowAnimated(sourceDirection);
        }

        public override void HideAnimated(AnimationDirection getAwayDirection)
        {
            base.HideAnimated(getAwayDirection);
        }

        // At the swipe beginning 
        private void On_SwipeStart(Gesture gesture)
        {
        }

        // During the swipe
        private void On_Swipe(Gesture gesture)
        {
        }

        // At the swipe end 
        private void On_SwipeEnd(Gesture gesture)
        {
            float angles = gesture.GetSwipeOrDragAngle();
            if (gesture.swipe == EasyTouch.SwipeDirection.Left)
            {
                View3.instance.HideAnimated(AnimationDirection.right);
                View4.instance.ShowAnimated(AnimationDirection.right);
            }
            if (gesture.swipe == EasyTouch.SwipeDirection.Right)
            {
                View3.instance.HideAnimated(AnimationDirection.left);
                View2.instance.ShowAnimated(AnimationDirection.left);
            }
        }

        public void TurnPlayButton()
        {
            Game game = GameManager.instance.GetNextActiveGame();
            if (game != null)
            {
                game.OpenGame();
                Hide();
            }
            else
            {
                ViewMatchMaking.instance.isRealtime = false;
                ViewController.instance.ChangeView(ViewController.instance.viewMatchMaking);
            }

            ViewController.instance.HideTabView();
            GameManager.instance.headerUI.SetActive(false);
        }

        public void FastPlayButton() {
            ViewMatchMaking.instance.isRealtime = true;
            ViewController.instance.ChangeView(ViewController.instance.viewMatchMaking);
            ViewController.instance.HideTabView();
            GameManager.instance.headerUI.SetActive(false);
        }

        public void PlayButtonOld() {
            Game game = GameManager.instance.GetNextActiveGame();
            if (game != null)
            {
                game.OpenGame();
                Hide();
            }
            else
            {
                ViewController.instance.ChangeView(ViewController.instance.viewMatchMaking);
            }

            ViewController.instance.HideTabView();
            GameManager.instance.headerUI.SetActive(false);
        }

        public void TrainingButton() {
            ViewController.instance.ChangeView(ViewController.instance.viewPuzzleSelection);
            ViewController.instance.HideTabView();
            GameManager.instance.headerUI.SetActive(false);
        }

        public void SettingsButton() {
            ViewController.instance.ChangeView(ViewController.instance.viewSettings);
            ViewController.instance.HideTabView();
        }

        public void AreaSelectButtonPress() {
            ViewController.instance.ChangeView(ViewController.instance.viewAreaSelect);
            ViewController.instance.HideTabView();
        }
    }
}
