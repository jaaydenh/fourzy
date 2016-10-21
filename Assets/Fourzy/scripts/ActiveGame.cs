using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace ConnectFour
{
public class ActiveGame : MonoBehaviour
{

	//We store the challengeId, next player's userId and the userId who initiated the challenge.
	public string challengeId, turnStatus, challengerId;

	//We create a list for playerNames and Ids
	public List<string> playerNames = new List<string>();
	public List<string> playerIds = new List<string>();

	//This is the array of strings we pass our Cloud Code gameBoard to
    public int[] gameBoard;

	//We use canDestroy to let the Tween Alpha know it's safe to remove the gameObject OnFinish animating
	public bool canDestroy = false;

	public Text challengeLabel, statusLabel;

	// Use this for initialization
	void Start()
	{
		//We set the challenge label with the names of both players
		challengeLabel.text = playerNames[0] + " VS " + playerNames[1];

		//We then check if the userId of the next player is equal to ours
		if (turnStatus == UserManager.instance.userId)
		{
			//If it is, then we say it's your turn
			statusLabel.text = "Your Turn!";
		}
		else
		{
			for (int i = 0; i < playerIds.Count; i++)
			{
				//else find the player whose Id does match and return their name
				if (playerIds[i] == turnStatus)
				{
					statusLabel.text = playerNames[i] + "'s Turn!";
				}
			}
		}
	}

	//Start game gets called OnClick of the play button
	public void StartGame()
	{
			//Clear any previous instances of Fourzy GameManager
            GameManager.instance.ClearInstance();

			//Pass the gameBoard we got from Cloud Code to the Fourzy GameManager instance
            GameManager.instance.SetGameBoard(gameBoard);
            GameManager.instance.isMultiplayer = true;

			//Update the Fourzy gamemanager instance's challenge Id to the current one
            Debug.Log("challengeId: " + challengeId);
            GameManager.instance.challengeInstanceId = challengeId;

			//If we initiated the challenge, we get to be player 1
			if (challengerId == UserManager.instance.userId)
			{
                GameManager.instance.isPlayerOneTurn = true;
			}
			//Otherwise we're player 2
			else
			{
                GameManager.instance.isPlayerOneTurn = false;
			}

			//If the user Id of the next player is equal to ours then it's out turn
//			if (turnStatus == UserManager.instance.userId)
//			{
//                GameManager.instance.isCurrentPlayerTurn = true;
//			}
//			//Otherwise it's not
//			else
//			{
//                GameManager.instance.isCurrentPlayerTurn = false;
//			}

			//We've passed enough information for the Tic Tac Toe instance to construct the board
            //GameManager.instance.GetBoard();

			//We've done everything we can do with the Running Game Entry, so we can remove it from the scene.
			canDestroy = true;
	}

	//When our tween is finished and we've accepted or declined the invite, we no longer need it
//	public void DestroyAfterTween()
//	{
//		if (canDestroy)
//		{
//			//First remove the gameObject from it's list, so we don't end up with a null reference when we destroy it
//			ChallengeManager.instance.gameInvites.Remove(gameObject);
//
//			//Then destroy the gameObject, removing it from the scene.
//			Destroy(gameObject);
//		}
//	}
}
}