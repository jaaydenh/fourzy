//@vadym udod

using Fourzy._Updates.Audio;
using Fourzy._Updates.Managers;
using SkillzSDK;
using System.Collections;
using UnityEngine;
using System;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

namespace Fourzy._Updates.UI.Menu
{
    public class SkillzMainMenuController : MenuController
    {
        public static SkillzMainMenuController Instance;

        protected override void Awake()
        {
            base.Awake();

            Instance = this;
        }

        protected override void Start()
        {
            base.Start();

            //play bg audio
            if (!AudioHolder.instance.IsBGAudioPlaying("bg_main_menu"))
            {
                AudioHolder.instance.PlayBGAudio("bg_main_menu", true, .75f, 1f);
            }
        }

        public void StartSkillzUI()
        {
#if UNITY_IOS
            Version currentVersion = new Version(UnityEngine.iOS.Device.systemVersion);
            Version ios14 = new Version("14.0");
            if (currentVersion >= ios14 &&
                ATTrackingStatusBinding.GetAuthorizationTrackingStatus() ==
                ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED) {
                // Trigger display of prompt
                ATTrackingStatusBinding.RequestAuthorizationTracking();

                // Start coroutine to wait for ATT status change before launching Skillz
                StartCoroutine(AttPrompt());
            } else {
                // Tracking status is determined already, launch Skillz
                LaunchSkillz();
            }
#else
            // Not iOS, launch Skillz
            LaunchSkillz();
#endif
        }

        private void LaunchSkillz()
        {
            SkillzCrossPlatform.setSkillzBackgroundMusic("MenuMusic.mp3");
            SkillzCrossPlatform.LaunchSkillz(SkillzGameController.Instance);
        }

        private IEnumerator AttPrompt()
        {
#if UNITY_IOS
            while (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() ==
                ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED) {
                // Pauses 1s between ATT status checks
                yield return new WaitForSecondsRealtime(1f);
            }
#endif
            // Status updated, launch Skillz
            LaunchSkillz();
            yield break;
        }
    }
}
