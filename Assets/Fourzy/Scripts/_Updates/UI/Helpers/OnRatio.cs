//@vadym udod

using ByteSheep.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Helpers
{
    [ExecuteInEditMode]
    public class OnRatio : MonoBehaviour
    {
        public static float LANDSCAPE_IPHONEX = 1.85f;
        public static float LANDSCAPE_IPHONE = 1.5f;
        public static float LANDSCAPE_IPAD = 1f;
        public static float PORTRAIT_IPAD = .7f;
        public static float PORTRAIT_IPHONE = .51f;

        public bool continious = false;

        public DeviceOrientation orientation = DeviceOrientation.Portrait;
        public AdvancedEvent onIphone;
        public AdvancedEvent onIphoneX;
        public AdvancedEvent onIpad;

        private DisplayRatioOption ratio = DisplayRatioOption.NONE;
        private DisplayRatioOption previousRatio = DisplayRatioOption.NONE;

        private float previousAspect;

        private static float aspect => Screen.width / Screen.height;

        protected void Start()
        {
            if (!Application.isPlaying) return;

            CheckOrientation();
        }

        protected void Update()
        {
            if (continious || Application.isEditor) CheckOrientation();
        }

        public void SetHeight(float value)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();

            if (rectTransform)
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, value);
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

        public static DisplayRatioOption GetRatio(DeviceOrientation orientation)
        {
            switch (orientation)
            {
                case DeviceOrientation.LandscapeLeft:
                case DeviceOrientation.LandscapeRight:
                    if (aspect > LANDSCAPE_IPHONEX)
                    {
                        return DisplayRatioOption.IPHONEX;
                    }
                    else if (aspect > LANDSCAPE_IPHONE)
                    {
                        return DisplayRatioOption.IPHONE;
                    }
                    else if (aspect > LANDSCAPE_IPAD)
                    {
                        return DisplayRatioOption.IPAD;
                    }

                    break;

                case DeviceOrientation.Portrait:
                case DeviceOrientation.PortraitUpsideDown:
                    if (aspect <= 1f)
                    {
                        if (aspect >= PORTRAIT_IPAD)
                        {
                            return DisplayRatioOption.IPAD;
                        }
                        else if (aspect >= PORTRAIT_IPHONE)
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