using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

/// <summary>
/// Bridge between the MoPub Unity AdUnit-specific API and Android implementation.
/// </summary>
/// <para>
/// Publishers integrating with MoPub should make all calls through the <see cref="MoPub"/> class, and handle any
/// desired MoPub Events from the <see cref="MoPubManager"/> class.
/// </para>
/// <para>
/// For other platform-specific implementations, see
/// <see cref="MoPubUnityEditorAdUnit"/> and <see cref="MoPubiOSAdUnit"/>.
/// </para>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
internal class MoPubAndroidAdUnit : MoPubAdUnit {
    private readonly AndroidJavaObject _plugin;
    private readonly Dictionary<MoPub.Reward, AndroidJavaObject> _rewardsDict =
        new Dictionary<MoPub.Reward, AndroidJavaObject>();

    internal MoPubAndroidAdUnit(string adUnitId, string adType = null)
    {
        if (adType != "Banner" && adType != "Interstitial" && adType != "RewardedVideo" && adType != "Native")
            Debug.LogErrorFormat("FATAL ERROR: Invalid ad type for Android Plugin: \"{0}\"", adType);

        _plugin = new AndroidJavaObject("com.mopub.unity.MoPub" + adType + "UnityPlugin", adUnitId);
        AdUnitId = adUnitId;
        SelectedReward = new MoPub.Reward { Label = string.Empty };
    }

    #region Banners

    internal override void RequestBanner(float width, float height, MoPub.AdPosition position)
    {
        _plugin.Call("requestBanner", width, height, (int) position);
    }


    [Obsolete("CreateBanner is deprecated and will be removed soon, please use RequestBanner instead.")]
    internal override void CreateBanner(MoPub.AdPosition position,
        MoPub.BannerType bannerType = MoPub.BannerType.Size320x50)
    {
        _plugin.Call("createBanner", (int) position);
    }


    internal override void ShowBanner(bool shouldShow)
    {
        _plugin.Call("hideBanner", !shouldShow);
    }


    internal override void RefreshBanner(string keywords = "", string userDataKeywords = "")
    {
        _plugin.Call("refreshBanner", keywords, userDataKeywords);
    }


    internal override void DestroyBanner()
    {
        _plugin.Call("destroyBanner");
    }


    internal override void SetAutorefresh(bool enabled)
    {
        _plugin.Call("setAutorefreshEnabled", enabled);
    }


    internal override void ForceRefresh()
    {
        _plugin.Call("forceRefresh");
    }

    #endregion

    #region Interstitials

    internal override void RequestInterstitialAd(string keywords = "", string userDataKeywords = "")
    {
        _plugin.Call("request", keywords, userDataKeywords);
    }


    internal override void ShowInterstitialAd()
    {
        _plugin.Call("show");
    }


    internal override bool IsInterstitialReady() {
        return _plugin.Call<bool>("isReady");
    }


    internal override void DestroyInterstitialAd()
    {
        _plugin.Call("destroy");
    }

    #endregion

    #region RewardedVideos

    internal override void RequestRewardedVideo(List<MoPub.LocalMediationSetting> mediationSettings = null,
        string keywords = null, string userDataKeywords = null,
        double latitude = MoPub.LatLongSentinel, double longitude = MoPub.LatLongSentinel,
        string customerId = null)
    {
        var json = MoPub.LocalMediationSetting.ToJson(mediationSettings);
        _plugin.Call("requestRewardedVideo", json, keywords, userDataKeywords, latitude, longitude, customerId);
    }


    internal override void ShowRewardedVideo(string customData)
    {
        _plugin.Call("showRewardedVideo", customData);
    }


    internal override bool HasRewardedVideo()
    {
        return _plugin.Call<bool>("hasRewardedVideo");
    }


    internal override List<MoPub.Reward> GetAvailableRewards()
    {
        // Clear any existing reward object mappings between Unity and Android Java
        _rewardsDict.Clear();

        using (var obj = _plugin.Call<AndroidJavaObject>("getAvailableRewards")) {
            var rewardsJavaObjArray = AndroidJNIHelper.ConvertFromJNIArray<AndroidJavaObject[]>(obj.GetRawObject());
            if (rewardsJavaObjArray.Length <= 1)
                return new List<MoPub.Reward>(_rewardsDict.Keys);

            foreach (var r in rewardsJavaObjArray) {
                _rewardsDict.Add(
                    new MoPub.Reward { Label = r.Call<string>("getLabel"), Amount = r.Call<int>("getAmount") }, r);
            }
        }

        return new List<MoPub.Reward>(_rewardsDict.Keys);
    }


    internal override void SelectReward(MoPub.Reward selectedReward)
    {
        AndroidJavaObject rewardJavaObj;
        if (_rewardsDict.TryGetValue(selectedReward, out rewardJavaObj))
            _plugin.Call("selectReward", rewardJavaObj);
        else
            Debug.LogWarning(string.Format("Selected reward {0} is not available.", selectedReward));
    }

    #endregion

#if mopub_native_beta
    #region NativeAds

    internal override void RequestNativeAd()
    {
        _plugin.Call("requestNativeAd");
    }

    #endregion
#endif
}
