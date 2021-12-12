//@vadym udod

using Fourzy._Updates.Tween;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class VSScreenPlayerSwitchButton : MonoBehaviour
    {
        private TweenBase[] tweens;

        private void Awake()
        {
            tweens = GetComponentsInChildren<TweenBase>();
        }

        public void SetPlayerState(bool isP2, bool animate)
        {
            foreach (TweenBase tween in tweens)
            {
                if (animate)
                {
                    if (isP2)
                    {
                        tween.PlayForward(true);
                    }
                    else
                    {
                        tween.PlayBackward(true);
                    }
                }
                else 
                { 
                    if (isP2)
                    {
                        tween.AtProgress(1f);
                    }
                    else
                    {
                        tween.AtProgress(0f);
                    }
                }
            }
        }
    }
}
