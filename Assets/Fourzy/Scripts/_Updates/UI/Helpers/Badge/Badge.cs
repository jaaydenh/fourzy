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
        public TextMeshProUGUI targetText;
        
        private bool initialized = false;

        protected void Awake()
        {
            Initialize();
        }

        public void SetValue(string value)
        {
            Initialize();
            
            if (!targetObject)
                return;

            if (hideOnEmpty)
            {
                if (string.IsNullOrEmpty(value))
                {
                    Hide();
                    return;
                }

                int intValue = -1;

                if (int.TryParse(value, out intValue) && intValue == 0)
                {
                    Hide();
                    return;
                }
            }

            if (!targetText)
                return;

            Show();

            targetText.text = string.Format(format, value);
        }

        public void SetValue(float value)
        {
            SetValue(value.ToString());
        }

        public void SetValue(int value)
        {
            SetValue(value.ToString());
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
            if (initialized)
                return;

            if (thisTarget)
                targetObject = gameObject;

            if (!targetText)
                targetText = targetObject.GetComponentInChildren<TextMeshProUGUI>();
        }

        //editor stuff
        public bool ShowCheck()
        {
            return !thisTarget;
        }
    }
}
