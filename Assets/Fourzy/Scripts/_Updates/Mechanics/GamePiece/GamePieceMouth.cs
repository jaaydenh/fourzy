//@vadym udod

using System.Collections.Generic;
using UnityEngine;
using StackableDecorator;

namespace Fourzy._Updates.Mechanics._GamePiece
{
    public class GamePieceMouth : MonoBehaviour
    {
        [List]
        public MouthStateSpritePairCollection states;

        public SpriteRenderer spriteRenderer { get; private set; }
        public Dictionary<MouthState, MouthStateSpritePair> statesFastAccess { get; private set; }

        protected void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            statesFastAccess = new Dictionary<MouthState, MouthStateSpritePair>();

            foreach (MouthStateSpritePair pair in states.list)
                if (!statesFastAccess.ContainsKey(pair.state))
                    statesFastAccess.Add(pair.state, pair);
        }

        public void SetState(MouthState state)
        {
            if (statesFastAccess.ContainsKey(state) && spriteRenderer && statesFastAccess[state].sprite)
                spriteRenderer.sprite = statesFastAccess[state].sprite;
        }

        public enum MouthState
        {
            OPENED,
            CLOSED,
            SLEEPY,
        }

        [System.Serializable]
        public class MouthStateSpritePairCollection
        {
            public List<MouthStateSpritePair> list;
        }

        [System.Serializable]
        public class MouthStateSpritePair
        {
            [HideInInspector]
            public string _name;

            [StackableField]
            [ShowIf("#Check")]
            public MouthState state;
            public Sprite sprite;

            public bool Check()
            {
                _name = state.ToString();

                return true;
            }
        }
    }
}