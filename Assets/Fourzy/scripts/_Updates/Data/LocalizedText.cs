//modded @vadym udod

namespace Fourzy
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class LocalizedText : MonoBehaviour
    {
        public string key;
        public bool updateOnStart = true;

        private Text text;
        private TextMeshProUGUI tmPro;

        void Start()
        {
            text = GetComponent<Text>();
            tmPro = GetComponent<TextMeshProUGUI>();

            if (updateOnStart) UpdateLocale();
        }

        public void UpdateLocale()
        {
            string newString;
            int _keyIndex = key.IndexOf("{");
            int _keyEnd = key.IndexOf("}");

            if (_keyIndex > -1 && _keyEnd > -1)
                newString = key.Replace(key.Substring(_keyIndex, _keyEnd - _keyIndex + 1), LocalizationManager.Value(key.Substring(_keyIndex + 1, _keyEnd - _keyIndex - 1)));
            else
                newString = LocalizationManager.Value(key);

            if (tmPro) tmPro.text = newString;
            else if (text) text.text = newString;
        }

        public void UpdateLocale(string _key)
        {
            key = _key;
            UpdateLocale();
        }
    }
}