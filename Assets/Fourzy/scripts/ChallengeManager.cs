using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GameSparks.Api.Requests;
using GameSparks.Core;
using System.Linq;
using System;
using UnityEngine.Analytics.Experimental;
//using Fabric.Answers;

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
        //public List<GameObject> gameInvites = new List<GameObject>();

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
                            Debug.Log("***** Error removing game: " + response.Errors.JSON);
                            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                            customAttributes.Add("errorJSON", response.Errors.JSON);
                            AnalyticsEvent.Custom("RemoveGame:Error", customAttributes);
                            //Answers.LogCustom("RemoveGame:Error", customAttributes);
                        }
                        else
                        {
                            Debug.Log("Remove Game was successful");
                            AnalyticsEvent.Custom("RemoveGame");
                            //Answers.LogCustom("RemoveGame");
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
                            Debug.Log("***** Error setting game viewed: " + response.Errors.JSON);
                            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                            customAttributes.Add("errorJSON", response.Errors.JSON);
                            AnalyticsEvent.Custom("ViewedCompletedGame:Error", customAttributes);
                            //Answers.LogCustom("ViewedCompletedGame:Error", customAttributes);
                        }
                        else
                        {
                            Debug.Log("Set Viewed Game was successful");
                            AnalyticsEvent.Custom("ViewedCompletedGame");
                            //Answers.LogCustom("ViewedCompletedGame");
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
                            Debug.Log(response.Errors.JSON);
                        }
                    });

        }

        //This function accepts a string of UserIds and invites them to a new challenge
        public void ChallengeUser(string userId, GameState gameState, int position, Direction direction)
        {
            Debug.Log("ChallengeUser");
            //CreateChallengeRequest takes a list of UserIds because you can challenge more than one user at a time
            List<string> gsId = new List<string>();
            //Add our friends UserId to the list
            gsId.Add(userId);

            GSRequestData data = new GSRequestData().AddNumberList("gameBoard", gameState.GetGameBoardData());
            data.AddNumberList("tokenBoard", gameState.tokenBoard.GetTokenBoardData());
            data.AddString("tokenBoardId", gameState.tokenBoard.id);
            data.AddString("tokenBoardName", gameState.tokenBoard.name);
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
                        Debug.Log("***** Error Challenging User: " + response.Errors.JSON);
                        Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                        customAttributes.Add("errorJSON", response.Errors.JSON);
                        AnalyticsEvent.Custom("CreateChallengeRequest:ChallengeUser:Error", customAttributes);
                        //Answers.LogCustom("CreateChallengeRequest:ChallengeUser:Error", customAttributes);
                    }
                    else
                    {
                        GameManager.instance.challengeInstanceId = response.ChallengeInstanceId;
                        Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                        customAttributes.Add("ChallengedId", userId);
                        customAttributes.Add("TokenBoardId", gameState.tokenBoard.id);
                        customAttributes.Add("TokenBoardName", gameState.tokenBoard.name);
                        AnalyticsEvent.Custom("CreateChallengeRequest:ChallengeUser", customAttributes);
                        //Answers.LogCustom("CreateChallengeRequest:ChallengeUser", customAttributes);
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
                        Debug.Log("***** Error Finding Random Challenge: " + response.Errors.JSON);
                        Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                        customAttributes.Add("errorJSON", response.Errors.JSON);
                        AnalyticsEvent.Custom("FindChallengeRequest:Error", customAttributes);
                        //Answers.LogCustom("FindChallengeRequest:Error", customAttributes);
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

        public void ChallengeRandomUser(GameState gameState, int position, Direction direction)
        {
            Debug.Log("ChallengeRandomUser");
            //List<long> gameBoardData = gameState.GetGameBoardData();
            GSRequestData data = new GSRequestData().AddNumberList("gameBoard", gameState.GetGameBoardData());
            //data.AddNumberList("lastGameBoard", gameBoardData);
            data.AddNumberList("tokenBoard", gameState.tokenBoard.GetTokenBoardData());
            data.AddString("tokenBoardId", gameState.tokenBoard.id);
            data.AddString("tokenBoardName", gameState.tokenBoard.name);
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
                            Debug.Log("***** Error Challenging Random User: " + response.Errors.JSON);
                            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                            customAttributes.Add("errorJSON", response.Errors.JSON);
                            AnalyticsEvent.Custom("CreateChallengeRequest:Error", customAttributes);
                            //Answers.LogCustom("CreateChallengeRequest:Error", customAttributes);
                        } else {
                            GameManager.instance.challengeInstanceId = response.ChallengeInstanceId;
                            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                            customAttributes.Add("ChallengeInstanceId", response.ChallengeInstanceId);
                            customAttributes.Add("TokenBoardId", gameState.tokenBoard.id);
                            customAttributes.Add("TokenBoardName", gameState.tokenBoard.name);
                            AnalyticsEvent.Custom("CreateChallengeRequest:ChallengeRandomUser", customAttributes);
                            //Answers.LogCustom("CreateChallengeRequest:ChallengeRandomUser", customAttributes);
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
                        Debug.Log("***** Error Joining Challenge: " + response.Errors.JSON);
                        Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                        customAttributes.Add("errorJSON", response.Errors.JSON);
                        AnalyticsEvent.Custom("JoinChallengeRequest:Error", customAttributes);
                        //Answers.LogCustom("JoinChallengeRequest:Error", customAttributes);
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
                        Debug.Log("***** Error Getting Challenge: " + response.Errors);
                        Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                        customAttributes.Add("errorJSON", response.Errors.JSON);
                        AnalyticsEvent.Custom("GetChallengeRequest:Error", customAttributes);
                        //Answers.LogCustom("GetChallengeRequest:Error", customAttributes);
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
            }
            GameState gameState = new GameState(Constants.numRows, Constants.numColumns, true, true, tokenBoard, null, false, null);
            GameManager.instance.gameState = gameState;

            GameManager.instance.ResetGamePiecesAndTokens();
            GameManager.instance.PopulateMoveArrows();
            GameManager.instance.CreateTokenViews();

            GameManager.instance.isMultiplayer = false;
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
            
            GameManager.instance.UpdatePlayersStatusView();
            GameManager.instance.ResetUI();

            UIScreen.SetActive(false);

            if (OnActiveGame != null)
                OnActiveGame();

            GameManager.instance.EnableTokenAudio();
            
            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            customAttributes.Add("TokenBoardId", tokenBoard.id);
            customAttributes.Add("TokenBoardName", tokenBoard.name);
            AnalyticsEvent.Custom("OpenPassAndPlayGame", customAttributes);
            //Answers.LogCustom("OpenPassAndPlayGame", customAttributes);
        }

        public void OpenAiGame() 
        {
            GameManager.instance.ResetGamePiecesAndTokens();
            GameManager.instance.PopulateMoveArrows();

            if (tokenBoard == null) {
                TokenBoard randomTokenBoard = TokenBoardLoader.instance.GetTokenBoard();
                tokenBoard = randomTokenBoard;
            }

            GameState gameState = new GameState(Constants.numRows, Constants.numColumns, true, true, tokenBoard, null, false, null);
            GameManager.instance.gameState = gameState;

            GameManager.instance.CreateTokenViews();

            GameManager.instance.aiPlayer = new AiPlayer("default");

            GameManager.instance.isMultiplayer = false;
            GameManager.instance.isAiActive = true;
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
            GameManager.instance.UpdatePlayersStatusView();

            UIScreen.SetActive(false);

            if (OnActiveGame != null)
                OnActiveGame();
        }

        public void OpenNewMultiplayerGame() 
        {
            Debug.Log("Open New Multiplayer Game");
            GameManager.instance.ResetGamePiecesAndTokens();
            //GameManager.instance.PopulateMoveArrows();

            if (tokenBoard == null) {
                TokenBoard randomTokenBoard = TokenBoardLoader.instance.GetTokenBoard();
                tokenBoard = randomTokenBoard;
            }

            GameState gameState = new GameState(Constants.numRows, Constants.numColumns, true, true, tokenBoard, null, false, null);
            GameManager.instance.gameState = gameState;

            GameManager.instance.CreateTokenViews();

            GameManager.instance.isMultiplayer = true;
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

            GameManager.instance.UpdatePlayersStatusView();

            UIScreen.SetActive(false);

            if (OnActiveGame != null)
                OnActiveGame();
            
            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            customAttributes.Add("PlayerName", UserManager.instance.userName);
            customAttributes.Add("TokenBoardId", tokenBoard.id);
            customAttributes.Add("TokenBoardName", tokenBoard.name);
            AnalyticsEvent.Custom("OpenNewMultiplayerGame", customAttributes);
            //Answers.LogCustom("OpenNewMultiplayerGame", customAttributes);
        }

        public void OpenJoinedMultiplayerGame(GameSparks.Api.Responses.GetChallengeResponse._Challenge challenge)
        {
            Debug.Log("Open Joined Multiplayer Game");
            GameManager.instance.isLoading = true;

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

            GameManager.instance.ResetGamePiecesAndTokens();
            GameManager.instance.PopulateMoveArrows();

            //List<int> boardData = challenge.ScriptData.GetIntList("gameBoard");
            //List<int> tokenData = challenge.ScriptData.GetIntList("tokenBoard");
            string tokenBoardId = challenge.ScriptData.GetString("tokenBoardId");
            string tokenBoardName = challenge.ScriptData.GetString("tokenBoardName");

            int[] lastGameboard = new int[0];
            if (challenge.ScriptData.GetIntList("lastGameBoard") != null) {
                lastGameboard = challenge.ScriptData.GetIntList("lastGameBoard").ToArray();
            }

            int[] tokenBoardArray = Enumerable.Repeat(0, 64).ToArray();
            tokenBoardArray = challenge.ScriptData.GetIntList("tokenBoard").ToArray();  
            tokenBoard = new TokenBoard(tokenBoardArray, tokenBoardId, tokenBoardName, true);

            List<GSData> moveList = challenge.ScriptData.GetGSDataList("moveList");
            int currentPlayerMove = challenge.ScriptData.GetInt("currentPlayerMove").GetValueOrDefault();
            bool isPlayerOneTurn = currentPlayerMove == (int)Piece.BLUE ? true : false;

            GameState gameState = new GameState(Constants.numRows, Constants.numColumns, isPlayerOneTurn, true, tokenBoard, lastGameboard, false, moveList);
            GameManager.instance.gameState = gameState;

            GameManager.instance.SetupGameWrapper();

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
            AnalyticsEvent.Custom("JoinedMultiplayerGame", customAttributes);
            //Answers.LogCustom("JoinedMultiplayerGame", customAttributes);
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
                GameManager.instance.activeGames.Clear();

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
                                Debug.Log("***** Error Listing Challenge Request: " + response.Errors.JSON);
                                Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                                customAttributes.Add("errorJSON", response.Errors.JSON);
                                AnalyticsEvent.Custom("ListChallengeRequest:Error", customAttributes);
                                //Answers.LogCustom("ListChallengeRequest:Error", customAttributes);
                            } else {
                                int challengeCount = 0;
                                foreach (var gsChallenge in response.ChallengeInstances)
                                {
                                    challengeCount++;
                                    GameObject go = Instantiate(activeGamePrefab) as GameObject;
                                    ActiveGame activeGame = go.GetComponent<ActiveGame>();
                                    

                                    bool? isVisible = gsChallenge.ScriptData.GetBoolean("isVisible");
                                    bool viewedResult = false;
                                    List<String> playersViewedResult = gsChallenge.ScriptData.GetStringList("playersViewedResult");
                                    if (playersViewedResult != null) {
                                        for (int i = 0; i < playersViewedResult.Count; i++)
                                        {
                                            if (String.Compare(playersViewedResult[i], UserManager.instance.userId) == 0) {
                                                viewedResult = true;
                                            }
                                        }
                                    }

                                    activeGame.viewedResult = viewedResult;
                                    activeGame.challengeState = gsChallenge.State;

                                    if (gsChallenge.Challenger.Id == UserManager.instance.userId) {
                                        activeGame.isCurrentPlayer_PlayerOne = true;
                                    } else {
                                        activeGame.isCurrentPlayer_PlayerOne = false;
                                    }

                                    activeGame.challengeId = gsChallenge.ChallengeId;
                                    activeGame.nextPlayerId = gsChallenge.NextPlayer;
                                    activeGame.challengeShortCode = gsChallenge.ShortCode;

                                    foreach (var player in gsChallenge.Accepted)
                                    {
                                        activeGame.playerNames.Add(player.Name);
                                        activeGame.playerIds.Add(player.Id);
                                        activeGame.playerFacebookIds.Add(player.ExternalIds.GetString("FB"));
                                    }

                                    activeGame.challengerId = gsChallenge.Challenger.Id;

                                    activeGame.winnerName = gsChallenge.ScriptData.GetString("winnerName");
                                    activeGame.winnerId = gsChallenge.ScriptData.GetString("winnerId");

                                    //if (activeGame.winnerId == null && challenge.State == "COMPLETE") {
                                        //activeGame.isExpired = true;
                                        //Debug.Log("WinnerName: " + activeGame.winnerName);
                                    //}

                                    // List<int> boardData = challenge.ScriptData.GetIntList("gameBoard");
                                    // if (boardData != null) {
                                    //     int[] gameboard = challenge.ScriptData.GetIntList("gameBoard").ToArray();
                                    //     activeGame.gameBoard = gameboard;
                                    // }

                                    //int currentPlayerMove = gsChallenge.ScriptData.GetInt("currentPlayerMove").GetValueOrDefault();
                                    //bool isPlayerOneTurn = currentPlayerMove == (int)Piece.BLUE ? true : false;
                                    bool isCurrentPlayerTurn = false;
                                    //bool isGameOver = false;
                                    if (gsChallenge.State == "RUNNING" || gsChallenge.State == "ISSUED") {
                                        //If the user Id of the next player is equal to the current player then it is the current player's turn
                                        if (gsChallenge.NextPlayer == UserManager.instance.userId)
                                        {
                                            isCurrentPlayerTurn = true;

                                            go.gameObject.transform.SetParent(yourMoveGameGrid.transform);
                                            activeGameIds.Add(gsChallenge.ChallengeId);
                                            //yourMoveGames++;
                                        } else {
                                            isCurrentPlayerTurn = false;
                                            go.gameObject.transform.SetParent(theirMoveGameGrid.transform);
                                        }
                                    } else if (gsChallenge.State == "COMPLETE" && isVisible == true && viewedResult == true) {
                                        isCurrentPlayerTurn = false;
                                        //isGameOver = true;
                                        go.gameObject.transform.SetParent(completedGameGrid.transform);
                                    } else if (gsChallenge.State == "COMPLETE" && isVisible == true && viewedResult == false) {
                                        isCurrentPlayerTurn = false;
                                        //isGameOver = true;
                                        go.gameObject.transform.SetParent(resultsGameGrid.transform);
                                    }

                                    // int[] lastGameBoard;
                                    // List<int> lastBoardData = gsChallenge.ScriptData.GetIntList("lastGameBoard");
                                    // if (lastBoardData != null) {
                                    //     int[] lastGameboard = gsChallenge.ScriptData.GetIntList("lastGameBoard").ToArray();
                                    //     lastGameBoard = lastGameboard;
                                    // } else {
                                    //     lastGameBoard = new int[64];
                                    // }

                                    // TokenBoard tokenBoard;
                                    // string tokenBoardId = gsChallenge.ScriptData.GetString("tokenBoardId");
                                    // string tokenBoardName = gsChallenge.ScriptData.GetString("tokenBoardName");
                                    // List<int> tokenData = gsChallenge.ScriptData.GetIntList("tokenBoard");
                                    // if (tokenData != null) {
                                    //     int[] tokenBoardData = gsChallenge.ScriptData.GetIntList("tokenBoard").ToArray();
                                    //     //activeGame.tokenBoardData = tokenBoardData;
                                    //     tokenBoard = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName, true);
                                    // } else {
                                    //     int[] tokenBoardData = Enumerable.Repeat(0, 64).ToArray();
                                    //     //activeGame.tokenBoardData = tokenBoardData;
                                    //     tokenBoard = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName, true);
                                    // }

                                    //List<GSData> moveList = gsChallenge.ScriptData.GetGSDataList("moveList");

                                    Challenge challenge = new Challenge(gsChallenge);
                                    GameState gameState = new GameState(Constants.numRows, Constants.numColumns, challenge, isCurrentPlayerTurn);
                                    //activeGame.Initialize(currentPlayerMove, isCurrentPlayerTurn, tokenBoard, lastGameBoard, isGameOver, moveList);
                                    activeGame.gameState = gameState;
                                    activeGame.transform.localScale = new Vector3(1f,1f,1f);
                                    GameManager.instance.activeGames.Add(activeGame);
                                    games.Add(go);
                                }

                                if (pulledToRefresh) {
                                    StartCoroutine(Wait());
                                    // pulledToRefresh = false;
                                }
                                gettingChallenges = false;
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