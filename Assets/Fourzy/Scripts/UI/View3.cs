using UnityEngine;
using HedgehogTeam.EasyTouch;

namespace Fourzy
{
    public class View3 : UIView
    {
        //Instance
        public static View3 instance;

        // Use this for initialization
        void Start()
        {
            instance = this;
            ViewController.instance.currentActiveView = TotalView.view3;

            if (PlayerPrefs.GetInt("puzzleChallengeLevel") <= 3)
            {
                ViewController.instance.view3.Hide();
                ViewController.instance.HideTabView();
                ChallengeManager.instance.OpenPuzzleChallengeGame();
            }
        }

        public override void Show()
        {
            //Debug.Log("View 3 Show");
            //EasyTouch.On_SwipeStart += On_SwipeStart;
            //EasyTouch.On_Swipe += On_Swipe;
            //EasyTouch.On_SwipeEnd += On_SwipeEnd;
            base.Show();
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

        public void PlayButton()
        {
            GameManager.instance.PlayButton();
            Hide();
            ViewController.instance.HideTabView();
        }

        public void TrainingButton() {
            ViewController.instance.ChangeView(ViewController.instance.viewTraining);
            ViewController.instance.HideTabView();
        }
    }
}
