//@vadym udod


using Fourzy._Updates.Mechanics._Vfx;
using Fourzy._Updates.Tween;
using UnityEngine;

namespace Fourzy._Updates.UI.Helpers
{
    public class FakeVirtualPointer : MonoBehaviour
    {
        public ScaleTween pressIndicator;
        public bool showTouchVfx = true;

        private AlphaTween alphaTween;
        private ScaleTween scaleTween;

        private bool hidden = true;

        protected void Awake()
        {
            alphaTween = GetComponent<AlphaTween>();
            scaleTween = GetComponent<ScaleTween>();
        }

        public void SetPosition(Vector2 position)
        {
            transform.position = position;

            if (hidden)
            {
                alphaTween.PlayForward(true);
                hidden = false;
            }
        }

        public void Hide()
        {
            hidden = true;
            alphaTween.PlayBackward(true);

            Relase();
        }

        public void Press()
        {
            if (pressIndicator) pressIndicator.PlayForward(true);
            scaleTween.PlayForward(true);

            if (showTouchVfx) VfxHolder.instance.GetVfx<Vfx>(VfxType.UI_TOUCH_VFX).StartVfx(transform.parent, transform.localPosition, 0f);
        }

        public void Relase()
        {
            if (pressIndicator) pressIndicator.PlayBackward(true);
            scaleTween.PlayBackward(true);
        }
    }
}