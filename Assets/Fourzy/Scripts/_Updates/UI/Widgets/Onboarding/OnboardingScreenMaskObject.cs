//@vydym udod

using Coffee.UIExtensions;
using Fourzy._Updates.Tween;
using System.Collections.Generic;
using UnityEngine;
using StackableDecorator;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class OnboardingScreenMaskObject : WidgetBase
    {
        [List]
        public MaskStyleDescCollection styles; 

        public SizeTween sizeTween { get; private set; }
        public Unmask unmask { get; private set; }
        public UnmaskRaycastFilter unmaskRaycast { get; set; }

        private Image image;

        public OnboardingScreenMaskObject Size(Vector2 size)
        {
            if (size != Vector2.zero) sizeTween.SetSize(size);

            return this;
        }

        public OnboardingScreenMaskObject SetStyle(MaskStyle maskStyle)
        {
            image.sprite = styles.list.Find(_style => _style.style == maskStyle).sprite;

            return this;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            sizeTween = GetComponent<SizeTween>();
            unmask = GetComponent<Unmask>();
            image = GetComponentInChildren<Image>();
        }

        [System.Serializable]
        public class MaskStyleDescCollection
        {
            public List<MaskStyleDesc> list;
        }

        [System.Serializable]
        public class MaskStyleDesc
        {
            public MaskStyle style;
            public Sprite sprite;
        }

        public enum MaskStyle
        {
            PX_24,
            PX_16,
            PX_12,
            PX_0,
        }
    }
}