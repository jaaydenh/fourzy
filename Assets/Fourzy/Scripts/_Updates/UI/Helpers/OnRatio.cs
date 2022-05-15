//@vadym udod

using ByteSheep.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Helpers
{
    public class OnRatio : MonoBehaviour
    {
        [SerializeField]
        private bool configureAtStart = true;

        public DeviceOrientation orientation = DeviceOrientation.Portrait;
        public AdvancedEvent onIphone;
        public AdvancedEvent onIphoneX;
        public AdvancedEvent onIpad;

        private DisplayRatioOption ratio = DisplayRatioOption.NONE;
        private DisplayRatioOption previousRatio = DisplayRatioOption.NONE;

        private float previousAspect;
        private RectTransform rectTransform;

        private static float aspect => (float)Screen.width / Screen.height;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        protected void Start()
        {
            if (configureAtStart)
            {
                CheckOrientation();
            }
        }

        public void SetHeight(float value)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();

            if (rectTransform)
            {
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, value);
            }
        }

        public void SetWidth(float value)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();

            if (rectTransform)
            {
                rectTransform.sizeDelta = new Vector2(value, rectTransform.sizeDelta.y);
            }
        }

        public void SetPaddingLeft(int value)
        {
            LayoutGroup layoutGroup = GetComponent<LayoutGroup>();

            if (layoutGroup)
            {
                layoutGroup.padding = new RectOffset(value, layoutGroup.padding.right, layoutGroup.padding.top, layoutGroup.padding.bottom);
            }
        }

        public void SetPaddingRight(int value)
        {
            LayoutGroup layoutGroup = GetComponent<LayoutGroup>();

            if (layoutGroup)
            {
                layoutGroup.padding = new RectOffset(layoutGroup.padding.left, value, layoutGroup.padding.top, layoutGroup.padding.bottom);
            }
        }

        public void SetPaddingTop(int value)
        {
            LayoutGroup layoutGroup = GetComponent<LayoutGroup>();

            if (layoutGroup)
            {
                layoutGroup.padding = new RectOffset(layoutGroup.padding.left, layoutGroup.padding.right, value, layoutGroup.padding.bottom);
            }
        }

        public void SetPaddingBottom(int value)
        {
            LayoutGroup layoutGroup = GetComponent<LayoutGroup>();

            if (layoutGroup)
            {
                layoutGroup.padding = new RectOffset(layoutGroup.padding.left, layoutGroup.padding.right, layoutGroup.padding.top, value);
            }
        }

        public void SetAnchorMin(Vector2 min)
        {
            rectTransform.anchorMin = min;
        }

        public void SetAnchorMax(Vector2 max)
        {
            rectTransform.anchorMax = max;
        }

        public static DisplayRatioOption GetRatio(DeviceOrientation orientation)
        {
            switch (orientation)
            {
                case DeviceOrientation.LandscapeLeft:
                case DeviceOrientation.LandscapeRight:
                    if (aspect > 1.78f)
                    {
                        return DisplayRatioOption.IPHONEX;
                    }
                    else if (aspect >= 1.34f)
                    {
                        return DisplayRatioOption.IPHONE;
                    }
                    else
                    {
                        return DisplayRatioOption.IPAD;
                    }

                case DeviceOrientation.Portrait:
                case DeviceOrientation.PortraitUpsideDown:
                    if (aspect <= 1f)
                    {
                        if (aspect > .74f)
                        {
                            return DisplayRatioOption.IPAD;
                        }
                        else if (aspect > .55f)
                        {
                            return DisplayRatioOption.IPHONE;
                        }
                        else
                        {
                            return DisplayRatioOption.IPHONEX;
                        }
                    }

                    break;
            }

            return DisplayRatioOption.NONE;
        }

        public void CheckOrientation()
        {
            ratio = GetRatio(orientation);
            UpdatePrevious();
        }

        private void OnRatioChanged(DisplayRatioOption option)
        {
            if (onIphone == null || onIphoneX == null || onIpad == null) return;
            
            switch (option)
            {
                case DisplayRatioOption.IPHONE:
                    onIphone.Invoke();

                    break;

                case DisplayRatioOption.IPHONEX:
                    onIphoneX.Invoke();

                    break;

                case DisplayRatioOption.IPAD:
                    onIpad.Invoke();

                    break;
            }
        }

        private void UpdatePrevious()
        {
            if (Application.isEditor)
            {
                if (previousAspect != aspect)
                {
                    OnRatioChanged(ratio);
                    previousRatio = ratio;
                }
            }
            else if (ratio != previousRatio)
            {
                OnRatioChanged(ratio);
                previousRatio = ratio;
            }
        }

        public enum DisplayRatioOption
        {
            NONE,

            IPHONE,
            IPHONEX,
            IPAD,
        }
    }
}