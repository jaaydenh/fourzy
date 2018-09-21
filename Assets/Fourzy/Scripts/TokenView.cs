using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy
{
    public class TokenView : MonoBehaviour
    {
        [SerializeField]
        public Token tokenType;

        [SerializeField]
        public bool justDisplaying;

        private void Awake()
        {
            if (justDisplaying)
            {
                var components = this.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour component in components)
                {
                    System.Type type = component.GetType();
                    if (type != typeof(SpriteRenderer) && type != typeof(Transform))
                    {
                        component.enabled = false;
                    }
                }
            }
        }
    }
}

