using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using Fabric.Answers;

namespace Fourzy
{
    public class LoginManager : MonoBehaviour {

        void Awake() {
            ConnectWithFacebook();  
        }

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
                Debug.Log("Something went wrong with FaceBook");
            }
        }

        //this is the callback that happens when gamesparks has been connected with FB
        private void AfterFBLogin(GameSparks.Api.Responses.AuthenticationResponse _resp)
        {
            Debug.Log(_resp.DisplayName );
            UserManager.instance.UpdateInformation();
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
                        Debug.Log("Error Logging into facebook");
                    }
                    Answers.LogLogin("facebook", success);
                });
        }
        #endregion
    }
}