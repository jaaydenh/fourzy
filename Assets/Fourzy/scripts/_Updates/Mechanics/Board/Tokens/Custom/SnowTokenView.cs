//@vadym udod

using System.Collections;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Board
{
    public class SnowTokenView : TokenView
    {
        public override IEnumerator OnActivated()
        {
            base.OnActivate();
            scaleTween.from = transform.localScale;
            scaleTween.PlayForward(true);

            yield return new WaitForSeconds(scaleTween.playbackTime - .2f);
        }
    }
}
