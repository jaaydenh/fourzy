using UnityEngine;
using HedgehogTeam.EasyTouch;

namespace Fourzy
{
    public class View1 : UIView
    {
        //Instance
        public static View1 instance;

        // Use this for initialization
        void Start()
        {
            instance = this;
        }

        public override void Show()
        {
            Debug.Log("View 1 Show");
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

        public override void ShowAnimated (AnimationDirection sourceDirection)
        {
            ViewController.instance.SetActiveView(TotalView.view1);
            base.ShowAnimated (sourceDirection);
        }

        public override void HideAnimated (AnimationDirection getAwayDirection)
        {  
            base.HideAnimated (getAwayDirection);
        }

        // At the swipe beginning 
        private void On_SwipeStart( Gesture gesture){
        }

        // During the swipe
        private void On_Swipe(Gesture gesture){
        }

        // At the swipe end 
        private void On_SwipeEnd(Gesture gesture){
            float angles = gesture.GetSwipeOrDragAngle();
            if (gesture.swipe == EasyTouch.SwipeDirection.Left) {
                HideAnimated(AnimationDirection.right);
                View2.instance.ShowAnimated(AnimationDirection.right);
            }
            if (gesture.swipe == EasyTouch.SwipeDirection.Right) {

            }
        }
    }
}
