//modded

using Fourzy._Updates.Mechanics;
using FourzyGameModel.Model;
using GameSparks.Api.Requests;
using GameSparks.Core;
using mixpanel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Fourzy._Updates.UI.Widgets;

namespace Fourzy
{
    [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene)]
    public class UserManager : UnitySingleton<UserManager>
    {
        public const int DEFAULT_GAME_PIECE = 2;
        public const int MAX_PLAYER_LEVEL = 32;
        public const int MIN_PLAYER_LEVEL = 1;

        readonly string[] firstNameSyllables = { "kit", "mon", "fay", "shi", "zag", "blarg", "rash", "izen", "boop", "pop", "moop", "foop" };
        readonly string[] lastNameSyllables = { "malo", "zak", "abo", "wonk", "zig", "wolf", "cat", "dog", "sheep", "goat" };

        public static Action OnUpdateUserInfo;
        public static Action<CurrencyWidget.CurrencyType> onCurrencyUpdate;
        public static Action<int> OnUpdateUserGamePieceID;

        public bool debug = false;

        public string userId { get; private set; }
        public int ratingElo { get; private set; }
        public Sprite profilePicture { get; private set; }

        public string userName
        {
            get
            {
                string playerPrefsValue = PlayerPrefsWrapper.GetUserName();

                return string.IsNullOrEmpty(playerPrefsValue) ? userName = CreateNewPlayerName() : playerPrefsValue;
            }

            private set => PlayerPrefsWrapper.SetUsetName(value);
        }

        public int gamePieceID
        {
            get
            {
                int selectedGamePiece = PlayerPrefsWrapper.GetSelectedGamePiece();

                return (selectedGamePiece < 0) ? DEFAULT_GAME_PIECE : selectedGamePiece;
            }

            private set => PlayerPrefsWrapper.SetSelectedGamePiece(value);
        }

        public int coins
        {
            get => PlayerPrefsWrapper.GetCoins();

            set
            {
                PlayerPrefsWrapper.SetCoins(value);

                onCurrencyUpdate?.Invoke(CurrencyWidget.CurrencyType.COINS);
            }
        }

        public int gems
        {
            get => PlayerPrefsWrapper.GetGems();

            set
            {
                PlayerPrefsWrapper.SetGems(value);

                onCurrencyUpdate?.Invoke(CurrencyWidget.CurrencyType.GEMS);
            }
        }

        public int xp
        {
            get => PlayerPrefsWrapper.GetXP();

            set
            {
                PlayerPrefsWrapper.SetXP(value);

                onCurrencyUpdate?.Invoke(CurrencyWidget.CurrencyType.XP);
            }
        }

        public int portals
        {
            get => PlayerPrefsWrapper.GetPortals();

            set
            {
                PlayerPrefsWrapper.SetPortals(value);

                onCurrencyUpdate?.Invoke(CurrencyWidget.CurrencyType.PORTALS);
            }
        }

        public int rarePortals
        {
            get => PlayerPrefsWrapper.GetRarePortals();

            set
            {
                PlayerPrefsWrapper.SetRarePortals(value);

                onCurrencyUpdate?.Invoke(CurrencyWidget.CurrencyType.RARE_PORTALS);
            }
        }

        public int portalPoints
        {
            get => PlayerPrefsWrapper.GetPortalPoints();

            set
            {
                PlayerPrefsWrapper.SetPortalPoints(value);

                onCurrencyUpdate?.Invoke(CurrencyWidget.CurrencyType.PORTAL_POINTS);
            }
        }

        public int rarePortalPoints
        {
            get => PlayerPrefsWrapper.GetRarePortalPoints();

            set
            {
                PlayerPrefsWrapper.SetRarePortalPoints(value);

                onCurrencyUpdate?.Invoke(CurrencyWidget.CurrencyType.RARE_PORTAL_POINTS);
            }
        }

        public int tickets
        {
            get => PlayerPrefsWrapper.GetTickets();

            set
            {
                PlayerPrefsWrapper.SetTickets(value);

                OnUpdateUserInfo?.Invoke();
            }
        }

        public int level => GetLevel(xp);

        public int xpLeft => GetLevelXPLeft(xp);

        public float levelProgress => GetProgression(xp);

        public Player meAsPlayer => new Player(1, userName) { PlayerString = userId, HerdId = gamePieceID + "", };

        protected override void Awake()
        {
            base.Awake();

            if (InstanceExists) return;

            userId = "none";
            NetworkAccess.onNetworkAccess += OnNetworkAccess;
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
            for (int __level = MIN_PLAYER_LEVEL; __level <= _level; __level++)
                sum += GetLevelXP(__level);

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
            if (xp < GetLevelXP(MIN_PLAYER_LEVEL))
                return xp;
            else
                return xp - GetTotalLevelXP(GetLevel(xp) - 1);
        }

        /// <summary>
        /// Get level from XP
        /// </summary>
        /// <param name="xp"></param>
        /// <returns></returns>
        public int GetLevel(int xp)
        {
            int _level = MIN_PLAYER_LEVEL;

            while (xp >= GetLevelXP(_level) && _level <= MAX_PLAYER_LEVEL)
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
            if (upper < lower) return 0f;

            float result = 0f;

            int fromLevel = GetLevel(lower);
            int _levelDifference = GetLevel(upper) - fromLevel;

            if (_levelDifference == 0)
                return GetProgression(upper) - GetProgression(lower);
            else
            {
                for (int _level = fromLevel; _level <= fromLevel + _levelDifference; _level++)
                {
                    if (_level == fromLevel)
                        result += 1f - GetProgression(lower);
                    else if (_level == fromLevel + _levelDifference)
                        result += GetProgression(upper);
                    else
                        result++;
                }

                return result;
            }
        }

        public void UpdateInformation()
        {
            new AccountDetailsRequest().Send((response) =>
            {
                string facebookId = response.ExternalIds.GetString("FB");
                int? rateElo = response.ScriptData.GetInt("ratingElo");
                Mixpanel.Identify(response.UserId);
                Mixpanel.people.Set("$name", userName);

                //if server username not matching local username, update server username
                if (response.DisplayName != userName)
                    UpdatePlayerDisplayName(userName);

                UpdateUserInfo(response.UserId, facebookId, response.Currency1, rateElo);
            });
        }

        public void UpdatePlayerDisplayName(string name)
        {
            userName = name;

            OnUpdateUserInfo?.Invoke();

            //update username on server
            new ChangeUserDetailsRequest()
                .SetDisplayName(name)
                .Send((response) =>
                {
                    if (response.HasErrors)
                    {
                        if (debug)
                            Debug.Log("Error updating player display name: " + response.Errors.ToString());
                    }
                    else
                    {
                        if (debug)
                            Debug.Log("Successfully updated player display name");
                    }
                });
        }

        public void GetUserGamePiece()
        {
            new LogEventRequest().SetEventKey("getGamePiece")
                .Send((response) =>
                {
                    if (response.HasErrors)
                    {
                        if (debug)
                            Debug.Log("***** Error getting player gamepiece: " + response.Errors.JSON);
                    }
                    else
                    {
                        int? serverGamePiece = response.ScriptData.GetInt("gamePieceId");

                        if (serverGamePiece != gamePieceID)
                            UpdateSelectedGamePiece(gamePieceID);
                    }
                });
        }

        public void UpdateUserInfo(string uid, string fbId, long? coins, int? rating)
        {
            userId = uid;

            ratingElo = rating.GetValueOrDefault(0);
            //this.coins = coins.GetValueOrDefault(0);

            if (fbId != null)
                StartCoroutine(GetFBPicture(fbId, (sprite) =>
                {
                    if (sprite)
                        profilePicture = sprite;
                }));

            GetUserGamePiece();

            OnUpdateUserInfo?.Invoke();
        }

        public void UpdateSelectedGamePiece(int _gamePieceID)
        {
            gamePieceID = _gamePieceID;

            OnUpdateUserGamePieceID?.Invoke(gamePieceID);

            if (NetworkAccess.ACCESS)
            {
                Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                customAttributes.Add("GamePieceId", _gamePieceID);
                AnalyticsManager.LogCustom("set_gamepiece");

                new LogEventRequest().SetEventKey("setGamePiece")
                    .SetEventAttribute("gamePieceId", _gamePieceID)
                    .Send((response) =>
                    {
                        if (response.HasErrors)
                        {
                            if (debug)
                                Debug.Log("***** Error setting gamepiece: " + response.Errors.JSON);

                        }
                        else
                        {
                            if (debug)
                                Debug.Log("***** Game piece set " + response.Errors.JSON);
                        }
                    });
            }
        }

        public string CreateNewPlayerName()
        {
            //Creates a first name with 2-3 syllables
            string firstName = "";
            int numberOfSyllablesInFirstName = UnityEngine.Random.Range(2, 4);
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

        public IEnumerator GetFBPicture(string facebookId, Action<Sprite> callback)
        {
            // FB.API("/" + facebookId + "/picture?type=square&height=210&width=210", HttpMethod.GET, UpdateProfileImage);

            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture("https://graph.facebook.com/" + facebookId + "/picture?width=210&height=210"))
            {
                yield return uwr.SendWebRequest();

                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    if (debug)
                        Debug.Log("get_fb_picture_error: " + uwr.error);

                    AnalyticsManager.LogError("get_fb_picture_error", uwr.error);
                }
                else
                {
                    Texture2D tempPic = new Texture2D(25, 25);
                    tempPic = DownloadHandlerTexture.GetContent(uwr);
                    Sprite profilePictureSprite = Sprite.Create(tempPic, new Rect(0, 0, tempPic.width, tempPic.height), new Vector2(0.5f, 0.5f));

                    callback(profilePictureSprite);
                }
            }
        }

        private void OnNetworkAccess(bool networkAccess)
        {
            if (networkAccess)
            {
                if (GS.Authenticated)
                    GetUserGamePiece();
            }
        }
    }
}