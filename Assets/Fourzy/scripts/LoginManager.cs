﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using GameSparks.Api;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
//using Fabric.Answers;
using VoxelBusters.NativePlugins;
using VoxelBusters.Utility;

namespace Fourzy
{
    public class LoginManager : MonoBehaviour {

        private GameObject facebookButton;
        string[] firstNameSyllables = new string[] {"mon","fay","shi","zag","blarg","rash","izen"};
        string[] lastNameSyllables = new string[] {"malo","zak","abo","wonk"};

        void Start() {
            facebookButton = GameObject.Find("Facebook Button");
        }

        void Awake() {
            ConnectWithFacebook();
            NPBinding.NotificationService.RegisterNotificationTypes(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);
        }

        private void OnEnable()
        {
            //Triggered when registration for remote notification event is done.
            NotificationService.DidFinishRegisterForRemoteNotificationEvent += DidFinishRegisterForRemoteNotificationEvent;
        }

        private void OnDisable()
        {
            NotificationService.DidFinishRegisterForRemoteNotificationEvent -= DidFinishRegisterForRemoteNotificationEvent;
        }

        private void DidFinishRegisterForRemoteNotificationEvent (string _deviceToken, string _error)
        {
            print("Request to register for remote notification finished. Error = " + _error.GetPrintableString());
            print("DeviceToken = " + _deviceToken);

            ManagePushNotifications(_deviceToken);
        }

        private void ManagePushNotifications(string token)
        {       
            //string deviceToken = System.BitConverter.ToString(token).Replace('-', '').ToLower ();

            new PushRegistrationRequest().SetPushId(token)
                .Send((response) =>
                    {
                        if (response.HasErrors)
                        {
                            Debug.Log(response.Errors);
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

        void DeviceLogin() {
            new DeviceAuthenticationRequest()
                .SetDisplayName(CreateNewName())
                .Send((response) => {
                    if (!response.HasErrors) {
                        UserManager.instance.UpdateGUI(response.DisplayName,response.UserId, null);

                        NPBinding.NotificationService.RegisterForRemoteNotifications();
                        ChallengeManager.instance.GetActiveChallenges();
                        //Debug.Log("Device Authenticated...UserId: " + response.UserId);
                        //Debug.Log("DisplayName: " + response.DisplayName);
                        //Debug.Log("NewPlayer: " + response.NewPlayer);
                        //Debug.Log("SwitchSummary: " + response.SwitchSummary);
                    } else {
                        Debug.Log("Error Authenticating Device: " + response.Errors.JSON);
                    }
                });
        }

        void CheckFacebookLogin()
        {
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
        }

        /// <summary>
        /// When Facebook is ready , this will connect the pleyer to Facebook
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
                GSFacebookLogin(AfterFBLogin);
            }
            else
            {
                Debug.LogWarning("Something went wrong with FaceBook: " + result.Error);
            }
        }

        //this is the callback that happens when gamesparks has been connected with FB
        private void AfterFBLogin(GameSparks.Api.Responses.AuthenticationResponse _resp)
        {
            //Debug.Log(_resp.DisplayName );
            facebookButton.SetActive(false);
            UserManager.instance.UpdateInformation();
            NPBinding.NotificationService.RegisterForRemoteNotifications();
            ChallengeManager.instance.GetActiveChallenges();
            FriendManager.instance.GetFriends();
        }

        //delegate for asynchronous callbacks
        public delegate void FacebookLoginCallback(AuthenticationResponse _resp);

        //This method will connect GameSparks with FaceBook
        public void GSFacebookLogin(FacebookLoginCallback _fbLoginCallback )
        {
            //bool success = false;
            new GameSparks.Api.Requests.FacebookConnectRequest()
                .SetAccessToken(AccessToken.CurrentAccessToken.TokenString)
                .SetDoNotLinkToCurrentPlayer(false)// we don't want to create a new account so link to the player that is currently logged in
                .SetSwitchIfPossible(true)//this will switch to the player with this FB account id if they already have an account from a separate login
                .SetSyncDisplayName(true)
                .Send((response) => {
                    if(!response.HasErrors)
                    {
                        Debug.Log("Logged into gamesparks with facebook");
                        //success = true;
                        _fbLoginCallback(response);
                    }
                    else
                    {
                        Debug.LogWarning("Error Logging into facebook: " + response.Errors.JSON);
                    }
                    //Answers.LogLogin("facebook", success);
                });
        }

        string CreateNewName()
        {
            //Creates a first name with 2-3 syllables
            string firstName = "";
            int numberOfSyllablesInFirstName = Random.Range(2, 4);
            for (int i = 0; i < numberOfSyllablesInFirstName; i++) {
                firstName += firstNameSyllables[Random.Range(0, firstNameSyllables.Length)];
            }

            string firstNameLetter = "";
            firstNameLetter = firstName.Substring(0,1);
            firstName = firstName.Remove(0,1);
            firstNameLetter = firstNameLetter.ToUpper();
            firstName = firstNameLetter + firstName;

            //Creates a last name with 1-2 syllables
            string lastName = "";
            int numberOfSyllablesInLastName = Random.Range(1, 3);
            for (int j = 0; j < numberOfSyllablesInLastName; j++)
            {
                lastName += lastNameSyllables[Random.Range(0, lastNameSyllables.Length)];
            }
            string lastNameLetter = "";
            lastNameLetter = lastName.Substring(0,1);
            lastName = lastName.Remove(0,1);
            lastNameLetter = lastNameLetter.ToUpper();
            lastName = lastNameLetter + lastName;

            //assembles the newly-created name
            return firstName + " " + lastName + Random.Range(0f,9999f).ToString();
        }
    }
}
