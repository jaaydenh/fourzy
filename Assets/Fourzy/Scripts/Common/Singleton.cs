using UnityEngine;

namespace Fourzy
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T instance { get; private set; }

        protected void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
            }
            else if (instance != this as T)
            {
                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);
            }

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);
        }
    }
}
