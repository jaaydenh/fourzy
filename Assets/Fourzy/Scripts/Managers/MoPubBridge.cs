using UnityEngine;

namespace Fourzy._Updates.Mechanics
{
    public class MoPubBridge : MonoBehaviour
    {
        public static MoPubBridge instance;

#if UNITY_IOS
    private readonly string[] _bannerAdUnits =
        { "0ac59b0996d947309c33f59d6676399f", "23b49916add211e281c11231392559e4" };

    private readonly string[] _interstitialAdUnits =
        { "4f117153f5c24fa6a3a92b818a5eb630", "3aba0056add211e281c11231392559e4" };

    private readonly string[] _rewardedVideoAdUnits =
        { "8f000bd5e00246de9c789eed39ff6096", "98c29e015e7346bd9c380b1467b33850" };

    private readonly string[] _rewardedRichMediaAdUnits = { };
#elif UNITY_ANDROID || UNITY_EDITOR
        private readonly string[] _bannerAdUnits = { "b195f8dd8ded45fe847ad89ed1d016da" };
        private readonly string[] _interstitialAdUnits = { "24534e1901884e398f1253216226017e" };
        private readonly string[] _rewardedVideoAdUnits = { "920b6145fb1546cf8b5cf2ac34638bb7" };
        private readonly string[] _rewardedRichMediaAdUnits = { "a96ae2ef41d44822af45c6328c4e1eb1" };
#endif
        private bool _canCollectPersonalInfo = false;
        private MoPub.Consent.Status _currentConsentStatus = MoPub.Consent.Status.Unknown;
        private bool _shouldShowConsentDialog = false;
        private bool? _isGdprApplicable = false;

        protected void Awake()
        {
            if (!instance) instance = this;
        }

        protected void OnEnable()
        {
            //mopub listeners
            MoPubManager.OnAdLoadedEvent += OnAdLoadedEvent;
            MoPubManager.OnAdFailedEvent += OnAdFailedEvent;

            MoPubManager.OnInterstitialLoadedEvent += OnInterstitialLoadedEvent;
            MoPubManager.OnInterstitialFailedEvent += OnInterstitialFailedEvent;
            MoPubManager.OnInterstitialDismissedEvent += OnInterstitialDismissedEvent;

            MoPubManager.OnRewardedVideoLoadedEvent += OnRewardedVideoLoadedEvent;
            MoPubManager.OnRewardedVideoFailedEvent += OnRewardedVideoFailedEvent;
            MoPubManager.OnRewardedVideoFailedToPlayEvent += OnRewardedVideoFailedToPlayEvent;
            MoPubManager.OnRewardedVideoClosedEvent += OnRewardedVideoClosedEvent;

            MoPubManager.OnImpressionTrackedEvent += OnImpressionTrackedEvent;
        }

        protected void OnDisable()
        {
            MoPubManager.OnAdLoadedEvent -= OnAdLoadedEvent;
            MoPubManager.OnAdFailedEvent -= OnAdFailedEvent;

            MoPubManager.OnInterstitialLoadedEvent -= OnInterstitialLoadedEvent;
            MoPubManager.OnInterstitialFailedEvent -= OnInterstitialFailedEvent;
            MoPubManager.OnInterstitialDismissedEvent -= OnInterstitialDismissedEvent;

            MoPubManager.OnRewardedVideoLoadedEvent -= OnRewardedVideoLoadedEvent;
            MoPubManager.OnRewardedVideoFailedEvent -= OnRewardedVideoFailedEvent;
            MoPubManager.OnRewardedVideoFailedToPlayEvent -= OnRewardedVideoFailedToPlayEvent;
            MoPubManager.OnRewardedVideoClosedEvent -= OnRewardedVideoClosedEvent;

            MoPubManager.OnImpressionTrackedEvent -= OnImpressionTrackedEvent;
        }

        public void SDKInitialized()
        {
            UpdateConsentValues();
        }

        public void RequestRewardedVideo()
        {
            MoPub.RequestRewardedVideo(
                adUnitId: _rewardedVideoAdUnits[0], keywords: "rewarded, video, mopub",
                latitude: 37.7833, longitude: 122.4167, customerId: UserManager.Instance.userId);
        }

        public void ConsentStatusChanged(MoPub.Consent.Status oldStatus, MoPub.Consent.Status newStatus, bool canCollectPersonalInfo)
        {
            _canCollectPersonalInfo = canCollectPersonalInfo;
            _currentConsentStatus = newStatus;
            _shouldShowConsentDialog = MoPub.ShouldShowConsentDialog;

            Debug.Log($"New consent status {newStatus}");
        }

        private void UpdateConsentValues()
        {
            _canCollectPersonalInfo = MoPub.CanCollectPersonalInfo;
            _currentConsentStatus = MoPub.CurrentConsentStatus;
            _shouldShowConsentDialog = MoPub.ShouldShowConsentDialog;
            _isGdprApplicable = MoPub.IsGdprApplicable;
        }

        #region Mopub

        private void OnAdLoadedEvent(string adUnitId, float height)
        {
            //_demoGUI.BannerLoaded(adUnitId, height);
        }

        private void OnAdFailedEvent(string adUnitId, string error)
        {
            AdFailed(adUnitId, "load banner", error);
        }

        private void OnInterstitialLoadedEvent(string adUnitId)
        {
            //_demoGUI.AdLoaded(adUnitId);
        }

        private void OnInterstitialFailedEvent(string adUnitId, string error)
        {
            AdFailed(adUnitId, "load interstitial", error);
        }

        private void OnInterstitialDismissedEvent(string adUnitId)
        {
            //_demoGUI.AdDismissed(adUnitId);
        }

        private void OnRewardedVideoLoadedEvent(string adUnitId)
        {
            var availableRewards = MoPub.GetAvailableRewards(adUnitId);

            Debug.Log("Reward video loaded");
            foreach (MoPub.Reward reward in availableRewards) Debug.Log(reward.Label + " " + reward.Amount);

            //show it
            MoPub.ShowRewardedVideo(adUnitId);
        }

        private void OnRewardedVideoFailedEvent(string adUnitId, string error)
        {
            AdFailed(adUnitId, "load rewarded video", error);

            Debug.Log("Failed to load reward video ad");
        }

        private void OnRewardedVideoFailedToPlayEvent(string adUnitId, string error)
        {
            AdFailed(adUnitId, "play rewarded video", error);
        }

        private void OnRewardedVideoClosedEvent(string adUnitId)
        {
            //_demoGUI.AdDismissed(adUnitId);
        }

        private void OnImpressionTrackedEvent(string adUnitId, MoPub.ImpressionData impressionData)
        {
            //_demoGUI.ImpressionTracked(adUnitId, impressionData);
        }

        private void AdFailed(string adUnitId, string action, string error)
        {
            var errorMsg = "Failed to " + action + " ad unit " + adUnitId;
            if (!string.IsNullOrEmpty(error)) errorMsg += ": " + error;

            //report and error
        }

        #endregion
    }
}