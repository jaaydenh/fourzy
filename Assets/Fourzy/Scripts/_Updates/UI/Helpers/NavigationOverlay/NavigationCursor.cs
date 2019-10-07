//@vadym udod

using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Widgets;
using UnityEngine;

namespace Fourzy._Updates.UI.Helpers
{
    public class NavigationCursor : WidgetBase
    {
        protected SizeTween sizeTween;

        public void SizeTo(Vector2 value)
        {
            sizeTween.from = rectTransform.rect.size;
            sizeTween.to = value;

            sizeTween.PlayForward(true);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            sizeTween = GetComponent<SizeTween>();
        }
    }
}