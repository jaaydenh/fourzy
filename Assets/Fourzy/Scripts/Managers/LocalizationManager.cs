

using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;
using System.Linq;
using UnityEngine.Networking;

namespace Fourzy
{
    [UnitySingleton(UnitySingletonAttribute.Type.CreateOnNewGameObject, false)]
    public class LocalizationManager : UnitySingleton<LocalizationManager>
    {
        private const string STR_LOCALIZATION_KEY = "locale";

        public CultureInfo cultureInfo;

        private static Dictionary<string, string> localizedText;
        private string missingTextString = "Localized text not found";

        public string languageCode => cultureInfo.TwoLetterISOLanguageName;

        public bool isReady { get; private set; } = false;

        protected override void Awake()
        {
            base.Awake();

            cultureInfo = GetCultureInfo(PlayerLanguage);

            LoadLocalizedText(PlayerLanguage);

            isReady = true;
        }

        public void LoadLocalizedText(SystemLanguage language)
        {
            localizedText = new Dictionary<string, string>();

            ResourceItem file = ResourceDB.GetFolder(Constants.LOCALIZATION_FOLDER).GetChild(language.ToString(), ResourceItem.Type.Asset);

            string dataAsJson;

            if (file != null)
                dataAsJson = file.Load<TextAsset>().text;
            else
                return;

            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++)
                localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
        }

        public string GetLocalizedValue(string key)
        {
            string result = missingTextString;
            if (localizedText.ContainsKey(key)) result = localizedText[key];

            return result;
        }

        public void SetCurrentLanguage(SystemLanguage language)
        {
            PlayerLanguage = language;
            LoadLocalizedText(PlayerLanguage);
            foreach (LocalizedText text in FindObjectsOfType<LocalizedText>()) text.UpdateLocale();
        }

        public static string Value(string key) => Instance.GetLocalizedValue(key);

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

        public static CultureInfo GetCultureInfo(SystemLanguage language)
        {
            return CultureInfo.GetCultures(CultureTypes.AllCultures).
                FirstOrDefault(x => x.EnglishName == System.Enum.GetName(language.GetType(), language));
        }
    }
}