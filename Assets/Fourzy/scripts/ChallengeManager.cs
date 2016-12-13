using UnityEngine;
using System.Collections;
using GameSparks.Api.Requests;
using GameSparks.Core;
using System.Collections.Generic;
using System;

namespace Fourzy
{
    
    public class ChallengeManager : MonoBehaviour
	{
		public static ChallengeManager instance;

		public GameObject activeGameGrid;
		public GameObject activeGamePrefab;

		public List<GameObject> activeGames = new List<GameObject>();

		public GameObject inviteGrid;
		public GameObject invitePrefab;
		public List<GameObject> gameInvites = new List<GameObject>();

        //delegate Action<GameSparks.Api.Responses.CreateChallengeResponse> ChallengeId(GameSparks.Api.Responses.CreateChallengeResponse response);

        //Action<GameSparks.Api.Responses.CreateChallengeResponse> test;

		void Start()
		{
			instance = this;

            //GetActiveChallenges();
		}

		//This function accepts a string of UserIds and invites them to a new challenge
        public void ChallengeUser(string userId, List<long> gameBoard, int position, Fourzy.GameManager.Direction direction)
        //public void ChallengeUser(string userId)
		{
            //ChallengeId challengeInstanceId;

            //CreateChallengeRequest takes a list of UserIds because you can challenge more than one user at a time
            List<string> gsId = new List<string>();
            //Add our friends UserId to the list
            gsId.Add(userId);
            //Debug.Log("GameBoard: " + gameBoard);

            GSRequestData data = new GSRequestData().AddNumberList("gameBoard", gameBoard);
            data.AddNumber("position", position);
            data.AddNumber("direction", (int)direction);
            // always player 1 plays first
            data.AddNumber("player", 1);
            //we use CreateChallengeRequest with the shortcode of our challenge, we set this in our GameSparks Portal
            //GameSparks.Api.Responses.CreateChallengeResponse chalResponse =
                new CreateChallengeRequest().SetChallengeShortCode("chalRanked")
                .SetUsersToChallenge(gsId) //We supply the userIds of who we wish to challenge
				.SetEndTime(System.DateTime.Today.AddDays(1)) //We set a date and time the challenge will end on
				.SetChallengeMessage("I've challenged you to Fourzy!") // We can send a message along with the invite
                .SetScriptData(data)
//                .Send(delegate(GameSparks.Api.Responses.CreateChallengeResponse response) {
//                    Test1(response.ChallengeInstanceId);
//                });
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

//        public void Test1(string challengeId) {
//            GameManager.instance.challengeInstanceId = challengeId;
//        }

        public void ChallengeUserOld(string userId)
        //public void ChallengeUser(string userId)
        {
            //CreateChallengeRequest takes a list of UserIds because you can challenge more than one user at a time
            List<string> gsId = new List<string>();
            //Add our friends UserId to the list
            gsId.Add(userId);
            //Debug.Log("GameBoard: " + gameBoard);


            //we use CreateChallengeRequest with the shortcode of our challenge, we set this in our GameSparks Portal
            new CreateChallengeRequest().SetChallengeShortCode("chalRanked")
                .SetUsersToChallenge(gsId) //We supply the userIds of who we wish to challenge
                .SetEndTime(System.DateTime.Today.AddDays(1)) //We set a date and time the challenge will end on
                .SetChallengeMessage("I've challenged you to Fourzy!") // We can send a message along with the invite
                .Send((response) =>
                    {
                        if (response.HasErrors)
                        {
                            Debug.Log(response.Errors);
                        }
                        else
                        {
                            //Show message saying sent!;
                        }
                    });
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
			//Every time we call GetChallengeInvites we'll refresh the list
			for (int i = 0; i < activeGames.Count; i++)
			{
				//Destroy all runningGame gameObjects currently in the scene
				Destroy(activeGames[i]);
			}
			//Clear the list of friends so we don't have null reference errors
			activeGames.Clear();
            List<string> challengeStates = new List<string> {"RUNNING","COMPLETE"};

			//We send a ListChallenge Request with the shortcode of our challenge, we set this in our GameSparks Portal
			new ListChallengeRequest().SetShortCode("chalRanked")
                .SetStates(challengeStates)
				.SetEntryCount(50) //We want to pull in the first 50
				.Send((response) =>
					{
						foreach (var challenge in response.ChallengeInstances)
						{
							GameObject go = Instantiate(activeGamePrefab) as GameObject;
							go.gameObject.transform.SetParent(activeGameGrid.transform);

                            ActiveGame activeGame = go.GetComponent<ActiveGame>();

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
					});
		}
	}
}