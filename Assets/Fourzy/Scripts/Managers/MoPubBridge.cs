//@vadym udod

using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.Mechanics
{
    public class MoPubBridge : RoutinesBase
    {
        public static MoPubBridge instance;

        public static Action<string> onAdLoaded;
        public static Action<string> onAdPlayed;
        public static Action<string> onAdFailed;
        public static Action<string> onAdCanceled;

#if UNITY_IOS
        public static string AdUnitID = "287e9b186b3146a0ac606985ad69d115";
#else
        public static string AdUnitID = "879ae95cf2084bb0aafee3b4f4d576d7";
#endif

#if UNITY_IOS
        private readonly string[] _rewardedVideoAdUnits = { "faedb21751d9475a8563b37cf3bf9c6a" };
#else
        private readonly string[] _rewardedVideoAdUnits = { "9b75826d5a7c44ccb91a6f73a55eec61" };
#endif

        private bool cancelRewardedVideo;
        private LoadingPromptScreen loadingPrompt;
        //private Dictionary<string, List<MoPub.Reward>> adUnitToRewardsMapping = new Dictionary<string, List<MoPub.Reward>>();

        protected override void Awake()
        {
            if (instance) return;

            base.Awake();

            instance = this;
        }

        protected void Start()
        {
            //if (instance != this || MoPub.IsSdkInitialized) return;

            //MoPub.InitializeSdk(new MoPub.SdkConfiguration
            //{
            //    AdUnitId = AdUnitID,

            //    LogLevel = MoPub.LogLevel.Info,

            //    // Uncomment the following line to allow supported SDK networks to collect user information on the basis
            //    // of legitimate interest.
            //    //AllowLegitimateInterest = true,
            //});

            //MoPub.ReportApplicationOpen("901599511");
            //MoPub.EnableLocationSupport(false);

            //MoPubManager.OnSdkInitializedEvent += OnSdkInitializedEvent;

            //MoPubManager.OnRewardedVideoLoadedEvent += OnRewardedVideoLoadedEvent;
            //MoPubManager.OnRewardedVideoFailedEvent += OnRewardedVideoFailedEvent;
            //MoPubManager.OnRewardedVideoFailedToPlayEvent += OnRewardedVideoFailedToPlayEvent;
            //MoPubManager.OnRewardedVideoClosedEvent += OnRewardedVideoClosedEvent;
        }

        protected void OnDestroy()
        {
            //MoPubManager.OnSdkInitializedEvent -= OnSdkInitializedEvent;

            //MoPubManager.OnRewardedVideoLoadedEvent -= OnRewardedVideoLoadedEvent;
            //MoPubManager.OnRewardedVideoFailedEvent -= OnRewardedVideoFailedEvent;
            //MoPubManager.OnRewardedVideoFailedToPlayEvent -= OnRewardedVideoFailedToPlayEvent;
            //MoPubManager.OnRewardedVideoClosedEvent -= OnRewardedVideoClosedEvent;
        }

        private void OnRewardedVideoClosedEvent(string adUnitID)
        {
            Debug.Log("rewarded video closed " + adUnitID);

            onAdPlayed?.Invoke(adUnitID);
        }

        private void OnRewardedVideoFailedToPlayEvent(string adUnitID, string error)
        {
            Debug.LogWarning("rewarded video failed to play " + adUnitID + " " + error);

            loadingPrompt.UpdateInfo(error);
            loadingPrompt.DelayedClose(2f);

            onAdFailed?.Invoke(adUnitID);
        }

        private void OnRewardedVideoFailedEvent(string adUnitID, string error)
        {
            Debug.LogWarning("rewarded video failed " + adUnitID + " " + error);

            loadingPrompt.UpdateInfo(error);
            loadingPrompt.DelayedClose(2f);

            onAdFailed?.Invoke(adUnitID);
        }

        private void OnRewardedVideoLoadedEvent(string adUnitID)
        {
            Debug.Log("rewarded video loaded " + adUnitID);

            if (cancelRewardedVideo) return;

            //if (!adUnitToRewardsMapping.ContainsKey(adUnitID)) adUnitToRewardsMapping.Add(adUnitID, MoPub.GetAvailableRewards(adUnitID));

            //if (adUnitToRewardsMapping.ContainsKey(adUnitID) && adUnitToRewardsMapping[adUnitID].Count > 0) MoPub.SelectReward(adUnitID, adUnitToRewardsMapping[adUnitID][0]);

            //loadingPrompt.CloseSelf();

            //MoPub.ShowRewardedVideo(adUnitID);

            //onAdLoaded?.Invoke(adUnitID);
        }

        public void ShowRewardedVideoAd()
        {
            string adUniID = _rewardedVideoAdUnits[0];

            Debug.Log("requesting video ad, location lat:" + GameManager.Instance.latitude + ", lon:" + GameManager.Instance.longitude);

            cancelRewardedVideo = false;
            loadingPrompt = PersistantMenuController.Instance.GetOrAddScreen<LoadingPromptScreen>();
            loadingPrompt._Prompt(LoadingPromptScreen.LoadingPromptType.BASIC, LocalizationManager.Value("loading"), () => CancelRewardedVideo(adUniID));

            ////if editor, mimic ad loading
            //if (Application.isEditor)
            //{
            //    Debug.Log("mimicing loading an ad for 3 seconds");
            //    StartRoutine("loadingAd", 3f, () =>
            //        MoPub.RequestRewardedVideo(
            //            adUniID,
            //            keywords: "rewarded, video, mopub",
            //            latitude: GameManager.Instance.latitude,
            //            longitude: GameManager.Instance.longitude,
            //            customerId: "customer101"));
            //}
            //else
            //{
            //    MoPub.RequestRewardedVideo(
            //        adUniID,
            //        keywords: "rewarded, video, mopub",
            //        latitude: GameManager.Instance.latitude,
            //        longitude: GameManager.Instance.longitude,
            //        customerId: "customer101");
            //}
        }

        public void CancelRewardedVideo(string adUniID)
        {
            cancelRewardedVideo = true;
            loadingPrompt.CloseSelf();

            onAdCanceled?.Invoke(adUniID);
        }

        private void OnSdkInitializedEvent(string message)
        {
            //MoPub.LoadRewardedVideoPluginsForAdUnits(_rewardedVideoAdUnits);
        }
    }
}