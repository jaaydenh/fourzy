using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class ViewSettings : UIView
    {
        //Instance
        public static ViewSettings instance;
        public Button resetTutorialButton;
        public Button changeNameButton;

        // Use this for initialization
        void Start()
        {
            instance = this;
            keepHistory = true;

            resetTutorialButton.onClick.AddListener(ResetTutorial);
            changeNameButton.onClick.AddListener(ChangeName);
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void ResetTutorial()
        {
            Debug.Log("Reset Tutorial");
            PlayerPrefs.DeleteKey("onboardingStage");
            PlayerPrefs.DeleteKey("onboardingStep");
        }

        public void ChangeName() 
        {
            PopupManager.Instance.OpenPopup<ChangeNamePopup>();
        }

        public void BackButton() 
        {
            Debug.Log("View Settings Back Button");
            ViewController.instance.ChangeView(ViewController.instance.view3);
            ViewController.instance.ShowTabView();
        }
    }
}
