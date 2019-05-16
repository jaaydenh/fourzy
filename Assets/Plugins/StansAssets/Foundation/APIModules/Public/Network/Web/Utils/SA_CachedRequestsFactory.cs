using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

using SA.Foundation.Async;

namespace SA.Foundation.Network.Web
{
    public static class SA_CachedRequestsFactory
    {

        private static Dictionary<string, UnityWebRequest> m_cache = new Dictionary<string, UnityWebRequest>();
        private static Dictionary<string, List<Action<UnityWebRequest>>> m_thumbnailLoadQueue = new Dictionary<string, List<Action<UnityWebRequest>>>();

        /// <summary>
        /// Sen'd a UnityWebRequest configured for HTTP GET.
        /// </summary>
        /// <param name="uri">The URI of the resource to retrieve via HTTP GET.</param>
        /// <param name="callback">Callback will be fired once request is completed</param>
        public static void Get(string uri, Action<UnityWebRequest> callback) {

            //TODO propert validation would be nice
            if(string.IsNullOrEmpty(uri)) {
                callback.Invoke(null);
                return;
            }

            if (m_thumbnailLoadQueue.ContainsKey(uri)) {
                List<Action<UnityWebRequest>> callbacks = m_thumbnailLoadQueue[uri];
                if (callback != null) { callbacks.Add(callback); }
            } else {

                List<Action<UnityWebRequest>> callbacks = new List<Action<UnityWebRequest>>();
                if (callback != null) { callbacks.Add(callback); }
                m_thumbnailLoadQueue.Add(uri, callbacks);

                UnityWebRequest request = UnityWebRequest.Get(uri);
                SendRequest(request, (requestResult) => {
                    List<Action<UnityWebRequest>> registredCallbacks = m_thumbnailLoadQueue[request.url];
                    m_thumbnailLoadQueue.Remove(request.url);

                    foreach (var cb in registredCallbacks) {
                        cb.Invoke(requestResult);
                    }
                });
            }
        }


        /// <summary>
        /// Sen'd a UnityWebRequest configured for HTTP GET.
        /// </summary>
        /// <param name="uri">The URI of the resource to retrieve via HTTP GET.</param>
        /// <param name="callback"> Returns the downloaded Texture2D, or null.</param>
        public static void GetTexture2D(string uri, Action<Texture2D> callback) {
            Get(uri, (unityWebRequest) => {
                if(unityWebRequest == null) {
                    callback.Invoke(null);
                } else {
                    var texture = new Texture2D(1,1);
                    texture.LoadImage(unityWebRequest.downloadHandler.data);
                    callback.Invoke(texture);
                }
            });
        }


        /// <summary>
        /// Sen'd a UnityWebRequest configured for HTTP GET.
        /// </summary>
        /// <param name="uri">The URI of the resource to retrieve via HTTP GET.</param>
        /// <param name="callback"> Returns the downloaded AudioClip, or null.</param>
        public static void GetAudioClip(string uri, Action<AudioClip> callback) {
            Get(uri, (request) => {
                var result = DownloadHandlerAudioClip.GetContent(request);
                callback.Invoke(result);
            });
        }


        /// <summary>
        /// Removes all elements from the cache
        /// </summary>
        public static void ClearCache() {
            m_cache.Clear();
        }


        private static void SendRequest(UnityWebRequest request, Action<UnityWebRequest> callback) {
            if (m_cache.ContainsKey(request.url)) {
                callback.Invoke(m_cache[request.url]);
                return;
            }
            SA_Coroutine.Start(SendRequestCorutine(request, (result) => {
                m_cache.Add(result.url, result);
                callback.Invoke(result);
            }));
        }


        private static IEnumerator SendRequestCorutine(UnityWebRequest request, Action<UnityWebRequest> callback) {
#if UNITY_2017_2_OR_NEWER 
            yield return request.SendWebRequest();
#else
            yield return request.Send();
#endif
            callback.Invoke(request);
        }

    }
}