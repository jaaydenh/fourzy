using UnityEngine;

namespace Fourzy
{
    public class ViewGameBoardSelection : UIView
    {
        //Instance
        public static ViewGameBoardSelection instance;

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

        public void PlayButton() {
            Debug.Log("ViewGameBoardSelection PlayButton");
            Hide();
            GameManager.instance.OpenNewGame();
        }

        public void BackButton()
        {
            Debug.Log("previous view type: " + ViewController.instance.GetPreviousView().GetType());
            if (ViewController.instance.GetPreviousView().GetType() != typeof(ViewTraining)) {
                ViewController.instance.ShowTabView();
            }

            ViewController.instance.ChangeView(ViewController.instance.GetPreviousView());
        }
    }
}
