//@vadym udod

using TMPro;
using UnityEngine;
using StackableDecorator;

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
        [ShowIf("#ShowCheck")]
        [StackableField]
        public TMP_Text targetText;
        
        private bool initialized = false;
        private RectTransform rectTransform;

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

            if (!targetText) return this;

            Show();

            targetText.text = string.Format(format, value);

            return this;
        }

        public Badge SetValue(float value) => SetValue(value.ToString());

        public Badge SetValue(int value) => SetValue(value.ToString());

        public Badge SetPivot(Vector2 pivot)
        {
            rectTransform.pivot = pivot;

            return this;
        }

        public Badge SetPosition(Vector2 position)
        {
            rectTransform.localPosition = position;

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

        private void Initialize()
        {
            if (initialized) return;

            if (thisTarget) targetObject = gameObject;

            if (!targetText) targetText = targetObject.GetComponentInChildren<TMP_Text>();

            rectTransform = GetComponent<RectTransform>();
        }

        //editor stuff
        public bool ShowCheck()
        {
            return !thisTarget;
        }
    }
}
