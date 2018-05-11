using UnityEngine;

namespace Fourzy
{
    public class ViewTraining : UIView
    {
        //Instance
        public static ViewTraining instance;

        // Use this for initialization
        void Start()
        {
            instance = this;
            keepHistory = true;
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void PassAndPlayButton() {
            //Debug.Log("PassAndPlayButton");
            ViewController.instance.ChangeView(ViewController.instance.viewGameboardSelection);
            GameManager.instance.TransitionToGameOptionsScreen(GameType.PASSANDPLAY);
        }

        public void PuzzleChallengeButton()
        {
            //Debug.Log("PuzzleChallengeButton");
            Hide();
            ChallengeManager.instance.OpenPuzzleChallengeGame();
        }

        public void AIGameButton()
        {
            Debug.Log("AIGameButton");
            ViewController.instance.ChangeView(ViewController.instance.viewGameboardSelection);
            GameManager.instance.TransitionToGameOptionsScreen(GameType.AI);
        }

        public void BackButton()
        {
            Debug.Log("View Training Back Button");
            ViewController.instance.ChangeView(ViewController.instance.view3);
            ViewController.instance.ShowTabView();
        }
    }
}
