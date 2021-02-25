//@vadym udod

using Fourzy._Updates.Tween;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class VSScreenReadyButton : WidgetBase
    {
        public Sprite buttonOnGraphics;
        public Sprite buttonOffGraphics;

        public Image buttonImage;
        public ColorTween[] tweens;

        private bool previousState = true;

        public void SetState(bool state)
        {
            if (previousState != state)
            {
                foreach (ColorTween tween in tweens)
                {
                    if (state)
                        tween.PlayBackward(true);
                    else
                        tween.PlayForward(true);
                }

                buttonImage.sprite = state ? buttonOnGraphics : buttonOffGraphics;

                previousState = state;
            }

            button.SetState(state);
        }
    }
}