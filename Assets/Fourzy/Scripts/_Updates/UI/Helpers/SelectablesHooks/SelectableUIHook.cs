//@vadym udod

using Fourzy._Updates.Tween;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Helpers
{
    public class SelectableUIHook : MonoBehaviour
    {
        public bool inverse = false;

        private List<TweenBase> tweens = new List<TweenBase>();

        protected void Awake()
        {
            tweens.AddRange(GetComponents<TweenBase>());
        }

        public void OnEnter()
        {
            if (inverse)
                tweens.ForEach(tween => tween.PlayBackward(true));
            else
                tweens.ForEach(tween => tween.PlayForward(true));
        }

        public void OnExit()
        {
            if (inverse)
                tweens.ForEach(tween => tween.PlayForward(true));
            else
                tweens.ForEach(tween => tween.PlayBackward(true));
        }
    }
}
