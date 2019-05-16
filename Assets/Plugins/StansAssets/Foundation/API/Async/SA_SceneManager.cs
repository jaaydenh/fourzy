using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;


namespace SA.Foundation.Async
{

    public class SA_SceneManager
    {
        public static event Action<string, LoadSceneMode> SceneLoadCompleted = delegate { };
        public static event Action<string> SceneUnloadCompleted = delegate { };


        private static List<Scene> s_additiveScenes = new List<Scene>();
        private static Dictionary<string, List<Action<Scene>>> s_loadSceneRequests = new Dictionary<string, List<Action<Scene>>>();
        private static Dictionary<string, List<Action>> s_unloadSceneCallbacks = new Dictionary<string, List<Action>>();




        //--------------------------------------
        // Initialization
        //--------------------------------------


        static SA_SceneManager() {
            SceneManager.sceneLoaded += AdditiveSceneLoaded;
            SceneManager.sceneUnloaded += SceneUnloadComplete;
        }



        //--------------------------------------
        // Public Methods
        //--------------------------------------

        /// <summary>
        /// Load Scene  Additively
        /// 
        /// <param name="sceneName"> scene to be loaded </param>
        /// <param name="callback"> callback will be fired once scene is loaded </param>
        /// </summary>
        public static void LoadAdditively(String sceneName, Action<Scene> callback = null) {
            if (!s_loadSceneRequests.ContainsKey(sceneName)) {
                List<Action<Scene>> callbacks = null;
                if (callback != null) {
                    callbacks = new List<Action<Scene>>();
                    callbacks.Add(callback);
                }
                s_loadSceneRequests.Add(sceneName, callbacks);
            } else {
                if (callback != null) {
                    List<Action<Scene>> callbacks = s_loadSceneRequests[sceneName];
                    if (callbacks == null)
                        callbacks = new List<Action<Scene>>();
                    callbacks.Add(callback);
                    s_loadSceneRequests[sceneName] = callbacks;
                }
            }
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        /// <summary>
        /// Unload scene
        /// 
        /// <param name="scene"> scene to be loaded </param>
        /// <param name="callback"> callback will be fired once scene is unloaded </param>
        /// </summary>
        public static void Unload(Scene scene, Action callback = null) {
            Unload(scene.name, callback);
        }


        public static void UnloadLastScene(Action callback = null) {
            if (s_additiveScenes.Count > 0)
                Unload(s_additiveScenes[s_additiveScenes.Count - 1].name, callback);
        }

        /// <summary>
        /// Unload scene
        /// 
        /// <param name="sceneName"> scene to be loaded </param>
        /// <param name="callback"> callback will be fired once scene is unloaded </param>
        /// </summary>
        public static void Unload(String sceneName, Action callback = null) {
            if (!s_unloadSceneCallbacks.ContainsKey(sceneName)) {

                List<Action> callbacks = null;
                if (callback != null) {
                    callbacks = new List<Action>();
                    callbacks.Add(callback);
                }

                s_unloadSceneCallbacks.Add(sceneName, callbacks);

                for (int i = 0; i < s_additiveScenes.Count; i++) {
                    if (s_additiveScenes[i].name == sceneName) {
                        s_additiveScenes.Remove(s_additiveScenes[i]);
                        break;
                    }
                }
#if !UNITY_5
                SceneManager.UnloadSceneAsync(sceneName);
#endif
            }
            else
            {
                List<Action> callbacks = s_unloadSceneCallbacks[sceneName];
                if (callback != null) {
                    if (callbacks == null)
                        callbacks = new List<Action>();
                    callbacks.Add(callback); 
                }
            }
        }


        //--------------------------------------
        // Get / Set
        //--------------------------------------


        //--------------------------------------
        // Internal Events
        //--------------------------------------

        private static void AdditiveSceneLoaded(Scene scene, LoadSceneMode mode) {
            s_additiveScenes.Add(scene);
            SceneLoadCompleted(scene.name, mode);

            List<Action<Scene>> callbacks = s_loadSceneRequests[scene.name];

            if(callbacks != null) {
                foreach (var callback in callbacks) {
                    callback(scene);
                }
            }

            s_loadSceneRequests.Remove(scene.name);
        }


        private static void SceneUnloadComplete(Scene scene) {
            SceneUnloadCompleted(scene.name);

            if (s_unloadSceneCallbacks.ContainsKey(scene.name)) {
                List<Action> callbacks = s_unloadSceneCallbacks[scene.name];

                if (callbacks != null) {
                    foreach (Action callback in callbacks) {
                        callback();
                    }
                }

                s_unloadSceneCallbacks.Remove(scene.name);
            }

        }
    }
}
