//@vadym udod

using ByteSheep.Events;
using Fourzy._Updates.Audio;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics;
using Fourzy._Updates.Mechanics.GameplayScene;
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
        private bool listensToMoveOriginState = false;

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
                    case ListenValues.PLAYER_TIMER:
                        GamePlayManager.OnTimerUpdate += UpdatePlayerTimer;
                        break;

                    case ListenValues.NO_INTERNET_ACCESS:
                        NetworkAccess.onNetworkAccess += UpdateNoInternet;
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

                    case ListenValues.SETTINGS_MOVE_ORIGIN_OFF:
                    case ListenValues.SETTINGS_MOVE_ORIGIN_ON:
                        if (!listensToMoveOriginState)
                        {
                            listensToMoveOriginState = true;
                            SettingsManager.onMoveOrigin += OnMoveOrigin;
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
                    case ListenValues.PLAYER_TIMER:
                        GamePlayManager.OnTimerUpdate -= UpdatePlayerTimer;
                        break;
                        break;

                    case ListenValues.NO_INTERNET_ACCESS:
                        NetworkAccess.onNetworkAccess -= UpdateNoInternet;
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
                            SettingsManager.onMoveOrigin -= OnMoveOrigin;
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
                        case ListenValues.PLAYER_TIMER:
                            UpdatePlayerTimer(new TimeSpan(0, 0, 0, 0, Constants.playerMoveTimer_InitialTime).Ticks);
                            break;

                        case ListenValues.NO_INTERNET_ACCESS:
                            UpdateNoInternet(NetworkAccess.ACCESS);
                            break;

                        case ListenValues.SETTINGS_SFX_OFF:
                        case ListenValues.SETTINGS_SFX_ON:
                            OnSfx(SettingsManager.Instance.Get(SettingsManager.KEY_SFX));
                            break;

                        case ListenValues.SETTINGS_AUDIO_OFF:
                        case ListenValues.SETTINGS_AUDIO_ON:
                            OnAudio(SettingsManager.Instance.Get(SettingsManager.KEY_AUDIO));
                            break;

                        case ListenValues.SETTINGS_MOVE_ORIGIN_OFF:
                        case ListenValues.SETTINGS_MOVE_ORIGIN_ON:
                            OnMoveOrigin(SettingsManager.Instance.Get(SettingsManager.KEY_MOVE_ORIGIN));
                            break;
                    }
        }

        public void UpdatePlayerTimer(long ticks)
        {
            DateTime time = new DateTime(ticks);

            foreach (ListenTarget target in sorted[ListenValues.PLAYER_TIMER])
                target.events.Invoke(string.Format(target.targetText, time.Minute, time.Second));
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

        public void OnMoveOrigin(bool state)
        {
            if (state)
                foreach (ListenTarget target in sorted[ListenValues.SETTINGS_MOVE_ORIGIN_ON])
                    target.events.Invoke(string.Format(target.targetText, state));
            else
                foreach (ListenTarget target in sorted[ListenValues.SETTINGS_MOVE_ORIGIN_OFF])
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
        CHELLENGE_ID = 0,
        PLAYER_TIMER = 1,
        NO_INTERNET_ACCESS = 4,
        LOGGED_IN = 8,

        SETTINGS_SFX_ON = 64,
        SETTINGS_SFX_OFF = 128,
        SETTINGS_AUDIO_ON = 256,
        SETTINGS_AUDIO_OFF = 512,
        SETTINGS_MOVE_ORIGIN_ON = 1024,
        SETTINGS_MOVE_ORIGIN_OFF = 2048,

        USES_TEXT = CHELLENGE_ID | PLAYER_TIMER,
    }
}