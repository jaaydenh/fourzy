//@vadym udod

using Firebase.Analytics;
using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics;
using Fourzy._Updates.Tools;
using FourzyGameModel.Model;
using GameAnalyticsSDK;
//using mixpanel;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy
{
    public class AnalyticsManager : RoutinesBase
    {
        public const string GAME_ID_KEY = "game_id";
        public const string ENTRY_POINT_KEY = "entry_point";
        public const string GAME_TYPE_KEY = "game_type";
        public const string AREA_KEY = "area";
        public const string USER_ID_KEY = "user_id";
        public const string OPPONENT_ID_KEY = "opponent_id";
        public const string PACK_ID_KEY = "pack_id";
        public const string SETTINGS_VALUE_NAME_KEY = "value_name";
        public const string SETTINGS_NEW_VALUE_KEY = "new_value";
        public const string SETTINGS_OLD_VALUE_KEY = "old_value";
        public const string HINT_USED_KEY = "hint_used";
        public const string TUTORIAL_NAME_KEY = "name";
        public const string TUTORIAL_STAGE_KEY = "key";

        public static AnalyticsManager Instance
        {
            get
            {
                if (instance == null)
                    Initialize();

                return instance;
            }
        }

        public static bool DEBUG = false;

        /// <summary>
        /// To check if event is enabled
        /// </summary>
        public static Dictionary<AnalyticsGameEvents, bool> gameEventsSwitch = new Dictionary<AnalyticsGameEvents, bool>()
        {
            [AnalyticsGameEvents.GAME_OPEN] = true,

            [AnalyticsGameEvents.GAME_CREATE] = true,

            [AnalyticsGameEvents.GAME_FINISHED] = true,

            [AnalyticsGameEvents.GAME_FAILED] = true,

            [AnalyticsGameEvents.TAKE_TURN] = true,

            [AnalyticsGameEvents.USE_HINT] = true,
        };

        public static Dictionary<AnalyticsEvents, bool> miscEventsSwitch = new Dictionary<AnalyticsEvents, bool>()
        {
            [AnalyticsEvents.SETTINGS_CHANGE] = true,

            [AnalyticsEvents.UI] = true,

            [AnalyticsEvents.TUTORIALS] = true,

            [AnalyticsEvents.ERROR] = true,
        };

        public static AnalyticsUIButtons lastLoggedButton;

        private static AnalyticsUIButtons lastLoggedButtonConsumable;

        private static AnalyticsManager instance;

        public enum AnalyticsProvider
        {
            FIREBASE = 1,
            MIX_PANEL = 2,
            GAME_ANALYTICS = 4,

            ALL = FIREBASE | MIX_PANEL | GAME_ANALYTICS,
        }

        public enum AnalyticsGameEvents
        {
            /// <summary>
            /// Params: game_id, entry_point, game_type, area, [user_id], [opponent_id], [pack_id]
            /// </summary>
            GAME_OPEN = 0,

            /// <summary>
            /// Params: entry_point, game_type, area, [user_id], [opponent_id]
            /// </summary>
            GAME_CREATE = 1,

            /// <summary>
            /// Params: game_id, game_type, area, [user_id], [opponent_id], [pack_id]
            /// </summary>
            GAME_FINISHED = 2,

            /// <summary>
            /// Params: game_id, game_type, area, [user_id], [opponent_id], [pack_id]
            /// </summary>
            GAME_FAILED = 3,

            /// <summary>
            /// Turn-base only game event
            /// Params: game_id
            /// </summary>
            TAKE_TURN = 10,

            USE_HINT = 11,
        }

        public enum AnalyticsEvents
        {

            /// <summary>
            /// Params: value_name, new_value, old_value
            /// </summary>
            SETTINGS_CHANGE = 0,

            /// <summary>
            /// Params: name, stage
            /// </summary>
            TUTORIALS = 1,

            UI = 2,

            ERROR = 20,
        }

        public enum AnalyticsUIButtons
        {
            none,

            create_game,
            pass_and_play,
            leaderboard_play,
            turn_play,
            puzzle_play,
            change_name,
        }

        public enum AnalyticsSettingsKeys
        {
            change_name,

        }

        public enum AnalyticsErrorType
        {
            puzzle_play,
            pass_play,
            turn_based,
            realtime,
            onboarding,
            main_menu,
            gameplay_scene,
            settings,
            challenge_manager,
            create_turn_base_game,
            create_realtime_game,

            unidentified,
        }

        protected override void Awake()
        {
            base.Awake();

            GameAnalytics.Initialize();
        }

        public static void Initialize(bool _DEBUG = false)
        {
            if (instance != null) return;

            GameObject go = new GameObject("AnalyticsManager");
            go.transform.SetParent(null);
            instance = go.AddComponent<AnalyticsManager>();

            DEBUG = _DEBUG;

            DontDestroyOnLoad(go);
        }

        public void Identify(string userID)
        {
            //Mixpanel.Identify(userID);
            FirebaseAnalytics.SetUserId(userID);

            if (DEBUG) Debug.Log("User identity: " + userID);
        }

        public void LogCreateGame(
            GameType gameType, Area area,
            string userID = "",
            string opponentID = "",
            AnalyticsProvider provider = AnalyticsProvider.ALL)
        {
            if (!NetworkPass()) return;
            if (!gameEventsSwitch[AnalyticsGameEvents.GAME_CREATE]) return;

            Dictionary<object, object> @params = new Dictionary<object, object>();

            @params.Add(GAME_TYPE_KEY, gameType);
            @params.Add(AREA_KEY, area.ToString());

            if (lastLoggedButtonConsumable != AnalyticsUIButtons.none) @params.Add(ENTRY_POINT_KEY, lastLoggedButtonConsumable);

            if (!string.IsNullOrEmpty(userID)) @params.Add(USER_ID_KEY, userID);
            if (!string.IsNullOrEmpty(opponentID)) @params.Add(OPPONENT_ID_KEY, opponentID);

            foreach (Enum value in Enum.GetValues(provider.GetType()))
            {
                if (provider.HasFlag(value))
                {
                    switch (value)
                    {
                        case AnalyticsProvider.FIREBASE:
                            LogFirebaseEvent(AnalyticsGameEvents.GAME_CREATE.ToString(), @params);

                            break;

                        case AnalyticsProvider.MIX_PANEL:
                            LogMixpanelEvent(AnalyticsGameEvents.GAME_CREATE.ToString(), @params);

                            break;

                        case AnalyticsProvider.GAME_ANALYTICS:
                            string values = $"{AnalyticsGameEvents.GAME_CREATE.ToString()}:{area.ToString()}";

                            if (lastLoggedButtonConsumable != AnalyticsUIButtons.none) values += $":{lastLoggedButtonConsumable.ToString()}";
                            if (!string.IsNullOrEmpty(userID)) values += $":{userID}";
                            if (!string.IsNullOrEmpty(opponentID)) values += $":{opponentID}";

                            LogGameAnalyticsDesignEvent(values);

                            break;
                    }
                }
            }

            lastLoggedButton = AnalyticsUIButtons.none;
        }

        public void LogGameEvent(
            AnalyticsGameEvents gameEventType,
            IClientFourzy gameModel,
            AnalyticsProvider provider = AnalyticsProvider.ALL)
        {
            if (!NetworkPass()) return;
            if (!gameEventsSwitch[gameEventType]) return;

            Dictionary<object, object> params1 = new Dictionary<object, object>();

            //basic values
            params1.Add(GAME_ID_KEY, gameModel.GameID);
            params1.Add(GAME_TYPE_KEY, gameModel._Type);
            params1.Add(AREA_KEY, gameModel._Area);
            params1.Add(USER_ID_KEY, gameModel.me.PlayerString);

            if (lastLoggedButtonConsumable != AnalyticsUIButtons.none)
            {
                params1.Add(ENTRY_POINT_KEY, lastLoggedButtonConsumable.ToString());
            }

            if (!string.IsNullOrEmpty(gameModel.opponent.PlayerString))
            {
                params1.Add(OPPONENT_ID_KEY, gameModel.opponent.PlayerString);
            }

            if (gameModel.puzzleData && gameModel.puzzleData.pack)
            {
                params1.Add(PACK_ID_KEY, gameModel.puzzleData.pack.packID);
            }

            foreach (Enum value in Enum.GetValues(provider.GetType()))
            {
                if (provider.HasFlag(value))
                {
                    switch (value)
                    {
                        case AnalyticsProvider.FIREBASE:
                            LogFirebaseEvent(gameEventType.ToString(), params1);

                            break;

                        case AnalyticsProvider.MIX_PANEL:
                            LogMixpanelEvent(gameEventType.ToString(), params1);

                            break;

                        case AnalyticsProvider.GAME_ANALYTICS:
                            string values = $"{gameEventType.ToString().ToLower()}";

                            //if (lastLoggedButtonConsumable != AnalyticsUIButtons.none) values += $":{lastLoggedButtonConsumable.ToString()}";

                            values += $":{gameModel._Type.ToString().ToLower()}";

                            values += $":{gameModel._Area.ToString().ToLower()}";

                            if (gameModel.puzzleData && gameModel.puzzleData.pack)
                            {
                                values += $":pack_id-{gameModel.puzzleData.pack.packID.Truncate(7)}";
                            }

                            LogGameAnalyticsDesignEvent(values);

                            break;
                    }
                }
            }

            lastLoggedButton = AnalyticsUIButtons.none;
        }

        public void LogSettingsChange(AnalyticsSettingsKeys settingsKey, string newValue, string oldValue, AnalyticsProvider provider = AnalyticsProvider.ALL)
        {
            if (!NetworkPass()) return;
            if (!miscEventsSwitch[AnalyticsEvents.SETTINGS_CHANGE]) return;

            Dictionary<object, object> @params = new Dictionary<object, object>();

            @params.Add(SETTINGS_VALUE_NAME_KEY, settingsKey);
            @params.Add(SETTINGS_NEW_VALUE_KEY, newValue);
            @params.Add(SETTINGS_OLD_VALUE_KEY, oldValue);

            foreach (Enum value in Enum.GetValues(provider.GetType()))
            {
                if (provider.HasFlag(value))
                {
                    switch (value)
                    {
                        case AnalyticsProvider.FIREBASE:
                            LogFirebaseEvent(AnalyticsEvents.SETTINGS_CHANGE.ToString(), @params);

                            break;

                        case AnalyticsProvider.MIX_PANEL:
                            LogMixpanelEvent(AnalyticsEvents.SETTINGS_CHANGE.ToString(), @params);

                            break;

                        case AnalyticsProvider.GAME_ANALYTICS:
                            string values = $"{settingsKey.ToString()}:{oldValue}:{newValue}";

                            LogGameAnalyticsDesignEvent(values);

                            break;
                    }
                }
            }
        }

        public void LogError(string errorString, AnalyticsErrorType analyticsError = AnalyticsErrorType.unidentified, AnalyticsProvider provider = AnalyticsProvider.ALL)
        {
            if (!NetworkPass()) return;
            if (!miscEventsSwitch[AnalyticsEvents.ERROR]) return;

            Dictionary<object, object> @params = new Dictionary<object, object>();

            @params.Add(analyticsError, errorString);

            foreach (Enum value in Enum.GetValues(provider.GetType()))
            {
                if (provider.HasFlag(value))
                {
                    switch (value)
                    {
                        case AnalyticsProvider.FIREBASE:
                            LogFirebaseEvent(AnalyticsEvents.ERROR.ToString(), @params);

                            break;

                        case AnalyticsProvider.MIX_PANEL:
                            LogMixpanelEvent(AnalyticsEvents.SETTINGS_CHANGE.ToString(), @params);

                            break;

                        case AnalyticsProvider.GAME_ANALYTICS:
                            string values = $"{analyticsError.ToString()}:{errorString.Truncate(15)}";

                            GAErrorSeverity severity = GAErrorSeverity.Undefined;

                            switch (analyticsError)
                            {
                                case AnalyticsErrorType.realtime:
                                case AnalyticsErrorType.turn_based:
                                case AnalyticsErrorType.puzzle_play:
                                case AnalyticsErrorType.pass_play:
                                case AnalyticsErrorType.challenge_manager:
                                case AnalyticsErrorType.create_realtime_game:
                                case AnalyticsErrorType.create_turn_base_game:
                                    severity = GAErrorSeverity.Critical;

                                    break;

                                case AnalyticsErrorType.gameplay_scene:
                                case AnalyticsErrorType.main_menu:
                                    severity = GAErrorSeverity.Error;

                                    break;

                                case AnalyticsErrorType.onboarding:
                                case AnalyticsErrorType.settings:
                                    severity = GAErrorSeverity.Warning;

                                    break;
                            }

                            LogGameAnalyticsErrorEvent(severity, errorString);

                            break;
                    }
                }
            }
        }

        public void LogTutorialEvent(string name, string stage, AnalyticsProvider provider = AnalyticsProvider.ALL)
        {
            if (!NetworkPass()) return;
            if (!miscEventsSwitch[AnalyticsEvents.TUTORIALS]) return;

            Dictionary<object, object> _params = new Dictionary<object, object>();

            _params.Add(TUTORIAL_NAME_KEY, name);
            _params.Add(TUTORIAL_STAGE_KEY, stage);

            foreach (Enum value in Enum.GetValues(provider.GetType()))
                if (provider.HasFlag(value))
                {
                    switch (value)
                    {
                        case AnalyticsProvider.FIREBASE:
                            LogFirebaseEvent(AnalyticsEvents.TUTORIALS.ToString(), _params);

                            break;

                        case AnalyticsProvider.MIX_PANEL:
                            LogMixpanelEvent(AnalyticsEvents.TUTORIALS.ToString(), _params);

                            break;

                        case AnalyticsProvider.GAME_ANALYTICS:
                            string values = $"{AnalyticsEvents.TUTORIALS.ToString()}";

                            foreach (KeyValuePair<object, object> param in _params)
                            {
                                values += $":{param.Value}";
                            }

                            LogGameAnalyticsDesignEvent(values);

                            break;
                    }
                }
        }

        public void LogUIButton(AnalyticsUIButtons buttonEvent, AnalyticsProvider provider = AnalyticsProvider.ALL, params KeyValuePair<object, object>[] @params)
        {
            if (!NetworkPass()) return;
            if (!miscEventsSwitch[AnalyticsEvents.UI]) return;

            lastLoggedButton = lastLoggedButtonConsumable = buttonEvent;

            Dictionary<object, object> _params = new Dictionary<object, object>();

            foreach (KeyValuePair<object, object> param in @params)
            {
                _params.Add(param.Key, param.Value);
            }

            foreach (Enum value in Enum.GetValues(provider.GetType()))
                if (provider.HasFlag(value))
                {
                    switch (value)
                    {
                        case AnalyticsProvider.FIREBASE:
                            LogFirebaseEvent(AnalyticsEvents.UI.ToString() + ":" + buttonEvent.ToString(), _params);

                            break;

                        case AnalyticsProvider.MIX_PANEL:
                            LogMixpanelEvent(AnalyticsEvents.UI.ToString() + ":" + buttonEvent.ToString(), _params);

                            break;

                        case AnalyticsProvider.GAME_ANALYTICS:
                            string values = $"{AnalyticsEvents.UI.ToString()}:{buttonEvent.ToString()}";

                            foreach (KeyValuePair<object, object> param in @params)
                            {
                                values += $":{param.Value}";
                            }

                            LogGameAnalyticsDesignEvent(values);

                            break;
                    }
                }
        }

        private void LogFirebaseEvent(string eventType, Dictionary<object, object> @params)
        {
            List<Parameter> firebaseParams = new List<Parameter>();

            if (@params != null)
            {
                foreach (KeyValuePair<object, object> _param in @params)
                {
                    if (_param.Key != null && _param.Value != null)
                    {
                        firebaseParams.Add(new Parameter(_param.Key.ToString(), _param.Value.ToString()));
                    }
                }
            }

            FirebaseAnalytics.LogEvent(eventType, firebaseParams.ToArray());

            if (DEBUG)
            {
                Debug.Log("Firebase event sent");
            }
        }

        private void LogMixpanelEvent(string eventType, Dictionary<object, object> @params)
        {
            //Value mixpanelParams = new Value();

            //if (@params != null)
            //{
            //    foreach (KeyValuePair<object, object> _param in @params)
            //    {
            //        if (_param.Key != null && _param.Value != null)
            //        {
            //            mixpanelParams[_param.Key.ToString()] = _param.Value.ToString();
            //        }

            //    }
            //}

            //Mixpanel.Track(eventType, mixpanelParams);

            //if (DEBUG)
            //{
            //    Debug.Log("Mixpanel event sent");
            //}
        }

        private void LogGameAnalyticsDesignEvent(string value)
        {
            GameAnalytics.NewDesignEvent(value);

            if (DEBUG)
            {
                Debug.Log("Game Analytics event sent: " + value);
            }
        }

        private void LogGameAnalyticsErrorEvent(GAErrorSeverity severity, string value)
        {
            GameAnalytics.NewErrorEvent(severity, value);

            if (DEBUG)
            {
                Debug.Log("Game Analytics Error event sent: " + value);
            }
        }

        private bool NetworkPass()
        {
            return NetworkAccess.HAVE_ACCESS;
        }
    }
}
