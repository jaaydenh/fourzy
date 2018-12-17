//@vadym udod

using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.Mechanics._GamePiece
{
    public class GamePieceMouth : MonoBehaviour
    {
        public MouthStateSpritePair[] states;

        public SpriteRenderer spriteRenderer { get; private set; }
        public Dictionary<MouthState, MouthStateSpritePair> statesFastAccess { get; private set; }

        protected void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            statesFastAccess = new Dictionary<MouthState, MouthStateSpritePair>();

            foreach (MouthStateSpritePair pair in states)
                if (!statesFastAccess.ContainsKey(pair.state))
                    statesFastAccess.Add(pair.state, pair);
        }

        public void SetState(MouthState state)
        {
            if (statesFastAccess.ContainsKey(state) && spriteRenderer)
                spriteRenderer.sprite = statesFastAccess[state].sprites[0];
        }

        public enum MouthState
        {
            OPENED,
            CLOSED,
            SLEEPY,
        }

        [System.Serializable]
        public class MouthStateSpritePair
        {
            public MouthState state;
            public Sprite[] sprites;
        }
    }
}