﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GameSparks.Api.Requests;
using GameSparks.Core;
using System.Linq;
using System;

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
        public GameObject resultsGameGrid;
		public GameObject activeGamePrefab;

		public List<GameObject> games = new List<GameObject>();
        public List<string> activeGameIds = new List<string>();

		public GameObject inviteGrid;
		public GameObject invitePrefab;
		public List<GameObject> gameInvites = new List<GameObject>();

        public GameObject NoMovesPanel;
        public GameObject loadingSpinner;
        public GameObject gamesListContainer;

        public Texture2D defaultProfilePicture;

        private GameObject UIScreen;
        private bool gettingChallenges = false;
        private bool pulledToRefresh = false;
        //private int yourMoveGames = 0;

		void Start()
		{
			instance = this;

            UIScreen = GameObject.Find("UI Screen");
            loadingSpinner.GetComponent<Animator>().enabled = false;
            loadingSpinner.GetComponent<Image>().enabled = false;
		}

        private void OnEnable()
        {
            ActiveGame.OnRemoveGame += RemoveGame;
        }

        public void GamesListPullToRefresh(Vector2 pos) {
            //Debug.Log("pos x:" + pos.x + "pos y: " + pos.y + "magnitude: " + pos.magnitude + "normal: " + pos.normalized);
            if (!pulledToRefresh && pos.y > 1.06) {
                pulledToRefresh = true;
                loadingSpinner.GetComponent<Animator>().enabled = true;
                loadingSpinner.GetComponent<Image>().enabled = true;
                gamesListContainer.GetComponent<VerticalLayoutGroup>().padding.top = 250;
                GetChallenges();
            }
            if (!gettingChallenges && pos.y <= 1.015) {
                pulledToRefresh = false;
            }
        }

        private void RemoveGame(string challengeInstanceId) {
            new LogEventRequest().SetEventKey("removeGame")
                .SetEventAttribute("challengeInstanceId", challengeInstanceId)
                .SetDurable(true)
                .Send((response) =>
                    {
                        if (response.HasErrors)
                        {
                            Debug.Log("Problem removing game");
                        }
                        else
                        {
                            Debug.Log("Remove Game was successful");
                        }
                    });
        }

        public void SetViewedCompletedGame(string challengeInstanceId) {
            Debug.Log("SetViewedCompletedGame: UserManager.instance.userId: " + UserManager.instance.userId);
            new LogEventRequest().SetEventKey("viewedGame")
                .SetEventAttribute("challengeInstanceId", challengeInstanceId)
                .SetEventAttribute("player", UserManager.instance.userId)
                .SetDurable(true)
                .Send((response) =>
                    {
                        if (response.HasErrors)
                        {
                            Debug.Log("Problem setting game viewed");
                        }
                        else
                        {
                            Debug.Log("Set Viewed Game was successful");
                        }
                    });
        }

        public void StartMatchmaking() {

            new LogEventRequest().SetEventKey("startMatchmaking")
                .SetEventAttribute("matchShortCode","matchRanked")
                .Send((response) => 
                    {
                        if (response.HasErrors)
                        {
                            Debug.Log(response.Errors);
                        }
                    });

        }

		//This function accepts a string of UserIds and invites them to a new challenge
        public void ChallengeUser(string userId, List<long> gameBoard, List<long> tokenBoard, int position, Direction direction)
		{
            //CreateChallengeRequest takes a list of UserIds because you can challenge more than one user at a time
            List<string> gsId = new List<string>();
            //Add our friends UserId to the list
            gsId.Add(userId);

            GSRequestData data = new GSRequestData().AddNumberList("gameBoard", gameBoard);
            data.AddNumberList("tokenBoard", tokenBoard);
            data.AddNumber("position", position);
            data.AddNumber("direction", (int)direction);
            // always player 1 plays first
            data.AddNumber("player", 1);
            //we use CreateChallengeRequest with the shortcode of our challenge, we set this in our GameSparks Portal
            new CreateChallengeRequest().SetChallengeShortCode("chalRanked")
            .SetUsersToChallenge(gsId) //We supply the userIds of who we wish to challenge
			.SetEndTime(System.DateTime.Today.AddDays(60)) //We set a date and time the challenge will end on
			.SetChallengeMessage("I've challenged you to Fourzy!") // We can send a message along with the invite
            .SetScriptData(data)
            .SetDurable(true)
            .Send((response) => 
				{
					if (response.HasErrors)
					{
						Debug.Log(response.Errors);
                            //return false;
					}
					else
					{
                        GameManager.instance.challengeInstanceId = response.ChallengeInstanceId;
                            //return true;
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

                            int randNum = UnityEngine.Random.Range(0, challengeInstanceIds.Count-1);

                            //reference the id at that random numbers location
                            string randomChallengeId = challengeInstanceIds[randNum];
                            //each time you run this code, a different id is set in the scriptdata
                            //Spark.setScriptData("challenge to join", randomChallengeId);

                            // For now players are joined to a random challenge
                            JoinChallenge(randomChallengeId);
                        } else {
                            //Send player to Game Screen to make the first move
                            OpenNewMultiplayerGame();
                        }
                    }
                });
        }

        public void ChallengeRandomUser(List<long> gameBoard, List<long> tokenBoard, int position, Direction direction)
        {
            GSRequestData data = new GSRequestData().AddNumberList("gameBoard", gameBoard);
            data.AddNumberList("tokenBoard", tokenBoard);
            data.AddNumber("position", position);
            data.AddNumber("direction", (int)direction);
            // always player 1 plays first
            data.AddNumber("player", 1);
            //we use CreateChallengeRequest with the shortcode of our challenge, we set this in our GameSparks Portal
            new CreateChallengeRequest().SetChallengeShortCode("chalRanked")
                .SetAccessType("PUBLIC")
                .SetAutoStartJoinedChallengeOnMaxPlayers(true)
                .SetMaxPlayers(2)
                .SetMinPlayers(1)
                .SetEndTime(System.DateTime.Today.AddDays(60)) //We set a date and time the challenge will end on
                //.SetChallengeMessage("I've challenged you to Fourzy!") // We can send a message along with the invite
                .SetScriptData(data)
                .SetDurable(true)
                .Send((response) => 
                    {
                        if (response.HasErrors) {
                            Debug.Log(response.Errors);
                            //return false;
                        } else {
                            GameManager.instance.challengeInstanceId = response.ChallengeInstanceId;
                            //return true;
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
                    OpenMultiplayerGame(challenge);
                });
        }

        public void OpenPassAndPlayGame() 
        {
            GameManager.instance.ResetGameBoard();
            GameManager.instance.PopulateEmptySpots();
            //GameManager.instance.SetSampleTokenBoard();
            int[] tokenData = TokenBoard.Instance.FindTokenBoardAll();
            string x = "";
            foreach (var item in tokenData)
            {
                x += item + ",";

            }
            Debug.Log(x);
            StartCoroutine(GameManager.instance.SetTokenBoard(tokenData));
            GameManager.instance.isMultiplayer = false;
            GameManager.instance.isPlayerOneTurn = true;
            GameManager.instance.isCurrentPlayerTurn = true;
            GameManager.instance.isNewRandomChallenge = false;
            GameManager.instance.isNewChallenge = false;
            GameManager.instance.challengeInstanceId = null;
            GameManager.instance.playerNameLabel.text = "Blue Player";
            GameManager.instance.playerProfilePicture.sprite = Sprite.Create(defaultProfilePicture, 
                new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height), 
                new Vector2(0.5f, 0.5f));
            GameManager.instance.opponentNameLabel.text = "Red Player";
            GameManager.instance.opponentProfilePicture.sprite = Sprite.Create(defaultProfilePicture, 
                new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height), 
                new Vector2(0.5f, 0.5f));
            GameManager.instance.UpdateGameStatusText();

            UIScreen.SetActive(false);

            if (OnActiveGame != null)
                OnActiveGame(true);
        }

        public void OpenNewMultiplayerGame() 
        {
            Debug.Log("OpenNewMultiplayerGame");
            GameManager.instance.ResetGameBoard();
            GameManager.instance.PopulateEmptySpots();
            int[] tokenData = TokenBoard.Instance.FindTokenBoardNoSticky();
            string x = "";
            foreach (var item in tokenData)
            {
                x += item + ",";

            }
            Debug.Log(x);
            StartCoroutine(GameManager.instance.SetTokenBoard(tokenData));
            Debug.Log("Aftet set token board");
            GameManager.instance.isMultiplayer = true;
            GameManager.instance.isPlayerOneTurn = true;
            GameManager.instance.isCurrentPlayerTurn = true;
            GameManager.instance.isNewRandomChallenge = true;
            GameManager.instance.isNewChallenge = false;
            GameManager.instance.challengeInstanceId = null;
            GameManager.instance.opponentNameLabel.text = "Waiting for Opponent";
            GameManager.instance.opponentProfilePicture.sprite = Sprite.Create(defaultProfilePicture, 
                new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height), 
                new Vector2(0.5f, 0.5f));

            GameManager.instance.playerNameLabel.text = UserManager.instance.userName;
            if (UserManager.instance.profilePicture) {
                GameManager.instance.playerProfilePicture.sprite = UserManager.instance.profilePicture;    
            } else {
                GameManager.instance.playerProfilePicture.sprite = Sprite.Create(defaultProfilePicture, 
                    new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height), 
                    new Vector2(0.5f, 0.5f));
            }

            GameManager.instance.UpdateGameStatusText();

            UIScreen.SetActive(false);

            if (OnActiveGame != null)
                OnActiveGame(true);
        }

        public void OpenMultiplayerGame(GameSparks.Api.Responses.GetChallengeResponse._Challenge challenge)
        {
            Debug.Log("Open MultiplayerGame");
            GameManager.instance.opponentProfilePicture.sprite = Sprite.Create(defaultProfilePicture, 
                new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height), 
                new Vector2(0.5f, 0.5f));
            GameManager.instance.opponentNameLabel.text = challenge.Challenger.Name;
            GameManager.instance.opponentFacebookId = challenge.Challenger.ExternalIds.GetString("FB");

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
            GameManager.instance.challengeInstanceId = challenge.ChallengeId;
            GameManager.instance.winner = challenge.ScriptData.GetString("winnerName");

            GameManager.instance.isCurrentPlayerTurn = true;

            GameManager.instance.ResetGameBoard();
            GameManager.instance.PopulateEmptySpots();

            List<int> boardData = challenge.ScriptData.GetIntList("gameBoard");
            List<int> tokenData = challenge.ScriptData.GetIntList("tokenBoard");
            if (boardData != null) {
                int[] gameboard = challenge.ScriptData.GetIntList("gameBoard").ToArray();
                int[] tokenboard = Enumerable.Repeat(0, 64).ToArray();
                if (tokenData != null) {
                    tokenboard = challenge.ScriptData.GetIntList("tokenBoard").ToArray();    
                }

                GameManager.instance.SetupGameWrapper(gameboard, tokenboard);
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

		public void GetChallenges()
		{
            //yourMoveGames = 0;
            if (!gettingChallenges)
            {
                gettingChallenges = true;

                //Every time we call GetChallengeInvites we'll refresh the list
                for (int i = 0; i < games.Count; i++)
                {
                    //Destroy all runningGame gameObjects currently in the scene
                    Destroy(games[i]);
                }

                games.Clear();
                List<string> challengeStates = new List<string> {"RUNNING","COMPLETE","ISSUED"};

                //We send a ListChallenge Request with the shortcode of our challenge, we set this in our GameSparks Portal
                new ListChallengeRequest()
                    //.SetShortCode("chalRanked")
                    .SetStates(challengeStates)
                    .SetEntryCount(50) //We want to pull in the first 50
                    .Send((response) =>
                        {
                            foreach (var challenge in response.ChallengeInstances)
                            {
                                GameObject go = Instantiate(activeGamePrefab) as GameObject;
                                ActiveGame activeGame = go.GetComponent<ActiveGame>();

                                bool? isVisible = challenge.ScriptData.GetBoolean("isVisible");
//                                bool? wasViewed = challenge.ScriptData.GetBoolean("wasViewed");

                                bool viewedResult = false;
                                List<String> playersViewedResult = challenge.ScriptData.GetStringList("playersViewedResult");
                                if (playersViewedResult != null) {
                                    Debug.Log("playersViewedResult: " + playersViewedResult.Count);
                                    for (int i = 0; i < playersViewedResult.Count; i++)
                                    {   
                                        //Debug.Log("playersViewedResult[i] " + playersViewedResult[i]);
                                        //Debug.Log("UserManager.instance.userId " + UserManager.instance.userId);
                                        //Debug.Log("string compare: " + String.Compare(playersViewedResult[i], UserManager.instance.userId));
                                        if (String.Compare(playersViewedResult[i], UserManager.instance.userId) == 0) {
                                            viewedResult = true;
                                        }
                                    }
                                }
                            
                                activeGame.viewedResult = viewedResult;

                                activeGame.challengeState = challenge.State;

                                if (challenge.State == "RUNNING" || challenge.State == "ISSUED") {
                                    //If the user Id of the next player is equal to the current player then it is the current player's turn
                                    if (challenge.NextPlayer == UserManager.instance.userId)
                                    {
                                        activeGame.isCurrentPlayerTurn = true;

                                        go.gameObject.transform.SetParent(yourMoveGameGrid.transform);
                                        activeGameIds.Add(challenge.ChallengeId);
                                        //yourMoveGames++;
                                    } else {
                                        activeGame.isCurrentPlayerTurn = false;
                                        go.gameObject.transform.SetParent(theirMoveGameGrid.transform);
                                    }
                                } else if (challenge.State == "COMPLETE" && isVisible == true && viewedResult == true) {
                                    activeGame.isCurrentPlayerTurn = false;
                                    go.gameObject.transform.SetParent(completedGameGrid.transform);
                                } else if (challenge.State == "COMPLETE" && isVisible == true && viewedResult == false) {
                                    activeGame.isCurrentPlayerTurn = false;
                                    go.gameObject.transform.SetParent(resultsGameGrid.transform);
                                }
                                Debug.Log("viewedResult: " + viewedResult);
                                activeGame.challengeId = challenge.ChallengeId;
                                activeGame.nextPlayerId = challenge.NextPlayer;
                                activeGame.challengeShortCode = challenge.ShortCode;

                                foreach (var player in challenge.Accepted)
                                {
                                    activeGame.playerNames.Add(player.Name);
                                    activeGame.playerIds.Add(player.Id);
                                    activeGame.playerFacebookIds.Add(player.ExternalIds.GetString("FB"));
                                }

                                activeGame.challengerId = challenge.Challenger.Id;

                                activeGame.winnerName = challenge.ScriptData.GetString("winnerName");
                                activeGame.winnerId = challenge.ScriptData.GetString("winnerId");

                                if (activeGame.winnerId == null && challenge.State == "COMPLETE") {
                                    activeGame.isExpired = true;
                                }

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

                                List<int> tokenData = challenge.ScriptData.GetIntList("tokenBoard");
                                if (tokenData != null) {
                                    int[] tokenboard = challenge.ScriptData.GetIntList("tokenBoard").ToArray();
                                    activeGame.tokenBoard = tokenboard;
                                } else {
                                    int[] tokenboard = Enumerable.Repeat(0, 64).ToArray();
                                    activeGame.tokenBoard = tokenboard;
                                }

                                List<GSData> moveList = challenge.ScriptData.GetGSDataList("moveList");
                                activeGame.moveList = moveList;
                                activeGame.transform.localScale = new Vector3(1f,1f,1f);
                                //Debug.Log("gameboard: " + stringDebug);
                                games.Add(go);
                            }

                            if (pulledToRefresh) {
                                StartCoroutine(Wait());

//                                pulledToRefresh = false;
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

        IEnumerator Wait() {
            yield return new WaitForSeconds(0.8f);
            loadingSpinner.GetComponent<Animator>().enabled = false;
            loadingSpinner.GetComponent<Image>().enabled = false;
            gamesListContainer.GetComponent<VerticalLayoutGroup>().padding.top = 170;
            gamesListContainer.GetComponent<VerticalLayoutGroup>().SetLayoutVertical();
        }

        //      public void GetChallengeInvites()
        //      {
        //          //Every time we call GetChallengeInvites we'll refresh the list
        //          for (int i = 0; i < gameInvites.Count; i++)
        //          {
        //              //Destroy all gameInvite gameObjects currently in the scene
        //              Destroy(gameInvites[i]);
        //          }
        //          //Clear the list of gameInvites so we don't have null reference errors
        //          gameInvites.Clear();
        //
        //          //We send a ListChallenge Request with the shortcode of our challenge, we set this in our GameSparks Portal
        //            new ListChallengeRequest().SetShortCode("chalRanked")
        //              .SetState("RECEIVED") //We only want to get games that we've received
        //              .SetEntryCount(50) //We want to pull in the first 50 we find
        //              .Send((response) =>
        //                  {
        //                      //For every challenge we get
        //                      foreach (var challenge in response.ChallengeInstances)
        //                      {
        //                          //Create a new gameObject, add invitePrefab as a child of the invite Grid GameObject
        //                          //GameObject go = NGUITools.AddChild(inviteGrid.gameObject, invitePrefab);
        //                          GameObject go = Instantiate(invitePrefab) as GameObject;
        //                          go.gameObject.transform.SetParent(inviteGrid.transform);
        //
        //                          //Update all the gameObject's variables
        //                            GameInvite gameInvite = go.GetComponent<GameInvite>();
        //                            gameInvite.challengeId = challenge.ChallengeId;
        //                            gameInvite.inviteName = challenge.Challenger.Name;
        //                            gameInvite.inviteExpiry = challenge.EndDate.ToString();
        //
        //                          //Add the gameObject to the list of friends
        //                          gameInvites.Add(go);
        //
        //                          //Tell the grid to reposition everything nicely
        //                          //inviteGrid.Reposition();
        //                      }
        //                  });
        //      }
	}
}