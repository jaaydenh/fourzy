using UnityEngine;

namespace Fourzy
{
    public class ViewTraining : UIView
    {
        public static ViewTraining instance;

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

        public void PassAndPlayButton()
        {
            ViewController.instance.ChangeView(ViewController.instance.viewGameboardSelection);
            ViewGameBoardSelection.instance.TransitionToViewGameBoardSelection(GameType.PASSANDPLAY);
        }

        public void PuzzleChallengeButton()
        {
            //Hide();
            ViewController.instance.ChangeView(ViewController.instance.viewPuzzleSelection);
            // GameManager.instance.OpenPuzzleChallengeGame();
        }

        public void AIGameButton()
        {
            ViewController.instance.ChangeView(ViewController.instance.viewGameboardSelection);
            ViewGameBoardSelection.instance.TransitionToViewGameBoardSelection(GameType.AI);
        }

        public void BackButton()
        {
            ViewController.instance.ChangeView(ViewController.instance.view3);
            ViewController.instance.ShowTabView();
        }
    }
}
