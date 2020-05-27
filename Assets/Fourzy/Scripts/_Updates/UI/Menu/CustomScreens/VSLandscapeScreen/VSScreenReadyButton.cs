//@vadym udod

using Fourzy._Updates.Tween;

namespace Fourzy._Updates.UI.Widgets
{
    public class VSScreenReadyButton : WidgetBase
    {
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

                previousState = state;
            }

            button.SetState(state);
        }
    }
}