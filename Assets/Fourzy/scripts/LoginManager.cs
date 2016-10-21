using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using GameSparks.Api.Requests;

namespace ConnectFour
{
public class LoginManager : MonoBehaviour {

	void Awake () {
		CallFBInit ();
	}

//	void Awake () {
//		CallFBInit ();
//	}
	
	private void CallFBInit() {
		if (!FB.IsInitialized) {
			FB.Init (InitCallback, OnHideUnity);
		} else {
			FB.ActivateApp ();
			UserManager.instance.UpdateInformation();
		}
	}

	private void InitCallback() {
		Debug.Log("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
		if (FB.IsInitialized) {
			FB.ActivateApp ();
			CallFBLogin ();
		} else {
			Debug.Log("Failed to Initialize the Facebook SDK");
		}
	}

	private void OnHideUnity(bool isGameShown) {
		Debug.Log("Is game showing? " + isGameShown);
		if (!isGameShown) {
			Time.timeScale = 0;
		} else {
			Time.timeScale = 1;
		}
	}

	private void CallFBLogin() {
		IEnumerable<string> perms = new List<string>() { "public_profile","email","user_friends" };
		FB.LogInWithReadPermissions (perms, AuthCallback);
	}

	private void AuthCallback(ILoginResult result) {
		if (FB.IsLoggedIn) {
			AccessToken aToken = AccessToken.CurrentAccessToken;
			new FacebookConnectRequest().SetAccessToken(aToken.TokenString).Send((response) =>
				{
					if (response.HasErrors)
					{
						Debug.Log("Something failed when connecting with Facebook " + response.Errors.JSON);
					}
					else
					{
						Debug.Log("Gamesparks Facebook Login Successful");
						UserManager.instance.UpdateInformation();
					}
				});
		}
	}
}
}