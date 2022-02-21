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
        private TMP_Text tmPro;

        void Awake()
        {
            text = GetComponent<Text>();
            tmPro = GetComponent<TMP_Text>();

            if (updateOnStart)
            {
                UpdateLocale();
            }
        }

        public void UpdateLocale()
        {
            string newString;
            int _keyIndex = key.IndexOf("{");
            int _keyEnd = key.IndexOf("}");

            if (_keyIndex > -1 && _keyEnd > -1)
            {
                newString = key.Replace(key.Substring(_keyIndex, _keyEnd - _keyIndex + 1), LocalizationManager.Value(key.Substring(_keyIndex + 1, _keyEnd - _keyIndex - 1)));
            }
            else
            {
                newString = LocalizationManager.Value(key);
            }

            SetText(newString);
        }

        public void UpdateLocale(string _key)
        {
            key = _key;
            UpdateLocale();
        }

        public void SetText(string newString)
        {
            if (tmPro)
            {
                tmPro.text = newString;
            }
            else if (text)
            {
                text.text = newString;
            }
        }
    }
}