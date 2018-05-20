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
            // GameManager.instance.TransitionToGameOptionsScreen(GameType.PASSANDPLAY);
        }

        public void PuzzleChallengeButton()
        {
            Hide();
            ChallengeManager.instance.OpenPuzzleChallengeGame();
        }

        public void AIGameButton()
        {
            ViewController.instance.ChangeView(ViewController.instance.viewGameboardSelection);
            ViewGameBoardSelection.instance.TransitionToViewGameBoardSelection(GameType.AI);
            // GameManager.instance.TransitionToGameOptionsScreen(GameType.AI);
        }

        public void BackButton()
        {
            ViewController.instance.ChangeView(ViewController.instance.view3);
            ViewController.instance.ShowTabView();
        }
    }
}
