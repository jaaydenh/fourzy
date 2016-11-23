using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Fourzy
{
    public class ActiveGame : MonoBehaviour
    {
        public delegate void GameActive(bool active);
        public static event GameActive OnActiveGame;

    	//We store the challengeId, next player's userId and the userId who initiated the challenge.
    	public string challengeId, nextPlayerId, challengerId;

    	//We create a list for playerNames and Ids
    	public List<string> playerNames = new List<string>();
    	public List<string> playerIds = new List<string>();
        public List<string> playerFacebookIds = new List<string>();

    	//This is the array of strings we pass our Cloud Code gameBoard to
        public int[] gameBoard;

    	public Text opponentNameLabel, statusLabel;
        public Image profilePicture;

        private GameObject UIScreen;
        private GameObject gameScreen;
        private int opponentIndex;

    	// Use this for initialization
    	void Start()
    	{
            UIScreen = GameObject.Find("UI Screen");

            if (playerIds[0] == UserManager.instance.userId)
            {
                opponentIndex = 1;
            } else {
                opponentIndex = 0;
            }

            opponentNameLabel.text = playerNames[opponentIndex];

    		//We then check if the userId of the next player is equal to ours
            if (nextPlayerId == UserManager.instance.userId)
    		{
    			//If it is, then we say it's your turn
    			statusLabel.text = "Your Turn!";
    		}
    		else
    		{
                statusLabel.text = "Their Turn!";
//                for (int i = 0; i < playerIds.Count; i++)
//    			{
//    				//else find the player whose Id does match and return their name
                //   if (playerIds[i] == nextPlayerId)
//    				{
//    					statusLabel.text = playerNames[i] + "'s Turn!";
//    				}
//    			}
    		}

            StartCoroutine(getFBPicture());
    	}

    	//Open game gets called OnClick of the play button
    	public void OpenGame()
    	{
            //Clear any previous instances of Fourzy GameManager
            GameManager.instance.CreateGameBoard();

			//Pass the gameBoard we got from Cloud Code to the Fourzy GameManager instance
            GameManager.instance.SetGameBoard(gameBoard);
            GameManager.instance.isMultiplayer = true;
            GameManager.instance.isNewChallenge = false;
			//Update the Fourzy gamemanager instance's challenge Id to the current one
            Debug.Log("challengeId: " + challengeId);
            GameManager.instance.challengeInstanceId = challengeId;

			//If we initiated the challenge, we get to be player 1
			if (challengerId == UserManager.instance.userId)
			{
                GameManager.instance.isPlayerOneTurn = true;
			} else {
                GameManager.instance.isPlayerOneTurn = false;
			}

			//If the user Id of the next player is equal to ours then it's out turn
            if (nextPlayerId == UserManager.instance.userId)
			{
                GameManager.instance.isCurrentPlayerTurn = true;
			} else {
                GameManager.instance.isCurrentPlayerTurn = false;
			}
                
            GameManager.instance.SetMultiplayerGameStatusText();

            UIScreen.SetActive(false);

            if (OnActiveGame != null)
                OnActiveGame(true);
    	}

        public IEnumerator getFBPicture()
        {
            //To get our facebook picture we use this address which we pass our facebookId into
            var www = new WWW("http://graph.facebook.com/" + playerFacebookIds[opponentIndex] + "/picture?width=210&height=210");

            yield return www;

            Texture2D tempPic = new Texture2D(25, 25);

            www.LoadImageIntoTexture(tempPic);
            Sprite tempSprite = Sprite.Create(tempPic, new Rect(0,0,tempPic.width, tempPic.height), new Vector2(0.5f, 0.5f));
            profilePicture.sprite = tempSprite;
        }
    }
}