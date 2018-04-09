using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GameSparks.Api.Messages;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Core;
using System.Linq;
using System;
using Lean.Pool;

namespace Fourzy
{
    public class ChallengeManager : Singleton<ChallengeManager>
    {
        public delegate void GameActive();
        public static event GameActive OnActiveGame;
        public delegate void ReceivedPlayerGamePiece(string gamePieceId);
        public static event ReceivedPlayerGamePiece OnReceivedPlayerGamePiece;
        public delegate void ReceivedOpponentGamePiece(string gamePieceId, string challengeId);
        public static event ReceivedOpponentGamePiece OnReceivedOpponentGamePiece;
        public delegate void SetGamePieceSuccess(string gamePieceId);
        public static event SetGamePieceSuccess OnSetGamePieceSuccess;

        public delegate void OpponentTurnTakenDelegate(ChallengeTurnTakenMessage message);
        public static event OpponentTurnTakenDelegate OnOpponentTurnTakenDelegate;
        public delegate void ChallengeJoinedDelegate(ChallengeJoinedMessage message);
        public static event ChallengeJoinedDelegate OnChallengeJoinedDelegate;
        public delegate void ChallengeWonDelegate(ChallengeWonMessage message);
        public static event ChallengeWonDelegate OnChallengeWonDelegate;
        public delegate void ChallengeLostDelegate(ChallengeLostMessage message);
        public static event ChallengeLostDelegate OnChallengeLostDelegate;
        public delegate void ChallengeIssuedDelegate(ChallengeIssuedMessage message);
        public static event ChallengeIssuedDelegate OnChallengeIssuedDelegate;

        public TokenBoard tokenBoard;
        public GameObject yourMoveGameGrid;
        public GameObject theirMoveGameGrid;
        public GameObject completedGameGrid;
        public GameObject resultsGameGrid;
        public GameObject activeGamePrefab;

        public List<GameObject> games = new List<GameObject>();
        public List<string> activeGameIds = new List<string>();

        public GameObject inviteGrid;
        public GameObject invitePrefab;
        public GameObject NoMovesPanel;
        public GameObject loadingSpinner;
        public GameObject gamesListContainer; 

        public GameObject UIScreen;
        public int daysUntilChallengeExpires = 40;
        private bool gettingChallenges = false;
        private bool pulledToRefresh = false;
        //private int yourMoveGames = 0;

        void Start()
        {
            loadingSpinner.GetComponent<Animator>().enabled = false;
            loadingSpinner.GetComponent<Image>().enabled = false;
        }

        void OnEnable() {
            GameUI.OnRemoveGame += RemoveGame;
            MiniGameBoard.OnSetTokenBoard += SetTokenBoard;
            GamePieceUI.OnSetGamePiece += SetGamePiece;
            GameManager.OnResign += Resign;
            ChallengeTurnTakenMessage.Listener += OnChallengeTurnTaken;
            ChallengeJoinedMessage.Listener += OnChallengeJoined;
            ChallengeWonMessage.Listener += OnChallengeWon;
            ChallengeLostMessage.Listener += OnChallengeLost;
            ChallengeIssuedMessage.Listener += OnChallengeIssued;
        }

        void OnDisable() {
            GameUI.OnRemoveGame -= RemoveGame;
            MiniGameBoard.OnSetTokenBoard -= SetTokenBoard;
            GamePieceUI.OnSetGamePiece -= SetGamePiece;
            GameManager.OnResign -= Resign;
            ChallengeTurnTakenMessage.Listener -= OnChallengeTurnTaken;
            ChallengeJoinedMessage.Listener -= OnChallengeJoined;
            ChallengeWonMessage.Listener -= OnChallengeWon;
            ChallengeLostMessage.Listener -= OnChallengeLost;
            ChallengeIssuedMessage.Listener -= OnChallengeIssued;
        }

        private void OnChallengeTurnTaken(ChallengeTurnTakenMessage message)
        {
            OnOpponentTurnTakenDelegate(message);
        }

        private void OnChallengeJoined(ChallengeJoinedMessage message)
        {
            OnChallengeJoinedDelegate(message);
        }

        private void OnChallengeWon(ChallengeWonMessage message)
        {
            OnChallengeWonDelegate(message);
        }

        private void OnChallengeLost(ChallengeLostMessage message)
        {
            OnChallengeLostDelegate(message);
        }

        private void OnChallengeIssued(ChallengeIssuedMessage message) 
        {
            OnChallengeIssuedDelegate(message);
        }

        private void SetTokenBoard(TokenBoard tokenboard) {
            
            if (tokenboard != null) {
                Debug.Log("SetTokenBoard tokenboard.name: " + tokenboard.name);
                this.tokenBoard = new TokenBoard(tokenboard.tokenBoardData, tokenboard.id, tokenboard.name, true);
            } else {
                Debug.Log("SetTokenBoard tokenboard is null");
                this.tokenBoard = null;
            }
        }

        private void SetGamePiece(string gamePieceId) {
            new LogEventRequest().SetEventKey("setGamePiece")
                .SetEventAttribute("gamePieceId", gamePieceId)
                .Send((response) =>
                {
                    if (response.HasErrors)
                    {
                        Debug.Log("***** Error setting gamepiece: " + response.Errors.JSON);
                        AnalyticsManager.LogError("set_gamepiece_error", response.Errors.JSON);
                    }
                    else
                    {
                        OnSetGamePieceSuccess(gamePieceId);    
                        Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                        customAttributes.Add("GamePieceId", gamePieceId);
                        AnalyticsManager.LogCustom("set_gamepiece");
                        //if (OnSetGamePieceSuccess != null)
                    }
                });
        }

        public void GetPlayerGamePiece() {
            string gamePieceId = "0";
            new LogEventRequest().SetEventKey("getGamePiece")
                .Send((response) =>
                {
                    if (response.HasErrors)
                    {
                        Debug.Log("***** Error getting player gamepiece: " + response.Errors.JSON);
                        AnalyticsManager.LogError("get_player_gamepiece_error", response.Errors.JSON);
                    }
                    else
                    {
                        gamePieceId = response.ScriptData.GetString("gamePieceId");
                        //Debug.Log("GetGamePiece was successful: gamePieceId: " + gamePieceId);
                        if (OnReceivedPlayerGamePiece != null)
                            OnReceivedPlayerGamePiece(gamePieceId);
                    }
                });
        }

        public void GetOpponentGamePiece(string userId, string challengeId = "")
        {
            string gamePieceId = "0";
            new LogEventRequest().SetEventKey("getOpponentGamePiece")
                .SetEventAttribute("userId", userId)
                .SetDurable(true)
                .Send((response) =>
                {
                    if (response.HasErrors)
                    {
                        Debug.Log("***** Error getting opponent gamepiece: " + response.Errors.JSON);
                        AnalyticsManager.LogError("get_opponent_gamepiece_error", response.Errors.JSON);
                    }
                    else
                    {
                        gamePieceId = response.ScriptData.GetString("gamePieceId");
                        //Debug.Log("GetOpponentGamePiece was successful: gamePieceId: " + gamePieceId);
                        if (OnReceivedOpponentGamePiece != null)
                            OnReceivedOpponentGamePiece(gamePieceId, challengeId);
                    }
                });
        }

        public void GamesListPullToRefresh(Vector2 pos)     {
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

        public void Resign(string challengeInstanceId) {
            new LogEventRequest().SetEventKey("resign")
                .SetEventAttribute("challengeInstanceId", challengeInstanceId)
                .SetDurable(true)
                .Send((response) => { 
                    if (response.HasErrors)
                    {
                        Debug.Log("***** Error resigning: " + response.Errors.JSON);
                        AnalyticsManager.LogError("resign_error", response.Errors.JSON);

                    }
                    else
                    {
                        Debug.Log("Resign was successful");
                        AnalyticsManager.LogCustom("resign_game");
                    }
                });
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
                            AnalyticsManager.LogError("remove_game_error", response.Errors.JSON);

                        }
                        else
                        {
                            Debug.Log("Remove Game was successful");
                            AnalyticsManager.LogCustom("remove_game");
                        }
                    });
        }

        public void SetViewedCompletedGame(string challengeInstanceId) {
            //Debug.Log("SetViewedCompletedGame: UserManager.instance.userId: " + UserManager.instance.userId);
            new LogEventRequest().SetEventKey("viewedGame")
                .SetEventAttribute("challengeInstanceId", challengeInstanceId)
                .SetEventAttribute("player", UserManager.instance.userId)
                .SetDurable(true)
                .Send((response) =>
                    {
                        if (response.HasErrors)
                        {
                            Debug.Log("***** Error setting game viewed: " + response.Errors.JSON);
                            AnalyticsManager.LogError("viewed_completed_game_error", response.Errors.JSON);
                        }
                        else
                        {
                            Debug.Log("Set Viewed Game was successful");
                            AnalyticsManager.LogCustom("viewed_completed_game");
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
        public void ChallengeUser(string userId, GameState gameState, int position, Direction direction, GameType gameType) {
            Debug.Log("ChallengeUser");
            //CreateChallengeRequest takes a list of UserIds because you can challenge more than one user at a time
            List<string> gsId = new List<string>();
            //Add our friends UserId to the list
            gsId.Add(userId);
            Debug.Log("ChallengeUser gameState.tokenBoard.id: " + gameState.tokenBoard.id);
            Debug.Log("ChallengeUser gameState.tokenBoard.name: " + gameState.tokenBoard.name);
            GSRequestData data = new GSRequestData().AddNumberList("gameBoard", gameState.GetGameBoardData());
            data.AddNumberList("tokenBoard", gameState.tokenBoard.GetTokenBoardData());
            data.AddNumberList("lastTokenBoard", gameState.previousTokenBoard.GetTokenBoardData());
            data.AddString("tokenBoardId", gameState.tokenBoard.id);
            data.AddString("tokenBoardName", gameState.tokenBoard.name);
            data.AddString("gameType", gameType.ToString());
            data.AddString("opponentId", userId);
            data.AddNumber("position", position);
            data.AddNumber("direction", (int)direction);
            // always player 1 plays first
            data.AddNumber("player", 1);
            //we use CreateChallengeRequest with the shortcode of our challenge, we set this in our GameSparks Portal
            new CreateChallengeRequest().SetChallengeShortCode("chalRanked")
            .SetUsersToChallenge(gsId) //We supply the userIds of who we wish to challenge
            .SetEndTime(System.DateTime.Today.AddDays(daysUntilChallengeExpires)) //We set a date and time the challenge will end on
            .SetChallengeMessage("I've challenged you to Fourzy!") // We can send a message along with the invite
            .SetScriptData(data)
            .SetDurable(true)
            .Send((response) => 
                {
                    if (response.HasErrors)
                    {
                        Debug.Log("***** Error Challenging User: " + response.Errors.JSON);
                        AnalyticsManager.LogError("create_challenge_request_error:challenge_user", response.Errors.JSON);
                    }
                    else
                    {
                        //TODO: Create an active game and add it to the list of ActiveGames
                        GameManager.instance.challengeInstanceId = response.ChallengeInstanceId;
                        GameManager.instance.CreateGame(response.ChallengeInstanceId);
                        Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                        customAttributes.Add("ChallengedId", userId);
                        customAttributes.Add("TokenBoardId", gameState.tokenBoard.id);
                        customAttributes.Add("TokenBoardName", gameState.tokenBoard.name);
                        AnalyticsManager.LogCustom("create_challenge_request:challenge_user", customAttributes);
                    }
                });
        }

        public void FindRandomChallenge() {
            tokenBoard = null;

            new FindChallengeRequest()
                .SetAccessType("PUBLIC")
                .SetCount(50)
                //.SetEligibility()
                //.SetOffset()
                //.SetShortCode()
                .Send(FindRandomChallengeSuccess, FindRandomChallengeError);
        }

        private void FindRandomChallengeSuccess(FindChallengeResponse response) {
            GSEnumerable<FindChallengeResponse._Challenge> challengeInstances = response.ChallengeInstances;
            //GSData scriptData = response.ScriptData; 

            if (challengeInstances.Count() > 0)
            {
                List<string> challengeInstanceIds = new List<string>();

                //for every object in the challenges array, get the challengeId field and push to challengeInstanceId[]
                foreach (var chalInstance in challengeInstances)
                {
                    challengeInstanceIds.Add(chalInstance.ChallengeId);
                }

                int randNum = UnityEngine.Random.Range(0, challengeInstanceIds.Count - 1);

                //reference the id at that random numbers location
                string randomChallengeId = challengeInstanceIds[randNum];
                //each time you run this code, a different id is set in the scriptdata
                //Spark.setScriptData("challenge to join", randomChallengeId);

                // For now players are joined to a random challenge
                JoinChallenge(randomChallengeId);
            }
            else
            {
                //Send player to Game Screen to make the first move
                OpenNewMultiplayerGame();
            }
        }

        private void FindRandomChallengeError(FindChallengeResponse response)
        {
            Debug.Log("***** Error Finding Random Challenge: " + response.Errors.JSON);
            AnalyticsManager.LogError("find_challenge_request_error", response.Errors.JSON);
            //TODO: show user facing error
            GameManager.instance.GameScreenBackButton();
        }

        public void ChallengeRandomUser(GameState gameState, int position, Direction direction, GameType gameType ) {
            Debug.Log("ChallengeRandomUser");

            GSRequestData data = new GSRequestData().AddNumberList("gameBoard", gameState.GetGameBoardData());
            data.AddNumberList("tokenBoard", gameState.tokenBoard.GetTokenBoardData());
            data.AddNumberList("lastTokenBoard", gameState.tokenBoard.GetTokenBoardData());
            data.AddString("tokenBoardId", gameState.tokenBoard.id);
            data.AddString("tokenBoardName", gameState.tokenBoard.name);
            data.AddString("gameType", gameType.ToString());
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
                .SetEndTime(System.DateTime.Today.AddDays(daysUntilChallengeExpires)) //We set a date and time the challenge will end on
                //.SetChallengeMessage("I've challenged you to Fourzy!") // We can send a message along with the invite
                .SetScriptData(data)
                .SetDurable(true)
                .Send((response) => 
                    {
                        if (response.HasErrors) {
                            Debug.Log("***** Error Challenging Random User: " + response.Errors.JSON);
                            AnalyticsManager.LogError("create_challenge_request_error", response.Errors.JSON);
                        } else {
                            //TODO: Create an active game and add it to the list of ActiveGames
                            GameManager.instance.challengeInstanceId = response.ChallengeInstanceId;
                            GameManager.instance.CreateGame(response.ChallengeInstanceId);
                            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                            customAttributes.Add("ChallengeInstanceId", response.ChallengeInstanceId);
                            customAttributes.Add("TokenBoardId", gameState.tokenBoard.id);
                            customAttributes.Add("TokenBoardName", gameState.tokenBoard.name);
                            AnalyticsManager.LogCustom("create_challenge_request:challenge_random_user", customAttributes);
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
                        AnalyticsManager.LogError("join_challenge_request_error", response.Errors.JSON);
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
                .Send((response) => {
                    if (response.HasErrors) {
                        Debug.Log("***** Error Getting Challenge: " + response.Errors);
                        AnalyticsManager.LogError("get_challenge_request_error", response.Errors.JSON);
                    } else {
                        var challenge = response.Challenge;
                        GSData scriptData = response.ScriptData;
                        OpenJoinedMultiplayerGame(challenge);
                    }
                });
        }

        public void OpenPuzzleChallengeGame() {
            PuzzleChallengeInfo puzzleChallenge = PuzzleChallengeLoader.instance.GetChallenge();
            if (puzzleChallenge == null) {
                // no more puzzle challenges
                GameManager.instance.alertUI.Open(LocalizationManager.instance.GetLocalizedValue("all_challenges_completed"));
                PlayerPrefs.DeleteAll();
            } else {
                GameManager.instance.puzzleChallengeInfo = puzzleChallenge;
                TokenBoard initialTokenBoard = new TokenBoard(puzzleChallenge.InitialTokenBoard.ToArray(), "", "", true);
                tokenBoard = initialTokenBoard;

                GameManager.instance.isLoading = true;
                GameState gameState = new GameState(Constants.numRows, Constants.numColumns, true, true, tokenBoard, puzzleChallenge.InitialGameBoard.ToArray(), false, null);
                GameManager.instance.gameState = gameState;

                GameManager.instance.challengeInstanceId = null;
                GameManager.instance.isCurrentPlayer_PlayerOne = true;
                GameManager.instance.isMultiplayer = false;
                GameManager.instance.isNewChallenge = false;
                GameManager.instance.isNewRandomChallenge = false;
                GameManager.instance.isPuzzleChallenge = true;
                GameManager.instance.gameType = GameType.PUZZLE;

                GameManager.instance.ResetGamePiecesAndTokens();
                GameManager.instance.ResetUIGameScreen();

                string subtitle = "";
                if (puzzleChallenge.MoveGoal > 1)
                {
                    subtitle = LocalizationManager.instance.GetLocalizedValue("puzzle_challenge_win_instructions_plural");
                }
                else
                {
                    subtitle = LocalizationManager.instance.GetLocalizedValue("puzzle_challenge_win_instructions_singular");
                }

                GameManager.instance.SetupGame(puzzleChallenge.Name, subtitle.Replace("%1", puzzleChallenge.MoveGoal.ToString()));
                GameManager.instance.InitPlayerUI();


                if (OnActiveGame != null)
                    OnActiveGame();

                //GameManager.instance.EnableTokenAudio();

                Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                customAttributes.Add("id", puzzleChallenge.ID);
                customAttributes.Add("level", puzzleChallenge.Level);
                AnalyticsManager.LogCustom("open_puzzle_challenge", customAttributes);
            }
        }

        public void OpenNewMultiplayerGame() {
            Debug.Log("Open New Multiplayer Game");

            GameManager.instance.ResetGamePiecesAndTokens();

            if (tokenBoard == null) {
                TokenBoard randomTokenBoard = TokenBoardLoader.instance.GetTokenBoard();
                tokenBoard = randomTokenBoard;
            }

            GameState gameState = new GameState(Constants.numRows, Constants.numColumns, true, true, tokenBoard, null, false, null);
            GameManager.instance.gameState = gameState;

            GameManager.instance.CreateTokenViews();

            GameManager.instance.gameType = GameType.RANDOM;
            GameManager.instance.challengeInstanceId = null;
            GameManager.instance.isMultiplayer = true;
            GameManager.instance.isNewRandomChallenge = true;
            GameManager.instance.isNewChallenge = false;
            GameManager.instance.isPuzzleChallenge = false;
            GameManager.instance.isCurrentPlayer_PlayerOne = true;
            GameManager.instance.challengerGamePieceId = UserManager.instance.gamePieceId;
            GameManager.instance.challengedGamePieceId = 0;

            GameManager.instance.ResetUIGameScreen();
            GameManager.instance.InitPlayerUI();
            GameManager.instance.UpdatePlayerUI();
            GameManager.instance.DisplayIntroUI(tokenBoard.name, LocalizationManager.instance.GetLocalizedValue("random_opponent_button"), true);

            if (OnActiveGame != null)
                OnActiveGame();
            
            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            customAttributes.Add("PlayerName", UserManager.instance.userName);
            customAttributes.Add("TokenBoardId", tokenBoard.id);
            customAttributes.Add("TokenBoardName", tokenBoard.name);
            AnalyticsManager.LogCustom("open_new_multiplayer_game", customAttributes);
        }

        public void OpenJoinedMultiplayerGame(GetChallengeResponse._Challenge challenge) {
            Debug.Log("Open Joined Multiplayer Game");
            GameManager.instance.isLoading = true;

            //GameManager.instance.opponentProfilePicture.sprite = Sprite.Create(defaultProfilePicture,
            //    new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height),
            //    new Vector2(0.5f, 0.5f));
            //GameManager.instance.opponentNameLabel.text = challenge.Challenger.Name;
            //GameManager.instance.opponentFacebookId = challenge.Challenger.ExternalIds.GetString("FB");

            GameManager.instance.gameType = GameType.RANDOM;
            GameManager.instance.challengeInstanceId = challenge.ChallengeId;
            GameManager.instance.isMultiplayer = true;
            GameManager.instance.isNewRandomChallenge = false;
            GameManager.instance.isNewChallenge = false;
            GameManager.instance.isPuzzleChallenge = false;
            GameManager.instance.isCurrentPlayer_PlayerOne = false;
            GameManager.instance.challengerGamePieceId = int.Parse(challenge.ScriptData.GetString("challengerGamePieceId"));
            GameManager.instance.challengedGamePieceId = UserManager.instance.gamePieceId;

            GameManager.instance.winner = challenge.ScriptData.GetString("winnerName");

            GameManager.instance.ResetGamePiecesAndTokens();

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

            GameManager.instance.ResetUIGameScreen();
            GameManager.instance.InitPlayerUI(challenge.Challenger.Name, null, challenge.Challenger.ExternalIds.GetString("FB"));
            GameManager.instance.UpdatePlayerUI();
            GameManager.instance.SetupGame(tokenBoardName, "Multiplayer");

            UIScreen.SetActive(false);

            if (OnActiveGame != null)
                OnActiveGame();

            GameManager.instance.CreateGame(challenge.ChallengeId);

            //GameManager.instance.EnableTokenAudio();

            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            customAttributes.Add("ChallengeId", challenge.ChallengeId);
            customAttributes.Add("PlayerName", UserManager.instance.userName);
            customAttributes.Add("OpponentName", challenge.Challenger.Name);
            customAttributes.Add("TokenBoardId", tokenBoardId);
            customAttributes.Add("TokenBoardName", tokenBoardName);
            AnalyticsManager.LogCustom("joined_multiplayer_game", customAttributes);
        }

        public void GetChallenges() {
            //yourMoveGames = 0;
            //Debug.Log("GetChallenges");
            if (!gettingChallenges)
            {
                gettingChallenges = true;

                //Every time we call GetChallengeInvites we'll refresh the list
                for (int i = 0; i < games.Count; i++)
                {
                    //Destroy all runningGame gameObjects currently in the scene
                    Destroy(games[i]);
                    //LeanPool.Despawn(games[i].gameObject);
                }

                games.Clear();
                if (GameManager.instance.games.Count > 0) {
                    GameManager.instance.games.Clear();    
                }

                List<string> challengeStates = new List<string> {"RUNNING","COMPLETE","ISSUED"};
                System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

                //We send a ListChallenge Request with the shortcode of our challenge, we set this in our GameSparks Portal
                new ListChallengeRequest()
                    .SetShortCode("chalRanked")
                    .SetMaxResponseTimeInMillis(20000)
                    .SetStates(challengeStates)
                    .SetEntryCount(50) // get 50 games
                    .Send(((response) =>
                        {
                            //Debug.Log("Time elapsed at response start: " + stopwatch.Elapsed);

                            if (response.HasErrors) {
                                Debug.Log("***** Error Listing Challenge Request: " + response.Errors.JSON);
                                AnalyticsManager.LogError("list_challenge_request_error", response.Errors.JSON);
                            } else {
                                int challengeCount = 0;
                                foreach (var gsChallenge in response.ChallengeInstances)
                                {
                                    challengeCount++;
                                    GameObject go = Instantiate(activeGamePrefab) as GameObject;
                                    //GameObject go = LeanPool.Spawn(activeGamePrefab) as GameObject;

                                    GameUI activeGame = go.GetComponent<GameUI>();

                                    bool? isVisible = gsChallenge.ScriptData.GetBoolean("isVisible");
                                    
                                    bool didViewResult = false;
                                    List<string> playersViewedResult = gsChallenge.ScriptData.GetStringList("playersViewedResult");
                                    if (playersViewedResult != null)
                                    {
                                        for (int i = 0; i < playersViewedResult.Count; i++)
                                        {
                                            if (string.Compare(playersViewedResult[i], UserManager.instance.userId) == 0)
                                            {
                                                didViewResult = true;
                                            }
                                        }
                                    }
                                    //activeGame.viewedResult = didViewResult;

                                    bool isCurrentPlayerTurn = false;
                                    if (gsChallenge.State == "RUNNING" || gsChallenge.State == "ISSUED")
                                    {
                                        //If the user Id of the next player is equal to the current player then it is the current player's turn
                                        if (gsChallenge.NextPlayer == UserManager.instance.userId)
                                        {
                                            isCurrentPlayerTurn = true;

                                            go.gameObject.transform.SetParent(yourMoveGameGrid.transform);
                                            activeGameIds.Add(gsChallenge.ChallengeId);
                                            //yourMoveGames++;
                                        }
                                        else
                                        {
                                            isCurrentPlayerTurn = false;
                                            go.gameObject.transform.SetParent(theirMoveGameGrid.transform);
                                        }
                                    }
                                    else if (gsChallenge.State == "COMPLETE" && isVisible == true && didViewResult == true)
                                    {
                                        isCurrentPlayerTurn = false;
                                        go.gameObject.transform.SetParent(completedGameGrid.transform);
                                    }
                                    else if (gsChallenge.State == "COMPLETE" && isVisible == true && didViewResult == false)
                                    {
                                        isCurrentPlayerTurn = false;
                                        go.gameObject.transform.SetParent(resultsGameGrid.transform);
                                    }

                                    bool isCurrentPlayer_PlayerOne = false;
                                    if (gsChallenge.Challenger.Id == UserManager.instance.userId)
                                    {
                                        activeGame.isCurrentPlayer_PlayerOne = true;
                                        isCurrentPlayer_PlayerOne = true;
                                    }
                                    else
                                    {
                                        activeGame.isCurrentPlayer_PlayerOne = false;
                                        isCurrentPlayer_PlayerOne = false;
                                    }

                                    activeGame.winnerName = gsChallenge.ScriptData.GetString("winnerName");
                                    string winnerId = gsChallenge.ScriptData.GetString("winnerId");
                                    activeGame.winnerId = winnerId;
                                    bool isExpired = false;
                                    if (activeGame.winnerId == null && gsChallenge.State == "COMPLETE")
                                    {
                                        activeGame.isExpired = true;
                                        isExpired = true;
                                    }

                                    PlayerEnum winner = PlayerEnum.EMPTY;
                                    //Debug.Log("winner id: " +winnerId);
                                    if (winnerId != null){
                                        if (winnerId == gsChallenge.Challenger.Id)
                                        {
                                            winner = PlayerEnum.ONE;
                                        } else {
                                            winner = PlayerEnum.TWO;
                                        }
                                    }

                                    string opponentFBId = "";
                                    string opponentName = "";
                                    
                                    if (gsChallenge.Accepted.Count() > 1)
                                    {
                                        if (gsChallenge.Accepted.ElementAt(0).Id == UserManager.instance.userId)
                                        {
                                            opponentFBId = gsChallenge.Accepted.ElementAt(1).ExternalIds.GetString("FB");
                                            opponentName = gsChallenge.Accepted.ElementAt(1).Name;
                                        }
                                        else
                                        {
                                            opponentFBId = gsChallenge.Accepted.ElementAt(0).ExternalIds.GetString("FB");
                                            opponentName = gsChallenge.Accepted.ElementAt(0).Name;
                                        }
                                    }

                                    List<string> playerFacebookIds = new List<string>();
                                    foreach (var player in gsChallenge.Accepted)
                                    {
                                        activeGame.playerNames.Add(player.Name);
                                        activeGame.playerIds.Add(player.Id);
                                        playerFacebookIds.Add(player.ExternalIds.GetString("FB"));
                                        activeGame.playerFacebookIds.Add(player.ExternalIds.GetString("FB"));
                                    }

                                    Opponent opponent = new Opponent(opponentName, opponentFBId);

                                    activeGame.challengeId = gsChallenge.ChallengeId;
                                    
                                    activeGame.challengeState = gsChallenge.State;
                                    ChallengeState challengeState = ChallengeState.NONE;
                                    if (gsChallenge.State == "RUNNING") {
                                        challengeState = ChallengeState.RUNNING;
                                    }
                                    else if (gsChallenge.State == "ISSUED") {
                                        challengeState = ChallengeState.ISSUED;
                                    }
                                    else if (gsChallenge.State == "COMPLETE")
                                    {
                                        challengeState = ChallengeState.COMPLETE;
                                    }

                                    activeGame.challengeShortCode = gsChallenge.ShortCode;
                                    ChallengeType challengeType = ChallengeType.NONE;
                                    if (gsChallenge.ShortCode == "chalRanked")
                                    {
                                        challengeType = ChallengeType.STANDARD;
                                    }
                                    else if (gsChallenge.ShortCode == "tournamentChallenge")
                                    {
                                        challengeType = ChallengeType.TOURNAMENT;
                                    }

                                    GameSparksChallenge challenge = new GameSparksChallenge(gsChallenge);
                                    GameState gameState = new GameState(Constants.numRows, Constants.numColumns, challenge, isCurrentPlayerTurn, winner);
                                    activeGame.gameState = gameState;

                                    string challengerGamePieceId = challenge.challengerGamePieceId;
                                    string challengedGamePieceId = challenge.challengedGamePieceId;
                                    //Debug.Log("challengerGamePieceId: "+ challengerGamePieceId);
                                    //Debug.Log("challengedGamePieceId: " + challengedGamePieceId);
                                    Game game = new Game(gsChallenge.ChallengeId, gameState, isCurrentPlayer_PlayerOne, isExpired, didViewResult, opponent, challengeState, challengeType, challenge.gameType, challengerGamePieceId, challengedGamePieceId);
                                    activeGame.game = game;
                                    //activeGame.nextPlayerId = gsChallenge.NextPlayer;

                                    activeGame.challengerId = gsChallenge.Challenger.Id;
                                    
                                    activeGame.transform.localScale = new Vector3(1f,1f,1f);
                                    GameManager.instance.games.Add(game);
                                    games.Add(go);
                                }

                                GameManager.instance.UpdateBadgeCounts();
                                
                                if (pulledToRefresh) {
                                    StartCoroutine(Wait());
                                    // pulledToRefresh = false;
                                }
                                gettingChallenges = false;
                                
                                stopwatch.Stop();
                                Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                                customAttributes.Add("endtime", stopwatch.Elapsed);
                                AnalyticsManager.LogCustom("ListChallengeRequest_response_endtime", customAttributes);
                                //Debug.Log("Time elapsed at response end: " + stopwatch.Elapsed);
                                stopwatch.Reset();   
                            }
                        }));
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

        public void ReloadGames() {
            
            if (!gettingChallenges)
            {
                //Debug.Log("ReloadActiveGames");

                gettingChallenges = true;

                //Every time we call GetChallengeInvites we'll refresh the list
                for (int i = 0; i < games.Count; i++)
                {
                    //Destroy all runningGame gameObjects currently in the scene
                    Destroy(games[i]);
                }
                games.Clear();

                foreach (var game in GameManager.instance.games)
                {
                    GameObject go = Instantiate(activeGamePrefab) as GameObject;
                    GameUI activeGame = go.GetComponent<GameUI>();
                    activeGame.game = game;

                    if (!game.gameState.isGameOver)
                    {
                        if (game.gameState.isCurrentPlayerTurn)
                        {
                            go.gameObject.transform.SetParent(yourMoveGameGrid.transform);
                        }
                        else
                        {
                            go.gameObject.transform.SetParent(theirMoveGameGrid.transform);
                        }
                    }
                    else if (game.gameState.isGameOver && game.isVisible == true && game.didViewResult == true)
                    {
                        go.gameObject.transform.SetParent(completedGameGrid.transform);
                    }
                    else if (game.gameState.isGameOver && game.isVisible == true && game.didViewResult == false)
                    {
                        go.gameObject.transform.SetParent(resultsGameGrid.transform);
                    }

                    activeGame.transform.localScale = new Vector3(1f, 1f, 1f);
                    games.Add(go);
                }

                gettingChallenges = false;
                GameManager.instance.UpdateBadgeCounts();
            }
        }

        IEnumerator Wait() {
            yield return new WaitForSeconds(0.8f);
            loadingSpinner.GetComponent<Animator>().enabled = false;
            loadingSpinner.GetComponent<Image>().enabled = false;
            gamesListContainer.GetComponent<VerticalLayoutGroup>().padding.top = 170;
            gamesListContainer.GetComponent<VerticalLayoutGroup>().SetLayoutVertical();
        }
    }
}