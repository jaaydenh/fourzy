namespace Fourzy
{
    using UnityEngine;
    using UnityEngine.UI;

    public class LocalizedText : MonoBehaviour
    {
        public string key;
        Text text;

        void Start()
        {
            text = GetComponent<Text>();
            UpdateLocale();
        }

        public void UpdateLocale()
        {
            text.text = LocalizationManager.Instance.GetLocalizedValue(key);
        }
    }
}