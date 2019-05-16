//modded

using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;
using System.Linq;

namespace Fourzy
{
    [UnitySingleton(UnitySingletonAttribute.Type.CreateOnNewGameObject, false)]
    public class LocalizationManager : UnitySingleton<LocalizationManager>
    {
        const string STR_LOCALIZATION_KEY = "locale";

        public CultureInfo cultureInfo;

        private static Dictionary<string, string> localizedText;
        private string missingTextString = "Localized text not found";

        public bool isReady { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            cultureInfo = GetCultureInfo(PlayerLanguage);
            LoadLocalizedText(PlayerLanguage);
        }

        public void LoadLocalizedText(SystemLanguage language)
        {
            string fileName = language.ToString() + ".json";
            //string fileName = "English" + ".json";
            localizedText = new Dictionary<string, string>();

            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

            string dataAsJson = "";

            if (Application.platform == RuntimePlatform.Android)
            {
                // Android only use WWW to read file
                WWW reader = new WWW(filePath);
                while (!reader.isDone) { }

                dataAsJson = reader.text;
            }
            else
                dataAsJson = File.ReadAllText(filePath);

            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++)
                localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);

            isReady = true;
        }

        public string GetLocalizedValue(string key)
        {
            string result = missingTextString;
            if (localizedText.ContainsKey(key))
            {
                result = localizedText[key];
            }

            return result;
        }

        public static SystemLanguage PlayerLanguage
        {
            get
            {
                if ((SystemLanguage) PlayerPrefs.GetInt(STR_LOCALIZATION_KEY, (int)Application.systemLanguage) == SystemLanguage.Spanish) {
                    return SystemLanguage.Spanish;
                }
                if ((SystemLanguage)PlayerPrefs.GetInt(STR_LOCALIZATION_KEY, (int)Application.systemLanguage) == SystemLanguage.Thai)
                {
                    return SystemLanguage.Thai;
                }
                return SystemLanguage.English;
            }
            set
            {
                PlayerPrefs.SetInt(STR_LOCALIZATION_KEY, (int)value);
                PlayerPrefs.Save();
            }
        }

        public static void SetCurrentLanguage(SystemLanguage language)
        {
            PlayerLanguage = language;
            LocalizedText[] allTexts = FindObjectsOfType<LocalizedText>();
            for (int i = 0; i < allTexts.Length; i++) {
                allTexts[i].UpdateLocale();
            }
        }

        public static CultureInfo GetCultureInfo(SystemLanguage language)
        {
            return CultureInfo.GetCultures(CultureTypes.AllCultures).
                FirstOrDefault(x => x.EnglishName == System.Enum.GetName(language.GetType(), language));
        }
    }
}