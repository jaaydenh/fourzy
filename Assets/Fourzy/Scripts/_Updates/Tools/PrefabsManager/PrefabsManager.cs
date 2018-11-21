//@vadym udod

using System;
using UnityEngine;

namespace Fourzy._Updates.Prefabs
{
    public class PrefabsManager : MonoBehaviour
    {
        public static PrefabsManager instance;

        public PrefabTypePair[] prefabs;

        protected void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public static T GetPrefab<T>(PrefabType type, Transform parent) where T : Component
        {
            GameObject result = null;

            foreach (PrefabTypePair pair in instance.prefabs)
                if (pair.prefabType == type)
                {
                    result = Instantiate(pair.prefab, parent);

                    result.transform.localScale = Vector3.one;
                }

            if (result)
                return result.GetComponent<T>();
            else
                return null;
        }

        public static T GetPrefab<T>(PrefabType type) where T : Component
        {
            return GetPrefab<T>(type, null);
        }

        [Serializable]
        public class PrefabTypePair
        {
            public GameObject prefab;
            public PrefabType prefabType;
        }
    }

    public enum PrefabType
    {
        NONE = 0,

        COINS_WIDGET_SMALL = 1,
        //
        GAME_PIECE_SMALL = 5,

        LENGTH,
    }
}