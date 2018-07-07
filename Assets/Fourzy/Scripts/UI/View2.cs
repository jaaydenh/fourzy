using UnityEngine;
using UnityEngine.UI;
using HedgehogTeam.EasyTouch;

namespace Fourzy
{
    public class View2 : UIView
    {
        //Instance
        public static View2 instance;
        public Text gamePiecesTabText;
        public Text tokensTabText;
        public GameObject gamePiecesGrid;
        public GameObject tokensGrid;
        //public RectTransform test;
        public ScrollRect scrollRect;

        // Use this for initialization
        void Start()
        {
            instance = this;
            keepHistory = true;
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

        public void LoadGamePiecesButton() {
            //GamePieceSelectionManager.instance.LoadGamePieces(UserManager.instance.gamePieceId.ToString());
            gamePiecesGrid.SetActive(true);
            tokensGrid.SetActive(false);
            scrollRect.content = gamePiecesGrid.GetComponent<RectTransform>();
            tokensTabText.fontSize = 35;
            gamePiecesTabText.fontSize = 40;
            gamePiecesTabText.GetComponent<Outline>().enabled = true;
            tokensTabText.GetComponent<Outline>().enabled = false;
        }

        public void LoadTokensButton() {
            //TokenSelectionManager.instance.LoadTokens();
            gamePiecesGrid.SetActive(false);
            tokensGrid.SetActive(true);
            scrollRect.content = tokensGrid.GetComponent<RectTransform>();
            tokensTabText.fontSize = 40;
            gamePiecesTabText.fontSize = 35;
            gamePiecesTabText.GetComponent<Outline>().enabled = false;
            tokensTabText.GetComponent<Outline>().enabled = true;
        }
    }
}
