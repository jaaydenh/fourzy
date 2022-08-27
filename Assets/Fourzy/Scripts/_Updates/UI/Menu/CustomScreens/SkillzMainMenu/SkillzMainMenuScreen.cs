//@vadym udod

using Fourzy._Updates.Managers;
using SkillzSDK;
using SkillzSDK.Internal.API.UnityEditor;
using SkillzSDK.Settings;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class SkillzMainMenuScreen : MenuScreen
    {
        [SerializeField]
        private GameObject missionRewardsButton;

        private int counter = 0;

        protected override void Awake()
        {
            base.Awake();

            if (SkillzGameController.Instance.LatestDefaultPlayerData == null)
            {
                SkillzGameController.OnDefaultPlayerDataReceived += OnPlayerDataReceived;
            }
        }

        private void OnDestroy()
        {
            SkillzGameController.OnDefaultPlayerDataReceived -= OnPlayerDataReceived;
        }

        private void Update()
        {
            if (isCurrent)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (Input.mousePosition.magnitude < Screen.width * .2f)
                    {
                        counter++;

                        if (counter > 4)
                        {
                            SettingsManager.Toggle(SettingsManager.KEY_DEMO_MODE);
                            counter = 0;
                        }
                    }
                    else
                    {
                        counter = 0;
                    }
                }
            }
        }

        public override void OnBack()
        {
            base.OnBack();

            GameManager.Instance.CloseApp();
        }

        public override void Open()
        {
            base.Open();

            if (SkillzGameController.Instance.LatestDefaultPlayerData != null)
            {
                OnPlayerDataReceived();
            }
        }

        public void StartSkillz()
        {
            SkillzMainMenuController.Instance.StartSkillzUI();
        }

        public void GetProgressionData()
        {
            SkillzGameController.Instance.GetProgressionData();
        }

        private void OnPlayerDataReceived()
        {
            if (!missionRewardsButton.activeInHierarchy)
            {
                missionRewardsButton.SetActive(true);
            }
        }
    }
}
