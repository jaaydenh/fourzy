////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;


namespace SA.Foundation.Patterns
{

    /// <summary>
    /// Class can be used for treating some prefabs as singletones.
    /// Class name should match a prefab name inside the Resources folder
    /// </summary>
    public abstract class SA_SingletonePrefab<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T s_instance = null;


        /// <summary>
        /// Returns a singleton class instance
        /// If current instance is not assigned it will try to find an object of the instance type,
        /// in case instance already exists on a scene. If not, new instance will be created.
        /// </summary>
        public static T Instance {
            get {
                if (s_instance == null) {
                    s_instance = FindObjectOfType(typeof(T)) as T;
                    if (s_instance == null) {


                        GameObject prefab = UnityEngine.Object.Instantiate(Resources.Load(typeof(T).Name)) as GameObject;
                        s_instance = prefab.GetComponent<T>();
                        DontDestroyOnLoad(prefab);
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
                return s_instance != null;
            }
        }
    }
    
}