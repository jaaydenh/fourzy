//@vadym udod

using System;
using UnityEngine;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "DefaultSFXHolder", menuName = "Create SFX data holder")]
    public class AudioDataHolder : ScriptableObject
    {
        public AudiosIDPair[] data;
    }

    [Serializable]
    public class AudiosIDPair
    {
        public AudioTypes type;
        public AudioClip[] audios;
    }

    [Serializable]
    public enum AudioTypes
    {
        NONE = 0,

        //menu sfxs
        NEGATIVE = 1,
        BUTTON_CLICK = 2,
        MENU_BACK = 3,
        SCROLL = 4,

        SIZE,
    }
}