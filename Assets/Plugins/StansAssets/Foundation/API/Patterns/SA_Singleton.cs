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
    /// This class simplifies a singleton pattern implementation,
    /// that can be used with classes extended from a MonoBehaviour
    /// Once instance is found or created gameobject will be marked as DontDestroyOnLoad
    /// </summary>
    public abstract class SA_Singleton<T> : MonoBehaviour where T : MonoBehaviour {

        private static T s_instance = null;
        private static bool s_applicationIsQuitting = false;



        protected virtual void Awake() {
            DontDestroyOnLoad(gameObject);
            gameObject.transform.SetParent(SA_SingletonService.Parent);
        }


        /// <summary>
        /// Returns a singleton class instance
        /// If current instance is not assigned it will try to find an object of the instance type,
        /// in case instance already exists on a scene. If not, new instance will be created
        /// </summary>
        public static T Instance {
			get {

                if (s_applicationIsQuitting) {
                    Debug.LogError(typeof(T) + " [SA_Singleton] is already destroyed. Returning null. Please check HasInstance first before accessing instance in destructor.");
                    return null;
                }

                if (s_instance == null) {
                    s_instance = Object.FindObjectOfType(typeof(T)) as T;
                    if (s_instance == null) {
                        Instantiate();
                    }
                }
				return s_instance;
			}
		}


        /// <summary>
        /// Methods will create new object Instantiate
        /// Normally method is called automatically when you referring to and Instance getter
        /// for a first time.
        /// But it may be useful if you want manually control when the instance is created,
        /// even if you do not this specific instance at the moment
        /// </summary>
        public static void Instantiate() {
            string name = typeof(T).FullName;
            s_instance = new GameObject(name).AddComponent<T>();


        }



        /// <summary>
        /// True if Singleton Instance exists
        /// </summary>
        public static bool HasInstance {
			get {
				return !IsDestroyed;
			}
		}


        /// <summary>
        /// True if Singleton Instance doesn't exist
        /// </summary>
        public static bool IsDestroyed {
			get {
				if(s_instance == null) {
					return true;
				} else {
					return false;
				}
			}
		}

        



		/// <summary>
		/// When Unity quits, it destroys objects in a random order.
		/// In principle, a Singleton is only destroyed when application quits.
		/// If any script calls Instance after it have been destroyed, 
		/// it will create a buggy ghost object that will stay on the Editor scene
		/// even after stopping playing the Application. Really bad!
		/// So, this was made to be sure we're not creating that buggy ghost object.
		/// </summary>
		protected virtual void OnDestroy () {
			s_instance = null;
			s_applicationIsQuitting = true;
		}
		
		protected virtual void OnApplicationQuit () {
			s_instance = null;
			s_applicationIsQuitting = true;
		}

	}


  

}
