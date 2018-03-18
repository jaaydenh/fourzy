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
            Debug.Log("PassAndPlayButton");
            ViewController.instance.ChangeView(ViewController.instance.viewGameboardSelection);
            GameManager.instance.TransitionToGameOptionsScreen(GameType.PASSANDPLAY);
        }

        public void PuzzleChallengeButton()
        {
            Debug.Log("PuzzleChallengeButton");
            Hide();
            ChallengeManager.instance.OpenPuzzleChallengeGame();
            //ViewController.instance.ChangeView(ViewController.instance.viewGameboardSelection);
            //GameManager.instance.TransitionToGameOptionsScreen(GameType.PASSANDPLAY);
        }

        public void AIGameButton()
        {
            Debug.Log("AIGameButton");
            ViewController.instance.ChangeView(ViewController.instance.viewGameboardSelection);
            GameManager.instance.TransitionToGameOptionsScreen(GameType.AI);
        }

        public void BackButton()
        {
            ViewController.instance.ChangeView(ViewController.instance.view3);
            ViewController.instance.ShowTabView();
        }
    }
}
