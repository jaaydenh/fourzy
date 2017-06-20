using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GameSparks.Api.Requests;
using GameSparks.Core;
using System.Linq;
using System;
using Fabric.Answers;

namespace Fourzy
{
    
    public class ChallengeManager : MonoBehaviour
	{
        public delegate void GameActive();
        public static event GameActive OnActiveGame;
        public static ChallengeManager instance;

        TokenBoard tokenBoard;
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
            MiniGameBoard.OnSetTokenBoard += SetTokenBoard;
        }

        private void OnDisable() {
            ActiveGame.OnRemoveGame -= RemoveGame;
            MiniGameBoard.OnSetTokenBoard -= SetTokenBoard;
        }

        private void SetTokenBoard(TokenBoard tokenboard) {
            if (tokenboard != null) {
                this.tokenBoard = new TokenBoard(tokenboard.tokenData, tokenboard.id, tokenboard.name, true);
            } else {
                this.tokenBoard = null;
            }
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
                            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                            customAttributes.Add("errorJSON", response.Errors.JSON);
                            Answers.LogCustom("RemoveGame:Error", customAttributes);
                        }
                        else
                        {
                            Debug.Log("Remove Game was successful");
                            Answers.LogCustom("RemoveGame");
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
                            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                            customAttributes.Add("errorJSON", response.Errors.JSON);
                            Answers.LogCustom("ViewedCompletedGame:Error", customAttributes);
                        }
                        else
                        {
                            Debug.Log("Set Viewed Game was successful");
                            Answers.LogCustom("ViewedCompletedGame");
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
        public void ChallengeUser(string userId, List<long> gameBoard, TokenBoard tokenBoard, int position, Direction direction)
		{
            Debug.Log("ChallengeUser");
            //CreateChallengeRequest takes a list of UserIds because you can challenge more than one user at a time
            List<string> gsId = new List<string>();
            //Add our friends UserId to the list
            gsId.Add(userId);

            GSRequestData data = new GSRequestData().AddNumberList("gameBoard", gameBoard);
            data.AddNumberList("tokenBoard", tokenBoard.GetTokenBoardData());
            data.AddString("tokenBoardId", tokenBoard.id);
            data.AddString("tokenBoardName", tokenBoard.name);
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
                        Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                        customAttributes.Add("errorJSON", response.Errors.JSON);
                        Answers.LogCustom("CreateChallengeRequest:ChallengeUser:Error", customAttributes);
					}
					else
					{
                        GameManager.instance.challengeInstanceId = response.ChallengeInstanceId;
                        Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                        customAttributes.Add("ChallengedId", userId);
                        customAttributes.Add("TokenBoardId", tokenBoard.id);
                        customAttributes.Add("TokenBoardName", tokenBoard.name);
                        Answers.LogCustom("CreateChallengeRequest:ChallengeUser", customAttributes);
					}
				});
		}

        public void FindRandomChallenge() {
            Debug.Log("FindRandomChallenge");
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
                        Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                        customAttributes.Add("errorJSON", response.Errors.JSON);
                        Answers.LogCustom("FindChallengeRequest:Error", customAttributes);
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

        public void ChallengeRandomUser(List<long> gameBoard, TokenBoard tokenBoard, int position, Direction direction)
        {
            Debug.Log("ChallengeRandomUser");
            GSRequestData data = new GSRequestData().AddNumberList("gameBoard", gameBoard);
            data.AddNumberList("tokenBoard", tokenBoard.GetTokenBoardData());
            data.AddString("tokenBoardId", tokenBoard.id);
            data.AddString("tokenBoardName", tokenBoard.name);
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
                            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                            customAttributes.Add("errorJSON", response.Errors.JSON);
                            Answers.LogCustom("CreateChallengeRequest:Error", customAttributes);
                        } else {
                            GameManager.instance.challengeInstanceId = response.ChallengeInstanceId;
                            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                            customAttributes.Add("ChallengeInstanceId", response.ChallengeInstanceId);
                            customAttributes.Add("TokenBoardId", tokenBoard.id);
                            customAttributes.Add("TokenBoardName", tokenBoard.name);
                            Answers.LogCustom("CreateChallengeRequest:ChallengeRandomUser", customAttributes);
                        }
                    });
        }

        public void JoinChallenge(string challengeInstanceId) {
            Debug.Log("JoinChallenge");
            new JoinChallengeRequest()
                .SetChallengeInstanceId(challengeInstanceId)
                .Send((response) => {
                    if (response.HasErrors)
                    {
                        Debug.Log(response.Errors);
                        Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                        customAttributes.Add("errorJSON", response.Errors.JSON);
                        Answers.LogCustom("JoinChallengeRequest:Error", customAttributes);
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
                    if (response.HasErrors) {
                        Debug.Log(response.Errors);
                        Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                        customAttributes.Add("errorJSON", response.Errors.JSON);
                        Answers.LogCustom("GetChallengeRequest:Error", customAttributes);
                    } else {
                        var challenge = response.Challenge;
                        GSData scriptData = response.ScriptData;
                        OpenJoinedMultiplayerGame(challenge);
                    }
                });
        }

        public void OpenPassAndPlayGame() 
        {
            if (tokenBoard == null) {
                TokenBoard randomTokenBoard = TokenBoardLoader.instance.GetTokenBoard();
                tokenBoard = randomTokenBoard;
                GameManager.instance.tokenBoard = randomTokenBoard;
            } else {
                GameManager.instance.tokenBoard = this.tokenBoard;
            }

            GameManager.instance.ResetGameBoard();
            GameManager.instance.PopulateMoveArrows();
            StartCoroutine(GameManager.instance.CreateTokens());

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
            GameManager.instance.ResetUI();

            UIScreen.SetActive(false);

            if (OnActiveGame != null)
                OnActiveGame();

            GameManager.instance.EnableTokenAudio();
            
            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            customAttributes.Add("TokenBoardId", tokenBoard.id);
            customAttributes.Add("TokenBoardName", tokenBoard.name);
            Answers.LogCustom("OpenPassAndPlayGame", customAttributes);
        }

        public void OpenAiGame() 
        {
            GameManager.instance.ResetGameBoard();
            GameManager.instance.PopulateMoveArrows();

            if (tokenBoard == null) {
                TokenBoard randomTokenBoard = TokenBoardLoader.instance.GetTokenBoard();
                tokenBoard = randomTokenBoard;
                GameManager.instance.tokenBoard = randomTokenBoard;
            } else {
                GameManager.instance.tokenBoard = this.tokenBoard;
            }

            // TokenBoard tokenBoard = TokenBoardLoader.instance.GetTokenBoard();
            // GameManager.instance.tokenBoard = tokenBoard;
            StartCoroutine(GameManager.instance.CreateTokens());

            //int[] tokenData = new int[64];            
            //StartCoroutine(GameManager.instance.SetTokenBoard(tokenData));

            GameManager.instance.aiPlayer = new AiPlayer("default");

            GameManager.instance.isMultiplayer = false;
            GameManager.instance.isAiActive = true;
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
                OnActiveGame();
        }

        public void OpenNewMultiplayerGame() 
        {
            Debug.Log("Open New Multiplayer Game");
            GameManager.instance.ResetGameBoard();
            GameManager.instance.PopulateMoveArrows();

            if (tokenBoard == null) {
                TokenBoard randomTokenBoard = TokenBoardLoader.instance.GetTokenBoard();
                tokenBoard = randomTokenBoard;
                GameManager.instance.tokenBoard = randomTokenBoard;
            } else {
                GameManager.instance.tokenBoard = this.tokenBoard;
            }
            
            // TokenBoard tokenBoard = TokenBoardLoader.instance.GetTokenBoard();
            // GameManager.instance.tokenBoard = tokenBoard;
            StartCoroutine(GameManager.instance.CreateTokens());

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
                OnActiveGame();
            
            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            customAttributes.Add("PlayerName", UserManager.instance.userName);
            customAttributes.Add("TokenBoardId", tokenBoard.id);
            customAttributes.Add("TokenBoardName", tokenBoard.name);
            Answers.LogCustom("OpenNewMultiplayerGame", customAttributes);
        }

        public void OpenJoinedMultiplayerGame(GameSparks.Api.Responses.GetChallengeResponse._Challenge challenge)
        {
            Debug.Log("Open Joined Multiplayer Game");

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
            GameManager.instance.PopulateMoveArrows();

            List<int> boardData = challenge.ScriptData.GetIntList("gameBoard");
            List<int> tokenData = challenge.ScriptData.GetIntList("tokenBoard");
            string tokenBoardId = challenge.ScriptData.GetString("tokenBoardId");
            string tokenBoardName = challenge.ScriptData.GetString("tokenBoardName");

            if (boardData != null) {
                int[] gameboard = challenge.ScriptData.GetIntList("gameBoard").ToArray();
                int[] tokenBoard = Enumerable.Repeat(0, 64).ToArray();

                if (tokenData != null) {
                    tokenBoard = challenge.ScriptData.GetIntList("tokenBoard").ToArray();  
                }

                GameManager.instance.SetupGameWrapper(gameboard, new TokenBoard(tokenBoard, tokenBoardId, tokenBoardName));
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
                OnActiveGame();

            GameManager.instance.EnableTokenAudio();

            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            customAttributes.Add("ChallengeId", challenge.ChallengeId);
            customAttributes.Add("PlayerName", UserManager.instance.userName);
            customAttributes.Add("OpponentName", challenge.Challenger.Name);
            customAttributes.Add("TokenBoardId", tokenBoardId);
            customAttributes.Add("TokenBoardName", tokenBoardName);
            Answers.LogCustom("JoinedMultiplayerGame", customAttributes);
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
                    .SetMaxQueueTimeInSeconds(25)
                    .SetMaxResponseTimeInSeconds(25)
                    .SetStates(challengeStates)
                    .SetEntryCount(50) //We want to pull in the first 50
                    .Send((response) =>
                        {
                            if (response.HasErrors) {
                                Debug.Log("response errors: " + response.Errors.JSON);
                                Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                                customAttributes.Add("errorJSON", response.Errors.JSON);
                                Answers.LogCustom("ListChallengeRequest:Error", customAttributes);
                            } else {
                                int challengeCount = 0;
                                foreach (var challenge in response.ChallengeInstances)
                                {
                                    challengeCount++;
                                    GameObject go = Instantiate(activeGamePrefab) as GameObject;
                                    ActiveGame activeGame = go.GetComponent<ActiveGame>();

                                    bool? isVisible = challenge.ScriptData.GetBoolean("isVisible");
                                    //bool? wasViewed = challenge.ScriptData.GetBoolean("wasViewed");

                                    bool viewedResult = false;
                                    List<String> playersViewedResult = challenge.ScriptData.GetStringList("playersViewedResult");
                                    if (playersViewedResult != null) {
                                        //Debug.Log("playersViewedResult: " + playersViewedResult.Count);
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
                                    //Debug.Log("viewedResult: " + viewedResult);
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
                                        //activeGame.isExpired = true;
                                        Debug.Log("WinnerName: " + activeGame.winnerName);
                                    }

                                    List<int> boardData = challenge.ScriptData.GetIntList("gameBoard");
                                    if (boardData != null) {
                                        int[] gameboard = challenge.ScriptData.GetIntList("gameBoard").ToArray();
                                        activeGame.gameBoard = gameboard;
                                    }

                                    string tokenBoardId = challenge.ScriptData.GetString("tokenBoardId");
                                    string tokenBoardName = challenge.ScriptData.GetString("tokenBoardName");
                                    List<int> tokenData = challenge.ScriptData.GetIntList("tokenBoard");
                                    if (tokenData != null) {
                                        int[] tokenBoardData = challenge.ScriptData.GetIntList("tokenBoard").ToArray();
                                        activeGame.tokenBoard = tokenBoardData;
                                        TokenBoard tb  = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName);
                                        activeGame.tokenBoard2 = tb;
                                    } else {
                                        int[] tokenBoardData = Enumerable.Repeat(0, 64).ToArray();
                                        activeGame.tokenBoard = tokenBoardData;
                                        TokenBoard tb  = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName);
                                        activeGame.tokenBoard2 = tb;
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
                                
                                Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                                customAttributes.Add("ChallengeCount", challengeCount);
                                Answers.LogCustom("ListChallengeRequest", customAttributes);
                            }
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