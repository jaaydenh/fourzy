using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Facebook.Unity;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Core;
using GameAnalyticsSDK;
using mixpanel;

namespace Fourzy
{
    public class LoginManager : Singleton<LoginManager>
    {
        private delegate void FacebookLoginCallback(AuthenticationResponse _resp);

        readonly string[] firstNameSyllables = { "kit", "mon", "fay", "shi", "zag", "blarg", "rash", "izen", "boop", "pop", "moop", "foop" };
        readonly string[] lastNameSyllables = { "malo", "zak", "abo", "wonk", "zig", "wolf", "cat", "dog", "sheep", "goat" };
        private bool readyForDeviceLogin;

        public List<Friend> Friends { get; private set; }

        public static event Action<string> OnLoginMessage;
        public static event Action<bool> OnFBLoginComplete;
        public static event Action OnGetFriends;

        new void Awake()
        {
            base.Awake();

            ConnectWithFacebook();
        }

        private void Start()
        {
            GameAnalytics.Initialize();
            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
            GS.GameSparksAvailable += GameSparksIsAvailable;
        }

        private void GameSparksIsAvailable(bool connected)
        {
            if (connected && readyForDeviceLogin && !FB.IsLoggedIn)
            {
                DeviceLogin();
            }
            else if (connected)
            {
                readyForDeviceLogin = true;
            }
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

                        AnalyticsManager.LogError("push_registration_request_error: ", response.Errors.JSON);
                    }
                    else
                    {
                        Debug.Log("***** PushRegistration Successful: Device OS: " + deviceOS);

                        Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                        customAttributes.Add("deviceOS", deviceOS);
                        AnalyticsManager.LogCustom("push_registration_request", customAttributes);
                    }
                });
        }

        void DeviceLogin()
        {
            new DeviceAuthenticationRequest()
                .SetDisplayName(CreateNewPlayerName())
                .Send((response) => {
                    if (!response.HasErrors)
                    {
                        Mixpanel.Identify(response.UserId);
                        UserManager.instance.UpdateUserInfo(response.DisplayName, response.UserId, null, null, null);
                        UserManager.instance.UpdateInformation();
                    ChallengeManager.instance.GetChallengesRequest();
                        //LeaderboardManager.instance.GetLeaderboard();

                        AnalyticsManager.LogCustom("device_authentication_request");
                        if (OnLoginMessage != null)
                            OnLoginMessage("Device Authentication Success: " + response.DisplayName);
                    }
                    else
                    {
                        Debug.Log("***** Error Authenticating Device: " + response.Errors.JSON);
                        if (OnLoginMessage != null)
                            OnLoginMessage("Error Authenticating Device: " + response.Errors.JSON);

                        AnalyticsManager.LogError("device_authentication_request_error", response.Errors.JSON);
                    }
                });
        }

        /// <summary>
        /// Below we will login with facebook.
        /// When FB is ready we will call the method that allows GS to connect to GameSparks
        /// </summary>
        public void ConnectWithFacebook()
        {
            if (OnLoginMessage != null)
                OnLoginMessage("Connecting to Facebook With GameSparks...");// first check if FB is ready, and then login //
            // if its not ready we just init FB and use the login method as the callback for the init method //
            if (!FB.IsInitialized)
            {
                Debug.Log("Initializing Facebook...");
                FB.Init(CheckFacebookLogin, null);
            }
            else
            {
                FB.ActivateApp();
                CheckFacebookLogin();
            }
        }

        void CheckFacebookLogin()
        {
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
                if (FB.IsLoggedIn)
                {
                    Debug.Log("Already Logged into Facebook");
                    GameSparksFBLogin(AfterFBLogin);
                    //facebookLoginButton.gameObject.SetActive(false);
                }
                else
                {
                    Debug.Log("Not Logged into Facebook: readyForDeviceLogin: " + readyForDeviceLogin);
                    //facebookLoginButton.gameObject.SetActive(true);
                    //facebookLoginButton.interactable = true;

                    if (readyForDeviceLogin)
                    {
                        DeviceLogin();
                    }
                    else
                    {
                        readyForDeviceLogin = true;
                    }
                }
            }
            else
            {
                ConnectWithFacebook(); // if we are still not connected, then try to process again
            }
        }

        /// <summary>
        /// When Facebook is ready , this will connect the player to Facebook
        /// After the Player is authenticated it will  call the GS connect
        /// </summary>
        public void FacebookLogin()
        {
            if (!FB.IsLoggedIn)
            {
                Debug.Log("Logging into Facebook");
                var perms = new List<string>() { "public_profile", "email", "user_friends" };
                FB.LogInWithReadPermissions(perms, GameSparksFBConnect);
            }
            else
            {
                GameSparksFBLogin(AfterFBLogin);
            }
        }

        void GameSparksFBConnect(ILoginResult result)
        {
            Debug.Log("GameSparksFBConnect");
            if (FB.IsLoggedIn)
            {
                Debug.Log("Logging into gamesparks with facebook details");

                AnalyticsManager.LogCustom("player_connects_with_facebook");
                GameSparksFBLogin(AfterFBLogin);
            }
            else
            {
                Debug.LogWarning("Something went wrong with connecting to FaceBook: " + result.Error);

                if (OnFBLoginComplete != null)
                {
                    OnFBLoginComplete(false);
                }

                if (OnLoginMessage != null)
                {
                    AnalyticsManager.LogError("gamesparks_fb_connect_error", result.Error);
                    OnLoginMessage("Gamesparks login error: " + result.Error);
                }
                // else {
                //     AnalyticsManager.LogCustom("gamesparks_fb_connect_decline");
                //     OnLoginMessage("Declined Facebook Login");
                // }
            }
        }

        //This is the callback that happens when gamesparks has been connected with FB
        private void AfterFBLogin(AuthenticationResponse response)
        {
            Debug.Log("AfterFBLogin:UserId: " + response.UserId);
            OnLoginMessage("Succesfully Logged into Facebook");

            if (OnFBLoginComplete != null)
            {
                OnFBLoginComplete(true);
            }

            UserManager.instance.UpdateInformation();
            ChallengeManager.instance.GetChallengesRequest();
            this.GetFriendsRequest();
            //LeaderboardManager.instance.GetLeaderboard();
        }

        //This method will connect GameSparks with FaceBook
        private void GameSparksFBLogin(FacebookLoginCallback _fbLoginCallback)
        {
            Debug.Log("Sending FacebookConnectRequest using AccessToken: " + AccessToken.CurrentAccessToken.TokenString);
            bool success = false;

            new FacebookConnectRequest()
                .SetAccessToken(AccessToken.CurrentAccessToken.TokenString)
                .SetMaxResponseTimeInMillis(15000)
                .SetDoNotLinkToCurrentPlayer(false)// we don't want to create a new account so link to the player that is currently logged in
                .SetSwitchIfPossible(true)//this will switch to the player with this FB account id if they already have an account from a separate login
                .SetSyncDisplayName(true)
                .Send((response) => {
                    if (!response.HasErrors)
                    {
                        Debug.Log("Logged into gamesparks with facebook");
                        success = true;
                        _fbLoginCallback(response);
                    }
                    else
                    {
                        Debug.LogWarning("***** Error Logging into Facebook: " + response.Errors.JSON);
                        if (OnFBLoginComplete != null)
                        {
                            OnFBLoginComplete(false);
                        }

                        if (OnLoginMessage != null)
                            OnLoginMessage("Error Logging into Facebook: " + response.Errors.JSON);
                    }

                    AnalyticsManager.LogLogin("facebook", success);
                });
        }

        private string CreateNewPlayerName()
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

        private void GetFriendsRequest()
        {
            if (!FB.IsLoggedIn)
            {
                return;
            }

            new ListGameFriendsRequest().Send((response) =>
            {
                if (Friends == null)
                {
                    Friends = new List<Friend>();
                }
                
                Friends.Clear();

                foreach (var friendResp in response.Friends)
                {
                    Friend friend = new Friend();
                    friend.userName = friendResp.DisplayName;
                    friend.id = friendResp.Id;
                    friend.isOnline = friendResp.Online.Value;
                    friend.facebookId = friendResp.ExternalIds.GetString("FB");
                    Friends.Add(friend);
                }

                if (OnGetFriends != null)
                {
                    OnGetFriends();
                }
            });
        }

        public void GetCoinsEarnedLeaderboard(Action<List<Leaderboard>, string> callback)
        {
            this.GetLeaderboard("puzzlesCompletedLeaderboard", callback, false);
        }

        public void GetWinsLeaderboard(Action<List<Leaderboard>, string> callback)
        {
            this.GetLeaderboard("winLossLeaderboard", callback, true);
        }

        private void GetLeaderboard(string leaderboardShortCode, Action<List<Leaderboard>, string> callback, bool isWinsLeaderboard)
        {
            new LeaderboardDataRequest()
                .SetLeaderboardShortCode(leaderboardShortCode)
                .SetEntryCount(100)
                .Send((response) =>
                {
                    if (response.HasErrors)
                    {
                        Debug.Log("***** Error Retrieving Leaderboard Data: " + response.Errors.JSON);
                        if (callback != null)
                        {
                            callback(null, "Error Retrieving Leaderboard Data: " + response.Errors.JSON);
                        }
                        return;
                    }

                    List<Leaderboard> leaderboards = new List<Leaderboard>();

                    foreach (LeaderboardDataResponse._LeaderboardData entry in response.Data)
                    {
                        Leaderboard leaderboard = new Leaderboard();
                        leaderboard.userId = entry.UserId;
                        leaderboard.userName = entry.UserName;
                        leaderboard.facebookId = entry.ExternalIds.GetString("FB");
                        leaderboard.rank = entry.Rank.Value;
                        leaderboard.isWinsLeaderboard = isWinsLeaderboard;
                        if (isWinsLeaderboard)
                        {
                            leaderboard.wins = entry.JSONData["wins"].ToString();
                        }
                        else
                        {
                            leaderboard.coins = entry.JSONData["completed"].ToString();
                        }
                        leaderboards.Add(leaderboard);
                    }

                    if (callback != null)
                    {
                        callback(leaderboards, string.Empty);
                    }
                });
        }
    }
}
