//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Serialized;
using FourzyGameModel.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Fourzy._Updates
{
    internal class InternalSettings
    {
        internal static Action onLoaded;

        internal static InternalSettings Current
        {
            get
            {
                if (current == null)
                {
                    current = new InternalSettings();
                }

                return current;
            }
        }
        private static InternalSettings current;

        internal const string PREFIX = "InternalSettings_";

        internal int DEFAULT_PLACEMENT_STYLE_POINTER { get; private set; } =
            PlayerPrefs.GetInt(PREFIX + "DEFAULT_PLACEMENT_STYLE_POINTER", Constants.DEFAULT_PLACEMENT_STYLE_POINTER);
        internal int DEFAULT_PLACEMENT_STYLE_TOUCH { get; private set; } =
            PlayerPrefs.GetInt(PREFIX + "DEFAULT_PLACEMENT_STYLE_TOUCH", Constants.DEFAULT_PLACEMENT_STYLE_TOUCH);
        internal Area DEFAULT_AREA { get; private set; } =
            (Area)PlayerPrefs.GetInt(PREFIX + "DEFAULT_AREA", (int)Constants.DEFAULT_AREA);
        internal int DEFAULT_STANDALONE_CPU_DIFFICULTY { get; private set; } =
            PlayerPrefs.GetInt(PREFIX + "DEFAULT_STANDALONE_CPU_DIFFICULTY", Constants.DEFAULT_STANDALONE_CPU_DIFFICULTY);
        internal float EXTRA_DELAY_BETWEEN_TURNS { get; private set; } =
            PlayerPrefs.GetFloat(PREFIX + "EXTRA_DELAY_BETWEEN_TURNS", Constants.EXTRA_DELAY_BETWEEN_TURNS);
        internal float BASE_MOVE_SPEED { get; private set; } =
            PlayerPrefs.GetFloat(PREFIX + "BASE_MOVE_SPEED", Constants.BASE_MOVE_SPEED);
        internal float MOVE_SPEED_CAP { get; private set; } =
            PlayerPrefs.GetFloat(PREFIX + "MOVE_SPEED_CAP", Constants.MOVE_SPEED_CAP);
        internal int REALTIME_COUNTDOWN_SECONDS { get; private set; } =
            PlayerPrefs.GetInt(PREFIX + "REALTIME_COUNTDOWN_SECONDS", Constants.REALTIME_COUNTDOWN_SECONDS);
        internal int GAMES_BEFORE_RATING_USED { get; private set; } =
            PlayerPrefs.GetInt(PREFIX + "GAMES_BEFORE_RATING_DISPLAYED", Constants.GAMES_BEFORE_RATING_USED);
        internal float LOBBY_GAME_LOAD_DELAY { get; private set; } =
            PlayerPrefs.GetFloat(PREFIX + "LOBBY_GAME_LOAD_DELAY", Constants.LOBBY_GAME_LOAD_DELAY);
        internal Area[] UNLOCKED_AREAS { get; private set; } =
            AreasFromString(PlayerPrefs.GetString(PREFIX + "UNLOCKED_AREAS", "")) ??
            Constants.UNLOCKED_AREAS;
        internal BotGameSettings BOT_SETTINGS { get; private set; } =
            BotSettingsFromString(PlayerPrefs.GetString(PREFIX + "BOT_SETTINGS", "")) ??
            Constants.BOT_SETTINGS;

        internal int GAUNTLET_DEFAULT_MOVES_COUNT { get; private set; } =
            PlayerPrefs.GetInt(PREFIX + "GAUNTLET_DEFAULT_MOVES_COUNT", Constants.GAUNTLET_DEFAULT_MOVES_COUNT);

        internal string DEFAULT_GAME_PIECE { get; private set; } =
            PlayerPrefs.GetString(PREFIX + "DEFAULT_GAME_PIECE", Constants.DEFAULT_GAME_PIECE);

        //timer
        internal int INITIAL_TIMER_SECTIONS { get; private set; } =
            PlayerPrefs.GetInt(PREFIX + "INITIAL_TIMER_SECTIONS", Constants.INITIAL_TIMER_SECTIONS);
        internal int CIRCULAR_TIMER_SECONDS { get; private set; } =
            PlayerPrefs.GetInt(PREFIX + "CIRCULAR_TIMER_SECONDS", Constants.CIRCULAR_TIMER_SECONDS);
        internal int RESET_TIMER_SECTIONS { get; private set; } =
            PlayerPrefs.GetInt(PREFIX + "RESET_TIMER_SECTIONS", Constants.RESET_TIMER_SECTIONS);
        internal int ADD_TIMER_BAR_EVERY_X_TURN { get; private set; } =
            PlayerPrefs.GetInt(PREFIX + "ADD_TIMER_BAR_EVERY_X_TURN", Constants.ADD_TIMER_BAR_EVERY_X_TURN);
        internal int BARS_TO_ADD { get; private set; } =
            PlayerPrefs.GetInt(PREFIX + "BARS_TO_ADD", Constants.BARS_TO_ADD);
        internal bool LOSE_ON_EMPTY_TIMER { get; private set; } =
            PlayerPrefs.GetInt(PREFIX + "LOSE_ON_EMPTY_TIMER", Constants.LOSE_ON_EMPTY_TIMER ? 1 : 0) == 1;

        internal AreaProgression ENCHANTED_FOREST { get; private set; } =
            AreaProgression.FromJsonString(PlayerPrefs.GetString(PREFIX + "_" + Area.ENCHANTED_FOREST.ToString(), ""));

        internal AreaProgression TRAINING_GARDEN { get; private set; } =
            AreaProgression.FromJsonString(PlayerPrefs.GetString(PREFIX + "_" + Area.TRAINING_GARDEN.ToString(), ""));

        internal AreaProgression SANDY_ISLAND { get; private set; } =
            AreaProgression.FromJsonString(PlayerPrefs.GetString(PREFIX + "_" + Area.SANDY_ISLAND.ToString(), ""));

        internal AreaProgression ICE_PALACE { get; private set; } =
            AreaProgression.FromJsonString(PlayerPrefs.GetString(PREFIX + "_" + Area.ICE_PALACE.ToString(), ""));

        internal TokenType[] DEFAULT_TOKENS { get; private set; } = PlayerPrefsWrapper.IsDefaultTokensSet() ? 
            PlayerPrefsWrapper.GetUnlockedTokens()
                .Where(_data => _data.unlockType == TokenUnlockType.DEFAULT)
                .Select(_data => _data.tokenType)
                .ToArray() : 
            Constants.DEFAULT_UNLOCKED_TOKENS;

        internal void Update(object data, bool debugData = true)
        {
            Dictionary<string, string> _data = data as Dictionary<string, string>;
            Dictionary<string, string> newValues = new Dictionary<string, string>();
            var properties = typeof(InternalSettings)
                .GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                .ToDictionary(_prop => _prop.Name);

            if (debugData)
            {
                Debug.Log("--------------- data loaded from server debug ---------------");
            }

            int valuesUsed = _data.Count;
            //fix keys
            foreach (var kvPair in _data)
            {
                string[] keyPieces = kvPair.Key.Split(' ');
                bool equal = false;

                if (keyPieces.Length > 1)
                {
                    if (properties.ContainsKey(keyPieces[1]))
                    {
                        switch (keyPieces[0])
                        {
                            case "bot":
                                if (PlayerPrefs.GetString(
                                    PREFIX + "BOT_SETTINGS",
                                    JsonConvert.SerializeObject(BOT_SETTINGS)) != kvPair.Value)
                                {
                                    newValues.Add(keyPieces[1], kvPair.Value);
                                    BOT_SETTINGS = BotSettingsFromString(kvPair.Value);

                                    PlayerPrefs.SetString(PREFIX + keyPieces[1], kvPair.Value);
                                }

                                break;

                            case "area":
                                Area[] areas = AreasFromString(kvPair.Value);

                                equal = areas.Length == UNLOCKED_AREAS.Length;
                                if (equal)
                                {
                                    //compare contents
                                    foreach (Area area in areas)
                                    {
                                        if (!UNLOCKED_AREAS.Contains(area))
                                        {
                                            equal = false;
                                            break;
                                        }
                                    }
                                }

                                if (!equal)
                                {
                                    newValues.Add(keyPieces[1], kvPair.Value);
                                    UNLOCKED_AREAS = areas;
                                }

                                break;

                            case "areaProgression":
                                PropertyInfo propertyInfo = properties[keyPieces[1]];

                                if (propertyInfo != null)
                                {
                                    AreaProgression _progression = (AreaProgression)propertyInfo.GetValue(this);

                                    if (_progression == null || _progression.jsonString != kvPair.Value)
                                    {
                                        PlayerPrefs.SetString(PREFIX + "_" + keyPieces[1], kvPair.Value);
                                        propertyInfo.SetValue(this, AreaProgression.FromJsonString(kvPair.Value));
                                        newValues.Add(keyPieces[1], kvPair.Value);
                                    }
                                }
                                else
                                {
                                    if (Application.isEditor || Debug.isDebugBuild)
                                    {
                                        Debug.LogWarning($"Unrecognized property in settings {keyPieces[1]} found while receiving area progression from server.");
                                    }
                                }

                                break;

                            case "defaultTokens":
                                TokenType[] _tokensFromServer = TokensFromString(kvPair.Value);

                                var _unlocked = DEFAULT_TOKENS;

                                if (_tokensFromServer.Length != _unlocked.Length || _tokensFromServer.Any(_fromServer => !_unlocked.Contains(_fromServer)))
                                {
                                    newValues.Add(keyPieces[1], kvPair.Value);
                                    PlayerPrefsWrapper.SetDefaultTokens(_tokensFromServer);
                                }

                                break;

                            case "b":
                                bool boolValue = bool.Parse(kvPair.Value);

                                if ((bool)properties[keyPieces[1]].GetValue(this) != boolValue)
                                {
                                    newValues.Add(keyPieces[1], boolValue.ToString());
                                    properties[keyPieces[1]].SetValue(this, boolValue);

                                    PlayerPrefs.SetInt(PREFIX + keyPieces[1], boolValue ? 1 : 0);
                                }

                                break;

                            case "f":
                                float floatValue = float.Parse(kvPair.Value);

                                if ((float)properties[keyPieces[1]].GetValue(this) != floatValue)
                                {
                                    newValues.Add(keyPieces[1], floatValue.ToString());
                                    properties[keyPieces[1]].SetValue(this, floatValue);

                                    PlayerPrefs.SetFloat(PREFIX + keyPieces[1], floatValue);
                                }

                                break;

                            case "i":
                                int intValue = int.Parse(kvPair.Value);

                                if ((int)properties[keyPieces[1]].GetValue(this) != intValue)
                                {
                                    newValues.Add(keyPieces[1], intValue.ToString());
                                    properties[keyPieces[1]].SetValue(this, intValue);

                                    PlayerPrefs.SetInt(PREFIX + keyPieces[1], intValue);
                                }

                                break;

                            case "s":
                                if ((string)properties[keyPieces[1]].GetValue(this) != kvPair.Value)
                                {
                                    newValues.Add(keyPieces[1], kvPair.Value);
                                    properties[keyPieces[1]].SetValue(this, kvPair.Value);

                                    PlayerPrefs.SetString(PREFIX + keyPieces[1], kvPair.Value);
                                }

                                break;

                            default:
                                valuesUsed--;
                                if (debugData)
                                {
                                    Debug.Log($"Unknown type {keyPieces[0]}");
                                }

                                break;
                        }
                    }
                    else
                    {
                        if (debugData)
                        {
                            Debug.Log($"Preperties key missing {keyPieces[1]}");
                        }
                    }
                }
                else
                {
                    if (debugData)
                    {
                        Debug.Log($"Wrong data format {kvPair.Key}");
                    }
                }
            }

            if (debugData)
            {
                Debug.Log($"Total values {_data.Count}, used {valuesUsed}.");
                foreach (var kvPair in newValues)
                {
                    Debug.Log($"{kvPair.Key} new value {kvPair.Value}");
                }
                Debug.Log("-------------------------------------------------------------");
            }

            onLoaded?.Invoke();
        }

        private static Area[] AreasFromString(string value)
        {
            try
            {
                return JsonConvert.DeserializeObject<string[]>(value)
                    .Select(_area => (Area)Enum.Parse(typeof(Area), _area, true))
                    .ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static TokenType[] TokensFromString(string value)
        {
            try
            {
                return JsonConvert.DeserializeObject<string[]>(value)
                    .Select(_token => (TokenType)Enum.Parse(typeof(TokenType), _token, true))
                    .ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static BotGameSettings BotSettingsFromString(string value)
        {
            try
            {
                return JsonConvert.DeserializeObject<BotGameSettings>(value);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    [Serializable]
    public class BotGameSettings
    {
        [JsonProperty]
        internal float[] botMatchAfter;
        [JsonProperty]
        internal float[] turnDelayTime;
        [JsonProperty]
        internal float[] rematchAcceptTime;
        [JsonProperty]
        internal int[] maxRematchTimes;

        public BotGameSettings() { }

        [JsonIgnore]
        public float randomMatchAfter => UnityEngine.Random.Range(botMatchAfter[0], botMatchAfter[1]);
        [JsonIgnore]
        public float randomTurnDelay => UnityEngine.Random.Range(turnDelayTime[0], turnDelayTime[1]);
        [JsonIgnore]
        public float randomRematchAcceptTime => UnityEngine.Random.Range(rematchAcceptTime[0], rematchAcceptTime[1]);
        [JsonIgnore]
        public int randomRematchTimes => UnityEngine.Random.Range(maxRematchTimes[0], maxRematchTimes[1]);
    }

    [Serializable]
    public class BotSettings
    {
        [JsonProperty]
        internal int r;
        [JsonProperty]
        internal string d;

        public AIProfile AIProfile => (AIProfile)Enum.Parse(typeof(AIProfile), d);

        public BotSettings() { }
    }
}
