using UnityEngine;

namespace Fourzy
{
    public class ViewGameBoardSelection : UIView
    {
        public static ViewGameBoardSelection instance;

        void Start()
        {
            instance = this;
            keepHistory = false;
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
            Hide();
            GameManager.instance.OpenNewGame();
        }

        public void BackButton()
        {
            Debug.Log("viewgameboardselection current view: " + ViewController.instance.GetCurrentView().name);
            Hide();
            if (ViewController.instance.GetCurrentView() != null) {
                if (ViewController.instance.GetCurrentView().GetType() != typeof(ViewTraining))
                {
                    ViewController.instance.ShowTabView();
                }

                ViewController.instance.ChangeView(ViewController.instance.GetCurrentView());
            } else {
                ViewController.instance.ShowTabView();
                ViewController.instance.ChangeView(ViewController.instance.view3);
            }
        }
    }
}
