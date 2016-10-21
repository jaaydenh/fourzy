using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GameSparks.Api.Requests;

namespace ConnectFour
{
public class GameInvite : MonoBehaviour 
{
	//ChallengeId is the important variable here, we use to reference the specific challenge we are playing
	public string inviteName, inviteExpiry, challengeId, facebookId;


	//We use canDestroy to let the Tween Alpha know it's safe to remove the gameObject OnFinish animating
	public bool canDestroy = false;

	public Text inviteNameLabel, inviteExpiryLabel;
	public Image profilePicture;

	// Use this for initialization
	void Start () 
	{
		inviteNameLabel.text = inviteName + "has invited you to play";
		inviteExpiryLabel.text = "Expires on " + inviteExpiry;
	}

	//This in the function we call OnClick
	public void AcceptChallenge()
	{
		//We set the Challenge Instance Id and Message of AcceptChallengeRequest
		new AcceptChallengeRequest().SetChallengeInstanceId(challengeId)
			.SetMessage("You're goin down!")
			.Send((response) =>
				{
					if (response.HasErrors)
					{
						Debug.Log(response.Errors);
					}
					else
					{
						//Since this challenge is no longer an invite, we need to update our running games
						ChallengeManager.instance.GetActiveChallenges();

						//Once we accept the challenge we can go ahead and remove the gameObject from the scene
						canDestroy = true;
					}
				});
	}

	public void DeclineChallenge()
	{
		//We set the Challenge Instance Id and Message of DeclineChallengeRequest
		new DeclineChallengeRequest().SetChallengeInstanceId(challengeId)
			.Send((response) =>
				{
					if (response.HasErrors)
					{
						Debug.Log(response.Errors);
					}
					else
					{
						//Once we decline the challenge we can go ahead and remove the gameObject from the scene
						canDestroy = true;
					}
				});
	}

	public IEnumerator getFBPicture()
	{
		//To get our facebook picture we use this address which we pass our facebookId into
		var www = new WWW("http://graph.facebook.com/" + facebookId + "/picture?width=210&height=210");

		yield return www;

		Texture2D tempPic = new Texture2D(25, 25);

		www.LoadImageIntoTexture(tempPic);
		Sprite tempSprite = Sprite.Create(tempPic, new Rect(0,0,tempPic.width, tempPic.height), new Vector2(0.5f, 0.5f));
		profilePicture.sprite = tempSprite;
	}


	public void DestroyAfterTween()
	{
		if (canDestroy)
		{
			//First remove the gameObject from it's list, so we don't end up with a null reference when we destroy it
			ChallengeManager.instance.gameInvites.Remove(gameObject);

			//Then destroy the gameObject, removing it from the scene.
			Destroy(gameObject);
		}
	}
}
}