using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class ViewSettings : UIView
    {
        //Instance
        public static ViewSettings instance;
        public GameObject resetTutorialButton;

        // Use this for initialization
        void Start()
        {
            instance = this;
            keepHistory = true;

            Button btn = resetTutorialButton.GetComponent<Button>();
            btn.onClick.AddListener(ResetTutorial);
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

        public void BackButton() 
        {
            Debug.Log("View Settings Back Button");
            ViewController.instance.ChangeView(ViewController.instance.view3);
            ViewController.instance.ShowTabView();
        }
    }
}
