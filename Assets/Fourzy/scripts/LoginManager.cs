﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using Fabric.Answers;
using VoxelBusters.NativePlugins;
using VoxelBusters.Utility;

namespace Fourzy
{
    public class LoginManager : MonoBehaviour {

        void Awake() {
            ConnectWithFacebook();
            //NPBinding.NotificationService.RegisterNotificationTypes(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);
        }

        private void OnEnable()
        {
            //Triggered when registration for remote notification event is done.
            //NotificationService.DidFinishRegisterForRemoteNotificationEvent += DidFinishRegisterForRemoteNotificationEvent;
        }

        private void OnDisable()
        {
            //NotificationService.DidFinishRegisterForRemoteNotificationEvent -= DidFinishRegisterForRemoteNotificationEvent;
        }

//        private void DidFinishRegisterForRemoteNotificationEvent (string _deviceToken, string _error)
//        {
//            print("Request to register for remote notification finished. Error = " + _error.GetPrintableString());
//            print("DeviceToken = " + _deviceToken);
//
//            ManagePushNotifications(_deviceToken);
//        }

//        private void ManagePushNotifications(string token)
//        {       
//            //string deviceToken = System.BitConverter.ToString(token).Replace('-', '').ToLower ();
//
//            new PushRegistrationRequest().SetPushId(token)
//                .Send((response) =>
//                    {
//                        if (response.HasErrors)
//                        {
//                            Debug.Log(response.Errors);
//                        }
//                    });
//        }

        #region FaceBook Authentication
        /// <summary>
        /// Below we will login with facebook.
        /// When FB is ready we will call the method that allows GS to connect to GameSparks
        /// </summary>
        public void ConnectWithFacebook()
        {
            if(!FB.IsInitialized)
            {
                Debug.Log("Initializing Facebook");
                FB.Init(FacebookLogin);
            }
            else
            {
                FacebookLogin();
            }
        }

        /// <summary>
        /// When Facebook is ready , this will connect the pleyer to Facebook
        /// After the Player is authenticated it will  call the GS connect
        /// </summary>
        void FacebookLogin()
        {
            if (!FB.IsLoggedIn)
            {
                Debug.Log("Logging into Facebook");
                FB.LogInWithReadPermissions(
                    new List<string>() { "public_profile", "email", "user_friends" },
                    GameSparksFBConnect
                );
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
                Debug.Log("Something went wrong with FaceBook: " + result.Error);
            }
        }

        //this is the callback that happens when gamesparks has been connected with FB
        private void AfterFBLogin(GameSparks.Api.Responses.AuthenticationResponse _resp)
        {
            Debug.Log(_resp.DisplayName );
            UserManager.instance.UpdateInformation();
            //NPBinding.NotificationService.RegisterForRemoteNotifications();
            ChallengeManager.instance.GetActiveChallenges();
        }

        //delegate for asynchronous callbacks
        public delegate void FacebookLoginCallback(AuthenticationResponse _resp);

        //This method will connect GameSparks with FaceBook
        public void GSFacebookLogin(FacebookLoginCallback _fbLoginCallback )
        {
            bool success = false;
            new GameSparks.Api.Requests.FacebookConnectRequest()
                .SetAccessToken(AccessToken.CurrentAccessToken.TokenString)
                .Send((response) => {
                    if(!response.HasErrors)
                    {
                        Debug.Log("Logged into gamesparks with facebook");
                        success = true;
                        _fbLoginCallback(response);
                    }
                    else
                    {
                        Debug.Log("Error Logging into facebook: " + response.Errors.ToString());
                    }
                    Answers.LogLogin("facebook", success);
                });
        }
        #endregion
    }
}