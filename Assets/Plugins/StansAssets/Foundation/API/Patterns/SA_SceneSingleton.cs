////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace SA.Foundation.Patterns {

    /// <summary>
    /// This class implement's a modified version of the singleton pattern implementation,
    /// that can be used with classes extended from a MonoBehaviour.
    /// The singleton object will be destroyed once scene it belongs to was destroyed as well. 
    /// New Singleton object will be created when you entering scene again.
    /// </summary>
    public abstract class SA_SceneSingleton<T> : MonoBehaviour where T : MonoBehaviour {

        private static T s_instance = null;


        /// <summary>
        /// Returns a singleton class instance.
        /// If current instance is not assigned it will try to find an object of the instance type,
        /// in case instance already exists on a scene. If not, new instance will be created
        /// </summary>
        public static T Instance {
			get {
                if (s_instance == null) {
                    s_instance = Object.FindObjectOfType(typeof(T)) as T;
                    if (s_instance == null) {
                        s_instance = new GameObject ().AddComponent<T> ();
                        s_instance.gameObject.name = s_instance.GetType ().FullName;
					}
				}
                return s_instance;
			}
		}


        /// <summary>
        /// True if Singleton Instance exists
        /// </summary>
        public static bool HasInstance {
            get {
                if (s_instance != null) {
                    return true;
                } else {
                    return false;
                }
            }
        }


	}

}
