//@vadym udod

using ByteSheep.Events;
using UnityEngine;
using StackableDecorator;
using UnityEngine.UI;
using System.Linq;

namespace Fourzy._Updates.UI.Helpers
{
    [ExecuteInEditMode]
    public class OnIPhoneX : MonoBehaviour
    {
        public static string[] NOTCH_MODELS = new string[] {
            "iPhone12,1",
            "iPhone11,8",
            "iPhone11,6",
            "iPhone11,2",
            "iPhone10,6",
            "Xiaomi Redmi 6 Pro",
            "Xiaomi Redmi Note 7",
        };

        [StackableField]
        [Buttons(titles = "Force all,Reset all", actions = "ForceAll,ResetAll", below = true)]
        public bool forceIPhoneX;

        public AdvancedEvent onIPhoneX;
        public AdvancedEvent other;

        private string model = "0";

        protected void Start()
        {
            if (!Application.isPlaying) return;

            model = "";
            CheckPlatform();
        }

        protected void Update()
        {
            if (!Application.isPlaying)
            {
                CheckPlatform(true);
            }
        }

        public void CheckPlatform(bool editor = false)
        {
            if (editor)
            {
                //for editor use
                if (forceIPhoneX && model == "0")
                {
                    onIPhoneX?.Invoke();
                    model = "1";
                }
                else if (!forceIPhoneX && model == "1")
                {
                    other?.Invoke();
                    model = "0";
                }
            }
            else
            {
                //designed to run only once
                if (model != SystemInfo.deviceModel)
                {
                    if (NOTCH_MODELS.Contains(SystemInfo.deviceModel))
                    {
                        onIPhoneX?.Invoke();
                    }
                    else
                    {
                        other?.Invoke();
                    }

                    model = SystemInfo.deviceModel;
                }
            }
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
            foreach (OnIPhoneX _controller in FindObjectsOfType<OnIPhoneX>()) _controller.forceIPhoneX = true;
        }

        public void ResetAll()
        {
            foreach (OnIPhoneX _controller in FindObjectsOfType<OnIPhoneX>()) _controller.forceIPhoneX = false;
        }
    }
}