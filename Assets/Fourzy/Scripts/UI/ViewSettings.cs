using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class ViewSettings : UIView
    {
        //Instance
        public static ViewSettings instance;
        public GameObject resetTutorialButton;
        public GameObject changeNameButton;

        // Use this for initialization
        void Start()
        {
            instance = this;
            keepHistory = true;

            Button btn = resetTutorialButton.GetComponent<Button>();
            btn.onClick.AddListener(ResetTutorial);

            Button changeNamebtn = changeNameButton.GetComponent<Button>();
            changeNamebtn.onClick.AddListener(ChangeName);
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

        public void ChangeName() {
            Debug.Log("Change Name");
        }

        public void BackButton() 
        {
            Debug.Log("View Settings Back Button");
            ViewController.instance.ChangeView(ViewController.instance.view3);
            ViewController.instance.ShowTabView();
        }
    }
}
