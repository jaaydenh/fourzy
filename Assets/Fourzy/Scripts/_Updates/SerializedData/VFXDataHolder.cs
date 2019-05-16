//@vadym udod

using Fourzy._Updates.Mechanics._Vfx;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "DefaultVFXHolder", menuName = "Create VFX Data Holder")]
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