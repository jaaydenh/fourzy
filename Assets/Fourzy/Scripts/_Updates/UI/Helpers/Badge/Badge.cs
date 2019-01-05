//@vadym udod

using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Helpers
{
    public class Badge : MonoBehaviour
    {
        public bool hideOnEmpty = true;
        public bool thisTarget = true;
        public string format = "{0}";

        [HideInInspector]
        [SerializeField]
        private GameObject targetObject;
        [HideInInspector]
        [SerializeField]
        private TextMeshProUGUI targetText;

        private bool shown = false;
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
            if (!targetObject || !shown)
                return;

            targetObject.SetActive(false);
        }

        public void Show()
        {
            if (!targetObject || shown)
                return;

            targetObject.SetActive(true);
        }

        private void Initialize()
        {
            if (initialized)
                return;

            if (thisTarget)
                targetObject = gameObject;

            if (!targetText)
                targetText = targetObject.GetComponentInChildren<TextMeshProUGUI>();

            shown = gameObject.activeInHierarchy;
        }
    }
}
