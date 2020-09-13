﻿//@vadym udod

using ByteSheep.Events;
using Fourzy._Updates.Managers;
using StackableDecorator;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Helpers
{
    public class ListenTo : MonoBehaviour
    {
        [List]
        public ListenTargetCollection targets;

        private Dictionary<ListenValues, List<ListenTarget>> sorted;

        private bool listensToNetworkState = false;
        private bool listensToSfxState = false;
        private bool listensToAudioState = false;
        private bool listensToDemoModeState = false;
        private bool listensToAnalyticsState = false;
        private bool listensToMagicState = false;
        private bool listensToLocalTimer = false;
        private bool listensToRealtimeTimer = false;

        protected void Awake()
        {
            sorted = new Dictionary<ListenValues, List<ListenTarget>>();

            foreach (ListenTarget target in targets.list)
            {
                if (!sorted.ContainsKey(target.type)) sorted.Add(target.type, new List<ListenTarget>());

                sorted[target.type].Add(target);

                //add to listeners
                switch (target.type)
                {
                    case ListenValues.NO_INTERNET_ACCESS:
                    case ListenValues.INTERNET_ACCESS:
                        if (!listensToNetworkState)
                        {
                            GameManager.onNetworkAccess += UpdateNoInternet;
                            listensToNetworkState = true;
                        }

                        break;

                    case ListenValues.SETTINGS_SFX_OFF:
                    case ListenValues.SETTINGS_SFX_ON:
                        if (!listensToSfxState)
                        {
                            listensToSfxState = true;
                            SettingsManager.onSfx += OnSfx;
                        }
                        break;

                    case ListenValues.SETTINGS_AUDIO_OFF:
                    case ListenValues.SETTINGS_AUDIO_ON:
                        if (!listensToAudioState)
                        {
                            listensToAudioState = true;
                            SettingsManager.onAudio += OnAudio;
                        }
                        break;

                    case ListenValues.SETTINGS_DEMO_MODE_OFF:
                    case ListenValues.SETTINGS_DEMO_MODE_ON:
                        if (!listensToDemoModeState)
                        {
                            listensToDemoModeState = true;
                            SettingsManager.onDemoMode += OnDemoMode;
                        }
                        break;

                    case ListenValues.SETTINGS_ANALYTICS_ON:
                    case ListenValues.SETTINGS_ANALYTICS_OFF:
                        if (!listensToAnalyticsState)
                        {
                            listensToAnalyticsState = true;
                            SettingsManager.onAnalyticsEvent += OnAnalyticsMode;
                        }

                        break;

                    case ListenValues.SETTINGS_MAGIC_ON:
                    case ListenValues.SETTINGS_MAGIC_OFF:
                        if (!listensToMagicState)
                        {
                            listensToMagicState = true;
                            SettingsManager.onMagic += OnMagic;
                        }

                        break;

                    case ListenValues.SETTINGS_LOCAL_TIMER_ON:
                    case ListenValues.SETTINGS_LOCAL_TIMER_OFF:
                        if (!listensToLocalTimer)
                        {
                            listensToLocalTimer = true;
                            SettingsManager.onLocalTimer += OnLocalTimer;
                        }

                        break;

                    case ListenValues.SETTINGS_REALTIME_TIMER_ON:
                    case ListenValues.SETTINGS_REALTIME_TIMER_OFF:
                        if (!listensToRealtimeTimer)
                        {
                            listensToRealtimeTimer = true;
                            SettingsManager.onRealtimeTimer += OnRealtimeTimer;
                        }

                        break;

                    case ListenValues.PLACEMENT_STYLE:
                        GameManager.onPlacementStyle += OnPlacementSyle;

                        break;
                }
            }
        }

        protected void Start()
        {
            InvokeTargets(false);
        }

        protected void OnDestroy()
        {
            foreach (ListenTarget target in targets.list)
                switch (target.type)
                {
                    case ListenValues.NO_INTERNET_ACCESS:
                    case ListenValues.INTERNET_ACCESS:
                        if (listensToNetworkState)
                        {
                            GameManager.onNetworkAccess -= UpdateNoInternet;
                            listensToNetworkState = false;
                        }

                        break;

                    case ListenValues.SETTINGS_SFX_OFF:
                    case ListenValues.SETTINGS_SFX_ON:
                        if (listensToSfxState)
                        {
                            listensToSfxState = false;
                            SettingsManager.onSfx -= OnSfx;
                        }

                        break;

                    case ListenValues.SETTINGS_AUDIO_OFF:
                    case ListenValues.SETTINGS_AUDIO_ON:
                        if (listensToAudioState)
                        {
                            listensToAudioState = false;
                            SettingsManager.onDemoMode -= OnDemoMode;
                        }

                        break;

                    case ListenValues.SETTINGS_ANALYTICS_ON:
                    case ListenValues.SETTINGS_ANALYTICS_OFF:
                        if (listensToAnalyticsState)
                        {
                            listensToAnalyticsState = false;
                            SettingsManager.onAnalyticsEvent -= OnAnalyticsMode;
                        }

                        break;

                    case ListenValues.SETTINGS_MAGIC_ON:
                    case ListenValues.SETTINGS_MAGIC_OFF:
                        if (listensToMagicState)
                        {
                            listensToMagicState = false;
                            SettingsManager.onMagic -= OnMagic;
                        }

                        break;

                    case ListenValues.SETTINGS_REALTIME_TIMER_ON:
                    case ListenValues.SETTINGS_REALTIME_TIMER_OFF:
                        if (listensToRealtimeTimer)
                        {
                            listensToRealtimeTimer = false;
                            SettingsManager.onRealtimeTimer -= OnRealtimeTimer;
                        }

                        break;

                    case ListenValues.SETTINGS_LOCAL_TIMER_ON:
                    case ListenValues.SETTINGS_LOCAL_TIMER_OFF:
                        if (listensToLocalTimer)
                        {
                            listensToLocalTimer = false;
                            SettingsManager.onLocalTimer -= OnLocalTimer;
                        }

                        break;

                    case ListenValues.PLACEMENT_STYLE:
                        SettingsManager.onDemoMode -= OnDemoMode;

                        break;
                }
        }

        public void InvokeTargets(bool force)
        {
            foreach (ListenTarget target in targets.list)
                if (force || target.updateOnStart)
                    switch (target.type)
                    {
                        case ListenValues.NO_INTERNET_ACCESS:
                            UpdateNoInternet(GameManager.NetworkAccess);

                            break;

                        case ListenValues.SETTINGS_SFX_OFF:
                        case ListenValues.SETTINGS_SFX_ON:
                            OnSfx(SettingsManager.Get(SettingsManager.KEY_SFX));

                            break;

                        case ListenValues.SETTINGS_AUDIO_OFF:
                        case ListenValues.SETTINGS_AUDIO_ON:
                            OnAudio(SettingsManager.Get(SettingsManager.KEY_AUDIO));

                            break;

                        case ListenValues.SETTINGS_DEMO_MODE_ON:
                        case ListenValues.SETTINGS_DEMO_MODE_OFF:
                            OnDemoMode(SettingsManager.Get(SettingsManager.KEY_DEMO_MODE));

                            break;

                        case ListenValues.SETTINGS_ANALYTICS_ON:
                        case ListenValues.SETTINGS_ANALYTICS_OFF:
                            OnAnalyticsMode(SettingsManager.Get(SettingsManager.KEY_ANALYTICS_EVENTS));

                            break;


                        case ListenValues.SETTINGS_MAGIC_ON:
                        case ListenValues.SETTINGS_MAGIC_OFF:
                            OnMagic(SettingsManager.Get(SettingsManager.KEY_MAGIC));

                            break;


                        case ListenValues.SETTINGS_LOCAL_TIMER_ON:
                        case ListenValues.SETTINGS_LOCAL_TIMER_OFF:
                            OnLocalTimer(SettingsManager.Get(SettingsManager.KEY_LOCAL_TIMER));

                            break;


                        case ListenValues.SETTINGS_REALTIME_TIMER_ON:
                        case ListenValues.SETTINGS_REALTIME_TIMER_OFF:
                            OnRealtimeTimer(SettingsManager.Get(SettingsManager.KEY_REALTIME_TIMER));

                            break;

                        case ListenValues.PLACEMENT_STYLE:
                            OnPlacementSyle(GameManager.Instance.placementStyle);

                            break;

                        case ListenValues.APP_VERSION:
                            OnVersion();

                            break;
                    }
        }

        public void UpdateNoInternet(bool state)
        {
            foreach (ListenTarget target in sorted[state ? ListenValues.INTERNET_ACCESS : ListenValues.NO_INTERNET_ACCESS]) 
                target.events.Invoke(string.Format(target.targetText, state));
        }

        public void OnSfx(bool state)
        {
            foreach (ListenTarget target in sorted[state ? ListenValues.SETTINGS_SFX_ON: ListenValues.SETTINGS_SFX_OFF]) 
                target.events.Invoke(string.Format(target.targetText, state));
        }

        public void OnAudio(bool state)
        {
            foreach (ListenTarget target in sorted[state ? ListenValues.SETTINGS_AUDIO_ON : ListenValues.SETTINGS_AUDIO_OFF]) 
                target.events.Invoke(string.Format(target.targetText, state));
        }

        public void OnDemoMode(bool state)
        {
            foreach (ListenTarget target in sorted[state ? ListenValues.SETTINGS_DEMO_MODE_ON : ListenValues.SETTINGS_DEMO_MODE_OFF])
                target.events.Invoke(string.Format(target.targetText, state));
        }

        public void OnAnalyticsMode(bool state)
        {
            foreach (ListenTarget target in sorted[state ? ListenValues.SETTINGS_ANALYTICS_ON : ListenValues.SETTINGS_ANALYTICS_OFF]) 
                target.events.Invoke(string.Format(target.targetText, state));
        }

        public void OnMagic(bool state)
        {
            foreach (ListenTarget target in sorted[state ? ListenValues.SETTINGS_MAGIC_ON : ListenValues.SETTINGS_MAGIC_OFF])
                target.events.Invoke(string.Format(target.targetText, state));
        }

        public void OnRealtimeTimer(bool state)
        {
            foreach (ListenTarget target in sorted[state ? ListenValues.SETTINGS_REALTIME_TIMER_ON : ListenValues.SETTINGS_REALTIME_TIMER_OFF]) 
                target.events.Invoke(string.Format(target.targetText, state));
        }

        public void OnLocalTimer(bool state)
        {
            foreach (ListenTarget target in sorted[state ? ListenValues.SETTINGS_LOCAL_TIMER_ON : ListenValues.SETTINGS_LOCAL_TIMER_OFF]) 
                target.events.Invoke(string.Format(target.targetText, state));
        }

        public void OnPlacementSyle(GameManager.PlacementStyle state)
        {
            foreach (ListenTarget target in sorted[ListenValues.PLACEMENT_STYLE]) target.events.Invoke(string.Format(target.targetText, (int)state));
        }

        public void OnVersion()
        {
            foreach (ListenTarget target in sorted[ListenValues.APP_VERSION]) target.events.Invoke(string.Format(target.targetText, Application.version));
        }
    }

    [Serializable]
    public class ListenTargetCollection
    {
        public List<ListenTarget> list;
    }

    [Serializable]
    public class ListenTarget
    {
        [HideInInspector]
        public string _name;

        [ShowIf("#Check")]
        [StackableField]
        public ListenValues type = ListenValues.CHELLENGE_ID;

        public string targetText = "{0}";

        public bool updateOnStart;
        [Header("Will format target text with value")]
        public QuickStringEvent events;

        public bool Check()
        {
            _name = type.ToString();

            return true;
        }
    }

    public enum ListenValues
    {
        CHELLENGE_ID,
        NO_INTERNET_ACCESS,
        INTERNET_ACCESS,

        SETTINGS_SFX_ON,
        SETTINGS_SFX_OFF,
        SETTINGS_AUDIO_ON,
        SETTINGS_AUDIO_OFF,
        SETTINGS_DEMO_MODE_ON,
        SETTINGS_DEMO_MODE_OFF,
        PLACEMENT_STYLE,
        SETTINGS_ANALYTICS_ON,
        SETTINGS_ANALYTICS_OFF,
        SETTINGS_MAGIC_ON,
        SETTINGS_MAGIC_OFF,
        SETTINGS_REALTIME_TIMER_ON,
        SETTINGS_REALTIME_TIMER_OFF,
        SETTINGS_LOCAL_TIMER_ON,
        SETTINGS_LOCAL_TIMER_OFF,
        APP_VERSION,
    }
}