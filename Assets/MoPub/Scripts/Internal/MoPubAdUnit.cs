using System;
using System.Collections.Generic;

/// <summary>
/// Bridge between the MoPub Unity AdUnit-specific API and platform-specific implementations.
/// </summary>
/// <para>
/// Publishers integrating with MoPub should make all calls through the <see cref="MoPub"/> class, and handle any
/// desired MoPub Events from the <see cref="MoPubManager"/> class.
/// </para>
/// <para>
/// For platform-specific implementations, see
/// <see cref="MoPubUnityEditorAdUnit"/>, <see cref="MoPubAndroidAdUnit"/>, and <see cref="MoPubiOSAdUnit"/>.
/// </para>
internal abstract class MoPubAdUnit
{
    protected string AdUnitId;

    internal static MoPubAdUnit CreateMoPubAdUnit(string adUnitId, string adType = null)
    {
        // Choose created class based on target platform...
        return new
#if UNITY_EDITOR
            MoPubUnityEditorAdUnit
#elif UNITY_ANDROID
            MoPubAndroidAdUnit
#else
            MoPubiOSAdUnit
#endif
        (adUnitId, adType);
    }


    #region Banners


    internal abstract void RequestBanner(float width, float height, MoPub.AdPosition position);


    [Obsolete("CreateBanner is deprecated and will be removed soon, please use RequestBanner instead.")]
    internal abstract void CreateBanner(MoPub.AdPosition position,
        MoPub.BannerType bannerType = MoPub.BannerType.Size320x50);


    internal abstract void ShowBanner(bool shouldShow);


    internal abstract void RefreshBanner(string keywords = "", string userDataKeywords = "");


    internal abstract void DestroyBanner();


    internal abstract void SetAutorefresh(bool enabled);


    internal abstract void ForceRefresh();

    #endregion Banners


    #region Interstitials

    internal abstract void RequestInterstitialAd(string keywords = "", string userDataKeywords = "");


    internal abstract bool IsInterstitialReady();


    internal abstract void ShowInterstitialAd();


    internal abstract void DestroyInterstitialAd();

    #endregion Interstitials


    #region RewardedVideos

    protected MoPub.Reward SelectedReward;

    internal abstract void RequestRewardedVideo(List<MoPub.LocalMediationSetting> mediationSettings = null,
        string keywords = null,
        string userDataKeywords = null, double latitude = MoPub.LatLongSentinel,
        double longitude = MoPub.LatLongSentinel, string customerId = null);


    // Queries if a rewarded video ad has been loaded for the given ad unit id.
    internal abstract bool HasRewardedVideo();


    // Queries all of the available rewards for the ad unit. This is only valid after
    // a successful requestRewardedVideo() call.
    internal abstract List<MoPub.Reward> GetAvailableRewards();


    // If a rewarded video ad is loaded this will take over the screen and show the ad
    internal abstract void ShowRewardedVideo(string customData);

    internal abstract void SelectReward(MoPub.Reward selectedReward);

    #endregion RewardedVideos


#if mopub_native_beta
    #region NativeAds

    internal abstract void RequestNativeAd();

    #endregion NativeAds

#endif
}
