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
        public static Action<bool> onAnalyticsEvent;

        public const string KEY_SFX = "SETTINGS_SFX";
        public const string KEY_AUDIO = "SETTINGS_AUDIO";
        public const string KEY_DEMO_MODE = "SETTINGS_DEMO_MODE";
        public const string KEY_ANALYTICS_EVENTS = "SETTINGS_ANALYTICS";
        public const string KEY_PASS_N_PLAY_TIMER = "SETTINGS_TIMER";

        public const bool DEFAULT_SFX = true;
        public const bool DEFAULT_AUDIO = true;
        public const bool DEFAULT_DEMO_MODE = false;
        public const bool DEFAULT_PASS_N_PLAY_TIMER = true;

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

        public Dictionary<string, bool> values = new Dictionary<string, bool>
        {
            [KEY_SFX] = DEFAULT_SFX,
            [KEY_AUDIO] = DEFAULT_AUDIO,
            [KEY_DEMO_MODE] = DEFAULT_DEMO_MODE,
            [KEY_PASS_N_PLAY_TIMER] = DEFAULT_PASS_N_PLAY_TIMER,

            //temp
            [KEY_ANALYTICS_EVENTS] = false,
        };

        public static void Set(string key, bool value)
        {
            PlayerPrefs.SetInt(key, (Instance.values[key] = value) ? 1 : 0);

            switch (key)
            {
                case KEY_SFX:
                    onSfx?.Invoke(value);

                    break;

                case KEY_AUDIO:
                    onAudio?.Invoke(value);

                    break;

                case KEY_DEMO_MODE:
                    onDemoMode?.Invoke(value);

                    break;

                case KEY_ANALYTICS_EVENTS:
                    onAnalyticsEvent?.Invoke(value);

                    break;
            }
        }

        public static void Toggle(string key) => Set(key, !Instance.values[key]);

        public static bool Get(string key) => Instance.values[key];

        public static void Initialize()
        {
            initialized = true;
            if (instance != null) return;

            GameObject go = new GameObject("SettingsManager");
            go.transform.SetParent(null);
            instance = go.AddComponent<SettingsManager>();

            DontDestroyOnLoad(go);

            List<string> keys = new List<string>(instance.values.Keys);
            foreach (string key in keys)
            {
                switch (key)
                {
                    case KEY_ANALYTICS_EVENTS:
#if UNITY_EDITOR
                        instance.values[key] = PlayerPrefs.GetInt(key, 0) == 1 ? true : false;
#else
                        instance.values[key] = PlayerPrefs.GetInt(key, 1) == 1 ? true : false;
#endif

                        break;

                    default:
                        instance.values[key] = PlayerPrefs.GetInt(key, instance.values[key] ? 1 : 0) == 1 ? true : false;

                        break;
                }
            }
        }
    }
}
