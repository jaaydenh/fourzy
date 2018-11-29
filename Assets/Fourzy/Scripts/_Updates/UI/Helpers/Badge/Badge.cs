//@vadym udod

using System;
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

        protected void Awake()
        {
            if (thisTarget)
                targetObject = gameObject;

            if (!targetText)
                targetText = targetObject.GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetValue(string value)
        {
            if (!targetObject)
                return;

            if (hideOnEmpty)
            {
                if (string.IsNullOrEmpty(value))
                {
                    targetObject.SetActive(false);
                    return;
                }

                int intValue = -1;

                if (int.TryParse(value, out intValue) && intValue == 0)
                {
                    targetObject.SetActive(false);
                    return;
                }
            }

            if (!targetText)
                return;

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
            if (!targetObject)
                return;

            targetObject.SetActive(false);
        }

        public void Show()
        {
            if (!targetObject)
                return;

            targetObject.SetActive(true);
        }
    }
}
