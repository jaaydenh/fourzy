//@vadym udod

using Fourzy._Updates.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.Managers
{
    public class SettingsManager : RoutinesBase
    {
        public static Action<bool> onSfx;
        public static Action<bool> onAudio;
        public static Action<bool> onDemoMode;

        public const string KEY_SFX = "SETTINGS_SFX";
        public const string KEY_AUDIO = "SETTINGS_AUDIO";
        public const string KEY_DEMO_MODE = "SETTINGS_DEMO_MODE";

        public const bool DEFAULT_SFX = true; 
        public const bool DEFAULT_AUDIO = true;
        public const bool DEFAULT_DEMO_MODE = false;

        private static bool initialized = false;

        public static SettingsManager Instance
        {
            get
            {
                if (!initialized) Initialize();

                return instance;
            }
        }
        private static SettingsManager instance;

        public Dictionary<string, OptionValueWrapper> values = new Dictionary<string, OptionValueWrapper>
        {
            [KEY_SFX] = new OptionValueWrapper() { state = false, @default = DEFAULT_SFX, },
            [KEY_AUDIO] = new OptionValueWrapper() { state = false, @default = DEFAULT_AUDIO, },
            [KEY_DEMO_MODE] = new OptionValueWrapper() { state = false, @default = DEFAULT_DEMO_MODE, },
        };

        public void Set(string key, bool value)
        {
            switch (key)
            {
                case KEY_SFX:
                    PlayerPrefs.SetInt(key, (values[key].state = value) ? 1 : 0);
                    onSfx?.Invoke(value);

                    break;

                case KEY_AUDIO:
                    PlayerPrefs.SetInt(key, (values[key].state = value) ? 1 : 0);
                    onAudio?.Invoke(value);

                    break;

                case KEY_DEMO_MODE:
                    PlayerPrefs.SetInt(key, (values[key].state = value) ? 1 : 0);
                    onDemoMode?.Invoke(value);

                    break;
            }
        }

        public void Toggle(string key) => Set(key, !values[key].state);

        public bool Get(string key) => values[key].state;

        public static void Initialize()
        {
            initialized = true;
            if (instance != null) return;

            GameObject go = new GameObject("SettingsManager");
            go.transform.SetParent(null);
            instance = go.AddComponent<SettingsManager>();

            DontDestroyOnLoad(go);

            //get values
            foreach (KeyValuePair<string, OptionValueWrapper> entry in instance.values)
                entry.Value.state = PlayerPrefs.GetInt(entry.Key, entry.Value.@default ? 1 : 0) == 1 ? true : false;
        }

        public class OptionValueWrapper
        {
            public bool state;
            public bool @default;
        }
    }
}
