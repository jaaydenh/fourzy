//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
using SkillzSDK;
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
                SkillzGameController.Instance.OnDefaultPlayerDataReceived += OnPlayerDataReceived;
            }
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

#if UNITY_EDITOR
            var matchInfoJson = SkillzSDK.Internal.API.UnityEditor.MatchInfoJson.Build(SkillzSDK.Settings.SkillzSettings.Instance.GameID);
            SkillzCrossPlatform.InitializeSimulatedMatch(matchInfoJson);
            SkillzState.NotifyMatchWillBegin(matchInfoJson);
#endif
        }

        public void StartFakeSyncGame()
        {
            SkillzGameController.StartEditorSkillzUI();

            string matchJson = SkillzInfoJson.BuildForSyncGame(1, true);

            SkillzCrossPlatform.InitializeSimulatedMatch(matchJson);
            SkillzState.NotifyMatchWillBegin(matchJson);
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
