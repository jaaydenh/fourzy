//@vadym udod

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "DefaultSFXHolder", menuName = "Create SFX Data Holder")]
    public class AudioDataHolder : ScriptableObject
    {
        [ListDrawerSettings(NumberOfItemsPerPage = 20, ListElementLabelName = "type")]
        public List<AudiosIDPair> data;
    }

    [Serializable]
    public class AudiosIDPair
    {
        public string type;
        public AudioClip[] clips;
    }
}