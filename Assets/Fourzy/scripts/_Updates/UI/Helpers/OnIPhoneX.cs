//@vadym udod

using ByteSheep.Events;
using UnityEngine;
using StackableDecorator;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Helpers
{
    [ExecuteInEditMode]
    public class OnIPhoneX : MonoBehaviour
    {
        [StackableField]
        [Buttons(titles = "Force all,Reset all", actions = "ForceAll,ResetAll", below = true)]
        public bool forceiPhoneX;

        public AdvancedEvent onIPhoneX;
        public AdvancedEvent other;

        private int generation = -1;

        protected void Start()
        {
            if (!Application.isPlaying)
                return;

            CheckPlatform();
        }

        protected void Update()
        {
            if (Application.isEditor)
                CheckPlatform();
        }

        public void CheckPlatform()
        {
#if UNITY_IOS || UNITY_EDITOR
            if (generation < 0 || (UnityEngine.iOS.DeviceGeneration)generation != (forceiPhoneX ? UnityEngine.iOS.DeviceGeneration.iPhoneX : UnityEngine.iOS.Device.generation))
            {
                if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneX ||
                    UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneXR ||
                    UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneXS ||
                    UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneXSMax || forceiPhoneX)
                    onIPhoneX?.Invoke();
                else
                    other?.Invoke();

                generation = (int)(forceiPhoneX ? UnityEngine.iOS.DeviceGeneration.iPhoneX : UnityEngine.iOS.Device.generation);
            }
#endif
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
                rectTransform.sizeDelta = new Vector2(value, rectTransform.sizeDelta.y);
        }

        public void SetPaddingLeft(int value)
        {
            LayoutGroup layoutGroup = GetComponent<LayoutGroup>();

            if (layoutGroup)
                layoutGroup.padding = new RectOffset(value, layoutGroup.padding.right, layoutGroup.padding.top, layoutGroup.padding.bottom);
        }

        public void SetPaddingRight(int value)
        {
            LayoutGroup layoutGroup = GetComponent<LayoutGroup>();

            if (layoutGroup)
                layoutGroup.padding = new RectOffset(layoutGroup.padding.left, value, layoutGroup.padding.top, layoutGroup.padding.bottom);
        }

        public void SetPaddingTop(int value)
        {
            LayoutGroup layoutGroup = GetComponent<LayoutGroup>();

            if (layoutGroup)
                layoutGroup.padding = new RectOffset(layoutGroup.padding.left, layoutGroup.padding.right, value, layoutGroup.padding.bottom);
        }

        public void SetPaddingBottom(int value)
        {
            LayoutGroup layoutGroup = GetComponent<LayoutGroup>();

            if (layoutGroup)
                layoutGroup.padding = new RectOffset(layoutGroup.padding.left, layoutGroup.padding.right, layoutGroup.padding.top, value);
        }

        public void AnchoredPositionY(float value)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();

            if (rectTransform)
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, value);
        }

        public void AnchoredPositionX(float value)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();

            if (rectTransform)
                rectTransform.anchoredPosition = new Vector2(value, rectTransform.anchoredPosition.y);
        }

        public void ForceAll()
        {
            foreach (OnIPhoneX _controller in FindObjectsOfType<OnIPhoneX>()) _controller.forceiPhoneX = true;
        }

        public void ResetAll()
        {
            foreach (OnIPhoneX _controller in FindObjectsOfType<OnIPhoneX>()) _controller.forceiPhoneX = false;
        }
    }
}