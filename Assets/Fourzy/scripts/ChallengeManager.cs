using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameSparks.Api.Requests;
using GameSparks.Core;
using System.Linq;

namespace Fourzy
{
    
    public class ChallengeManager : MonoBehaviour
	{
        public delegate void GameActive(bool active);
        public static event GameActive OnActiveGame;

        public static ChallengeManager instance;

		public GameObject yourMoveGameGrid;
        public GameObject theirMoveGameGrid;
        public GameObject completedGameGrid;
		public GameObject activeGamePrefab;

		public List<GameObject> activeGames = new List<GameObject>();

		public GameObject inviteGrid;
		public GameObject invitePrefab;
		public List<GameObject> gameInvites = new List<GameObject>();

        public GameObject NoMovesPanel;

        private GameObject UIScreen;

        private bool gettingChallenges = false;

        //private int yourMoveGames = 0;

		void Start()
		{
			instance = this;

            UIScreen = GameObject.Find("UI Screen");
		}

		//This function accepts a string of UserIds and invites them to a new challenge
        public void ChallengeUser(string userId, List<long> gameBoard, int position, Fourzy.GameManager.Direction direction)
		{
            //CreateChallengeRequest takes a list of UserIds because you can challenge more than one user at a time
            List<string> gsId = new List<string>();
            //Add our friends UserId to the list
            gsId.Add(userId);

            GSRequestData data = new GSRequestData().AddNumberList("gameBoard", gameBoard);
            data.AddNumber("position", position);
            data.AddNumber("direction", (int)direction);
            // always player 1 plays first
            data.AddNumber("player", 1);
            //we use CreateChallengeRequest with the shortcode of our challenge, we set this in our GameSparks Portal
            new CreateChallengeRequest().SetChallengeShortCode("chalRanked")
            .SetUsersToChallenge(gsId) //We supply the userIds of who we wish to challenge
			.SetEndTime(System.DateTime.Today.AddDays(15)) //We set a date and time the challenge will end on
			.SetChallengeMessage("I've challenged you to Fourzy!") // We can send a message along with the invite
            .SetScriptData(data)
            .Send((response) => 
				{
					if (response.HasErrors)
					{
						Debug.Log(response.Errors);
					}
					else
					{
                        GameManager.instance.challengeInstanceId = response.ChallengeInstanceId;
					}
				});
		}

        public void FindRandomChallenge() {

            new FindChallengeRequest()
                .SetAccessType("PUBLIC")
                .SetCount(50)
                //.SetEligibility()
                //.SetOffset()
                //.SetShortCode()
                .Send((response) => {

                    if (response.HasErrors)
                    {
                        Debug.Log(response.Errors);
                    } else {
                        GSEnumerable<GameSparks.Api.Responses.FindChallengeResponse._Challenge> challengeInstances = response.ChallengeInstances; 
                        //GSData scriptData = response.ScriptData; 

                        if (challengeInstances.Count() > 0) {
                            List<string> challengeInstanceIds = new List<string>();

                            //for every object in the challenges array, get the challengeId field and push to challengeInstanceId[]
                            foreach (var instance in challengeInstances)
                            {
                                challengeInstanceIds.Add(instance.ChallengeId);
                            }

                            int randNum = Random.Range(0, challengeInstanceIds.Count-1);

                            //reference the id at that random numbers location
                            string randomChallengeId = challengeInstanceIds[randNum];
                            //each time you run this code, a different id is set in the scriptdata
                            //Spark.setScriptData("challenge to join", randomChallengeId);
                            Debug.Log("JoinChallenge: challengeId: " + randomChallengeId);
                            JoinChallenge(randomChallengeId);
                        } else {

                            //Send player to Game Screen to make the first move
                            
                            OpenNewGame();
                        }
                    }
                });
        }

        public void OpenNewGame()
        {
            GameManager.instance.ResetGameBoard();

            GameManager.instance.isMultiplayer = true;
            //If we initiated the challenge, we get to be player 1
            GameManager.instance.isPlayerOneTurn = true;
            GameManager.instance.isCurrentPlayerTurn = true;
            GameManager.instance.isNewChallenge = true;
            GameManager.instance.challengeInstanceId = null;

            GameManager.instance.UpdateGameStatusText();

            UIScreen.SetActive(false);

            if (OnActiveGame != null)
                OnActiveGame(true);
        }

        public void ChallengeRandomUser(List<long> gameBoard, int position, Fourzy.GameManager.Direction direction)
        {
            GSRequestData data = new GSRequestData().AddNumberList("gameBoard", gameBoard);
            data.AddNumber("position", position);
            data.AddNumber("direction", (int)direction);
            // always player 1 plays first
            data.AddNumber("player", 1);
            //we use CreateChallengeRequest with the shortcode of our challenge, we set this in our GameSparks Portal
            new CreateChallengeRequest().SetChallengeShortCode("chalRanked")
                .SetAccessType("PUBLIC")
                //.SetAutoStartJoinedChallengeOnMaxPlayers(true)
                //.SetMaxPlayers(2)
                //.SetMinPlayers(1)
                .SetEndTime(System.DateTime.Today.AddDays(15)) //We set a date and time the challenge will end on
                .SetChallengeMessage("I've challenged you to Fourzy!") // We can send a message along with the invite
                .SetScriptData(data)
                .Send((response) => 
                    {
                        if (response.HasErrors)
                        {
                            Debug.Log(response.Errors);
                        }
                        else
                        {
                            GameManager.instance.challengeInstanceId = response.ChallengeInstanceId;
                        }
                    });
        }

        public void JoinChallenge(string challengeInstanceId) {
            new JoinChallengeRequest()
                .SetChallengeInstanceId(challengeInstanceId)
                .Send((response) => {
                    if (response.HasErrors)
                    {
                        Debug.Log(response.Errors);
                    }
                    else
                    {
                        GameManager.instance.challengeInstanceId = challengeInstanceId;
                        //Send Player to Game Screen to make a move
                        GetChallenge(challengeInstanceId);
                    }
                });
        }

        public void GetChallenge(string challengeInstanceId) {
            new GetChallengeRequest()
                .SetChallengeInstanceId(challengeInstanceId)
                //.SetMessage(message)
                .Send((response) => {
                    var challenge = response.Challenge; 
                    GSData scriptData = response.ScriptData;
                    OpenGame(challenge);
                });
        }
            
        public void OpenGame(GameSparks.Api.Responses.GetChallengeResponse._Challenge challenge)
        {
            //GameManager.instance.opponentProfilePictureSprite = opponentProfilePictureSprite;
            //GameManager.instance.opponentNameLabel.text = opponentNameLabel.text;

            GameManager.instance.isMultiplayer = true;
            GameManager.instance.isNewChallenge = false;
            GameManager.instance.challengeInstanceId = challenge.ChallengeId;
            GameManager.instance.winner = challenge.ScriptData.GetString("winnerName");

            GameManager.instance.isCurrentPlayerTurn = true;

            GameManager.instance.ResetGameBoard();
            
            List<int> boardData = challenge.ScriptData.GetIntList("gameBoard");
            if (boardData != null) {
                int[] gameboard = challenge.ScriptData.GetIntList("gameBoard").ToArray();

                GameManager.instance.SetupGame(gameboard);
            }

            List<GSData> moveList = challenge.ScriptData.GetGSDataList("moveList");
            
            GSData lastPlayerMove = moveList.LastOrDefault();
            int lastPlayer = lastPlayerMove.GetInt("player").GetValueOrDefault(0);

            if (lastPlayer == 0 || lastPlayer == 2)
            {
                GameManager.instance.isPlayerOneTurn = true;
            } else {
                GameManager.instance.isPlayerOneTurn = false;
            }
                            
            GameManager.instance.ResetUI();

            UIScreen.SetActive(false);

            if (OnActiveGame != null)
                OnActiveGame(true);
        }

//		public void GetChallengeInvites()
//		{
//			//Every time we call GetChallengeInvites we'll refresh the list
//			for (int i = 0; i < gameInvites.Count; i++)
//			{
//				//Destroy all gameInvite gameObjects currently in the scene
//				Destroy(gameInvites[i]);
//			}
//			//Clear the list of gameInvites so we don't have null reference errors
//			gameInvites.Clear();
//
//			//We send a ListChallenge Request with the shortcode of our challenge, we set this in our GameSparks Portal
//            new ListChallengeRequest().SetShortCode("chalRanked")
//				.SetState("RECEIVED") //We only want to get games that we've received
//				.SetEntryCount(50) //We want to pull in the first 50 we find
//				.Send((response) =>
//					{
//						//For every challenge we get
//						foreach (var challenge in response.ChallengeInstances)
//						{
//							//Create a new gameObject, add invitePrefab as a child of the invite Grid GameObject
//							//GameObject go = NGUITools.AddChild(inviteGrid.gameObject, invitePrefab);
//							GameObject go = Instantiate(invitePrefab) as GameObject;
//							go.gameObject.transform.SetParent(inviteGrid.transform);
//
//							//Update all the gameObject's variables
//                            GameInvite gameInvite = go.GetComponent<GameInvite>();
//                            gameInvite.challengeId = challenge.ChallengeId;
//                            gameInvite.inviteName = challenge.Challenger.Name;
//                            gameInvite.inviteExpiry = challenge.EndDate.ToString();
//
//							//Add the gameObject to the list of friends
//							gameInvites.Add(go);
//
//							//Tell the grid to reposition everything nicely
//							//inviteGrid.Reposition();
//						}
//					});
//		}

		public void GetActiveChallenges()
		{
            //yourMoveGames = 0;

            if (!gettingChallenges)
            {
                gettingChallenges = true;

                //Every time we call GetChallengeInvites we'll refresh the list
                for (int i = 0; i < activeGames.Count; i++)
                {
                    //Destroy all runningGame gameObjects currently in the scene
                    Destroy(activeGames[i]);
                }

                activeGames.Clear();
                List<string> challengeStates = new List<string> {"RUNNING","COMPLETE","ISSUED"};

                //We send a ListChallenge Request with the shortcode of our challenge, we set this in our GameSparks Portal
                new ListChallengeRequest().SetShortCode("chalRanked")
                    .SetStates(challengeStates)
                    .SetEntryCount(50) //We want to pull in the first 50
                    .Send((response) =>
                        {
                            foreach (var challenge in response.ChallengeInstances)
                            {
                                GameObject go = Instantiate(activeGamePrefab) as GameObject;
                                ActiveGame activeGame = go.GetComponent<ActiveGame>();

                                if (challenge.State == "RUNNING" || challenge.State == "ISSUED") {
                                    //If the user Id of the next player is equal to the current player then it is the current player's turn
                                    if (challenge.NextPlayer == UserManager.instance.userId)
                                    {
                                        activeGame.isCurrentPlayerTurn = true;
                                        go.gameObject.transform.SetParent(yourMoveGameGrid.transform);
                                        //Debug.Log("Add game to yourMoveGameGrid");
                                        //yourMoveGames++;
                                    } else {
                                        activeGame.isCurrentPlayerTurn = false;
                                        go.gameObject.transform.SetParent(theirMoveGameGrid.transform);
                                    }
                                } else if (challenge.State == "COMPLETE") {
                                    activeGame.isCurrentPlayerTurn = false;
                                    go.gameObject.transform.SetParent(completedGameGrid.transform);
                                }

                                activeGame.challengeId = challenge.ChallengeId;
                                activeGame.nextPlayerId = challenge.NextPlayer;

                                foreach (var player in challenge.Accepted)
                                {
                                    activeGame.playerNames.Add(player.Name);
                                    activeGame.playerIds.Add(player.Id);
                                    activeGame.playerFacebookIds.Add(player.ExternalIds.GetString("FB"));
                                }

                                activeGame.challengerId = challenge.Challenger.Id;

                                activeGame.winnerName = challenge.ScriptData.GetString("winnerName");
                                activeGame.winnerId = challenge.ScriptData.GetString("winnerId");
                                List<int> boardData = challenge.ScriptData.GetIntList("gameBoard");
                                if (boardData != null) {
                                    int[] gameboard = challenge.ScriptData.GetIntList("gameBoard").ToArray();
                                    //                                int i;
                                    //                                String stringDebug = "";
                                    //                                for (i = 0; i < gameboard.Length; i++){
                                    //                                    stringDebug = stringDebug + " , " + gameboard[i].ToString();
                                    //                                }
                                    activeGame.gameBoard = gameboard;
                                }

                                List<GSData> moveList = challenge.ScriptData.GetGSDataList("moveList");
                                activeGame.moveList = moveList;

                                //Debug.Log("gameboard: " + stringDebug);
                                activeGames.Add(go);
                            }

                            gettingChallenges = false;
                        });
                //            Debug.Log("yourMoveGameGrid.transform.childCount: " + yourMoveGameGrid.transform.childCount);
                //            if (yourMoveGames == 0)
                //            {
                //                NoMovesPanel.SetActive(false);
                //                Debug.Log("NoMovesPanel Active: true " + yourMoveGames);
                //            }
                //            else
                //            {
                //                Debug.Log("NoMovesPanel Active: false " + yourMoveGames);
                //                NoMovesPanel.SetActive(true);
                //            }
            }
 
		}
	}
}