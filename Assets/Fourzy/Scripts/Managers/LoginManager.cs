//modded

using Fourzy._Updates;
using Fourzy._Updates.UI.Toasts;
//#if UNITY_IOS
//using UnityEngine.SocialPlatforms.GameCenter;
//#endif
//using SA.iOS.GameKit;
//using SA.Foundation.Templates;
using FourzyGameModel.Model;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.ProfilesModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fourzy
{
    [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene)]
    public class LoginManager : UnitySingleton<LoginManager>
    {
        public static event Action<string> OnLoginMessage;
        public static event Action<bool> OnDeviceLoginComplete;

        public static string playfabId;
        public static string playerTitleId;
        public static string masterAccountId;

        private bool isConnecting;
        private bool loadingInventory = false;

        private PlayFabAuthService _AuthService = PlayFabAuthService.Instance;

        protected override void Awake()
        {
            base.Awake();

            if (InstanceExists) return;

            GameManager.onNetworkAccess += OnNetworkAccess;
            UserManager.onPlayfabValuesLoaded += OnPlayfabValueLoaded;

            //Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            //Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        }

        protected void Start()
        {
            PlayFabAuthService.OnLoginSuccess += PlayFabLoginSuccess;
            PlayFabAuthService.OnPlayFabError += OnPlayFabError;
        }

        protected void OnDestroy()
        {
            GameManager.onNetworkAccess -= OnNetworkAccess;
            UserManager.onPlayfabValuesLoaded -= OnPlayfabValueLoaded;

            //Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
            //Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
        }

        // public void GetCoinsEarnedLeaderboard(Action<List<RankingScreen.LeaderboardEntry>, string> callback)
        // {
        //     GetLeaderboard("puzzlesCompletedLeaderboard", callback, false);
        // }

        // public void GetWinsLeaderboard(Action<List<RankingScreen.LeaderboardEntry>, string> callback)
        // {
        //     GetLeaderboard("winLossLeaderboard", callback, true);
        // }

        //private void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
        //{
        //    Debug.Log("Firebase: Received Registration Token: " + token.Token);

        //    ManagePushNotifications(token.Token, "fcm");
        //}

        //private void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
        //{
        //    Debug.Log("Firebase: Received a new message from: " + e.Message.From);
        //}

        private void ManagePushNotifications(string token, string deviceOS)
        {
            // new PushRegistrationRequest().SetDeviceOS(deviceOS)
            //     .SetPushId(token)
            //     .Send((response) =>
            //     {
            //         if (response.HasErrors)
            //         {
            //             Debug.Log("***** PushRegistration Request Error: " + response.Errors.JSON);

            //             //AnalyticsManager.LogError("push_registration_request_error: ", response.Errors.JSON);
            //         }
            //         else
            //         {
            //             Debug.Log("***** PushRegistration Successful: Device OS: " + deviceOS);

            //             //Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            //             //customAttributes.Add("deviceOS", deviceOS);
            //             //AnalyticsManager.LogCustom("push_registration_request", customAttributes);
            //         }
            //     });
        }

        //private void GameCenterLogin()
        //{
        //    isConnecting = true;

        //    ISN_GKLocalPlayer.Authenticate((SA_Result result) =>
        //    {
        //        if (result.IsSucceeded)
        //        {
        //            Debug.Log("Authenticate succeeded!");

        //            GameSparksGameCenterConnect();
        //        }
        //        else
        //        {
        //            Debug.Log("Authenticate failed! Error with code: " + result.Error.Code + " and description: " + result.Error.Message);
        //            DeviceLogin();
        //        }
        //    });
        //}

        //private void GameSparksGameCenterConnect()
        //{
        //    ISN_GKLocalPlayer player = ISN_GKLocalPlayer.LocalPlayer;
        //    Debug.Log("ISN_GKLocalPlayer.LocalPlayer:Authenticated: " + player.Authenticated);
        //    Debug.Log("ISN_GKLocalPlayer.LocalPlayer:PlayerID: " + player.PlayerID);

        //    if (player.Authenticated)
        //    {
        //        player.GenerateIdentityVerificationSignatureWithCompletionHandler((signatureResult) =>
        //        {
        //            if (signatureResult.IsSucceeded)
        //            {
        //                Debug.Log("signatureResult.PublicKeyUrl: " + signatureResult.PublicKeyUrl);
        //                Debug.Log("signatureResult.Timestamp: " + signatureResult.Timestamp);
        //                Debug.Log("signatureResult.Salt.Length: " + signatureResult.Salt.Length);
        //                Debug.Log("signatureResult.Signature.Length: " + signatureResult.Signature.Length);

        //                new GameCenterConnectRequest()
        //                    .SetDisplayName(UserManager.Instance.userName)
        //                    .SetPublicKeyUrl(signatureResult.PublicKeyUrl)
        //                    .SetTimestamp(signatureResult.Timestamp)
        //                    .SetSalt(signatureResult.SaltAsBse64String)
        //                    .SetSignature(signatureResult.SignatureAsBse64String)
        //                    .SetExternalPlayerId(player.PlayerID)
        //                    .Send((response) =>
        //                    {
        //                        if (!response.HasErrors)
        //                        {
        //                            //AnalyticsManager.Instance.Identify(response.UserId);
        //                            UserManager.Instance.userId = response.UserId;
        //                            //Mixpanel.Identify(response.UserId);
        //                            //AnalyticsManager.LogCustom("gamecenter_authentication_request");
        //                            //UserManager.Instance.UpdateInformation();
        //                            ChallengeManager.Instance.GetChallengesRequest();

        //                            //OnLoginMessage?.Invoke("GameCenter Authentication Success: " + response.DisplayName);

        //                            //if (GameManager.Instance.showInfoToasts)
        //                            //    GamesToastsController.ShowTopToast("GameCenter Authentication Success: " + response.DisplayName);
        //                        }
        //                        else
        //                        {
        //                            Debug.Log("***** Error Authenticating GameCenter: " + response.Errors.JSON);
        //                            //OnLoginMessage?.Invoke("Error Authenticating GameCenter: " + response.Errors.JSON);

        //                            //if (GameManager.Instance.showInfoToasts)
        //                            //    GamesToastsController.ShowTopToast("Error Authenticating GameCenter: " + response.Errors.JSON);

        //                            //AnalyticsManager.LogError("gamecenter_authentication_request_error", response.Errors.JSON);
        //                        }

        //                        isConnecting = false;

        //                        OnDeviceLoginComplete?.Invoke(!response.HasErrors);
        //                    });
        //            }
        //            else
        //            {
        //                Debug.Log("IdentityVerificationSignature has failed: " + signatureResult.Error.FullMessage);
        //            }
        //        });
        //    }
        //    else
        //    {
        //        DeviceLogin();
        //    }
        //}

        //private void GooglePlayLogin()
        //{
        //    isConnecting = true;

        //    new GooglePlayConnectRequest()
        //    .SetDisplayName(UserManager.Instance.userName)
        //    .Send((response) =>{
        //        if (!response.HasErrors)
        //        {
        //            AnalyticsManager.Instance.Identify(response.UserId);
        //            //Mixpanel.Identify(response.UserId);
        //            //AnalyticsManager.LogCustom("googleplay_authentication_request");
        //            UserManager.Instance.UpdateInformation();
        //            ChallengeManager.Instance.GetChallengesRequest();

        //            OnLoginMessage?.Invoke("GooglePlay Authentication Success: " + response.DisplayName);

        //            if (GameManager.Instance.showInfoToasts)
        //                GamesToastsController.ShowTopToast("GooglePlay Authentication Success: " + response.DisplayName);
        //        }
        //        else
        //        {
        //            Debug.Log("***** Error Authenticating GooglePlay: " + response.Errors.JSON);
        //            OnLoginMessage?.Invoke("Error Authenticating GooglePlay: " + response.Errors.JSON);

        //            if (GameManager.Instance.showInfoToasts)
        //                GamesToastsController.ShowTopToast("Error Authenticating GooglePlay: " + response.Errors.JSON);

        //            //AnalyticsManager.LogError("googleplay_authentication_request_error", response.Errors.JSON);
        //        }

        //        isConnecting = false;

        //        OnDeviceLoginComplete?.Invoke(!response.HasErrors);
        //    });
        //}

        // private void DeviceLogin()
        // {
        //     isConnecting = true;

        //     new DeviceAuthenticationRequest()
        //         .SetDisplayName(UserManager.Instance.userName)
        //         .Send((response) =>
        //         {
        //             if (!response.HasErrors)
        //             {
        //                 //AnalyticsManager.Instance.Identify(response.UserId);
        //                 UserManager.Instance.userId = response.UserId;
        //                 //Mixpanel.Identify(response.UserId);
        //                 //AnalyticsManager.LogCustom("device_authentication_request");
        //                 //UserManager.Instance.UpdateInformation();
        //                 ChallengeManager.Instance.GetChallengesRequest();

        //                 //OnLoginMessage?.Invoke("Device Authentication Success: " + response.DisplayName);

        //                 //if (GameManager.Instance.showInfoToasts)
        //                 //    GamesToastsController.ShowTopToast("Device Authentication Success: " + response.DisplayName);
        //             }
        //             else
        //             {
        //                 Debug.Log("***** Error Authenticating Device: " + response.Errors.JSON);
        //                 //OnLoginMessage?.Invoke("Error Authenticating Device: " + response.Errors.JSON);

        //                 //if (GameManager.Instance.showInfoToasts)
        //                 //    GamesToastsController.ShowTopToast("Error Authenticating Device: " + response.Errors.JSON);

        //                 //AnalyticsManager.LogError("device_authentication_request_error", response.Errors.JSON);
        //             }

        //             isConnecting = false;

        //             OnDeviceLoginComplete?.Invoke(!response.HasErrors);
        //         });
        // }

        private void OnNetworkAccess(bool networkAccess)
        {
            Debug.Log("OnNetworkAccess: networkAccess: " + networkAccess + " isConnecting: " + isConnecting);
            //string identifierForVendor = SA.iOS.UIKit.ISN_UIDevice.CurrentDevice.IdentifierForVendor;
            //Debug.Log("IdentifierForVendor:" + SystemInfo.deviceUniqueIdentifier);

            if (networkAccess)
            {
                if (!PlayFabClientAPI.IsClientLoggedIn())
                {
                    _AuthService.Authenticate(Authtypes.Silent);
                    //#if UNITY_ANDROID
                    //                    PlayFabAndroidDeviceLogin();
                    //#elif UNITY_IOS
                    //                    PlayFabIOSDeviceLogin();
                    //#else
                    //                    PlayFabCustomIDLogin();
                    //#endif
                }

                // if (!GS.Authenticated)
                // {
                //     if (!isConnecting)
                //     {
                //#if UNITY_IOS && !UNITY_EDITOR
                //Debug.Log("GameCenterLogin");
                //GameCenterLogin();    
                //#else
                // Debug.Log("GameSparks DeviceLogin");
                // DeviceLogin();
                //#endif
                //         }
                //     }
                //     else
                //     {
                //         Debug.Log("CONNECTED");
                //         //UserManager.Instance.UpdateInformation();
                //         ChallengeManager.Instance.GetChallengesRequest();
                //         OnDeviceLoginComplete?.Invoke(true);
                //     }
            }
        }

        //private void PlayFabIOSDeviceLogin()
        //{
        //    LoginWithIOSDeviceIDRequest request = new LoginWithIOSDeviceIDRequest();
        //    request.DeviceId = SystemInfo.deviceUniqueIdentifier;
        //    request.TitleId = "9EB47";
        //    request.CreateAccount = true;
        //    request.DeviceModel = SystemInfo.deviceModel;
        //    request.OS = SystemInfo.operatingSystem;

        //    PlayFabClientAPI.LoginWithIOSDeviceID(request, PlayFabLoginSuccess, PlayFabLoginError);
        //}

        //private void PlayFabAndroidDeviceLogin()
        //{
        //    LoginWithAndroidDeviceIDRequest request = new LoginWithAndroidDeviceIDRequest();
        //    request.AndroidDeviceId = SystemInfo.deviceUniqueIdentifier;
        //    request.TitleId = "9EB47";
        //    request.CreateAccount = true;
        //    request.AndroidDevice = SystemInfo.deviceModel;
        //    request.OS = SystemInfo.operatingSystem;

        //    PlayFabClientAPI.LoginWithAndroidDeviceID(request, PlayFabLoginSuccess, PlayFabLoginError);
        //}

        //private void PlayFabCustomIDLogin()
        //{
        //    LoginWithCustomIDRequest request = new LoginWithCustomIDRequest();
        //    request.CustomId = SystemInfo.deviceUniqueIdentifier;
        //    request.TitleId = "9EB47";
        //    request.CreateAccount = true;

        //    PlayFabClientAPI.LoginWithCustomID(request, PlayFabLoginSuccess, PlayFabLoginError);
        //}

        private void PlayFabLoginSuccess(LoginResult result)
        {
            playfabId = result.PlayFabId;
            AnalyticsManager.SetUsetID(result.PlayFabId);
            AnalyticsManager.Instance.FlushPlayfabEvents();

            if (Application.isEditor || Debug.isDebugBuild)
            {
                Debug.Log("PlayFabId: " + result.PlayFabId);
                Debug.Log("SessionTicket: " + result.SessionTicket);
                Debug.Log("EntityToken: " + result.EntityToken.EntityToken);
                Debug.Log("TokenExpiration: " + result.EntityToken.TokenExpiration);
                Debug.Log("Entity.Id: " + result.EntityToken.Entity.Id);
                Debug.Log("Entity.Type: " + result.EntityToken.Entity.Type);
                Debug.Log(result);
            }

            if (!GameManager.Instance.Landscape)
            {
                GamesToastsController.ShowTopToast("Device Authentication Success");
            }

            if (result.NewlyCreated)
            {
                //try set name
                Debug.Log($"New device");
                UserManager.Instance.settingRandomName = true;
                UserManager.Instance.SetDisplayName(CharacterNameFactory.GeneratePlayerName());
                UserManager.Instance.UpdateSelectedGamePiece(InternalSettings.Current.DEFAULT_GAME_PIECE);

                Amplitude.Instance.setUserProperties(new Dictionary<string, object>()
                {
                    ["hasMonetized"] = false,
                    ["hasWatchedAd"] = false,
                    ["totalAdventurePuzzlesCompleted"] = 0,
                    ["totalAdventurePuzzleFailures"] = 0,
                    ["totalAdventurePuzzlesReset"] = 0,
                    ["totalRealtimeGamesCompleted"] = 0,
                    ["totalRealtimeGamesWon"] = 0,
                    ["totalRealtimeGamesLost"] = 0,
                    ["totalRealtimeGamesDraw"] = 0,
                    ["totalRealtimeGamesAbandoned"] = 0,
                    ["totalRealtimeGamesOpponentAbandoned"] = 0,
                    ["totalSpent"] = 0,
                    ["totalAdsWatched"] = 0,
                });
            }

            int timesOpened = PlayerPrefsWrapper.GetAppOpened();
            if (timesOpened == 1)
            {
                Amplitude.Instance.setUserProperty("firstEntry", false);
            }
            else if (timesOpened == 2)
            {
                Amplitude.Instance.setUserProperty("firstEntry", false);
            }

            //get seconds since last opened
            long lastOpened = PlayerPrefsWrapper.GetSecondsSinceLastOpen();
            PlayerPrefsWrapper.AddDaysPlayed(lastOpened / 60f / 24f);
            Amplitude.Instance.setUserProperty("totalDaysPlayed", PlayerPrefsWrapper.GetDaysPlayed());
            PlayerPrefsWrapper.SetAppOpenedTime();

            Amplitude.Instance.setUserProperty("totalSessions", timesOpened);

            UserManager.Instance.AddPlayfabValueLoaded(PlayfabValuesLoaded.LOGGED_IN);

            PlayFabClientAPI.GetAccountInfo(
                new GetAccountInfoRequest()
                {
                    PlayFabId = result.PlayFabId,
                },
                OnGetPlayerAccount,
                error => Debug.Log("Error retrieving players profile: " + error.ErrorMessage));

            RequestPhotonToken(result);
        }

        private void RequestPhotonToken(LoginResult obj)
        {
            Debug.Log("PlayFab authenticated. Requesting photon token...");

            PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest()
            {
                PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime
            }, AuthenticateWithPhoton, OnPlayFabError);
        }

        private void OnPlayfabValueLoaded()
        {
            if (!loadingInventory && UserManager.Instance.IsPlayfabValueLoaded(PlayfabValuesLoaded.CATALOG_INFO_RECEIVED))
            {
                loadingInventory = true;

                //get inventory
                PlayFabClientAPI.GetUserInventory(
                    new GetUserInventoryRequest(),
                    OnUserInventoryFetched,
                    error => {
                        loadingInventory = false;
                        OnPlayFabError(error);
                    });
            }
        }

        private void AuthenticateWithPhoton(GetPhotonAuthenticationTokenResult obj)
        {
            LogMessage("Photon token acquired: " + obj.PhotonCustomAuthenticationToken + "  Authentication complete.");

            var customAuth = new Photon.Realtime.AuthenticationValues { AuthType = Photon.Realtime.CustomAuthenticationType.Custom };
            customAuth.AddAuthParameter("username", playfabId);
            customAuth.AddAuthParameter("token", obj.PhotonCustomAuthenticationToken);

            PhotonNetwork.AuthValues = customAuth;

            FourzyPhotonManager.Instance.JoinLobby();
        }

        public void LogMessage(string message)
        {
            if (Application.isEditor || Debug.isDebugBuild)
            {
                Debug.Log("Login Manager: " + message);
            }
            else
            {
                Debug.Log("Photon token aquired");
            }
        }

        private void OnProfileOK(GetEntityProfileResponse profile)
        {
            StartCoroutine(SetProfileLanguage(profile.Profile.Language));
        }

        private void OnGetPlayerAccount(GetAccountInfoResult result)
        {
            OnLoginMessage?.Invoke("Device Authentication Success: " + result.AccountInfo.TitleInfo.DisplayName);

            // check display name
            if (string.IsNullOrEmpty(result.AccountInfo.TitleInfo.DisplayName))
            {
                UserManager.Instance.settingRandomName = true;
                UserManager.Instance.SetDisplayName(CharacterNameFactory.GeneratePlayerName());
            }
            else
            {
                UserManager.Instance.SetDisplayName(result.AccountInfo.TitleInfo.DisplayName, false);
            }

            playerTitleId = result.AccountInfo.TitleInfo.TitlePlayerAccount.Id;
            masterAccountId = result.AccountInfo.PlayFabId;

            UserManager.Instance.AddPlayfabValueLoaded(PlayfabValuesLoaded.ACCOUNT_INFO_RECEIVED);

            UserManager.GetMyStats();
            GameManager.GetTitleData(data =>
            {
                InternalSettings.Current.Update(data);
                UserManager.Instance.AddPlayfabValueLoaded(PlayfabValuesLoaded.TITLE_DATA_RECEIVED);
            }, 
            null);

            //get player profile to see total spent
            PlayFabClientAPI.GetPlayerProfile(
                new GetPlayerProfileRequest()
                {
                    PlayFabId = playfabId
                },
                OnPlayerProfile,
                error => Debug.Log("Error getting player profile: " + error.ErrorMessage));

            //get profile
            PlayFabProfilesAPI.GetProfile(
                new GetEntityProfileRequest()
                {
                    Entity = new PlayFab.ProfilesModels.EntityKey()
                    {
                        Id = playerTitleId,
                        Type = "title_player_account"
                    }
                },
                OnProfileOK,
                error =>
                {
                    Debug.Log("Error getting profile: " + error.ErrorMessage);
                    GameManager.Instance.ReportPlayFabError(error.ErrorMessage);
                });

            //get catalog items
            GameContentManager.Instance.GetItemsCataloge();
        }

        private void OnUserInventoryFetched(GetUserInventoryResult inventoryRequestResult)
        {
            loadingInventory = false;

            //clear all tokens exept default
            PlayerPrefsWrapper.RemoveAllButDefault();
            List<TokenType> inventoryTokens = new List<TokenType>();

            GameContentManager.Instance.piecesDataHolder.ResetPieces();
            foreach (ItemInstance itemInstance in inventoryRequestResult.Inventory)
            {
                switch (itemInstance.ItemClass)
                {
                    case Constants.PLAYFAB_GAMEPIECE_CLASS:
                        GamePieceData gp =
                            GameContentManager.Instance.piecesDataHolder.GetGamePieceData(itemInstance.ItemId);

                        if (gp != null)
                        {
                            gp.Pieces = itemInstance.RemainingUses.Value;
                        }
                        else
                        {
                            string message = $"Gamepiece id {itemInstance.ItemId} not found";

                            if (Application.isEditor || Debug.isDebugBuild)
                            {
                                Debug.LogWarning(message);
                            }

                            GameManager.Instance.ReportPlayFabError(message);
                        }

                        break;

                    case Constants.PLAYFAB_TOKEN_CLASS:
                        TokenType _token = (TokenType)Enum.Parse(typeof(TokenType), itemInstance.ItemId);

                        if (_token == TokenType.NONE)
                        {
                            string message = $"Token {itemInstance.ItemId} not found";

                            if (Application.isEditor || Debug.isDebugBuild)
                            {
                                Debug.LogWarning(message);
                            }

                            GameManager.Instance.ReportPlayFabError(message);
                        }
                        else
                        {
                            inventoryTokens.Add(_token);
                        }

                        break;

                    case Constants.PLAYFAB_BUNDLE_CLASS:
                        GameContentManager.Instance.bundlesInPlayerInventory.Add(itemInstance.ItemId);

                        break;
                }
            }

            if (inventoryTokens.Count > 0)
            {
                PlayerPrefsWrapper.AddUnlockedTokens(inventoryTokens, _Updates.Serialized.TokenUnlockType.AREA_PROGRESS);
            }

            foreach (var currentcyData in inventoryRequestResult.VirtualCurrency)
            {
                switch (currentcyData.Key)
                {
                    case Constants.HINTS_CURRENCY_KEY:
                        UserManager.OnHintsValueUpdated(currentcyData.Value);

                        break;
                }
            }

            UserManager.Instance.AddPlayfabValueLoaded(PlayfabValuesLoaded.USER_INVENTORY_RECEIVED);
        }

        private void OnPlayerProfile(GetPlayerProfileResult playerProfile)
        {
            UserManager.Instance.totalSpentUSD = playerProfile.PlayerProfile.TotalValueToDateInUSD ?? 0;

            UserManager.Instance.AddPlayfabValueLoaded(PlayfabValuesLoaded.PLAYER_PROFILE_RECEIVED);
        }

        private void OnPlayFabError(PlayFabError error)
        {
            GameManager.Instance.ReportPlayFabError(error.ErrorMessage);
            LogMessage(error.GenerateErrorReport());
        }

        private IEnumerator SetProfileLanguage(string currentLanguageCode)
        {
            while (!LocalizationManager.Instance.isReady || string.IsNullOrEmpty(playerTitleId)) yield return null;

            Debug.Log($"Language state: server {currentLanguageCode} ours {LocalizationManager.Instance.languageCode}");
            if (currentLanguageCode != LocalizationManager.Instance.languageCode)
            {
                //updating system language
                PlayFabProfilesAPI.SetProfileLanguage(new SetProfileLanguageRequest()
                {
                    Entity = new PlayFab.ProfilesModels.EntityKey()
                    {
                        Id = playerTitleId,
                        Type = "title_player_account"
                    },
                    ExpectedVersion = 0,
                    Language = LocalizationManager.Instance.languageCode,
                }, OnLanguageSetOK, OnLanguageSetError);
            }
            else
            {
                UserManager.Instance.AddPlayfabValueLoaded(PlayfabValuesLoaded.LANGUAGE_CHECKED);
                GameManager.Instance.CheckNews();
            }
        }

        private void OnLanguageSetOK(SetProfileLanguageResponse response)
        {
            Debug.Log("Language updated");

            UserManager.Instance.AddPlayfabValueLoaded(PlayfabValuesLoaded.LANGUAGE_CHECKED);
            GameManager.Instance.CheckNews();
        }

        private void OnLanguageSetError(PlayFabError error)
        {
            Debug.Log("Language set error " + error.ErrorMessage);
        }
    }
}
