//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.Mechanics
{
    public class MoPubBridge : MonoBehaviour
    {
        public static MoPubBridge instance;

        public static string AdUnitID = "b195f8dd8ded45fe847ad89ed1d016da";

#if UNITY_IOS
    //private readonly string[] _bannerAdUnits =
    //    { "0ac59b0996d947309c33f59d6676399f", "23b49916add211e281c11231392559e4" };

    //private readonly string[] _interstitialAdUnits =
    //    { "4f117153f5c24fa6a3a92b818a5eb630", "3aba0056add211e281c11231392559e4" };

    private readonly string[] _rewardedVideoAdUnits =
        { "8f000bd5e00246de9c789eed39ff6096", "98c29e015e7346bd9c380b1467b33850" };

    private readonly string[] _rewardedRichMediaAdUnits = { };
#elif UNITY_ANDROID || UNITY_EDITOR
        //private readonly string[] _bannerAdUnits = { "b195f8dd8ded45fe847ad89ed1d016da" };
        //private readonly string[] _interstitialAdUnits = { "24534e1901884e398f1253216226017e" };
        private readonly string[] _rewardedVideoAdUnits = { "920b6145fb1546cf8b5cf2ac34638bb7" };
        private readonly string[] _rewardedRichMediaAdUnits = { "a96ae2ef41d44822af45c6328c4e1eb1" };
#endif

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

    private void OnRewardedVideoClosedEvent(string addUnitID)
    {
        if (GameManager.Instance.debugMessages) Debug.Log("rewarded video closed " + addUnitID);
    }

    private void OnRewardedVideoFailedToPlayEvent(string addUnitID, string error)
    {
        Debug.LogWarning("rewarded video failed to play " + addUnitID + " " + error);
    }

    private void OnRewardedVideoFailedEvent(string addUnitID, string error)
    {
        Debug.LogWarning("rewarded video failed " + addUnitID + " " + error);
    }

    private void OnRewardedVideoLoadedEvent(string addUnitID)
    {
        if (GameManager.Instance.debugMessages) Debug.Log("rewarded video loaded " + addUnitID);

        MoPub.ShowRewardedVideo(addUnitID);
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
        MoPub.LoadRewardedVideoPluginsForAdUnits(_rewardedRichMediaAdUnits);
    }
}
}