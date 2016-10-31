using UnityEngine;
using System.Collections;
using GameSparks.Api.Requests;
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

		void Start()
		{
			instance = this;
		}

		//This function accepts a string of UserIds and invites them to a new challenge
		public void ChallengeUser(List<string> userIds)
		{
			//we use CreateChallengeRequest with the shortcode of our challenge, we set this in our GameSparks Portal
			new CreateChallengeRequest().SetChallengeShortCode("chalRanked")
				.SetUsersToChallenge(userIds) //We supply the userIds of who we wish to challenge
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

		public void GetChallengeInvites()
		{
			//Every time we call GetChallengeInvites we'll refresh the list
			for (int i = 0; i < gameInvites.Count; i++)
			{
				//Destroy all gameInvite gameObjects currently in the scene
				Destroy(gameInvites[i]);
			}
			//Clear the list of gameInvites so we don't have null reference errors
			gameInvites.Clear();

			//We send a ListChallenge Request with the shortcode of our challenge, we set this in our GameSparks Portal
			new ListChallengeRequest().SetShortCode("ticTacToe")
				.SetState("RECEIVED") //We only want to get games that we've received
				.SetEntryCount(50) //We want to pull in the first 50 we find
				.Send((response) =>
					{
						//For every challenge we get
						foreach (var challenge in response.ChallengeInstances)
						{
							//Create a new gameObject, add invitePrefab as a child of the invite Grid GameObject
							//GameObject go = NGUITools.AddChild(inviteGrid.gameObject, invitePrefab);
							GameObject go = Instantiate(invitePrefab) as GameObject;
							go.gameObject.transform.SetParent(inviteGrid.transform);

							//Update all the gameObject's variables
                            GameInvite gameInvite = go.GetComponent<GameInvite>();
                            gameInvite.challengeId = challenge.ChallengeId;
                            gameInvite.inviteName = challenge.Challenger.Name;
                            gameInvite.inviteExpiry = challenge.EndDate.ToString();

							//Add the gameObject to the list of friends
							gameInvites.Add(go);

							//Tell the grid to reposition everything nicely
							//inviteGrid.Reposition();
						}
					});
		}


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

			//We send a ListChallenge Request with the shortcode of our challenge, we set this in our GameSparks Portal
			new ListChallengeRequest().SetShortCode("chalRanked")
				.SetState("RUNNING") //We want to get all games that are running
				.SetEntryCount(50) //We want to pull in the first 50
				.Send((response) =>
					{
						//For every challenge we receive
						foreach (var challenge in response.ChallengeInstances)
						{
							GameObject go = Instantiate(activeGamePrefab) as GameObject;
							go.gameObject.transform.SetParent(activeGameGrid.transform);

                            ActiveGame activeGame = go.GetComponent<ActiveGame>();
							//Set our variables
                            activeGame.challengeId = challenge.ChallengeId;
                            activeGame.turnStatus = challenge.NextPlayer;

							//For every player in the collection of players who have accepted the challenge
							foreach (var player in challenge.Accepted)
							{
								//Add their names and their Ids to the list i each respective Running Game Entry
                                activeGame.playerNames.Add(player.Name);
                                activeGame.playerIds.Add(player.Id);
							}

                            activeGame.challengerId = challenge.Challenger.Id;

							//We've saved the gameBoard in the cloud in ScriptData, we saved it as a String List and we need to convert it to a Unity Array
                            //go.GetComponent<ActiveGame>().gameBoard = challenge.ScriptData.GetString("gameBoard");
                            int[] gameboard = challenge.ScriptData.GetIntList("gameBoard").ToArray();
                            activeGame.gameBoard = gameboard;
//							int[] data1 = challenge.ScriptData.GetIntList("gameBoard").ToArray();
//							List<string> data2 = challenge.ScriptData.GetStringList("gameBoard");
//							String[] data3 = challenge.ScriptData.GetStringList("gameBoard").ToArray();
//							String test = challenge.ScriptData.JSON;
//							Debug.Log("gameboard data1: " + data1);
//							Debug.Log("gameboard data2: " + data2);
//							Debug.Log("gameboard data3: " + data3);
//							Debug.Log("gameboard json: " + test);
							int i;
							String stringDebug = "";
                            Debug.Log("gameboard length: " + gameboard.Length);
                            for (i = 0; i < gameboard.Length; i++){
                                stringDebug = stringDebug + " , " + gameboard[i].ToString();
							}
							Debug.Log("gameboard: " + stringDebug);
							//Add the gameObject to the list of friends
							activeGames.Add(go);
						}
					});
		}
	}
}