//@vydym udod

using Coffee.UIExtensions;
using Fourzy._Updates.Tween;
using System.Collections.Generic;
using UnityEngine;
using StackableDecorator;
using UnityEngine.UI;
using TMPro;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.Tools;

namespace Fourzy._Updates.UI.Widgets
{
    public class OnboardingScreenMaskObject : WidgetBase
    {
        [List]
        public MaskStyleDescCollection styles; 

        public SizeTween sizeTween { get; private set; }

        public List<UnmaskRaycastFilter> unmaskRaycasts { get; private set; }

        private Image image;

        public OnboardingScreenMaskObject Size(Vector2 size)
        {
            if (size != Vector2.zero)
            {
                sizeTween.SetSize(size);
            }

            return this;
        }

        public OnboardingScreenMaskObject SetPivot(Vector2 pivot)
        {
            rectTransform.pivot = pivot;

            return this;
        }

        public OnboardingScreenMaskObject SetStyle(MaskStyle maskStyle)
        {
            image.sprite = styles.list.Find(_style => _style.style == maskStyle).sprite;

            Unmask _unmask = gameObject.AddComponent<Unmask>();
            UnmaskRaycastFilter _unmaskRaycastFilter = transform.parent.gameObject.AddComponent<UnmaskRaycastFilter>();
            unmaskRaycasts.Add(_unmaskRaycastFilter);

            _unmaskRaycastFilter.targetUnmask = _unmask;

            return this;
        }

        public OnboardingScreenMaskObject SetStyle(RectTransform toCopy, Vector3 scale)
        {
            RectTransform copy = Instantiate(toCopy, transform);
            copy.anchorMin = copy.anchorMax = Vector2.one * .5f;
            copy.pivot = Vector2.one * .5f;
            copy.localPosition = Vector3.zero;
            copy.localScale = scale;

            image.enabled = false;

            ClearObject(transform);

            return this;
        }

        public void ClearFilters()
        {
            foreach (UnmaskRaycastFilter filter in unmaskRaycasts) Destroy(filter);
            unmaskRaycasts.Clear();
        }

        private void ClearObject(Transform root)
        {
            foreach (Transform child in root)
            {
                ClearObject(child);

                if (!child.gameObject.activeInHierarchy)
                {
                    Destroy(child.gameObject);
                    continue;
                }

                if (child.GetComponent<Image>() ||
                    child.GetComponent<TMP_Text>())
                {
                    Unmask _unmask = child.gameObject.AddComponent<Unmask>();
                    UnmaskRaycastFilter _unmaskRaycastFilter = transform.parent.gameObject.AddComponent<UnmaskRaycastFilter>();
                    unmaskRaycasts.Add(_unmaskRaycastFilter);

                    _unmaskRaycastFilter.targetUnmask = _unmask;
                }

                foreach (Component comp in child.GetComponents<Component>())
                {
                    //if (comp.GetType() != typeof(Transform) &&
                    //    comp.GetType() != typeof(RectTransform) &&
                    //    comp.GetType() != typeof(TMP_Text) &&
                    //    comp.GetType() != typeof(CanvasRenderer) &&
                    //    comp.GetType() != typeof(Image))
                    if (comp.GetType() == typeof(Button) &&
                        comp.GetType() == typeof(ButtonExtended))
                            Destroy(comp);
                }
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            sizeTween = GetComponent<SizeTween>();
            image = GetComponentInChildren<Image>();

            unmaskRaycasts = new List<UnmaskRaycastFilter>();
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
            COPY,
        }
    }
}