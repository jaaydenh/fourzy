using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GameSparks.Api.Requests;

namespace Fourzy
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
    						Debug.Log("***** Error Accepting Challenge Request: " + response.Errors.JSON);
    					}
    					else
    					{
    						//Since this challenge is no longer an invite, we need to update our running games
    						ChallengeManager.instance.GetChallenges();

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
    						Debug.Log("***** Error Declining Challenge Request: " + response.Errors.JSON);
    					}
    					else
    					{
    						//Once we decline the challenge we can go ahead and remove the gameObject from the scene
    						canDestroy = true;
    					}
    				});
    	}
    }
}