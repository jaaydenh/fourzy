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
        public delegate void GameActive();
        public static event GameActive OnActiveGame;

        public delegate void RemoveGame(string challengeInstanceId);
        public static event RemoveGame OnRemoveGame;

    	//We store the challengeId, next player's userId and the userId who initiated the challenge.
        public string challengeId, nextPlayerId, challengerId, winnerName, winnerId, challengeState, challengeShortCode;

    	//We create a list for playerNames and Ids
    	public List<string> playerNames = new List<string>();
    	public List<string> playerIds = new List<string>();
        public List<string> playerFacebookIds = new List<string>();

    	//This is the array of strings we pass our Cloud Code gameBoard to
        public int[] gameBoard;
        public int[] lastGameBoard;
        public int[] tokenBoardData;

        public TokenBoard tokenBoard;

        public List<GSData> moveList;

    	public Text opponentNameLabel, statusLabel, moveTimeAgo;
        public Image opponentProfilePicture;
        public Texture2D defaultProfilePicture;
        public Image tournamentIcon;

        //private GameObject UIScreen;
        private GameObject gameScreen;
        private GameObject deleteGameButton;

        private int opponentIndex;
        public bool isCurrentPlayerTurn = false;
        public bool viewedResult = false;
        public bool isExpired = false;

        private Sprite opponentProfilePictureSprite;

    	void Start()
    	{
            //UIScreen = GameObject.Find("UI Screen");

            if (challengeShortCode == "tournamentChallenge") {
                tournamentIcon.enabled = true;
            } else {
                tournamentIcon.enabled = false;
            }

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

                string fbID = playerFacebookIds[opponentIndex];
                if (fbID != null) {
                    StartCoroutine(UserManager.instance.GetFBPicture(playerFacebookIds[opponentIndex], (sprite)=>
                        {
                            opponentProfilePictureSprite = sprite;
                            opponentProfilePicture.sprite = sprite;
                        }));
                }
            }
            else
            {
                opponentNameLabel.text = "Waiting for Opponent";
                opponentProfilePicture.sprite = Sprite.Create(defaultProfilePicture, 
                    new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height), 
                    new Vector2(0.5f, 0.5f));
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
            } else if (isExpired == true) {
                statusLabel.text = "Game Expired";
            } else {
                //We then check if the userId of the next player is equal to ours
                if (nextPlayerId == UserManager.instance.userId)
                {
                    statusLabel.text = "Your Move!";
                } else {
                    statusLabel.text = "Their Move!";
                }
            }

            GSData lastPlayerMove = moveList.LastOrDefault();
            long timestamp = lastPlayerMove.GetLong("timestamp").GetValueOrDefault();

            if (timestamp != 0) {
                System.DateTime timestampDateTime = new System.DateTime (1970, 1, 1, 0, 0, 0,System.DateTimeKind.Utc).AddMilliseconds(timestamp).ToLocalTime();
                moveTimeAgo.text = TimeAgo.DateTimeExtensions.TimeAgo(timestampDateTime);                
            }
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
            //GameManager.instance.tokenBoard = new TokenBoard(tokenBoard);

            GameManager.instance.opponentProfilePicture.sprite = opponentProfilePictureSprite;
            GameManager.instance.opponentNameLabel.text = opponentNameLabel.text;
            GameManager.instance.playerNameLabel.text = UserManager.instance.userName;
            if (UserManager.instance.profilePicture) {
                GameManager.instance.playerProfilePicture.sprite = UserManager.instance.profilePicture;    
            } else {
                GameManager.instance.playerProfilePicture.sprite = Sprite.Create(defaultProfilePicture, 
                    new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height), 
                    new Vector2(0.5f, 0.5f));
            }

            GameManager.instance.isMultiplayer = true;
            GameManager.instance.isNewChallenge = false;
            GameManager.instance.challengeInstanceId = challengeId;

            Debug.Log("Open Active Game: challengeInstanceId: " + challengeId);
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
            //TokenBoard tokenBoard1 = new TokenBoard(tokenBoard);
			//Pass the gameBoard we got from Cloud Code to the Fourzy GameManager instance


            if (moveList != null && moveList.Count > 0)
            {
                GSData lastPlayerMove = moveList.LastOrDefault();
                int lastPlayer = lastPlayerMove.GetInt("player").GetValueOrDefault(0);
                Debug.Log("ActiveGame:lastPlayer: " + lastPlayer);
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
            
            GameManager.instance.SetupGameWrapper(lastGameBoard, tokenBoard, moveList);
            GameManager.instance.PopulateMoveArrows();
            StartCoroutine(GameManager.instance.CreateTokens());

            GameManager.instance.ResetUI();

            if (!viewedResult && challengeState == "COMPLETE") {
                ChallengeManager.instance.SetViewedCompletedGame(challengeId);
            }

            // UIScreen.SetActive(false);

            if (OnActiveGame != null)
                OnActiveGame();
    	}
    }
}