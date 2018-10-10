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
    [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene)]
    public class ChallengeManager : UnitySingleton<ChallengeManager>
    {
        public delegate void ReceivedPlayerGamePiece(string gamePieceId);
        public static event ReceivedPlayerGamePiece OnReceivedPlayerGamePiece;
        public delegate void ReceivedOpponentGamePiece(string gamePieceId, string challengeId);
        public static event ReceivedOpponentGamePiece OnReceivedOpponentGamePiece;
        public delegate void SetGamePieceSuccess(string gamePieceId);
        public static event SetGamePieceSuccess OnSetGamePieceSuccess;
        public delegate void FindChallengeResult(bool success);
        public static event FindChallengeResult OnFindChallengeResult;
        public delegate void ReceivedRatingDelta(int ratingDelta);
        public static event ReceivedRatingDelta OnReceivedRatingDelta;

        public delegate void OpponentTurnTakenDelegate(ChallengeTurnTakenMessage message);
        public static event OpponentTurnTakenDelegate OnOpponentTurnTakenDelegate;
        public delegate void ChallengeJoinedDelegate(ChallengeJoinedMessage message);
        public static event ChallengeJoinedDelegate OnChallengeJoinedDelegate;
        public delegate void ChallengeWonDelegate(ChallengeWonMessage message);
        public static event ChallengeWonDelegate OnChallengeWonDelegate;
        public delegate void ChallengeLostDelegate(ChallengeLostMessage message);
        public static event ChallengeLostDelegate OnChallengeLostDelegate;
        public delegate void ChallengeIssuedDelegate(ChallengeIssuedMessage message);
        public static event ChallengeDrawnDelegate OnChallengeDrawnDelegate;
        public delegate void ChallengeDrawnDelegate(ChallengeDrawnMessage message);
        public static event ChallengeIssuedDelegate OnChallengeIssuedDelegate;

        public TokenBoard tokenBoard;
        public string tokenBoardId;

        public GetChallengeResponse._Challenge challenge;

        public double daysUntilChallengeExpires = 60;
        private bool gettingChallenges = false;

        void OnEnable() 
        {
            MiniGameBoard.OnSetTokenBoard += SetTokenBoard;
            GamePieceUI.OnSetGamePiece += SetGamePiece;
            ChallengeTurnTakenMessage.Listener += OnChallengeTurnTaken;
            ChallengeJoinedMessage.Listener += OnChallengeJoined;
            ChallengeWonMessage.Listener += OnChallengeWon;
            ChallengeLostMessage.Listener += OnChallengeLost;
            ChallengeIssuedMessage.Listener += OnChallengeIssued;
            ChallengeDrawnMessage.Listener += OnChallengeDrawn;
            // RealtimeManager.OnRealtimeReady += OpenNewRealtimeGame;
        }

        void OnDisable() 
        {
            MiniGameBoard.OnSetTokenBoard -= SetTokenBoard;
            GamePieceUI.OnSetGamePiece -= SetGamePiece;
            ChallengeTurnTakenMessage.Listener -= OnChallengeTurnTaken;
            ChallengeJoinedMessage.Listener -= OnChallengeJoined;
            ChallengeWonMessage.Listener -= OnChallengeWon;
            ChallengeLostMessage.Listener -= OnChallengeLost;
            ChallengeIssuedMessage.Listener -= OnChallengeIssued;
            ChallengeDrawnMessage.Listener -= OnChallengeDrawn;
            // RealtimeManager.OnRealtimeReady -= OpenNewRealtimeGame;
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

        private void OnChallengeDrawn(ChallengeDrawnMessage message)
        {
            OnChallengeDrawnDelegate(message);
        }

        private void OnChallengeIssued(ChallengeIssuedMessage message) 
        {
            OnChallengeIssuedDelegate(message);
        }

        private void SetTokenBoard(string tokenBoardId) {
            
            if (tokenBoardId != null) {
                Debug.Log("SetTokenBoard tokenboardid: " + tokenBoardId);
                // this.tokenBoard = new TokenBoard(tokenBoard);
                this.tokenBoardId = tokenBoardId;
                //   ObjectCopier.Clone(tokenboard);
                // this.tokenBoard = new TokenBoard(tokenboard.tokenBoard, tokenboard.id, tokenboard.name, tokenboard.initialMoves, tokenboard.initialGameBoard, true);
            } else {
                Debug.Log("SetTokenBoard tokenboardid is null");
                // this.tokenBoard = null;
                this.tokenBoardId = null;
            }
        }

        public TokenBoard GetTokenBoard(string tokenBoardId = null) 
        {
            // Debug.Log("ChallengeManager: GetTokenBoard: " + tokenBoardId);
            if (tokenBoardId != null) 
            {
                return TokenBoardLoader.Instance.GetTokenBoard(tokenBoardId);
            }

            if (!string.IsNullOrEmpty(this.tokenBoardId))
            {
                return TokenBoardLoader.Instance.GetTokenBoard(this.tokenBoardId);
            }

            Debug.Log("GetRandomTokenBoard");
            return tokenBoard = TokenBoardLoader.Instance.GetRandomTokenBoard();
        }

        public void SubmitPuzzleCompleted() {
            new LogEventRequest().SetEventKey("submitPuzzleCompleted")
                .SetEventAttribute("completed", 1)
                .Send((response) =>
                {
                    if (response.HasErrors)
                    {
                        Debug.Log("***** Error submitting puzzle completed for leaderboard: " + response.Errors.JSON);
                        AnalyticsManager.LogError("set_puzzle_completed_error", response.Errors.JSON);
                    }
                    else
                    {
                        Debug.Log("Succssfully submitted puzzle complted");
                    }
                });
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

        public void GetGamePiece(string userId, Action<LogEventResponse> successCallback, Action<LogEventResponse> errorCallback) {
            if (userId != null && userId != "") {
                new LogEventRequest().SetEventKey("getOpponentGamePiece")
                .SetEventAttribute("userId", userId)
                .SetDurable(true)
                .Send(successCallback, errorCallback);
            }
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

        public void GetRatingDelta(string challengeId)
        {
            int ratingDelta = 0;
            new LogEventRequest().SetEventKey("getRatingDelta")
                //.SetEventAttribute("opponentId", opponentId)
                //.SetEventAttribute("gameResult", gameResult.ToString())
                .SetEventAttribute("challengeId", challengeId)
                .SetDurable(true)
                .Send((response) =>
                {
                    if (response.HasErrors)
                    {
                        Debug.Log("***** Error for GetRatingDelta: " + response.Errors.JSON);
                        AnalyticsManager.LogError("get_rating_delta_error", response.Errors.JSON);
                    }
                    else
                    {
                        ratingDelta = response.ScriptData.GetInt("ratingDelta").GetValueOrDefault(-1);
                        Debug.Log("GetRatingDelta was successful: ratingDelta: " + ratingDelta);
                        if (OnReceivedRatingDelta != null)
                            OnReceivedRatingDelta(ratingDelta);
                    }
                });
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
            //Debug.Log("SetViewedCompletedGame: UserManager.Instance.userId: " + UserManager.Instance.userId);
            new LogEventRequest().SetEventKey("viewedGame")
                .SetEventAttribute("challengeInstanceId", challengeInstanceId)
                .SetEventAttribute("player", UserManager.Instance.userId)
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
        public void ChallengeUser(Game game, int position, Direction direction) {
            Debug.Log("ChallengeUser");
            //CreateChallengeRequest takes a list of UserIds because you can challenge more than one user at a time
            List<string> gsId = new List<string>();
            //Add our friends UserId to the list
            gsId.Add(game.opponent.opponentId);

            GSRequestData data = new GSRequestData().AddNumberList("gameBoard", game.gameState.GetGameBoardData());
            data.AddNumberList("tokenBoard", game.gameState.TokenBoard.GetTokenBoardData());
            data.AddNumberList("lastTokenBoard", game.gameState.PreviousTokenBoard.GetTokenBoardData());
            data.AddNumberList("initialGameBoard", game.gameState.TokenBoard.GetInitialGameBoardData());
            data.AddString("tokenBoardId", game.gameState.TokenBoard.id);
            data.AddString("tokenBoardName", game.gameState.TokenBoard.name);
            data.AddString("gameType", game.gameState.GameType.ToString());
            data.AddString("opponentId", game.opponent.opponentId);
            data.AddNumber("position", position);
            data.AddNumber("direction", (int)direction);
            // always player 1 plays first
            data.AddNumber("player", 1);
            //we use CreateChallengeRequest with the shortcode of our challenge, we set this in our GameSparks Portal
            new CreateChallengeRequest().SetChallengeShortCode("chalRanked")
            .SetUsersToChallenge(gsId) //We supply the userIds of who we wish to challenge
            .SetEndTime(System.DateTime.Today.AddMonths(1)) //We set a date and time the challenge will end on
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
                        GameManager.Instance.activeGame.challengeId = response.ChallengeInstanceId;
                        GamePlayManager.Instance.game.challengeId = response.ChallengeInstanceId;
                        // GameManager.Instance.CreateGame(response.ChallengeInstanceId, userId);
                        game.challengeId = response.ChallengeInstanceId;
                        game.gameState.SetRandomGuid(response.ChallengeInstanceId);
                        GameManager.Instance.Games.Add(game);

                        Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                        customAttributes.Add("ChallengedId", game.opponent.opponentId);
                        customAttributes.Add("TokenBoardId", game.gameState.TokenBoard.id);
                        customAttributes.Add("TokenBoardName", game.gameState.TokenBoard.name);
                        AnalyticsManager.LogCustom("create_challenge_request:challenge_user", customAttributes);
                    }
                });
        }

        public void FindRandomChallenge(Action<FindChallengeResponse> successCallback, Action<FindChallengeResponse> errorCallback) {
            tokenBoard = null;

            new FindChallengeRequest()
                .SetAccessType("PUBLIC")
                .SetCount(50)
                .SetMaxResponseTimeInMillis(17000)
                .Send(successCallback, errorCallback);
        }

        public void ChallengeRandomUser(Game game, int position, Direction direction ) {
            Debug.Log("ChallengeRandomUser");

            GSRequestData data = new GSRequestData().AddNumberList("gameBoard", game.gameState.GetGameBoardData());
            data.AddNumberList("tokenBoard", game.gameState.TokenBoard.GetTokenBoardData());
            data.AddNumberList("lastTokenBoard", game.gameState.TokenBoard.GetTokenBoardData());
            data.AddNumberList("initialGameBoard", game.gameState.TokenBoard.GetInitialGameBoardData());
            data.AddString("tokenBoardId", game.gameState.TokenBoard.id);
            data.AddString("tokenBoardName", game.gameState.TokenBoard.name);
            data.AddString("gameType", game.gameState.GameType.ToString());
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
                .SetEndTime(System.DateTime.Today.AddMonths(1)) //We set a date and time the challenge will end on
                //.SetChallengeMessage("I've challenged you to Fourzy!") // We can send a message along with the invite
                .SetScriptData(data)
                .SetDurable(true)
                .Send((response) => 
                    {
                        if (response.HasErrors) {
                            Debug.Log("***** Error Challenging Random User: " + response.Errors.JSON);
                            AnalyticsManager.LogError("create_challenge_request_error", response.Errors.JSON);
                        } else {
                            GameManager.Instance.activeGame.challengeId = response.ChallengeInstanceId;
                            GamePlayManager.Instance.game.challengeId = response.ChallengeInstanceId;
                            // GameManager.Instance.CreateGame(response.ChallengeInstanceId, "");
                            game.challengeId = response.ChallengeInstanceId;
                            game.gameState.SetRandomGuid(response.ChallengeInstanceId);
                            GameManager.Instance.Games.Add(game);

                            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                            customAttributes.Add("ChallengeInstanceId", response.ChallengeInstanceId);
                            customAttributes.Add("TokenBoardId", game.gameState.TokenBoard.id);
                            customAttributes.Add("TokenBoardName", game.gameState.TokenBoard.name);
                            AnalyticsManager.LogCustom("create_challenge_request:challenge_random_user", customAttributes);
                        }
                    });
        }

        public void JoinChallenge(string challengeInstanceId, Action<JoinChallengeResponse> successCallback, Action<JoinChallengeResponse> errorCallback) {
            new JoinChallengeRequest()
                .SetChallengeInstanceId(challengeInstanceId)
                .SetMaxResponseTimeInMillis(17000)
                .Send(successCallback, errorCallback);
        }

        public void GetChallenge(string challengeInstanceId, Action<GetChallengeResponse> successCallback, Action<GetChallengeResponse> errorCallback) {
            new GetChallengeRequest()
                .SetChallengeInstanceId(challengeInstanceId)
                .SetMaxResponseTimeInMillis(17000)
                .Send(successCallback, errorCallback);
        }

        public void OpenNewRealtimeGame(int firstPlayerPeerId, int seed) {
            
            Debug.Log("Open New Realtime Game");
            bool isFirstPlayer = false;
            if (RealtimeManager.Instance.GetRTSession().PeerId == firstPlayerPeerId) {
                isFirstPlayer = true;
            }
            
            string opponentId = string.Empty;
            string opponentName = string.Empty;

            for (int playerIndex = 0; playerIndex < RealtimeManager.Instance.GetSessionInfo().GetPlayerList().Count; playerIndex++) { // loop through all players
                // If the player's peerId is the same as this player's peerId then we know this is the player and we can setup players and opponents //
                if (RealtimeManager.Instance.GetSessionInfo().GetPlayerList()[playerIndex].peerId == RealtimeManager.Instance.GetRTSession().PeerId) {
                    // Current player
                    string playerId = RealtimeManager.Instance.GetSessionInfo ().GetPlayerList () [playerIndex].id;
                    string playerName = RealtimeManager.Instance.GetSessionInfo ().GetPlayerList () [playerIndex].displayName;
                } else {
                    // The opponent
                    opponentId = RealtimeManager.Instance.GetSessionInfo ().GetPlayerList () [playerIndex].id;
                    opponentName = RealtimeManager.Instance.GetSessionInfo ().GetPlayerList () [playerIndex].displayName;
                }
            }

            GetGamePiece(opponentId, GetGamePieceIdSuccess, GetGamePieceIdFailure);
            Opponent opponent = new Opponent(opponentId, opponentName, null);

            // tokenBoard = TokenBoardLoader.Instance.GetRandomTokenBoardByIndex(tokenBoardIndex);
            tokenBoard = TokenBoardLoader.Instance.GetRandomTokenBoard(seed);

            GameState gameState = new GameState(Constants.numRows, Constants.numColumns, GameType.REALTIME, true, isFirstPlayer, tokenBoard, tokenBoard.initialGameBoard, false, null);
            Game game = new Game(null, gameState, isFirstPlayer, false, false, opponent, ChallengeState.NONE, ChallengeType.STANDARD, UserManager.Instance.gamePieceId.ToString(), "0", null, null, null, null, true);
            GameManager.Instance.activeGame = game;

            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            customAttributes.Add("PlayerName", UserManager.Instance.userName);
            customAttributes.Add("TokenBoardId", tokenBoard.id);
            customAttributes.Add("TokenBoardName", tokenBoard.name);
            AnalyticsManager.LogCustom("open_new_realtime_game", customAttributes);
        }

        private void GetGamePieceIdSuccess(LogEventResponse response) {
            if (response.ScriptData != null) {
                
                var gamePieceIdString = response.ScriptData.GetString("gamePieceId");
                Debug.Log("GetGamePieceIdSuccess: " + gamePieceIdString);

                int gamePieceId = int.Parse(gamePieceIdString);

                // GamePlayManager.Instance.UpdateOpponentUI(gamePieceId);
                GameManager.Instance.activeGame.opponent.gamePieceId = gamePieceId;
                GameManager.Instance.OpenGame(GameManager.Instance.activeGame);
            }
        }

        private void GetGamePieceIdFailure(LogEventResponse response) {
            Debug.Log("***** Error getting player gamepiece: " + response.Errors.JSON);
            AnalyticsManager.LogError("get_player_gamepiece_error", response.Errors.JSON);
        }

        public void OpenNewMultiplayerGame() {
            Debug.Log("Open New Multiplayer Game");

            if (tokenBoard == null) {
                TokenBoard randomTokenBoard = TokenBoardLoader.Instance.GetRandomTokenBoard();
                tokenBoard = randomTokenBoard;
            }

            GameState gameState = new GameState(Constants.numRows, Constants.numColumns, GameType.RANDOM, true, true, tokenBoard, tokenBoard.initialGameBoard, false, null);
            Game game = new Game(null, gameState, true, false, false, null, ChallengeState.NONE, ChallengeType.STANDARD, UserManager.Instance.gamePieceId.ToString(), "0", null, null, null, null, true);
            GameManager.Instance.OpenGame(game);
            
            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            customAttributes.Add("PlayerName", UserManager.Instance.userName);
            customAttributes.Add("TokenBoardId", tokenBoard.id);
            customAttributes.Add("TokenBoardName", tokenBoard.name);
            AnalyticsManager.LogCustom("open_new_multiplayer_game", customAttributes);
        }

        public void OpenJoinedMultiplayerGame() {
            Debug.Log("Open Joined Multiplayer Game");

            //GameManager.Instance.opponentProfilePicture.sprite = Sprite.Create(defaultProfilePicture,
            //    new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height),
            //    new Vector2(0.5f, 0.5f));

            string challengerGamePieceId = challenge.ScriptData.GetString("challengerGamePieceId");
            string challengedGamePieceId = UserManager.Instance.gamePieceId.ToString();
            string winnerName = challenge.ScriptData.GetString("winnerName");

            string tokenBoardId = challenge.ScriptData.GetString("tokenBoardId");
            string tokenBoardName = challenge.ScriptData.GetString("tokenBoardName");

            int[] lastGameboard = new int[0];
            if (challenge.ScriptData.GetIntList("lastGameBoard") != null) {
                lastGameboard = challenge.ScriptData.GetIntList("lastGameBoard").ToArray();
            }

            int[] tokenBoardArray = Enumerable.Repeat(0, 64).ToArray();
            tokenBoardArray = challenge.ScriptData.GetIntList("tokenBoard").ToArray();  
            tokenBoard = new TokenBoard(tokenBoardArray, tokenBoardId, tokenBoardName, null, null, true);

            List<GSData> moveList = challenge.ScriptData.GetGSDataList("moveList");

            int currentPlayerMove = challenge.ScriptData.GetInt("currentPlayerMove").GetValueOrDefault();
            bool isPlayerOneTurn = currentPlayerMove == (int)Piece.BLUE ? true : false;

            GameState gameState = new GameState(Constants.numRows, Constants.numColumns, GameType.RANDOM, isPlayerOneTurn, true, tokenBoard, lastGameboard, false, ConvertMoveList(moveList));

            Opponent opponent = new Opponent(challenge.Challenger.Id, challenge.Challenger.Name, challenge.Challenger.ExternalIds.GetString("FB"));

            Game game = new Game(challenge.ChallengeId, gameState, false, false, false, opponent, ChallengeState.RUNNING, ChallengeType.STANDARD, challengerGamePieceId, challengedGamePieceId, null, winnerName, null, null, true);
            GameManager.Instance.AddGame(game);
            GameManager.Instance.OpenGame(game);

            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            customAttributes.Add("ChallengeId", challenge.ChallengeId);
            customAttributes.Add("PlayerName", UserManager.Instance.userName);
            customAttributes.Add("OpponentName", challenge.Challenger.Name);
            customAttributes.Add("TokenBoardId", tokenBoardId);
            customAttributes.Add("TokenBoardName", tokenBoardName);
            AnalyticsManager.LogCustom("joined_multiplayer_game", customAttributes);
        }

        public List<Move> ConvertMoveList(List<GSData> moveList) {
            List<Move> newMoveList = new List<Move>();
            for (int i = 0; i < moveList.Count; i++)
            {
                int position = moveList[i].GetInt("position").GetValueOrDefault();
                Direction direction = (Direction)moveList[i].GetInt("direction").GetValueOrDefault();
                PlayerEnum player = (PlayerEnum)moveList[i].GetInt("player").GetValueOrDefault();
                long timestamp = moveList[i].GetLong("timestamp").GetValueOrDefault();
                Move move = new Move(position, direction, player);
                move.timeStamp = timestamp;
                newMoveList.Add(move);
            }

            return newMoveList;
        }

        public void GetChallengesRequest()
        {
            if (gettingChallenges)
            {
                return;
            }

            gettingChallenges = true;

            List<string> challengeStates = new List<string> { "RUNNING", "COMPLETE", "ISSUED" };
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

            //We send a ListChallenge Request with the shortcode of our challenge, we set this in our GameSparks Portal
            new ListChallengeRequest()
                .SetShortCode("chalRanked")
                .SetMaxResponseTimeInMillis(20000)
                .SetStates(challengeStates)
                .SetEntryCount(50) // get 50 games
                .Send(((response) =>
                {
                    if (response.HasErrors)
                    {
                        Debug.Log("***** Error Listing Challenge Request: " + response.Errors.JSON);
                        AnalyticsManager.LogError("list_challenge_request_error", response.Errors.JSON);
                        return;
                    }

                    List<Game> games = new List<Game>();

                    foreach (var gsChallenge in response.ChallengeInstances)
                    {
                        bool didViewResult = false;
                        List<string> playersViewedResult = gsChallenge.ScriptData.GetStringList("playersViewedResult");
                        if (playersViewedResult != null)
                        {
                            for (int i = 0; i < playersViewedResult.Count; i++)
                            {
                                if (playersViewedResult[i] == UserManager.Instance.userId)
                                {
                                    didViewResult = true;
                                    break;
                                }
                            }
                        }

                        ChallengeState challengeState = ChallengeState.NONE;
                        if (gsChallenge.State == "RUNNING")
                        {
                            challengeState = ChallengeState.RUNNING;
                        }
                        else if (gsChallenge.State == "ISSUED")
                        {
                            challengeState = ChallengeState.ISSUED;
                        }
                        else if (gsChallenge.State == "COMPLETE")
                        {
                            challengeState = ChallengeState.COMPLETE;
                        }

                        bool isCurrentPlayerTurn = false;
                        if (challengeState == ChallengeState.RUNNING || challengeState == ChallengeState.ISSUED)
                        {
                            if (gsChallenge.NextPlayer == UserManager.Instance.userId)
                            {
                                isCurrentPlayerTurn = true;
                            }
                            else
                            {
                                isCurrentPlayerTurn = false;
                            }
                        }

                        bool isCurrentPlayer_PlayerOne = false;
                        if (gsChallenge.Challenger.Id == UserManager.Instance.userId)
                        {
                            isCurrentPlayer_PlayerOne = true;
                        }
                        else
                        {
                            isCurrentPlayer_PlayerOne = false;
                        }

                        string winnerName = gsChallenge.ScriptData.GetString("winnerName");
                        string winnerId = gsChallenge.ScriptData.GetString("winnerId");
                        string winnerString = gsChallenge.ScriptData.GetString("winner");
                        bool isExpired = false;
                        if (winnerId == null && winnerString == null && challengeState == ChallengeState.COMPLETE)
                        {
                            isExpired = true;
                        }

                        PlayerEnum winner = PlayerEnum.EMPTY;
                        if (winnerId != null)
                        {
                            if (winnerId == gsChallenge.Challenger.Id)
                            {
                                winner = PlayerEnum.ONE;
                            }
                            else
                            {
                                winner = PlayerEnum.TWO;
                            }
                        }
                        if (winnerString == "Draw")
                        {
                            winner = PlayerEnum.NONE;
                        }

                        string opponentFBId = "";
                        string opponentName = LocalizationManager.Instance.GetLocalizedValue("waiting_opponent_text");
                        string opponentId = "";

                        if (gsChallenge.Accepted.Count() > 1)
                        {
                            if (gsChallenge.Accepted.ElementAt(0).Id == UserManager.Instance.userId)
                            {
                                opponentId = gsChallenge.Accepted.ElementAt(1).Id;
                                opponentFBId = gsChallenge.Accepted.ElementAt(1).ExternalIds.GetString("FB");
                                opponentName = gsChallenge.Accepted.ElementAt(1).Name;
                            }
                            else
                            {
                                opponentId = gsChallenge.Accepted.ElementAt(0).Id;
                                opponentFBId = gsChallenge.Accepted.ElementAt(0).ExternalIds.GetString("FB");
                                opponentName = gsChallenge.Accepted.ElementAt(0).Name;
                            }
                        }

                        Opponent opponent = new Opponent(opponentId, opponentName, opponentFBId);

                        ChallengeType challengeType = ChallengeType.NONE;
                        if (gsChallenge.ShortCode == "chalRanked")
                        {
                            challengeType = ChallengeType.STANDARD;
                        }
                        else if (gsChallenge.ShortCode == "tournamentChallenge")
                        {
                            challengeType = ChallengeType.TOURNAMENT;
                        }
                        else
                        {
                            challengeType = ChallengeType.STANDARD;
                        }


                    GameSparksChallenge gameSparksChallenge = new GameSparksChallenge(gsChallenge);

                    // TODO: use gameSparksChallenge.gameType
                        GameState gameState = new GameState(Constants.numRows,
                                                            Constants.numColumns,
                                                            GameType.RANDOM,
                                                            gameSparksChallenge.isPlayerOneTurn,
                                                            isCurrentPlayerTurn,
                                                            gameSparksChallenge.isGameOver,
                                                            gameSparksChallenge.tokenBoard,
                                                            winner,
                                                            ConvertMoveList(gameSparksChallenge.moveList),
                                                            gameSparksChallenge.previousGameboardData);

                    string challengerGamePieceId = gameSparksChallenge.challengerGamePieceId;
                    string challengedGamePieceId = gameSparksChallenge.challengedGamePieceId;

                        Game game = new Game(gsChallenge.ChallengeId,
                                                 gameState,
                                                 isCurrentPlayer_PlayerOne,
                                                 isExpired,
                                                 didViewResult,
                                                 opponent,
                                                 challengeState,
                                                 challengeType,
                                                 challengerGamePieceId,
                                                 challengedGamePieceId,
                                                 null,
                                                 winnerName, null, null, false);
                        game.gameState.SetRandomGuid(gsChallenge.ChallengeId);

                        if (challengeState == ChallengeState.COMPLETE)
                        {
                            game.challengerRatingDelta = gsChallenge.ScriptData.GetInt("challengedRatingDelta").GetValueOrDefault();
                            game.challengedRatingDelta = gsChallenge.ScriptData.GetInt("challengedRatingDelta").GetValueOrDefault();
                        }

                        games.Add(game);
                    }

                    GameManager.Instance.Games = games;

                    //GameManager.Instance.UpdatePlayButtonBadgeCount();

                    gettingChallenges = false;

                    stopwatch.Stop();
                    Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                    customAttributes.Add("endtime", stopwatch.Elapsed);
                    AnalyticsManager.LogCustom("ListChallengeRequest_response_endtime", customAttributes);
                        //Debug.Log("Time elapsed at response end: " + stopwatch.Elapsed);
                        stopwatch.Reset();

                }));
        }
    }
}