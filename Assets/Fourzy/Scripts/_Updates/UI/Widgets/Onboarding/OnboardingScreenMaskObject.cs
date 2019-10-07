//@vydym udod

using Coffee.UIExtensions;
using Fourzy._Updates.Tween;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class OnboardingScreenMaskObject : WidgetBase
    {
        public SizeTween sizeTween { get; private set; }
        public Unmask unmask { get; private set; }
        public UnmaskRaycastFilter unmaskRaycast { get; set; }

        public OnboardingScreenMaskObject Size(Vector2 size)
        {
            sizeTween.SetSize(size);

            return this;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            sizeTween = GetComponent<SizeTween>();
            unmask = GetComponent<Unmask>();
        }
    }
}