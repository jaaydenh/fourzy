//modded @vadym udod

namespace Fourzy
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class LocalizedText : MonoBehaviour
    {
        public string key;

        private Text text;
        private TextMeshProUGUI tmPro;

        void Start()
        {
            text = GetComponent<Text>();
            tmPro = GetComponent<TextMeshProUGUI>();

            UpdateLocale();
        }

        public void UpdateLocale()
        {
            if (tmPro) tmPro.text = LocalizationManager.Value(key);
            else if (text) text.text = LocalizationManager.Value(key);
        }

        public void UpdateLocale(string _key)
        {
            key = _key;
            UpdateLocale();
        }
    }
}