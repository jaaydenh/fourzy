//@vadym udod

using Fourzy._Updates.Mechanics._Vfx;
using System;
using System.Collections.Generic;
using UnityEngine;
using StackableDecorator;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "DefaultVFXHolder", menuName = "Create VFX Data Holder")]
    public class VFXDataHolder : ScriptableObject
    {
        [List]
        public VfxNameList data;
    }

    [Serializable]
    public class VfxNameList
    {
        public List<VfxNamePair> list;
    }

    [Serializable]
    public class VfxNamePair
    {
        [HideInInspector]
        public string _name;

        [ShowIf("#Check")]
        [StackableField]
        public VfxType type;
        public Vfx[] options;

        public bool Check()
        {
            _name = type.ToString();

            return true;
        }
    }

    public class VfxPool
    {
        public VfxType type;
        public List<Vfx>[] pool;
    }
}