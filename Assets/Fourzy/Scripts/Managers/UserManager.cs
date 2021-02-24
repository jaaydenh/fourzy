//modded

using Fourzy._Updates;
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

        public static Action<CurrencyType> onCurrencyUpdate;
        public static Action<string> OnUpdateUserGamePieceID;
        public static Action<int> onRatingUpdate;
        public static Action<int> onWinsUpdate;
        public static Action<int> onLosesUpdate;
        public static Action<int> onDrawsUpdate;
        public static Action<int> onTotalGamesUpdate;

        public bool settingRandomName = false;

        public bool currentlyChangingName => settingRandomName;

        public string userId { get; set; }

        public string userName
        {
            get
            {
                string playerPrefsValue = PlayerPrefsWrapper.GetUserName();

                return string.IsNullOrEmpty(playerPrefsValue) ? "Player" : playerPrefsValue;
            }
        }

        public string gamePieceID
        {
            get => PlayerPrefsWrapper.GetSelectedGamePiece();

            private set
            {
                PlayerPrefsWrapper.SetSelectedGamePiece(value);
                FourzyPhotonManager.SetMyProperty(Constants.REALTIME_ROOM_GAMEPIECE_KEY, value);

                PlayFabClientAPI.UpdateAvatarUrl(
                    new UpdateAvatarUrlRequest() { ImageUrl = value, },
                    OnAvatarUrlUpdate,
                    OnPlayFabError);
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

            set
            {
                PlayerPrefsWrapper.SetHints(Mathf.Clamp(value, 0, int.MaxValue));

                onCurrencyUpdate?.Invoke(CurrencyType.HINTS);
            }
        }

        public int lastCachedRating
        {
            get => _lastCachedRating;

            set
            {
                _lastCachedRating = value;

                onRatingUpdate?.Invoke(_lastCachedRating);

                FourzyPhotonManager.SetMyProperty(Constants.REALTIME_RATING_KEY, _lastCachedRating);
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

        public bool ratingAssigned => totalPlayfabGames >= Constants.GAMES_BEFORE_RATING_DISPLAYED;

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

        public int totalPlayfabGames
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

        public Player meAsPlayer => new Player(1, userName) { PlayerString = userId, HerdId = gamePieceID };

        private int _lastCachedRating = -1;
        private int _lastCachedWins = 0;
        private int _lastCachedLoses = 0;
        private int _lastCachedDraws = 0;

        protected override void Awake()
        {
            base.Awake();

            if (InstanceExists) return;

            userId = SystemInfo.deviceUniqueIdentifier;
            GameManager.onNetworkAccess += OnNetworkAccess;
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

        public void SetDisplayName(string value, bool updatePlayFabDisplayName = true)
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.NickName = value;
            }

            PlayerPrefsWrapper.SetUserName(value);

            if (updatePlayFabDisplayName)
            {
                AnalyticsManager.Instance.LogEvent(
                    "CHANGE_NAME",
                    AnalyticsManager.AnalyticsProvider.ALL,
                    new KeyValuePair<string, object>("oldName", userName),
                    new KeyValuePair<string, object>("newName", value));

                PlayFabClientAPI.UpdateUserTitleDisplayName(
                    new UpdateUserTitleDisplayNameRequest()
                    {
                        DisplayName = value,
                    },
                    ChangeDisplayNameResult,
                    OnPlayFabError);
            }
            else
            {
                settingRandomName = false;
            }
        }

        public void UpdateSelectedGamePiece(string _gamePieceID)
        {
            Debug.Log($"-------------{_gamePieceID}");
            gamePieceID = _gamePieceID;

            OnUpdateUserGamePieceID?.Invoke(gamePieceID);

            AnalyticsManager.Instance.LogEvent(
                AnalyticsManager.AnalyticsEvents.SELECT_GAMEPIECE,
                values: new KeyValuePair<string, object>(AnalyticsManager.GAMEPIECE_SELECT_KEY, _gamePieceID));
        }

        public static void GetPlayerRating(
            Action<int> _onRatingAquired = null,
            Action onFailed = null)
        {
            if (string.IsNullOrEmpty(LoginManager.playfabID))
            {
                Debug.LogError("User account id must be set");

                return;
            }

            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "checkPlayerRating",
                FunctionParameter = new { playerID = LoginManager.playfabID, }
            }, result =>
            {
                CheckRatingResult data =
                    JsonConvert.DeserializeObject<CheckRatingResult>(result.FunctionResult.ToString());

                Instance.lastCachedRating = data.rating;
                _onRatingAquired?.Invoke(data.rating);
            }, error =>
            {
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
            if (string.IsNullOrEmpty(LoginManager.playfabID))
            {
                Debug.LogError("User account id must be set");

                return;
            }

            GetPlayerStats(LoginManager.playfabID, false, data =>
            {
                Instance.playfabWinsCount = data.wins;
                Instance.playfabLosesCount = data.loses;
                Instance.playfabDrawsCount = data.drawGames;

                Instance.lastCachedRating = data.rating;

                Debug.Log($"Stats received: wins {data.wins} loses {data.loses} draws {data.drawGames} rating {data.rating}");
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
                Debug.Log(error.ErrorMessage);
                onFailed?.Invoke();
            });
        }

        public static string CreateNewPlayerName()
        {
            string[] firstNameSyllables = { "kit", "mon", "fay", "shi", "zag", "blarg", "rash", "izen", "boop", "pop", "moop", "foop" };
            string[] lastNameSyllables = { "malo", "zak", "abo", "wonk", "zig", "wolf", "cat", "dog", "sheep", "goat" };

            //Creates a first name with 2-3 syllables
            string firstName = "";
            int numberOfSyllablesInFirstName = UnityEngine.Random.Range(1, 3);
            for (int i = 0; i < numberOfSyllablesInFirstName; i++)
            {
                firstName += firstNameSyllables[UnityEngine.Random.Range(0, firstNameSyllables.Length)];
            }

            string firstNameLetter = "";
            firstNameLetter = firstName.Substring(0, 1);
            firstName = firstName.Remove(0, 1);
            firstNameLetter = firstNameLetter.ToUpper();
            firstName = firstNameLetter + firstName;

            //Creates a last name with 1-2 syllables
            string lastName = "";
            int numberOfSyllablesInLastName = UnityEngine.Random.Range(1, 3);
            for (int j = 0; j < numberOfSyllablesInLastName; j++)
            {
                lastName += lastNameSyllables[UnityEngine.Random.Range(0, lastNameSyllables.Length)];
            }
            string lastNameLetter = "";
            lastNameLetter = lastName.Substring(0, 1);
            lastName = lastName.Remove(0, 1);
            lastNameLetter = lastNameLetter.ToUpper();
            lastName = lastNameLetter + lastName;

            //assembles the newly-created name
            return firstName + " " + lastName + Mathf.CeilToInt(UnityEngine.Random.Range(0f, 9999f)).ToString();
        }

        public static int DefaultBotRating()
        {
            return Instance.lastCachedRating + InternalSettings.Current.BOT_SETTINGS.randomRating;
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
            Debug.Log(response.ToJson());
        }

        private void OnPlayFabError(PlayFabError error)
        {
            if (settingRandomName)
            {
                SetDisplayName(CreateNewPlayerName());
            }

            onDisplayNameChangeFailed?.Invoke(error.ErrorMessage);
        }

        private void ChangeDisplayNameResult(UpdateUserTitleDisplayNameResult result)
        {
            Amplitude.Instance.setUserProperty("playerName", userName);
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
            public string displayName;
            public string fourzy;
        }
    }
}