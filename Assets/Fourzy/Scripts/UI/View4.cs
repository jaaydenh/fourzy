using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class View4 : UIView
    {
        public static View4 instance;

        [SerializeField]
        private Button btnFBLogin;

        private void Start()
        {
            instance = this;
            keepHistory = true;
        }

        public override void Show()
        {
            base.Show();

            LoginManager.OnFBLoginComplete += LoginManager_OnFBLoginComplete;
        }

        public override void Hide()
        {
            LoginManager.OnFBLoginComplete -= LoginManager_OnFBLoginComplete;

            base.Hide();
        }

        public override void ShowAnimated(AnimationDirection sourceDirection)
        {
            ViewController.instance.SetActiveView(TotalView.view4);
            base.ShowAnimated(sourceDirection);
        }

        public override void HideAnimated(AnimationDirection getAwayDirection)
        {
            base.HideAnimated(getAwayDirection);
        }

        public void PlayButton()
        {
            ViewController.instance.ChangeView(ViewController.instance.viewGameboardSelection);
            ViewController.instance.HideTabView();
        }

        public void ConnectFBOnClick()
        {
            btnFBLogin.interactable = false;

            LoginManager.instance.FacebookLogin();
        }

        void LoginManager_OnFBLoginComplete(bool isSuccessful)
        {
            btnFBLogin.interactable = true;

            if (isSuccessful)
            {
                btnFBLogin.gameObject.SetActive(false);
            }
            else
            {
                btnFBLogin.gameObject.SetActive(true);
            }
        }
    }
}
