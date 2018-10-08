using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy
{
    public class GameManagersDontDestroy : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
