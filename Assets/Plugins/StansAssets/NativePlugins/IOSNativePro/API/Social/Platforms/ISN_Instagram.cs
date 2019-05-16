////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using System.Collections.Generic;
#if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API_ENABLED) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

using SA.Foundation.Templates;
using SA.iOS.Social.Internal;
using SA.iOS.Utilities;

namespace SA.iOS.Social
{

    public static class ISN_Instagram
    {
        public static event Action OnPostStart = delegate { };
        public static event Action<SA_Result> OnPostResult = delegate { };


        //--------------------------------------
        //  INITIALIZATION
        //--------------------------------------

        static ISN_Instagram() {
            NativeListener.Instantiate();
        }

        //--------------------------------------
        //  PUBLIC METHODS
        //--------------------------------------


        public static void Post(Texture2D image, Action<SA_Result> callback = null) {
            Post(image, null, callback);
        }


        public static void Post(Texture2D image, string message, Action<SA_Result> callback = null) {
           
            if (message == null) { message = string.Empty; }

            if (callback != null) {
                OnPostResult += callback;
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer) {
                OnPostStart();
            }
        
            byte[] val = image.EncodeToPNG();
            string encodedMedia = Convert.ToBase64String (val);

      
            Internal.ISN_InstaShare(encodedMedia, message);
  
        }


        //--------------------------------------
        //  SUPPORT CLASSES
        //--------------------------------------



        private class NativeListener : ISN_Singleton<NativeListener> {
            
            private void OnInstaPostSuccess() {
                SA_Result result = new SA_Result();
                OnPostResult(result);
            }


            private void OnInstaPostFailed(string data) {
                int code = Convert.ToInt32(data);

                SA_Error error = new SA_Error(code, "Posting Failed");
                SA_Result result = new SA_Result(error);
                OnPostResult(result);

            }
        }


        private static class Internal {

            #if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API_ENABLED) || SA_DEBUG_MODE
            [DllImport ("__Internal")]
            private static extern void _ISN_InstaShare(string encodedMedia, string message);
            #endif

            public static void ISN_InstaShare(string encodedMedia, string message) {
                #if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API_ENABLED) || SA_DEBUG_MODE
                _ISN_InstaShare(encodedMedia, message);
                #endif
            }
        }



    }
}
