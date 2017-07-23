using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Facebook.Unity;
using GameSparks.Api;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using UnityEngine.Analytics.Experimental;
using Firebase;
//using Fabric.Answers;

namespace Fourzy
{
    public class LoginManager : MonoBehaviour {

        public delegate void LoginError();
        public static event LoginError OnLoginError;

        private GameObject facebookButton;
        string[] firstNameSyllables = new string[] {"mon","fay","shi","zag","blarg","rash","izen"};
        string[] lastNameSyllables = new string[] {"malo","zak","abo","wonk","zig","wolf","cat"};

        void Start() {
            facebookButton = GameObject.Find("Facebook Button");
            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        }

        void Awake() {
            ConnectWithFacebook();
        }

        public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) {
            Debug.Log("Firebase: Received Registration Token: " + token.Token);

            ManagePushNotifications(token.Token, "fcm");
        }

        public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) {
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
                            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                            customAttributes.Add("errorJSON", response.Errors.JSON);
                            AnalyticsEvent.Custom("PushRegistrationRequest:Error: ", customAttributes);
                            //Answers.LogCustom("PushRegistrationRequest:Error: " + response.Errors.JSON);
                        } else {
                            Debug.Log("***** PushRegistration Successful: Device OS: " + deviceOS);
                            AnalyticsEvent.Custom("PushRegistrationRequest");
                            //Answers.LogCustom("PushRegistrationRequest");
                        }
                    });
        }
            
        void DeviceLogin() {
            new DeviceAuthenticationRequest()
                .SetDisplayName(CreateNewName())
                .Send((response) => {
                    if (!response.HasErrors) {
                        UserManager.instance.UpdateGUI(response.DisplayName,response.UserId, null);
                        ChallengeManager.instance.GetChallenges();
                        LeaderboardManager.instance.GetLeaderboard();
                        //Debug.Log("Device Authenticated...UserId: " + response.UserId);
                        //Debug.Log("DisplayName: " + response.DisplayName);
                        //Debug.Log("NewPlayer: " + response.NewPlayer);
                        //Debug.Log("SwitchSummary: " + response.SwitchSummary);
                        AnalyticsEvent.Custom("DeviceAuthenticationRequest");
                        //Answers.LogCustom("DeviceAuthenticationRequest");
                    } else {
                        Debug.Log("***** Error Authenticating Device: " + response.Errors.JSON);
                        if (OnLoginError != null)
                            OnLoginError();
                        Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                        customAttributes.Add("errorJSON", response.Errors.JSON);
                        AnalyticsEvent.Custom("DeviceAuthenticationRequest:Error: ", customAttributes);
                        //Answers.LogCustom("DeviceAuthenticationRequest:Error");
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
                    GSFacebookLogin(AfterFBLogin);
                    facebookButton.SetActive(false);
                }
                else
                {
                    Debug.Log("Not Logged into Facebook");
                    facebookButton.SetActive(true);
                    Invoke("DeviceLogin", 0.5f);
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
            if (!FB.IsLoggedIn)
            {
                Debug.Log("Logging into Facebook");
                var perms = new List<string>(){"public_profile", "email", "user_friends"} ;
                FB.LogInWithReadPermissions(perms, GameSparksFBConnect);
            }
            else
            {
                GSFacebookLogin(AfterFBLogin);
            }
        }

        void GameSparksFBConnect(ILoginResult result)
        {
            if(FB.IsLoggedIn)
            {
                Debug.Log("Logging into gamesparks with facebook details");
                AnalyticsEvent.Custom("PlayerConnectsWithFacebook");
                //Answers.LogCustom("PlayerConnectsWithFacebook");
                GSFacebookLogin(AfterFBLogin);
            }
            else
            {
                Debug.LogWarning("Something went wrong with connectin to FaceBook: " + result.Error);
                
                if (OnLoginError != null) {
                    AnalyticsEvent.Custom("GameSparksFBConnect:Error");
                    //Answers.LogCustom("GameSparksFBConnect:Error");
                    OnLoginError();
                } else {
                    AnalyticsEvent.Custom("GameSparksFBConnect:Decline");
                    //Answers.LogCustom("GameSparksFBConnect:Decline");
                }
            }
        }

        //this is the callback that happens when gamesparks has been connected with FB
        private void AfterFBLogin(GameSparks.Api.Responses.AuthenticationResponse _resp)
        {
            //Debug.Log(_resp.DisplayName );
            facebookButton.SetActive(false);
            UserManager.instance.UpdateInformation();
            ChallengeManager.instance.GetChallenges();
            FriendManager.instance.GetFriends();
            LeaderboardManager.instance.GetLeaderboard();
        }

        //delegate for asynchronous callbacks
        public delegate void FacebookLoginCallback(AuthenticationResponse _resp);

        //This method will connect GameSparks with FaceBook
        public void GSFacebookLogin(FacebookLoginCallback _fbLoginCallback )
        {
            Debug.Log("Sending FacebookConnectRequest using AccessToken: " + AccessToken.CurrentAccessToken.TokenString);
            bool success = false;

            new GameSparks.Api.Requests.FacebookConnectRequest()
                .SetAccessToken(AccessToken.CurrentAccessToken.TokenString)
                .SetMaxResponseTimeInSeconds(15)
                .SetMaxQueueTimeInSeconds(15)
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
                        facebookButton.SetActive(true);
                        if (OnLoginError != null)
                            OnLoginError();
                    }
                    Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                    customAttributes.Add("success", success);
                    AnalyticsEvent.Custom("FacebookLogin", customAttributes);
                    //Answers.LogLogin("facebook", success);
                });
        }

        string CreateNewName()
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
