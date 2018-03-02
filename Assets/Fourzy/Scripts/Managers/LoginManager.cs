using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Facebook.Unity;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Core;

namespace Fourzy
{
    public class LoginManager : Singleton<LoginManager> {

        public delegate void FacebookLoginCallback(AuthenticationResponse _resp);

        public delegate void LoginError();
        public static event LoginError OnLoginError;
        [SerializeField]
        private Button facebookLoginButton;
        readonly string[] firstNameSyllables = { "mon","fay","shi","zag","blarg","rash","izen"};
        readonly string[] lastNameSyllables = { "malo","zak","abo","wonk","zig","wolf","cat"};
        bool readyForDeviceLogin;

        void Start() {
            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        }

        new void Awake() {
            base.Awake();
            facebookLoginButton.onClick.AddListener(FacebookLogin);
            ConnectWithFacebook();
        }

        void OnEnable() {
            GS.GameSparksAvailable += GameSparksIsAvailable;
        }

        void OnDisable() {
            GS.GameSparksAvailable -= GameSparksIsAvailable;
        }

        private void GameSparksIsAvailable(bool connected) {
            //Debug.Log("LoginManager: GameSparksIsAvailable: connect: " + connected + " readyForDeviceLogin: "+ readyForDeviceLogin);
            if (connected && readyForDeviceLogin && !FB.IsLoggedIn) {
                DeviceLogin();
            } else if (connected) {
                readyForDeviceLogin = true;
            }
        }

        private void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) {
            Debug.Log("Firebase: Received Registration Token: " + token.Token);

            ManagePushNotifications(token.Token, "fcm");
        }

        private void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) {
            Debug.Log("Firebase: Received a new message from: " + e.Message.From);
        }

        // private void DidFinishRegisterForRemoteNotificationEvent (string _deviceToken, string _error)
        // {
        //     // print("Request to register for remote notification finished. Error = " + _error.GetPrintableString());
        //     print("DeviceToken = " + _deviceToken);

        //     ManagePushNotifications(_deviceToken, "ios");
        // }

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
                        } else {
                            Debug.Log("***** PushRegistration Successful: Device OS: " + deviceOS);

                            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                            customAttributes.Add("deviceOS", deviceOS);
                            AnalyticsManager.LogCustom("push_registration_request", customAttributes);
                        }
                    });
        }

        void DeviceLogin() {
            new DeviceAuthenticationRequest()
                .SetDisplayName(CreateNewPlayerName())
                .Send((response) => {
                    if (!response.HasErrors) {
                    UserManager.instance.UpdateGUI(response.DisplayName,response.UserId, null, null);
                        ChallengeManager.instance.GetChallenges();
                        //LeaderboardManager.instance.GetLeaderboard();

                        //Debug.Log("Device Authenticated...UserId: " + response.UserId);
                        //Debug.Log("DisplayName: " + response.DisplayName);
                        //Debug.Log("NewPlayer: " + response.NewPlayer);
                        //Debug.Log("SwitchSummary: " + response.SwitchSummary);

                        AnalyticsManager.LogCustom("device_authentication_request");
                    } else {
                        Debug.Log("***** Error Authenticating Device: " + response.Errors.JSON);
                        if (OnLoginError != null)
                            OnLoginError();

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
            Debug.Log("Connecting Facebook With GameSparks...");// first check if FB is ready, and then login //
            // if its not ready we just init FB and use the login method as the callback for the init method //
            if (!FB.IsInitialized) {
                Debug.Log("Initializing Facebook...");
                FB.Init(CheckFacebookLogin, null);
            } else {
                FB.ActivateApp();
                CheckFacebookLogin();
            }
        }

        void CheckFacebookLogin()
        {
            if (FB.IsInitialized) {
                FB.ActivateApp();
                if (FB.IsLoggedIn)
                {
                    Debug.Log("Already Logged into Facebook");
                    GameSparksFBLogin(AfterFBLogin);
                    facebookLoginButton.gameObject.SetActive(false);
                }
                else
                {
                    Debug.Log("Not Logged into Facebook: readyForDeviceLogin: " + readyForDeviceLogin);
                    facebookLoginButton.gameObject.SetActive(true);
                    facebookLoginButton.interactable = true;
                    //Invoke("DeviceLogin", 0.5f);
                    if (readyForDeviceLogin) {
                        DeviceLogin();    
                    } else {
                        readyForDeviceLogin = true;
                    }
                }
            } else {
                ConnectWithFacebook(); // if we are still not connected, then try to process again
            }
        }

        /// <summary>
        /// When Facebook is ready , this will connect the player to Facebook
        /// After the Player is authenticated it will  call the GS connect
        /// </summary>
        public void FacebookLogin()
        {
            facebookLoginButton.interactable = false;
            if (!FB.IsLoggedIn)
            {
                Debug.Log("Logging into Facebook");
                var perms = new List<string>(){"public_profile", "email", "user_friends"} ;
                FB.LogInWithReadPermissions(perms, GameSparksFBConnect);
            }
            else
            {
                GameSparksFBLogin(AfterFBLogin);
            }
        }

        void GameSparksFBConnect(ILoginResult result)
        {
            if(FB.IsLoggedIn)
            {
                Debug.Log("Logging into gamesparks with facebook details");

                AnalyticsManager.LogCustom("player_connects_with_facebook");
                GameSparksFBLogin(AfterFBLogin);
            }
            else
            {
                Debug.LogWarning("Something went wrong with connectin to FaceBook: " + result.Error);
                facebookLoginButton.interactable = true;

                if (OnLoginError != null) {
                    AnalyticsManager.LogError("gamesparks_fb_connect_error", result.Error);
                    OnLoginError();
                } else {
                    AnalyticsManager.LogCustom("gamesparks_fb_connect_decline");
                }
            }
        }

        //This is the callback that happens when gamesparks has been connected with FB
        private void AfterFBLogin(AuthenticationResponse response)
        {
            facebookLoginButton.gameObject.SetActive(false);
            UserManager.instance.UpdateInformation();
            ChallengeManager.instance.GetChallenges();
            FriendManager.instance.GetFriends();
            //LeaderboardManager.instance.GetLeaderboard();
        }

        //This method will connect GameSparks with FaceBook
        private void GameSparksFBLogin(FacebookLoginCallback _fbLoginCallback )
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
                    if(!response.HasErrors)
                    {
                        Debug.Log("Logged into gamesparks with facebook");
                        success = true;
                        _fbLoginCallback(response);
                    }
                    else
                    {
                        Debug.LogWarning("***** Error Logging into facebook: " + response.Errors.JSON);
                        facebookLoginButton.gameObject.SetActive(true);
                        facebookLoginButton.interactable = true;
                        if (OnLoginError != null)
                            OnLoginError();
                    }

                    AnalyticsManager.LogLogin("facebook", success);
                });
        }

        private string CreateNewPlayerName()
        {
            //Creates a first name with 2-3 syllables
            string firstName = "";
            int numberOfSyllablesInFirstName = UnityEngine.Random.Range(2, 4);
            for (int i = 0; i < numberOfSyllablesInFirstName; i++) {
                firstName += firstNameSyllables[UnityEngine.Random.Range(0, firstNameSyllables.Length)];
            }

            string firstNameLetter = "";
            firstNameLetter = firstName.Substring(0,1);
            firstName = firstName.Remove(0,1);
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
            lastNameLetter = lastName.Substring(0,1);
            lastName = lastName.Remove(0,1);
            lastNameLetter = lastNameLetter.ToUpper();
            lastName = lastNameLetter + lastName;

            //assembles the newly-created name
            return firstName + " " + lastName + Mathf.CeilToInt(UnityEngine.Random.Range(0f,9999f)).ToString();
        }
    }
}
