//@vadym udod

using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.Mechanics
{
    public class MoPubBridge : MonoBehaviour
    {
        public static MoPubBridge instance;

#if UNITY_ANDROID || UNITY_EDITOR
        public static string AdUnitID = "879ae95cf2084bb0aafee3b4f4d576d7";
#elif UNITY_ISO
        public static string AdUnitID = "287e9b186b3146a0ac606985ad69d115";
#endif

#if UNITY_IOS

    private readonly string[] _rewardedVideoAdUnits =
        { "8f000bd5e00246de9c789eed39ff6096", "faedb21751d9475a8563b37cf3bf9c6a" };

#elif UNITY_ANDROID || UNITY_EDITOR
        private readonly string[] _rewardedVideoAdUnits = { "9b75826d5a7c44ccb91a6f73a55eec61" };
#endif

        private Dictionary<string, List<MoPub.Reward>> adUnitToRewardsMapping = new Dictionary<string, List<MoPub.Reward>>();

        protected void Awake()
        {
            if (instance) return;

            instance = this;
        }

        protected void Start()
        {
            if (instance != this) return;

            MoPub.InitializeSdk(new MoPub.SdkConfiguration
            {
                AdUnitId = AdUnitID,
                
                LogLevel = MoPub.LogLevel.Info,

                // Uncomment the following line to allow supported SDK networks to collect user information on the basis
                // of legitimate interest.
                //AllowLegitimateInterest = true,
            });

            MoPubManager.OnSdkInitializedEvent += OnSdkInitializedEvent;

            MoPubManager.OnRewardedVideoLoadedEvent += OnRewardedVideoLoadedEvent;
            MoPubManager.OnRewardedVideoFailedEvent += OnRewardedVideoFailedEvent;
            MoPubManager.OnRewardedVideoFailedToPlayEvent += OnRewardedVideoFailedToPlayEvent;
            MoPubManager.OnRewardedVideoClosedEvent += OnRewardedVideoClosedEvent;
        }

        protected void OnDestroy()
        {
            MoPubManager.OnSdkInitializedEvent -= OnSdkInitializedEvent;

            MoPubManager.OnRewardedVideoLoadedEvent -= OnRewardedVideoLoadedEvent;
            MoPubManager.OnRewardedVideoFailedEvent -= OnRewardedVideoFailedEvent;
            MoPubManager.OnRewardedVideoFailedToPlayEvent -= OnRewardedVideoFailedToPlayEvent;
            MoPubManager.OnRewardedVideoClosedEvent -= OnRewardedVideoClosedEvent;
        }

        private void OnRewardedVideoClosedEvent(string adUnitID)
        {
            if (GameManager.Instance.debugMessages) Debug.Log("rewarded video closed " + adUnitID);
        }

        private void OnRewardedVideoFailedToPlayEvent(string adUnitID, string error)
        {
            Debug.LogWarning("rewarded video failed to play " + adUnitID + " " + error);
        }

        private void OnRewardedVideoFailedEvent(string adUnitID, string error)
        {
            Debug.LogWarning("rewarded video failed " + adUnitID + " " + error);
        }

        private void OnRewardedVideoLoadedEvent(string adUnitID)
        {
            if (GameManager.Instance.debugMessages) Debug.Log("rewarded video loaded " + adUnitID);

            if (!adUnitToRewardsMapping.ContainsKey(adUnitID)) adUnitToRewardsMapping.Add(adUnitID, MoPub.GetAvailableRewards(adUnitID));

            if (adUnitToRewardsMapping.ContainsKey(adUnitID) && adUnitToRewardsMapping[adUnitID].Count > 0) MoPub.SelectReward(adUnitID, adUnitToRewardsMapping[adUnitID][0]);

            MoPub.ShowRewardedVideo(adUnitID);
        }

        public void ShowRewardedVideoAd()
        {
            if (GameManager.Instance.debugMessages) Debug.Log("requesting video ad, location lat:" + GameManager.Instance.latitude + ", lon:" + GameManager.Instance.longitude);

            MoPub.RequestRewardedVideo(
            _rewardedVideoAdUnits[0],
            keywords: "rewarded, video, mopub",
            latitude: GameManager.Instance.latitude,
            longitude: GameManager.Instance.longitude,
            customerId: "customer101");
        }

        private void OnSdkInitializedEvent(string message)
        {
            MoPub.LoadRewardedVideoPluginsForAdUnits(_rewardedVideoAdUnits);
        }
    }
}