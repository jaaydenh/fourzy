﻿//modded

using Fourzy._Updates.UI.Toasts;
using GameAnalyticsSDK;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif
using SA.iOS.GameKit;
using SA.Foundation.Templates;
using Fourzy._Updates.UI.Menu.Screens;
using PlayFab;
using PlayFab.ClientModels;

namespace Fourzy
{
    [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene)]
    public class LoginManager : UnitySingleton<LoginManager>
    {
        private delegate void FacebookLoginCallback(AuthenticationResponse _resp);

        public static event Action<string> OnLoginMessage;
        public static event Action<bool> OnDeviceLoginComplete;

        public static string playfabID;

        private bool isConnecting;

        private PlayFabAuthService _AuthService = PlayFabAuthService.Instance;

        protected override void Awake()
        {
            base.Awake();

            if (InstanceExists) return;

            GameManager.onNetworkAccess += OnNetworkAccess;

            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        }

        protected void Start()
        {
            GameAnalytics.Initialize();

            PlayFabAuthService.OnLoginSuccess += PlayFabLoginSuccess;
            PlayFabAuthService.OnPlayFabError += PlayFabLoginError;
        }

        protected void OnDestroy()
        {
            GameManager.onNetworkAccess -= OnNetworkAccess;

            Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
        }

        public void GetCoinsEarnedLeaderboard(Action<List<RankingScreen.LeaderboardEntry>, string> callback)
        {
            GetLeaderboard("puzzlesCompletedLeaderboard", callback, false);
        }

        public void GetWinsLeaderboard(Action<List<RankingScreen.LeaderboardEntry>, string> callback)
        {
            GetLeaderboard("winLossLeaderboard", callback, true);
        }

        private void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
        {
            Debug.Log("Firebase: Received Registration Token: " + token.Token);

            ManagePushNotifications(token.Token, "fcm");
        }

        private void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
        {
            Debug.Log("Firebase: Received a new message from: " + e.Message.From);
        }

        private void ManagePushNotifications(string token, string deviceOS)
        {
            new PushRegistrationRequest().SetDeviceOS(deviceOS)
                .SetPushId(token)
                .Send((response) =>
                {
                    if (response.HasErrors)
                    {
                        Debug.Log("***** PushRegistration Request Error: " + response.Errors.JSON);

                        //AnalyticsManager.LogError("push_registration_request_error: ", response.Errors.JSON);
                    }
                    else
                    {
                        Debug.Log("***** PushRegistration Successful: Device OS: " + deviceOS);

                        //Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                        //customAttributes.Add("deviceOS", deviceOS);
                        //AnalyticsManager.LogCustom("push_registration_request", customAttributes);
                    }
                });
        }

        private void GameCenterLogin()
        {
            isConnecting = true;

            ISN_GKLocalPlayer.Authenticate((SA_Result result) =>
            {
                if (result.IsSucceeded)
                {
                    Debug.Log("Authenticate succeeded!");

                    GameSparksGameCenterConnect();
                }
                else
                {
                    Debug.Log("Authenticate failed! Error with code: " + result.Error.Code + " and description: " + result.Error.Message);
                    DeviceLogin();
                }
            });
        }

        private void GameSparksGameCenterConnect()
        {
            ISN_GKLocalPlayer player = ISN_GKLocalPlayer.LocalPlayer;
            Debug.Log("ISN_GKLocalPlayer.LocalPlayer:Authenticated: " + player.Authenticated);
            Debug.Log("ISN_GKLocalPlayer.LocalPlayer:PlayerID: " + player.PlayerID);

            if (player.Authenticated)
            {
                player.GenerateIdentityVerificationSignatureWithCompletionHandler((signatureResult) =>
                {
                    if (signatureResult.IsSucceeded)
                    {
                        Debug.Log("signatureResult.PublicKeyUrl: " + signatureResult.PublicKeyUrl);
                        Debug.Log("signatureResult.Timestamp: " + signatureResult.Timestamp);
                        Debug.Log("signatureResult.Salt.Length: " + signatureResult.Salt.Length);
                        Debug.Log("signatureResult.Signature.Length: " + signatureResult.Signature.Length);

                        new GameCenterConnectRequest()
                            .SetDisplayName(UserManager.Instance.userName)
                            .SetPublicKeyUrl(signatureResult.PublicKeyUrl)
                            .SetTimestamp(signatureResult.Timestamp)
                            .SetSalt(signatureResult.SaltAsBse64String)
                            .SetSignature(signatureResult.SignatureAsBse64String)
                            .SetExternalPlayerId(player.PlayerID)
                            .Send((response) =>
                            {
                                if (!response.HasErrors)
                                {
                                    //AnalyticsManager.Instance.Identify(response.UserId);
                                    UserManager.Instance.userId = response.UserId;
                                    //Mixpanel.Identify(response.UserId);
                                    //AnalyticsManager.LogCustom("gamecenter_authentication_request");
                                    //UserManager.Instance.UpdateInformation();
                                    ChallengeManager.Instance.GetChallengesRequest();

                                    //OnLoginMessage?.Invoke("GameCenter Authentication Success: " + response.DisplayName);

                                    //if (GameManager.Instance.showInfoToasts)
                                    //    GamesToastsController.ShowTopToast("GameCenter Authentication Success: " + response.DisplayName);
                                }
                                else
                                {
                                    Debug.Log("***** Error Authenticating GameCenter: " + response.Errors.JSON);
                                    //OnLoginMessage?.Invoke("Error Authenticating GameCenter: " + response.Errors.JSON);

                                    //if (GameManager.Instance.showInfoToasts)
                                    //    GamesToastsController.ShowTopToast("Error Authenticating GameCenter: " + response.Errors.JSON);

                                    //AnalyticsManager.LogError("gamecenter_authentication_request_error", response.Errors.JSON);
                                }

                                isConnecting = false;

                                OnDeviceLoginComplete?.Invoke(!response.HasErrors);
                            });
                    }
                    else
                    {
                        Debug.Log("IdentityVerificationSignature has failed: " + signatureResult.Error.FullMessage);
                    }
                });
            }
            else
            {
                DeviceLogin();
            }
        }

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

        private void DeviceLogin()
        {
            isConnecting = true;

            new DeviceAuthenticationRequest()
                .SetDisplayName(UserManager.Instance.userName)
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        //AnalyticsManager.Instance.Identify(response.UserId);
                        UserManager.Instance.userId = response.UserId;
                        //Mixpanel.Identify(response.UserId);
                        //AnalyticsManager.LogCustom("device_authentication_request");
                        //UserManager.Instance.UpdateInformation();
                        ChallengeManager.Instance.GetChallengesRequest();

                        //OnLoginMessage?.Invoke("Device Authentication Success: " + response.DisplayName);

                        //if (GameManager.Instance.showInfoToasts)
                        //    GamesToastsController.ShowTopToast("Device Authentication Success: " + response.DisplayName);
                    }
                    else
                    {
                        Debug.Log("***** Error Authenticating Device: " + response.Errors.JSON);
                        //OnLoginMessage?.Invoke("Error Authenticating Device: " + response.Errors.JSON);

                        //if (GameManager.Instance.showInfoToasts)
                        //    GamesToastsController.ShowTopToast("Error Authenticating Device: " + response.Errors.JSON);

                        //AnalyticsManager.LogError("device_authentication_request_error", response.Errors.JSON);
                    }

                    isConnecting = false;

                    OnDeviceLoginComplete?.Invoke(!response.HasErrors);
                });
        }

        private void OnNetworkAccess(bool networkAccess)
        {
            Debug.Log("OnNetworkAccess: networkAccess: " + networkAccess + " isConnecting: " + isConnecting);
            //string identifierForVendor = SA.iOS.UIKit.ISN_UIDevice.CurrentDevice.IdentifierForVendor;
            //Debug.Log("IdentifierForVendor:" + SystemInfo.deviceUniqueIdentifier);

            if (networkAccess)
            {
                //initialize photon
                FourzyPhotonManager.Initialize(DEBUG: true);

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

                if (!GS.Authenticated)
                {
                    if (!isConnecting)
                    {
#if UNITY_IOS && !UNITY_EDITOR
                            Debug.Log("GameCenterLogin");
                            GameCenterLogin();    
#else
                        Debug.Log("GameSparks DeviceLogin");
                        DeviceLogin();
#endif
                    }
                }
                else
                {
                    Debug.Log("CONNECTED");
                    //UserManager.Instance.UpdateInformation();
                    ChallengeManager.Instance.GetChallengesRequest();
                    OnDeviceLoginComplete?.Invoke(true);
                }
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
            playfabID = result.PlayFabId;
            //AnalyticsManager.Instance.Identify(result.PlayFabId);

            GameManager.Instance.CheckNews();

            if (GameManager.Instance.showInfoToasts) GamesToastsController.ShowTopToast("Device Authentication Success");

            if (result.NewlyCreated)
            {
                //try set name
                Debug.Log($"New device");
                UserManager.Instance.settingRandomName = true;
                UserManager.Instance.SetDisplayName(UserManager.CreateNewPlayerName());
            }
            else
            {
                PlayFabClientAPI.GetAccountInfo(
                    new GetAccountInfoRequest()
                    {
                        PlayFabId = result.PlayFabId,
                    },
                    OnGetPlayerAccount,
                    OnPlayFabGetProfileError);
            }
        }

        private void OnPlayFabGetProfileError(PlayFabError error)
        {
            if (GameManager.Instance.showInfoToasts) GamesToastsController.ShowTopToast("Error retrieving players profile: " + error.ErrorMessage);
        }

        private void OnGetPlayerAccount(GetAccountInfoResult result)
        {
            OnLoginMessage?.Invoke("Device Authentication Success: " + result.AccountInfo.TitleInfo.DisplayName);

            UserManager.Instance.SetDisplayName(result.AccountInfo.TitleInfo.DisplayName, false);

            //update language?

        }

        private void PlayFabLoginError(PlayFabError error)
        {
            Debug.Log("***** PlayFab Login Error: " + error.ErrorMessage);
        }

        private void GetLeaderboard(string leaderboardShortCode, Action<List<RankingScreen.LeaderboardEntry>, string> callback, bool isWinsLeaderboard)
        {
            new LeaderboardDataRequest()
                .SetLeaderboardShortCode(leaderboardShortCode)
                .SetEntryCount(100)
                .Send((response) =>
                {
                    if (response.HasErrors)
                    {
                        Debug.Log("***** Error Retrieving Leaderboard Data: " + response.Errors.JSON);
                        callback?.Invoke(null, "Error Retrieving Leaderboard Data: " + response.Errors.JSON);
                        return;
                    }

                    List<RankingScreen.LeaderboardEntry> leaderboards = new List<RankingScreen.LeaderboardEntry>();

                    foreach (LeaderboardDataResponse._LeaderboardData entry in response.Data)
                    {
                        RankingScreen.LeaderboardEntry leaderboard = new RankingScreen.LeaderboardEntry();
                        leaderboard.userId = entry.UserId;
                        leaderboard.userName = entry.UserName;
                        leaderboard.facebookId = entry.ExternalIds.GetString("FB");
                        leaderboard.rank = entry.Rank.Value;
                        leaderboard.isWinsLeaderboard = isWinsLeaderboard;
                        leaderboard.value = isWinsLeaderboard ? entry.JSONData["wins"].ToString() : leaderboard.value = entry.JSONData["completed"].ToString();

                        leaderboards.Add(leaderboard);
                    }

                    callback?.Invoke(leaderboards, string.Empty);
                });
        }
    }
}
