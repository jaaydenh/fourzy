//@vadym udod

using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.Mechanics._Vfx
{
    public class VfxHolder : MonoBehaviour
    {
        public static VfxHolder instance;

        public VFXDataHolder vfxDataHolder;

        public int defaultPoolSize = 10;

        [HideInInspector]
        public VfxPool[] pools;

        private Dictionary<string, int> vfxDictionary;

        protected void Awake()
        {
            if (instance != null) return;

            instance = this;

            vfxDictionary = new Dictionary<string, int>();

            pools = new VfxPool[vfxDataHolder.data.Count];

            for (int poolIndex = 0; poolIndex < vfxDataHolder.data.Count; poolIndex++)
            {
                vfxDictionary.Add(vfxDataHolder.data[poolIndex].type, poolIndex);

                pools[poolIndex] = new VfxPool();

                pools[poolIndex].pool = new List<Vfx>[vfxDataHolder.data[poolIndex].options.Length];
                pools[poolIndex].type = vfxDataHolder.data[poolIndex].type;

                for (int decayIndex = 0; decayIndex < vfxDataHolder.data[poolIndex].options.Length; decayIndex++)
                {
                    pools[poolIndex].pool[decayIndex] = new List<Vfx>();

                    for (int b = 0; b < defaultPoolSize; b++)
                    {
                        AddVfx(poolIndex, decayIndex);
                    }
                }
            }
        }

        public T GetVfx<T>(string type, int poolIndex = - 1) where T : Vfx
        {
            if (pools.Length == 0) return default;

            VfxPool upperPool = pools[vfxDictionary[type]];

            Vfx result = null;
            List<Vfx> pool;

            if (poolIndex == -1)
            {
                pool = upperPool.pool[Random.Range(0, upperPool.pool.Length)];
            }
            else
            {
                pool = upperPool.pool[poolIndex];
            }

            foreach (Vfx vfx in pool)
            {
                if (!vfx.isActive)
                {
                    result = vfx;
                    break;
                }
            }

            if (!result)
            {
                result = AddVfx(vfxDictionary[type], upperPool.pool.ElementIndex(pool));
            }

            return result as T;
        }

        public int GetRandomPoolIndex(string type)
        {
            VfxPool upperPool = pools[vfxDictionary[type]];

            return Random.Range(0, upperPool.pool.Length);
        }

        public Vfx AddVfx(int poolIndex, int decayIndex)
        {
            Vfx vfx = Instantiate(vfxDataHolder.data[poolIndex].options[decayIndex]);
            vfx.Initialize(this);

            vfx.transform.SetParent(transform);

            pools[poolIndex].pool[decayIndex].Add(vfx);

            return vfx;
        }
    }
}