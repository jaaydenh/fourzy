using System.Collections.Generic;

public class UnityAdsPackageConfig : PackageConfig
{
    public override string Name
    {
        get { return "UnityAds"; }
    }

    public override string Version
    {
        get { return /*UNITY_PACKAGE_VERSION*/"1.3.4"; }
    }

    public override Dictionary<Platform, string> NetworkSdkVersions
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, /*ANDROID_SDK_VERSION*/"3.6.0" },
                { Platform.IOS, /*IOS_SDK_VERSION*/"3.6.0" }
            };
        }
    }

    public override Dictionary<Platform, string> AdapterClassNames
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, "com.mopub.mobileads.Unity" },
                { Platform.IOS, "UnityAds" }
            };
        }
    }
}
