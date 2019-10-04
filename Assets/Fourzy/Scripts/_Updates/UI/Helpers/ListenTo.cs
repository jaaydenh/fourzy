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

        private bool listensToSfxState = false;
        private bool listensToAudioState = false;
        private bool listensToDemoModeState = false;
        private bool listensToPlacementStyle = false;

        protected void Awake()
        {
            sorted = new Dictionary<ListenValues, List<ListenTarget>>();

            foreach (ListenTarget target in targets.list)
            {
                if (!sorted.ContainsKey(target.type))
                    sorted.Add(target.type, new List<ListenTarget>());

                sorted[target.type].Add(target);

                //add to listeners
                switch (target.type)
                {
                    case ListenValues.NO_INTERNET_ACCESS:
                        GameManager.onNetworkAccess += UpdateNoInternet;
                        break;

                    case ListenValues.LOGGED_IN:
                        LoginManager.OnDeviceLoginComplete += OnLogin;
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

                    case ListenValues.PLACEMENT_STYLE_1:
                    case ListenValues.PLACEMENT_STYLE_2:
                        if (!listensToPlacementStyle)
                        {
                            listensToPlacementStyle = true;
                            GameManager.onPlacementStyle += OnPlacementSyle;
                        }
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
                        GameManager.onNetworkAccess -= UpdateNoInternet;

                        break;

                    case ListenValues.LOGGED_IN:
                        LoginManager.OnDeviceLoginComplete -= OnLogin;

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

                    case ListenValues.PLACEMENT_STYLE_1:
                    case ListenValues.PLACEMENT_STYLE_2:
                        if (listensToPlacementStyle)
                        {
                            listensToPlacementStyle = false;
                            SettingsManager.onDemoMode -= OnDemoMode;
                        }

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
                            OnSfx(SettingsManager.Instance.Get(SettingsManager.KEY_SFX));

                            break;

                        case ListenValues.SETTINGS_AUDIO_OFF:
                        case ListenValues.SETTINGS_AUDIO_ON:
                            OnAudio(SettingsManager.Instance.Get(SettingsManager.KEY_AUDIO));

                            break;

                        case ListenValues.SETTINGS_DEMO_MODE_ON:
                        case ListenValues.SETTINGS_DEMO_MODE_OFF:
                            OnDemoMode(SettingsManager.Instance.Get(SettingsManager.KEY_DEMO_MODE));

                            break;

                        case ListenValues.PLACEMENT_STYLE_1:
                        case ListenValues.PLACEMENT_STYLE_2:
                            OnPlacementSyle(GameManager.Instance.placementStyle);

                            break;
                    }
        }

        public void UpdateNoInternet(bool state)
        {
            if (!state)
                foreach (ListenTarget target in sorted[ListenValues.NO_INTERNET_ACCESS])
                    target.events.Invoke(string.Format(target.targetText, state));
        }

        public void OnLogin(bool result)
        {
            if (result)
                foreach (ListenTarget target in sorted[ListenValues.LOGGED_IN])
                    target.events.Invoke(string.Format(target.targetText, result));
        }

        public void OnSfx(bool state)
        {
            if (state)
                foreach (ListenTarget target in sorted[ListenValues.SETTINGS_SFX_ON])
                    target.events.Invoke(string.Format(target.targetText, state));
            else
                foreach (ListenTarget target in sorted[ListenValues.SETTINGS_SFX_OFF])
                    target.events.Invoke(string.Format(target.targetText, state));
        }

        public void OnAudio(bool state)
        {
            if (state)
                foreach (ListenTarget target in sorted[ListenValues.SETTINGS_AUDIO_ON])
                    target.events.Invoke(string.Format(target.targetText, state));
            else
                foreach (ListenTarget target in sorted[ListenValues.SETTINGS_AUDIO_OFF])
                    target.events.Invoke(string.Format(target.targetText, state));
        }

        public void OnDemoMode(bool state)
        {
            if (state)
                foreach (ListenTarget target in sorted[ListenValues.SETTINGS_DEMO_MODE_ON])
                    target.events.Invoke(string.Format(target.targetText, state));
            else
                foreach (ListenTarget target in sorted[ListenValues.SETTINGS_DEMO_MODE_OFF])
                    target.events.Invoke(string.Format(target.targetText, state));
        }

        public void OnPlacementSyle(GameManager.PlacementStyle state)
        {
            if (state == GameManager.PlacementStyle.DEFAULT)
                foreach (ListenTarget target in sorted[ListenValues.PLACEMENT_STYLE_1])
                    target.events.Invoke(string.Format(target.targetText, state));
            else
                foreach (ListenTarget target in sorted[ListenValues.PLACEMENT_STYLE_2])
                    target.events.Invoke(string.Format(target.targetText, state));
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

        [ShowIf("#TargetTextCheck")]
        [StackableField]
        public string targetText = "{0}";

        public bool updateOnStart;
        [Header("Will format target text with value")]
        public QuickStringEvent events;

        public bool TargetTextCheck() => ListenValues.USES_TEXT.HasFlag(type);

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
        LOGGED_IN,

        SETTINGS_SFX_ON,
        SETTINGS_SFX_OFF,
        SETTINGS_AUDIO_ON,
        SETTINGS_AUDIO_OFF,
        SETTINGS_DEMO_MODE_ON,
        SETTINGS_DEMO_MODE_OFF,
        PLACEMENT_STYLE_1,
        PLACEMENT_STYLE_2,

        USES_TEXT = CHELLENGE_ID,
    }
}