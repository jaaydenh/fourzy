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
using SA.iOS.Utilities;
using SA.iOS.Social.Internal;



namespace SA.iOS.Social
{

    public static class ISN_Twitter
    {
        public static event Action OnPostStart = delegate { };
        public static event Action<SA_Result> OnPostResult = delegate { };


        //--------------------------------------
        //  INITIALIZATION
        //--------------------------------------

        static ISN_Twitter() {
            NativeListener.Instantiate();
        }

        //--------------------------------------
        //  PUBLIC METHODS
        //--------------------------------------

        public static void Post(string text, Action<SA_Result> callback = null) {
            Post(text, null, new Texture2D[] { }, callback);
        }

        public static void Post(Texture2D image, Action<SA_Result> callback = null) {
            Post(null, null, new Texture2D[] { image }, callback);
        }

        public static void Post(Texture2D[] images, Action<SA_Result> callback = null) {
            Post(null, null, images, callback);
        }

        public static void Post(string text, string url, Action<SA_Result> callback = null) {
            Post(text, url, new Texture2D[] { }, callback);
        }

        public static void Post(string text, Texture2D image, Action<SA_Result> callback = null) {
            Post(text, null, new Texture2D[] { image }, callback);
        }

        public static void Post(string text, Texture2D[] images, Action<SA_Result> callback = null) {
            Post(text, null, images, callback);
        }

        public static void Post(string text, string url, Texture2D image, Action<SA_Result> callback = null) {
            Post(text, url, new Texture2D[] { image }, callback);
        }


        public static void Post(string text, string url, Texture2D[] images, Action<SA_Result> callback = null) {

            if (url == null) { url = string.Empty; }
            if (text == null) { text = string.Empty; }
            if (images == null) { images = new Texture2D[] { }; }

            if (callback != null) {
                OnPostResult += callback;
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer) {
                OnPostStart();
            }

            List<string> media = new List<string>();
            foreach (Texture2D image in images) {
                byte[] val = image.EncodeToPNG();
                media.Add(Convert.ToBase64String(val));
            }

            string encodedMedia = ISN_SocialConverter.SerializeArray(media.ToArray());
            Internal.ISN_TwPost(text, url, encodedMedia);
        }


        public static void PostGif(string text, string url) {
            Internal.ISN_TwPostGIF(text, url);
        }



        //--------------------------------------
        //  SUPPORT CLASSES
        //--------------------------------------



        private class NativeListener : ISN_Singleton<NativeListener>
        {

            private void OnTwitterPostFailed() {
                SA_Result result = new SA_Result(new SA_Error(0, "Unknown Error"));
                OnPostResult(result);
            }

            private void OnTwitterPostSuccess() {
                SA_Result result = new SA_Result();
                OnPostResult(result);
            }
        }


        private static class Internal
        {

            #if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API_ENABLED) || SA_DEBUG_MODE
            [DllImport ("__Internal")]
            private static extern void _ISN_TwPost(string text, string url, string encodedMedia);

            [DllImport ("__Internal")]
            private static extern void _ISN_TwPostGIF(string text, string url);
            #endif


            public static void ISN_TwPost(string text, string url, string encodedMedia) {
                #if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API_ENABLED) || SA_DEBUG_MODE
                _ISN_TwPost(text, url, encodedMedia);
                #endif
            }

            public static void ISN_TwPostGIF(string text, string url) {
                #if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API_ENABLED) || SA_DEBUG_MODE
                _ISN_TwPostGIF(text, url);
                #endif
            }
        }



    }
}
