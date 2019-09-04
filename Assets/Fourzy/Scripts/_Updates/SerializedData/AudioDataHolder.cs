//@vadym udod

using StackableDecorator;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "DefaultSFXHolder", menuName = "Create SFX Data Holder")]
    public class AudioDataHolder : ScriptableObject
    {
        [List(expandable = false)]
        public AudiosList data;
    }

    [Serializable]
    public class AudiosList
    {
        public List<AudiosIDPair> list;
    }

    [Serializable]
    public class AudiosIDPair
    {
        [HideInInspector]
        public string _name;
        public AudioTypes type;
        [ShowIf("#ShowIf")]
        [StackableField]
        public AudioClip[] clips;

        private bool ShowIf()
        {
            _name = type.ToString();

            return type != AudioTypes.NONE;
        }
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
        #endregion

        #region Game piece sounds
        GAME_PIECE_MOVE = 20,
        #endregion

        #region Common gameplay sounds
        GAME_WON = 40,
        GAME_LOST = 41,
        GAME_LOGO = 42,

        GAME_TURNBASED_LOST = 43,
        GAME_TURNBASED_WON = 44,

        GAME_FOUND = 50,


        REWARD_SPAWN = 61,
        TIMER_BAR_LOST = 62,
        #endregion

        #region Audio
        BG_GARDEN_REALTIME = 100,
        BG_GARDEN_TURN_BASED = 101,
        BG_MAIN_MENU = 102,
        BG_ISLAND = 103,
        BG_FOREST = 104,
        BG_ICE_PALACE = 105,
        #endregion

        #region Tokens Sfxs
        TOKEN_ARROW = 130,
        TOKEN_STICKY = 131,
        TOKEN_SAND = 132,
        TOKEN_WATER = 133,
        TOKEN_PIT = 134,
        TOKEN_GHOST = 135,
        TOKEN_FRUIT = 137,
        TOKEN_BUMPER = 138,
        TOKEN_NINGHTY_LEFT = 139,
        TOKEN_NINGHTY_RIGHT = 140,
        TOKEN_CIRCLE_BOMB_EXPLOSION = 141,
        TOKEN_FRUIT_TREE = 142,
        TOKEN_ICE_BLOCK = 143,

        #endregion
    }
}