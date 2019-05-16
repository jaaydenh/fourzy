using System;
using SA.iOS.Utilities;

#if UNITY_IPHONE && PHOTOS_API_ENABLED
using System.Runtime.InteropServices;
#endif


namespace SA.iOS.Photos.Internal
{

    internal class ISN_PHNativeAPI : ISN_Singleton<ISN_PHNativeAPI>, ISN_iPHAPI
    {
        #if UNITY_IPHONE && PHOTOS_API_ENABLED
        [DllImport("__Internal")] static extern int _ISN_PH_GetAuthorizationStatus();
        [DllImport("__Internal")] static extern void _ISN_PH_RequestAuthorization();
        #endif


        public ISN_PHAuthorizationStatus GetAuthorizationStatus() {
            #if UNITY_IPHONE && PHOTOS_API_ENABLED && !UNITY_EDITOR
            return (ISN_PHAuthorizationStatus)_ISN_PH_GetAuthorizationStatus();
            #else
            return ISN_PHAuthorizationStatus.Authorized;
            #endif
        }


        Action<ISN_PHAuthorizationStatus> m_RequestAccessCallback;
        public void RequestAuthorization(Action<ISN_PHAuthorizationStatus> callback) {
            m_RequestAccessCallback = callback;

            #if UNITY_IPHONE && PHOTOS_API_ENABLED && !UNITY_EDITOR
            _ISN_PH_RequestAuthorization();
            #else
            m_RequestAccessCallback.Invoke(ISN_PHAuthorizationStatus.Authorized);
            #endif
        } 

        void OnRequestAuthorizationCompleted(string data) {
            int index = Convert.ToInt32(data);
            ISN_PHAuthorizationStatus status = (ISN_PHAuthorizationStatus) index;
            m_RequestAccessCallback.Invoke(status);
        }
      
    }
}