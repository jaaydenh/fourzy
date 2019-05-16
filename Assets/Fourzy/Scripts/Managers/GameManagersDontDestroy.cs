//@vadym udod

using UnityEngine;

namespace Fourzy
{
    public class GameManagersDontDestroy : MonoBehaviour
    {
        private static bool activeGameManagers;

        private void Awake()
        {
            if (activeGameManagers)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                activeGameManagers = true;
                DontDestroyOnLoad(this.gameObject);
            }
        }
    }
}
