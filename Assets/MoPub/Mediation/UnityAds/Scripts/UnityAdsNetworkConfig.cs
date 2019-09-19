#if mopub_manager
using UnityEngine;

public class UnityAdsNetworkConfig : MoPubNetworkConfig
{
    public override string AdapterConfigurationClassName
    {
        get { return Application.platform == RuntimePlatform.Android
                  ? "com.mopub.mobileads.UnityAdsAdapterConfiguration"
                  : "UnityAdsAdapterConfiguration"; }
    }
}
#endif
