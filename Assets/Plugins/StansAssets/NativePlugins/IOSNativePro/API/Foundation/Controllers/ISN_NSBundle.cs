#if UNITY_IPHONE || UNITY_TVOS
using System.Runtime.InteropServices;
using UnityEngine;
#endif

namespace SA.iOS.Foundation
{
    public class ISN_NSBundle
    {
        
        #if UNITY_IPHONE || UNITY_TVOS
            [DllImport("__Internal")] static extern bool _ISN_NS_IsRunningInAppStoreEnvironment();
            [DllImport("__Internal")] static extern string _ISN_NS_GetBuildInfo();
        #endif

        /// <summary>
        /// Gets a value indicating whether this application is running in AppStore environment.
        /// </summary>
        public static bool IsRunningInAppStoreEnvironment {
            get {
                #if (UNITY_IPHONE || UNITY_TVOS) && !UNITY_EDITOR
                    return _ISN_NS_IsRunningInAppStoreEnvironment();
                #else
                    return false;
                #endif
            }
        }

        /// <summary>
        /// Gets the information about current build 
        /// </summary>
        public static ISN_NSBuildInfo BuildInfo {
            get {
                #if (UNITY_IPHONE || UNITY_TVOS) && !UNITY_EDITOR
                    string data = _ISN_NS_GetBuildInfo();
                    return JsonUtility.FromJson<ISN_NSBuildInfo>(data);
                #else
                    return new ISN_NSBuildInfo();
                #endif
            }
        }
    }
}

