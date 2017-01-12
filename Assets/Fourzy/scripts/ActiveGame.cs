using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GameSparks.Core;
using System.Linq;

namespace Fourzy
{
    public class ActiveGame : MonoBehaviour
    {
        public delegate void GameActive(bool active);
        public static event GameActive OnActiveGame;

        public delegate void RemoveGame(string challengeInstanceId);
        public static event RemoveGame OnRemoveGame;

    	//We store the challengeId, next player's userId and the userId who initiated the challenge.
    	public string challengeId, nextPlayerId, challengerId, winnerName, winnerId, challengeState;

    	//We create a list for playerNames and Ids
    	public List<string> playerNames = new List<string>();
    	public List<string> playerIds = new List<string>();
        public List<string> playerFacebookIds = new List<string>();

    	//This is the array of strings we pass our Cloud Code gameBoard to
        public int[] gameBoard;

        public List<GSData> moveList;

    	public Text opponentNameLabel, statusLabel;
        public Image opponentProfilePicture;

        private GameObject UIScreen;
        private GameObject gameScreen;
        private GameObject deleteGameButton;

        private int opponentIndex;
        public bool isCurrentPlayerTurn = false;

        private Sprite opponentProfilePictureSprite;

    	void Start()
    	{
            UIScreen = GameObject.Find("UI Screen");

            if (playerIds.Count > 1)
            {
                if (playerIds[0] == UserManager.instance.userId)
                {
                    opponentIndex = 1;
                }
                else
                {
                    opponentIndex = 0;
                }

                opponentNameLabel.text = playerNames[opponentIndex];
            }
            else
            {
                opponentNameLabel.text = "Waiting for Opponent";
            }

            if (winnerId != null)
            {
                if (winnerId == UserManager.instance.userId)
                {
                    statusLabel.text = "You won!";
                }
                else
                {
                    statusLabel.text = "They won!";
                }
            }
            else
            {
                //We then check if the userId of the next player is equal to ours
                if (nextPlayerId == UserManager.instance.userId)
                {
                    //If it is, then we say it's your turn
                    statusLabel.text = "Your Move!";
                }
                else
                {
                    statusLabel.text = "Their Move!";
                    //                for (int i = 0; i < playerIds.Count; i++)
                    //              {
                    //                  //else find the player whose Id does match and return their name
                    //   if (playerIds[i] == nextPlayerId)
                    //                  {
                    //                      statusLabel.text = playerNames[i] + "'s Turn!";
                    //                  }
                    //              }
                }
            }

            StartCoroutine(UserManager.instance.GetFBPicture(playerFacebookIds[opponentIndex], (sprite)=>
                {
                    opponentProfilePictureSprite = sprite;
                    opponentProfilePicture.sprite = sprite;
                }));
    	}

        private void OnEnable()
        {
            GamesListManager.OnShowDeleteGame += ShowDeleteButton;

            deleteGameButton = transform.Find("DeleteGameButton").gameObject;
            deleteGameButton.SetActive(false);
        }

        private void ShowDeleteButton(bool show) {
            if (challengeState == "COMPLETE" && deleteGameButton != null)
            {
                if (show)
                {
                    deleteGameButton.SetActive(true);
                }
                else
                {
                    deleteGameButton.SetActive(false);
                }
            }
        }

        public void DeleteGame() {
            if (OnRemoveGame != null)
            {
                OnRemoveGame(challengeId);
                gameObject.SetActive(false);
            }
        }

    	//Open game gets called OnClick of the play button which happens when pressing or clicking an active game in the games list
        public void OpenGame()
    	{
            GameManager.instance.opponentProfilePictureSprite = opponentProfilePictureSprite;
            GameManager.instance.opponentNameLabel.text = opponentNameLabel.text;
            GameManager.instance.isMultiplayer = true;
            GameManager.instance.isNewChallenge = false;
            GameManager.instance.challengeInstanceId = challengeId;
            GameManager.instance.winner = winnerName;
            GameManager.instance.isCurrentPlayerTurn = isCurrentPlayerTurn;

//            //If the user Id of the next player is equal to the current player then it is the current player's turn
//            if (nextPlayerId == UserManager.instance.userId)
//            {
//                GameManager.instance.isCurrentPlayerTurn = true;
//            } else {
//                GameManager.instance.isCurrentPlayerTurn = false;
//            }
                
            GameManager.instance.ResetGameBoard();
			//Pass the gameBoard we got from Cloud Code to the Fourzy GameManager instance
            GameManager.instance.SetupGame(gameBoard);
            //StartCoroutine(GameManager.instance.SetGameBoard(gameBoard));
            //print("called SetGameBoard");

            if (moveList != null && moveList.Count > 0)
            {
                GSData lastPlayerMove = moveList.LastOrDefault();
                int lastPlayer = lastPlayerMove.GetInt("player").GetValueOrDefault(0);

                if (lastPlayer == 0 || lastPlayer == 2)
                {
                    GameManager.instance.isPlayerOneTurn = true;
                }
                else
                {
                    GameManager.instance.isPlayerOneTurn = false;
                }
            } else {
                GameManager.instance.isPlayerOneTurn = true;
            }
                
//            GameManager.instance.SetMultiplayerGameStatusText();
            GameManager.instance.ResetUI();

            UIScreen.SetActive(false);

            if (OnActiveGame != null)
                OnActiveGame(true);
    	}
    }
}