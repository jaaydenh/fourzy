using System.Collections;
using UnityEngine;
using HedgehogTeam.EasyTouch;

namespace Fourzy
{
    public class ViewTabs : UIView
    {
        //Instance
        public static ViewTabs instance;
        public bool isAnimating;

        // Use this for initialization
        void Start()
        {
            instance = this;
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public override void ShowAnimated(AnimationDirection sourceDirection)
        {
            base.ShowAnimated(sourceDirection);
        }

        public override void HideAnimated(AnimationDirection getAwayDirection)
        {
            base.HideAnimated(getAwayDirection);
        }
        //public void SwipeAnimated()
        //{
        //    base.SwipeAnimated();
        //}
        public void OnTabButtonPress(int index)
        {
            //Debug.Log("ViewTabs: isAnimating: " + isAnimating);
            //if (isAnimating)
                //return;
            isAnimating = true;
            SwipeScreen(index);
        }

        private void SwipeScreen(int screenNumber)
        {
            Debug.Log("currentActiveView: " + (int)ViewController.instance.currentActiveView);
            Debug.Log("new screen number: " + screenNumber);

            if (screenNumber != (int)ViewController.instance.currentActiveView)
            {
                if (screenNumber == ((int)ViewController.instance.currentActiveView + 1))
                {
                    ViewController.instance.TabsViewList[(int)ViewController.instance.currentActiveView].HideAnimated(AnimationDirection.right);
                    //ViewController.instance.currentActiveView = (TotalView)screenNumber;
                    ViewController.instance.TabsViewList[screenNumber].ShowAnimated(AnimationDirection.right);
                }
                else if (screenNumber == ((int)ViewController.instance.currentActiveView - 1))
                {
                    ViewController.instance.TabsViewList[(int)ViewController.instance.currentActiveView].HideAnimated(AnimationDirection.left);
                    //ViewController.instance.currentActiveView = (TotalView)screenNumber;
                    ViewController.instance.TabsViewList[screenNumber].ShowAnimated(AnimationDirection.left);
                }
                else if (screenNumber > ((int)ViewController.instance.currentActiveView + 1))
                {
                    ViewController.instance.TabsViewList[(int)ViewController.instance.currentActiveView].HideAnimated(AnimationDirection.right);
                    int tempNumber = Random.Range((int)ViewController.instance.currentActiveView + 1, screenNumber);
                    //ViewController.instance.currentActiveView = (TotalView)screenNumber;
                    ViewController.instance.TabsViewList[tempNumber].ShowAnimated(AnimationDirection.right);
                    StartCoroutine(fastScrollBetweenScreen(screenNumber, tempNumber, AnimationDirection.right));
                }
                else if (screenNumber < ((int)ViewController.instance.currentActiveView - 1))
                {
                    ViewController.instance.TabsViewList[(int)ViewController.instance.currentActiveView].HideAnimated(AnimationDirection.left);
                    int tempNumber = Random.Range(screenNumber + 1, (int)ViewController.instance.currentActiveView);
                    //ViewController.instance.currentActiveView = (TotalView)screenNumber;
                    ViewController.instance.TabsViewList[tempNumber].ShowAnimated(AnimationDirection.left);
                    StartCoroutine(fastScrollBetweenScreen(screenNumber, tempNumber, AnimationDirection.left));
                }
            }
        }

        IEnumerator fastScrollBetweenScreen(int screenNumber, int tempNumber, AnimationDirection direction)
        {
            Debug.Log("fastScroll");
            yield return new WaitForSeconds(0.20f);
            ViewController.instance.TabsViewList[tempNumber].HideAnimated(direction);
            ViewController.instance.TabsViewList[screenNumber].ShowAnimated(direction);
        }

        //public void LeftSwipe(Gesture gesture)
        //{
        //    //      Debug.Log ("Gesture" + gesture.startPosition);
        //    Debug.Log("Gesture Vector" + gesture.swipeVector);
        //}

        //public void RightSwipe(Gesture gesture)
        //{
        //    Debug.Log("Gesture" + gesture.swipeLength);
        //}
    }
}
