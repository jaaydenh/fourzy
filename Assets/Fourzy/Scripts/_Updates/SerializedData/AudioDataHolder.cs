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

        #region Menu Sfxs
        NEGATIVE = 1,
        BUTTON_CLICK = 2,
        MENU_BACK = 3,
        SCROLL = 4,
        TOGGLE_ON = 5,
        TOGGLE_OFF = 6,
        #endregion

        #region Game piece sounds
        GAME_PIECE_MOVE = 20,
        #endregion

        #region Common gameplay sounds
        GAME_WON = 40,
        GAME_LOST = 41,

        REWARD_SPAWN = 61,
        #endregion

        #region Audio
        GARDEN_REALTIME = 100,
        GARDEN_TURN_BASED = 101,
        #endregion

        #region Tokens Sfxs
        TOKEN_ARROW = 130,
        TOKEN_STICKY = 131,

        #endregion
    }
}