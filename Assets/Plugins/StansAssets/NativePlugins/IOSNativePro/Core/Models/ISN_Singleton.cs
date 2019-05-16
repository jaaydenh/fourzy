using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Patterns;

namespace SA.iOS.Utilities
{
    public abstract class ISN_Singleton<T> : SA_Singleton<T> where T : MonoBehaviour
    {
        protected override void Awake() {
            DontDestroyOnLoad(gameObject);
            gameObject.transform.SetParent(ISN_Services.Parent);
        }
    }
}
