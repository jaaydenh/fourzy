//modded @vadym udod

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Board
{
    public class FruitTokenView : TokenView
    {
        [SerializeField]
        private Animator animator;

        private int h_FruitIntoSticky = Animator.StringToHash("FruitIntoSticky");

        public void PlayFruitIntoStickyAnimation()
        {
            animator.Play(h_FruitIntoSticky);

            this.tokenType = Token.STICKY;
        }
    }
}
