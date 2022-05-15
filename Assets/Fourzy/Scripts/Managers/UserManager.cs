//modded

using Fourzy._Updates;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Toasts;
using FourzyGameModel.Model;
using Newtonsoft.Json;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy
{
    [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene)]
    public class UserManager : UnitySingleton<UserManager>
    {
        public static Action onDisplayNameChanged;
        public static Action<string> onDisplayNameChangeFailed;
        public static Action<IEnumerable<TokenType>, TokenUnlockType> onTokenUnlocked;
        public static Action<Area, int> onAreaProgression;

        public static Action<PlayfabValuesLoaded> onPlayfabValuesLoaded;

        public static Action<CurrencyType> onCurrencyUpdate;
        public static Action<int, string> onHintsUpdate;
        public static Action<string> OnUpdateUserGamePieceID;
        public static Action<int> onRatingUpdate;
        public static Action<int> onWinsUpdate;
        public static Action<int> onLosesUpdate;
        public static Action<int> onDrawsUpdate;
        public static Action<int> onRealtimeGamesCompleteUdpate;
        public static Action<int> onTotalGamesUpdate;

        public bool settingRandomName = false;

        public bool currentlyChangingName => settingRandomName;

        public string userId
        {
            get
            {
                string id = LoginManager.playfabId;
                if (string.IsNullOrEmpty(id))
                {
                    id = PlayerPrefs.GetString("mapReferenceSeed", "");
                    if (string.IsNullOrEmpty(id))
                    {
                        id = SystemInfo.deviceUniqueIdentifier;
                        PlayerPrefs.SetString("mapReferenceSeed", id);
                    }
                }

                return id;
            }
        }

        public string userName
        {
            get
            {
                string playerPrefsValue = PlayerPrefsWrapper.GetUserName();

                return string.IsNullOrEmpty(playerPrefsValue) ? "Player" : playerPrefsValue;
            }
        }

        public string gamePieceId
        {
            get => PlayerPrefsWrapper.GetSelectedGamePiece();

            private set
            {
                PlayerPrefsWrapper.SetSelectedGamePiece(value);
                FourzyPhotonManager.SetMyProperty(Constants.REALTIME_ROOM_GAMEPIECE_KEY, value);

                PlayFabClientAPI.UpdateAvatarUrl(new UpdateAvatarUrlRequest() { ImageUrl = value, }, OnAvatarUrlUpdate, OnChangeNamePlayfabError);
                Amplitude.Instance.setUserProperty("gamePieceId", value);
            }
        }

        public int coins
        {
            get => PlayerPrefsWrapper.GetCoins();

            set
            {
                PlayerPrefsWrapper.SetCoins(Mathf.Clamp(value, 0, int.MaxValue));

                onCurrencyUpdate?.Invoke(CurrencyType.COINS);
            }
        }

        public int gems
        {
            get => PlayerPrefsWrapper.GetGems();

            set
            {
                PlayerPrefsWrapper.SetGems(Mathf.Clamp(value, 0, int.MaxValue));

                onCurrencyUpdate?.Invoke(CurrencyType.GEMS);
            }
        }

        public int xp
        {
            get => PlayerPrefsWrapper.GetXP();

            set
            {
                PlayerPrefsWrapper.SetXP(Mathf.Clamp(value, 0, int.MaxValue));

                onCurrencyUpdate?.Invoke(CurrencyType.XP);
            }
        }

        public int portalPoints
        {
            get => PlayerPrefsWrapper.GetPortalPoints();

            set
            {
                PlayerPrefsWrapper.SetPortalPoints(Mathf.Clamp(value, 0, int.MaxValue));

                onCurrencyUpdate?.Invoke(CurrencyType.PORTAL_POINTS);
            }
        }

        public int rarePortalPoints
        {
            get => PlayerPrefsWrapper.GetRarePortalPoints();

            set
            {
                PlayerPrefsWrapper.SetRarePortalPoints(Mathf.Clamp(value, 0, int.MaxValue));

                onCurrencyUpdate?.Invoke(CurrencyType.RARE_PORTAL_POINTS);
            }
        }

        public int tickets
        {
            get => PlayerPrefsWrapper.GetTickets();

            set
            {
                PlayerPrefsWrapper.SetTickets(Mathf.Clamp(value, 0, int.MaxValue));

                onCurrencyUpdate?.Invoke(CurrencyType.TICKETS);
            }
        }

        public int hints
        {
            get => PlayerPrefsWrapper.GetHints();
        }

        public int lastCachedRating
        {
            get => _lastCachedRating;

            set
            {
                _lastCachedRating = value;

                onRatingUpdate?.Invoke(value);

                FourzyPhotonManager.SetMyProperty(Constants.REALTIME_RATING_KEY, value);
                Amplitude.Instance.setUserProperty("realtimeRating", value);
            }
        }

        public string lastCachedRatingFiltered
        {
            get
            {
                if (ratingAssigned)
                {
                    return lastCachedRating.ToString();
                }
                else
                {
                    return LocalizationManager.Value("apprentice");
                }
            }
        }

        /// <summary>
        /// In cents
        /// </summary>
        public uint totalSpentUSD { get; set; }

        public bool ratingAssigned => totalRatedGames >= InternalSettings.Current.GAMES_BEFORE_RATING_USED;

        public int playfabWinsCount
        {
            get => _lastCachedWins;

            set
            {
                _lastCachedWins = value;

                onWinsUpdate?.Invoke(_lastCachedWins);

                FourzyPhotonManager.SetMyProperty(Constants.REALTIME_WINS_KEY, value);
            }
        }

        public int playfabLosesCount
        {
            get => _lastCachedLoses;

            set
            {
                _lastCachedLoses = value;

                onLosesUpdate?.Invoke(_lastCachedLoses);

                FourzyPhotonManager.SetMyProperty(Constants.REALTIME_LOSES_KEY, value);
            }
        }

        public int playfabDrawsCount
        {
            get => _lastCachedDraws;

            set
            {
                _lastCachedDraws = value;

                onDrawsUpdate?.Invoke(_lastCachedDraws);

                FourzyPhotonManager.SetMyProperty(Constants.REALTIME_DRAWS_KEY, value);
            }
        }

        public int realtimeGamesComplete
        {
            get => _lastCachedRealtimeGamesComplete;

            set
            {
                Amplitude.Instance.setUserProperty("totalRealtimeGamesCompleted", value);
                _lastCachedRealtimeGamesComplete = value;

                onRealtimeGamesCompleteUdpate?.Invoke(_lastCachedRealtimeGamesComplete);
            }
        }

        public int totalRatedGames
        {
            get
            {
                int _result = playfabWinsCount + playfabLosesCount + playfabDrawsCount;

                onTotalGamesUpdate?.Invoke(_result);

                return _result;
            }
        }

        public int level => GetLevel(xp);

        public int xpLeft => GetLevelXPLeft(xp);

        public float levelProgress => GetProgression(xp);

        public Player meAsPlayer => new Player(1, userName) { PlayerString = userId, HerdId = gamePieceId };

        private int _lastCachedRating = -1;
        private int _lastCachedWins = 0;
        private int _lastCachedLoses = 0;
        private int _lastCachedDraws = 0;
        private int _lastCachedRealtimeGamesComplete = 0;
        private PlayfabValuesLoaded _playfabValueLoaded;
        private Dictionary<Area, int> _areaProgression = new Dictionary<Area, int>()
        {
            [Area.TRAINING_GARDEN] = 0,
            [Area.ENCHANTED_FOREST] = 0,
            [Area.SANDY_ISLAND] = 0,
            [Area.ICE_PALACE] = 0
        };

        protected override void Awake()
        {
            base.Awake();

            if (InstanceExists) return;

            GameManager.onNetworkAccess += OnNetworkAccess;
        }

        public void UnlockToken(TokenType tokenType, TokenUnlockType unlockType)
        {
            PlayerPrefsWrapper.AddUnlockedToken(tokenType, unlockType);

            onTokenUnlocked?.Invoke(
                new TokenType[] { tokenType },
                unlockType);
        }

        public int GetAreaProgression(Area area)
        {
            return _areaProgression[area];
        }

        public void SetAreaProgression(Area area, int value)
        {
            _areaProgression[area] = value;
            onAreaProgression?.Invoke(area, value);
        }

        public void AddPlayfabValueLoaded(PlayfabValuesLoaded playfabValue)
        {
            _playfabValueLoaded |= playfabValue;

            onPlayfabValuesLoaded?.Invoke(playfabValue);
        }

        public bool IsPlayfabValueLoaded(params PlayfabValuesLoaded[] values)
        {
            foreach (PlayfabValuesLoaded value in values)
            {
                if (!_playfabValueLoaded.HasFlag(value))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// XP fot givel level
        /// </summary>
        /// <param name="_level"></param>
        /// <returns></returns>
        public int GetLevelXP(int _level) => _level * 100;

        /// <summary>
        /// Returns XP amount required to get to specified level
        /// </summary>
        /// <param name="_level"></param>
        /// <returns></returns>
        public int GetTotalLevelXP(int _level)
        {
            int sum = 0;
            for (int __level = Constants.MIN_PLAYER_LEVEL; __level <= _level; __level++)
            {
                sum += GetLevelXP(__level);
            }

            return sum;
        }

        /// <summary>
        /// How much XP of current left compared to level from this XP
        /// Exmaple: XP = 150, so its level 2, XP left is (150 - 100 = 50)
        /// </summary>
        /// <param name="xp"></param>
        /// <returns></returns>
        public int GetLevelXPLeft(int xp)
        {
            if (xp < GetLevelXP(Constants.MIN_PLAYER_LEVEL))
            {
                return xp;
            }
            else
            {
                return xp - GetTotalLevelXP(GetLevel(xp) - 1);
            }
        }

        /// <summary>
        /// Get level from XP
        /// </summary>
        /// <param name="xp"></param>
        /// <returns></returns>
        public int GetLevel(int xp)
        {
            int _level = Constants.MIN_PLAYER_LEVEL;

            while (xp >= GetLevelXP(_level) && _level <= Constants.MAX_PLAYER_LEVEL)
            {
                xp -= GetLevelXP(_level);
                _level++;
            }

            return _level;
        }

        /// <summary>
        /// Get level progression using given XP
        /// </summary>
        /// <param name="xp"></param>
        /// <returns></returns>
        public float GetProgression(int xp) => GetLevelXPLeft(xp) / (float)GetLevelXP(GetLevel(xp));

        public float GetProgressionDifference(int lower, int upper)
        {
            if (upper < lower)
            {
                return 0f;
            }

            float result = 0f;

            int fromLevel = GetLevel(lower);
            int _levelDifference = GetLevel(upper) - fromLevel;

            if (_levelDifference == 0)
            {
                return GetProgression(upper) - GetProgression(lower);
            }
            else
            {
                for (int _level = fromLevel; _level <= fromLevel + _levelDifference; _level++)
                {
                    if (_level == fromLevel)
                    {
                        result += 1f - GetProgression(lower);
                    }
                    else if (_level == fromLevel + _levelDifference)
                    {
                        result += GetProgression(upper);
                    }
                    else
                    {
                        result++;
                    }
                }

                return result;
            }
        }

        public int MyComplexityPercent() => GetComplexityPercent(totalRatedGames, lastCachedRating);

        public static int GetComplexityPercent(int games, int rating)
        {
            return (int)UnityEngine.Random.Range(
                Mathf.Min(20 + games / 4, 50),
                50 + (rating / 2000f) * 50);
        }

        public static void AddHints(int number, string ticket = "")
        {

            switch (GameManager.Instance.buildIntent)
            {
                case BuildIntent.DESKTOP_REGULAR:
                case BuildIntent.MOBILE_REGULAR:
                    if (number > 0)
                    {
                        PlayFabClientAPI.AddUserVirtualCurrency(
                            new AddUserVirtualCurrencyRequest()
                            {
                                VirtualCurrency = Constants.HINTS_CURRENCY_KEY,
                                Amount = number
                            },
                            OnHintsAdded,
                            ModifyCurrencyError,
                            ticket);
                    }
                    else if (number < 0)
                    {
                        PlayFabClientAPI.SubtractUserVirtualCurrency(
                            new SubtractUserVirtualCurrencyRequest()
                            {
                                VirtualCurrency = Constants.HINTS_CURRENCY_KEY,
                                Amount = -number,
                            },
                            OnHintsAdded,
                            ModifyCurrencyError,
                            ticket);
                    }
                    break;

                case BuildIntent.MOBILE_INFINITY:
                case BuildIntent.MOBILE_SKILLZ:
                    OnHintsValueUpdated(PlayerPrefsWrapper.GetHints() + number, ticket);

                    break;
            }
        }

        public static void OnHintsValueUpdated(int value, string token = "")
        {
            PlayerPrefsWrapper.SetHints(value);
            onHintsUpdate?.Invoke(value, token);
        }

        public void SetDisplayName(string value, bool updatePlayFabDisplayName = true)
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.NickName = value;
            }

            if (updatePlayFabDisplayName)
            {
                AnalyticsManager.Instance.LogEvent(
                    "playerNameChanged",
                    AnalyticsManager.AnalyticsProvider.ALL,
                    new KeyValuePair<string, object>("oldName", userName),
                    new KeyValuePair<string, object>("newName", value));

                PlayFabClientAPI.UpdateUserTitleDisplayName(
                    new UpdateUserTitleDisplayNameRequest()
                    {
                        DisplayName = value,
                    },
                    ChangeDisplayNameResult,
                    OnChangeNamePlayfabError);
            }
            else
            {
                settingRandomName = false;
            }

            PlayerPrefsWrapper.SetUserName(value);
        }

        public void UpdateSelectedGamePiece(string _gamePieceID)
        {
            gamePieceId = _gamePieceID;

            OnUpdateUserGamePieceID?.Invoke(gamePieceId);

            AnalyticsManager.Instance.LogEvent(
                AnalyticsManager.AnalyticsEvents.selectGamepiece,
                AnalyticsManager.AnalyticsProvider.ALL,
                new KeyValuePair<string, object>(AnalyticsManager.GAMEPIECE_SELECT_KEY, _gamePieceID),
                new KeyValuePair<string, object>(
                    AnalyticsManager.GAMEPIECE_NAME_KEY,
                    GameContentManager.Instance.piecesDataHolder.GetGamePieceData(_gamePieceID).name));
        }

        public static void GetPlayerRating(
            Action<int> _onRatingAquired = null,
            Action onFailed = null)
        {
            if (string.IsNullOrEmpty(LoginManager.playfabId))
            {
                Debug.LogError("User account id must be set");

                return;
            }

            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "checkPlayerRating",
                FunctionParameter = new { playerID = LoginManager.playfabId, }
            }, result =>
            {
                CheckRatingResult data =
                    JsonConvert.DeserializeObject<CheckRatingResult>(result.FunctionResult.ToString());

                Instance.lastCachedRating = data.rating;
                _onRatingAquired?.Invoke(data.rating);
            }, error =>
            {
                GameManager.Instance.ReportPlayFabError(error.ErrorMessage);
                Debug.Log(error.ErrorMessage);
                GamesToastsController.ShowTopToast(error.ErrorMessage);

                onFailed?.Invoke();
            });
        }

        /// <summary>
        /// Only called after user logged in into playfab
        /// </summary>
        public static void GetMyStats(Action onFailed = null)
        {
            if (string.IsNullOrEmpty(LoginManager.playfabId))
            {
                Debug.LogError("User account id must be set");

                return;
            }

            GetPlayerStats(LoginManager.playfabId, false, data =>
            {
                Instance.playfabWinsCount = data.wins;
                Instance.playfabLosesCount = data.loses;
                Instance.playfabDrawsCount = data.drawGames;
                Instance.realtimeGamesComplete = data.realtimeGamesComplete;

                Instance.SetAreaProgression(Area.TRAINING_GARDEN, data.totalRealtimeGamesCompleteTrainingGarden);
                Instance.SetAreaProgression(Area.ENCHANTED_FOREST, data.totalRealtimeGamesCompleteEnchantedForest);
                Instance.SetAreaProgression(Area.SANDY_ISLAND, data.totalRealtimeGamesCompleteSandyIsland);
                Instance.SetAreaProgression(Area.ICE_PALACE, data.totalRealtimeGamesCompleteIcePalace);

                Instance.lastCachedRating = data.rating;

                Debug.Log($"Stats received: wins {data.wins} loses {data.loses} draws {data.drawGames} rating {data.rating}");

                Instance.AddPlayfabValueLoaded(PlayfabValuesLoaded.PLAYER_STATS_RECEIVED);
            }, onFailed);
        }

        public static void GetPlayerStats(
            string playerID,
            bool full,
            Action<CheckPlayerStatsResult> onSuccess,
            Action onFailed)
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "userStats",
                FunctionParameter = new
                {
                    playerID,
                    full,
                }
            }, result =>
            {
                onSuccess?.Invoke(
                    JsonConvert.DeserializeObject<CheckPlayerStatsResult>(result.FunctionResult.ToString()));
            }, error =>
            {
                GameManager.Instance.ReportPlayFabError(error.ErrorMessage);
                Debug.Log(error.ErrorMessage);
                onFailed?.Invoke();
            });
        }

        private void OnNetworkAccess(bool networkAccess)
        {
            if (networkAccess)
            {
                // if (GS.Authenticated)
                //     GetUserGamePiece();
            }
        }

        private void OnAvatarUrlUpdate(EmptyResponse response)
        {
        }

        private void OnChangeNamePlayfabError(PlayFabError error)
        {
            if (settingRandomName)
            {
                SetDisplayName(CharacterNameFactory.GeneratePlayerName());
            }

            onDisplayNameChangeFailed?.Invoke(error.ErrorMessage);
            GameManager.Instance.ReportPlayFabError(error.ErrorMessage);
        }

        private static void OnHintsAdded(ModifyUserVirtualCurrencyResult result)
        {
            switch (result.VirtualCurrency)
            {
                case Constants.HINTS_CURRENCY_KEY:
                    OnHintsValueUpdated(result.Balance, result.CustomData.ToString());

                    break;
            }
        }

        private static void ModifyCurrencyError(PlayFabError error)
        {
            if (Application.isEditor || Debug.isDebugBuild)
            {
                Debug.LogError(error.ErrorMessage);
            }

            GameManager.Instance.ReportPlayFabError(error.ErrorMessage);
        }

        private void ChangeDisplayNameResult(UpdateUserTitleDisplayNameResult result)
        {
            Amplitude.Instance.setUserProperty("playerName", result.DisplayName);
            settingRandomName = false;

            onDisplayNameChanged?.Invoke();
        }

        [System.Serializable]
        public struct CheckRatingResult
        {
            public int rating;
            public bool justAdded;
        }

        [System.Serializable]
        public struct CheckPlayerStatsResult
        {
            public string id;
            public int rating;
            public int wins;
            public int loses;
            public int drawGames;
            public int realtimeGamesComplete;
            public int totalRealtimeGamesCompleteTrainingGarden;
            public int totalRealtimeGamesCompleteEnchantedForest;
            public int totalRealtimeGamesCompleteSandyIsland;
            public int totalRealtimeGamesCompleteIcePalace;
            public string displayName;
            public string fourzy;
        }
    }

    [Flags]
    public enum PlayfabValuesLoaded
    {
        NONE = 0,
        LOGGED_IN = 1 << 1,
        ACCOUNT_INFO_RECEIVED = 1 << 2,
        PLAYER_STATS_RECEIVED = 1 << 3,
        TITLE_DATA_RECEIVED = 1 << 4,
        PLAYER_PROFILE_RECEIVED = 1 << 5,
        LANGUAGE_CHECKED = 1 << 6,
        USER_INVENTORY_RECEIVED = 1 << 7,
        CATALOG_INFO_RECEIVED = 1 << 8,
        NEWS_CHECKED = 1 << 9,
    }
}