//@vadym udod

using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Vfx
{
    public class VfxHolder : MonoBehaviour
    {
        public static VfxHolder instance;

        public VFXDataHolder vfxDataHolder;

        public int defaultPoolSize = 10;

        [HideInInspector]
        public VfxPool[] pools;

        private Dictionary<VfxType, int> decaysDictionary;

        protected void Awake()
        {
            if (instance != null)
                return;

            instance = this;
            decaysDictionary = new Dictionary<VfxType, int>();
        }

        protected void Start()
        {
            pools = new VfxPool[vfxDataHolder.data.Length];

            for (int poolIndex = 0; poolIndex < vfxDataHolder.data.Length; poolIndex++)
            {
                decaysDictionary.Add(vfxDataHolder.data[poolIndex].type, poolIndex);

                pools[poolIndex] = new VfxPool();

                pools[poolIndex].pool = new List<Vfx>[vfxDataHolder.data[poolIndex].options.Length];
                pools[poolIndex].type = vfxDataHolder.data[poolIndex].type;

                for (int decayIndex = 0; decayIndex < vfxDataHolder.data[poolIndex].options.Length; decayIndex++)
                {
                    pools[poolIndex].pool[decayIndex] = new List<Vfx>();

                    for (int b = 0; b < defaultPoolSize; b++)
                        AddVfx(poolIndex, decayIndex);
                }
            }
        }

        public Vfx ShowVfx(VfxType type, Transform target, Vector3 offset, int poolIndex = -1, float rotation = 0f, float customDuration = 0f)
        {
            Vfx vfxToActivate = GetVfx<Vfx>(type, poolIndex);

            if (!vfxToActivate)
                return null;

            if (customDuration != 0f)
                vfxToActivate.StartVfx(target, offset, rotation, customDuration);
            else
                vfxToActivate.StartVfx(target, offset, rotation);

            return vfxToActivate;
        }

        public Vfx ShowVfx(VfxType type, Transform target)
        {
            return ShowVfx(type, target, Vector3.zero);
        }

        public Vfx ShowVfx(VfxType type, Vector3 position)
        {
            return ShowVfx(type, transform, position);
        }

        public T GetVfx<T>(VfxType type, int poolIndex) where T : Vfx
        {
            if (pools.Length == 0)
                return default(T);

            VfxPool upperPool = pools[decaysDictionary[type]];

            Vfx result = null;
            List<Vfx> pool;

            if (poolIndex == -1)
                pool = upperPool.pool[Random.Range(0, upperPool.pool.Length)];
            else
                pool = upperPool.pool[poolIndex];

            foreach (Vfx vfx in pool)
            {
                if (!vfx.isActive)
                {
                    result = vfx;
                    break;
                }
            }

            if (!result)
                result = AddVfx(decaysDictionary[type], upperPool.pool.ElementIndex(pool));

            return result as T;
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