using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class ViewAreaSelect : UIView
    {
        public static ViewAreaSelect instance;
        public GameObject closeButton;

        void Start()
        {
            instance = this;
            keepHistory = true;

            Button btn = closeButton.GetComponent<Button>();
            btn.onClick.AddListener(CloseButton);
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void CloseButton()
        {
            ViewController.instance.ChangeView(ViewController.instance.view3);
            ViewController.instance.ShowTabView();
        }
    }
}
