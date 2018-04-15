using UnityEngine;
using UnityEngine.UI;
using HedgehogTeam.EasyTouch;

namespace Fourzy
{
    public class View5 : UIView
    {
        //Instance
        public static View5 instance;
        public Text winTabText;
        public Text coinsEarnedTabText;

        // Use this for initialization
        void Start()
        {
            instance = this;
            keepHistory = true;
        }

        public override void Show()
        {
            //Debug.Log("View 5 Show");
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
            ViewController.instance.SetActiveView(TotalView.view5);
            base.ShowAnimated(sourceDirection);
            LeaderboardManager.instance.GetWinsLeaderboard();
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

            }
            if (gesture.swipe == EasyTouch.SwipeDirection.Right)
            {
                HideAnimated(AnimationDirection.left);
                View4.instance.ShowAnimated(AnimationDirection.left);
            }
        }

        public void WinsButton() {
            LeaderboardManager.instance.GetWinsLeaderboard();
            coinsEarnedTabText.fontSize = 35;
            winTabText.fontSize = 40;
            winTabText.GetComponent<Outline>().enabled = true;
            coinsEarnedTabText.GetComponent<Outline>().enabled = false;
        }

        public void CoinsEarnedButton() {
            LeaderboardManager.instance.GetCoinsEarnedLeaderboard();
            coinsEarnedTabText.fontSize = 40;
            winTabText.fontSize = 35;
            winTabText.GetComponent<Outline>().enabled = false;
            coinsEarnedTabText.GetComponent<Outline>().enabled = true;
        }
    }
}
