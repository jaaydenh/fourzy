using UnityEngine;
using HedgehogTeam.EasyTouch;

namespace Fourzy
{
    public class View2 : UIView
    {
        //Instance
        public static View2 instance;

        // Use this for initialization
        void Start()
        {
            instance = this;
        }

        public override void Show()
        {
            //Debug.Log("View 2 Show");
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
            //ViewController.instance.currentActiveView = TotalView.view2;
            ViewController.instance.SetActiveView(TotalView.view2);
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
                View2.instance.HideAnimated(AnimationDirection.right);
                View3.instance.ShowAnimated(AnimationDirection.right);
            }
            if (gesture.swipe == EasyTouch.SwipeDirection.Right)
            {
                View2.instance.HideAnimated(AnimationDirection.left);
                View1.instance.ShowAnimated(AnimationDirection.left);
            }
        }
    }
}
