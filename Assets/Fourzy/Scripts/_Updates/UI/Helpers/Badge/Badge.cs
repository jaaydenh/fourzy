//@vadym udod

using StackableDecorator;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Helpers
{
    public class Badge : MonoBehaviour
    {
        public bool hideOnEmpty = true;
        public bool thisTarget = true;
        public string format = "{0}";

        [ShowIf("#ShowCheck")]
        [StackableField]
        public GameObject targetObject;

        private bool initialized = false;
        private RectTransform rectTransform;
        private TextMeshPro meshText;
        private TextMeshProUGUI uiText;
        private Canvas canvas;

        protected void Awake()
        {
            Initialize();
        }

        public Badge SetValue(string value)
        {
            Initialize();

            if (!targetObject) return this;

            if (hideOnEmpty)
            {
                if (string.IsNullOrEmpty(value))
                {
                    Hide();
                    return this;
                }

                int intValue = -1;

                if (int.TryParse(value, out intValue) && intValue == 0)
                {
                    Hide();
                    return this;
                }
            }

            if (!meshText && !uiText)
            {
                Debug.Log($"No mesh text or uiText on {name}");
                return this;
            }

            Show();

            UpdateTextComponents();

            if (canvas) uiText.text = string.Format(format, value);
            else meshText.text = string.Format(format, value);

            return this;
        }

        public Badge SetValue(float value) => SetValue(value.ToString());

        public Badge SetValue(int value) => SetValue(value.ToString());

        public Badge SetPivot(Vector2 pivot)
        {
            rectTransform.pivot = pivot;

            return this;
        }

        public Badge SetAnchors(Vector2 value)
        {
            rectTransform.anchorMin = rectTransform.anchorMax = value;
            rectTransform.anchoredPosition = Vector2.zero;

            return this;
        }

        public Badge ResetAnchors() => SetAnchors(Vector2.one * .5f);

        public Badge SetPosition(Vector2 position)
        {
            rectTransform.anchoredPosition = position;

            return this;
        }

        public void Hide()
        {
            Initialize();

            if (!targetObject)
                return;

            targetObject.SetActive(false);
        }

        public void Show()
        {
            Initialize();

            if (!targetObject)
                return;

            targetObject.SetActive(true);
        }

        public void SetState(bool state)
        {
            if (state)
                Show();
            else
                Hide();
        }

        public void SetColor(Color color)
        {
            if (!meshText && !uiText)
            {
                Debug.Log($"No mesh text or uiText on {name}");
                return;
            }

            UpdateTextComponents();

            if (canvas) uiText.color = color;
            else meshText.color = color;
        }

        public void Initialize()
        {
            if (initialized) return;

            if (thisTarget) targetObject = gameObject;

            meshText = targetObject.GetComponentInChildren<TextMeshPro>(true);
            uiText = targetObject.GetComponentInChildren<TextMeshProUGUI>(true);

            rectTransform = GetComponent<RectTransform>();

            UpdateValues();
        }

        public Badge UpdateValues()
        {
            canvas = GetComponentInParent<Canvas>();

            return this;
        }

        public Badge UpdateTextComponents()
        {
            if (!meshText && !uiText)
            {
                Debug.Log($"No mesh text or uiText on {name}");
                return this;
            }

            if (canvas)
            {
                uiText.gameObject.SetActive(true);
                if (meshText) meshText.gameObject.SetActive(false);
            }
            else
            {
                if (uiText) uiText.gameObject.SetActive(false);
                meshText.gameObject.SetActive(true);
            }

            return this;
        }

        //editor stuff
        private bool ShowCheck() => !thisTarget;
    }
}
