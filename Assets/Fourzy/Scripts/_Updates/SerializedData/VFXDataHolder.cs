//@vadym udod

using Fourzy._Updates.Mechanics.Vfx;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "DefaultVFXHolder", menuName = "Create VFX data holder")]
    public class VFXDataHolder : ScriptableObject
    {
        public VfxNamePair[] data;
    }

    [Serializable]
    public class VfxNamePair
    {
        public VfxType type;
        public Vfx[] options;
    }

    public class VfxPool
    {
        public VfxType type;
        public List<Vfx>[] pool;
    }
}