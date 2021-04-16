//@vadym udod

using Fourzy._Updates.Mechanics._Vfx;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "DefaultVFXHolder", menuName = "Create VFX Data Holder")]
    public class VFXDataHolder : ScriptableObject
    {
        [ListDrawerSettings(ListElementLabelName = "type", NumberOfItemsPerPage = 20)]
        public List<VfxNamePair> data;
    }

    [Serializable]
    public class VfxNamePair
    {
        public string type;
        public Vfx[] options;
    }

    public class VfxPool
    {
        public string type;
        public List<Vfx>[] pool;
    }
}