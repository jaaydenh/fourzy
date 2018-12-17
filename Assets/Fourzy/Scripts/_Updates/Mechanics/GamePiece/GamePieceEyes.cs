//@vadym udod

using Fourzy._Updates.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.Mechanics._GamePiece
{
    public class GamePieceEyes : RoutinesBase
    {
        public EyesStateSpritePair[] states;

        public SpriteRenderer spriteRenderer { get; private set; }
        public Dictionary<EyesState, EyesStateSpritePair> statesFastAccess { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            spriteRenderer = GetComponent<SpriteRenderer>();

            statesFastAccess = new Dictionary<EyesState, EyesStateSpritePair>();

            foreach (EyesStateSpritePair pair in states)
                if (!statesFastAccess.ContainsKey(pair.state))
                    statesFastAccess.Add(pair.state, pair);
        }

        public void SetState(EyesState state)
        {
            if (statesFastAccess.ContainsKey(state) && spriteRenderer)
                spriteRenderer.sprite = statesFastAccess[state].sprites[0];
        }

        public void Blink(float duration)
        {
            CancelRoutine("blink");
            SetState(EyesState.CLOSED);
            StartRoutine("blink", duration, () => { SetState(EyesState.OPENED); }, null);
        }

        public void Blink(float delay, float duration)
        {
            StartRoutine("blink", delay, () => { Blink(duration); }, null);
        }

        public enum EyesState
        {
            OPENED,
            CLOSED,
            WIN,
        }

        [System.Serializable]
        public class EyesStateSpritePair
        {
            public EyesState state;
            public Sprite[] sprites;
        }
    }
}