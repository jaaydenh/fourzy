//modded

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics;
using FourzyGameModel.Model;
using GameSparks.Api.Messages;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fourzy
{
    [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene)]
    public class ChallengeManager : UnitySingleton<ChallengeManager>
    {
        public static Action<List<ChallengeData>> OnChallengesUpdate;

        //when challenge update received from a server
        public static Action<ChallengeData> OnChallengeUpdate;
        //when challenge was modified localy
        public static Action<ChallengeData> OnChallengeUpdateLocal;

        public static Action<string, string> OnReceivedOpponentGamepiece;
        public static Action<bool> OnFindChallengeResult;
        public static Action<int> OnReceivedRatingDelta;

        public static Action<ChallengeTurnTakenMessage> OnChallengeTakeTurn;
        public static Action<ChallengeJoinedMessage> OnChallengeJoined;
        public static Action<ChallengeWonMessage> OnChallengeWon;
        public static Action<ChallengeLostMessage> OnChallengeLost;
        public static Action<ChallengeIssuedMessage> OnChallengeIssued;
        public static Action<ChallengeDrawnMessage> OnChallengeDrawn;
        public static Action<ChallengeStartedMessage> OnChallengeStarted;

        public double daysUntilChallengeExpires = 60;

        private List<ChallengeData> challenges = new List<ChallengeData>();
        private bool gettingChallenges = false;

        public List<ChallengeData> Challenges
        {
            get
            {
                return challenges;
            }
            set
            {
                challenges = value;

                OnChallengesUpdate?.Invoke(challenges);
            }
        }

        public List<ChallengeData> NextChallenges => challenges.Where(challenge => challenge.canBeNext).ToList();

        protected void OnEnable()
        {
            ChallengeTurnTakenMessage.Listener += _OnChallengeTurnTaken;
            ChallengeJoinedMessage.Listener += _OnChallengeJoined;
            ChallengeWonMessage.Listener += _OnChallengeWon;
            ChallengeLostMessage.Listener += _OnChallengeLost;
            ChallengeIssuedMessage.Listener += _OnChallengeIssued;
            ChallengeDrawnMessage.Listener += _OnChallengeDrawn;
            ChallengeStartedMessage.Listener += _OnChallengeStarted;
        }

        protected void OnDisable()
        {
            ChallengeTurnTakenMessage.Listener -= _OnChallengeTurnTaken;
            ChallengeJoinedMessage.Listener -= _OnChallengeJoined;
            ChallengeWonMessage.Listener -= _OnChallengeWon;
            ChallengeLostMessage.Listener -= _OnChallengeLost;
            ChallengeDrawnMessage.Listener -= _OnChallengeDrawn;
            ChallengeIssuedMessage.Listener -= _OnChallengeIssued;
            ChallengeStartedMessage.Listener -= _OnChallengeStarted;
        }

        public void SubmitPuzzleCompleted()
        {
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

        public void GetGamePiece(string userId, Action<LogEventResponse> successCallback, Action<LogEventResponse> errorCallback)
        {
            if (userId != null && userId != "")
            {
                new LogEventRequest().SetEventKey("getOpponentGamePiece")
                .SetEventAttribute("userId", userId)
                .SetDurable(true)
                .Send(successCallback, errorCallback);
            }
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

                        OnReceivedOpponentGamepiece?.Invoke(gamePieceId, challengeId);
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
                        OnReceivedRatingDelta?.Invoke(ratingDelta);
                    }
                });
        }

        public void Resign(string challengeInstanceId)
        {
            new LogEventRequest().SetEventKey("resign")
                .SetEventAttribute("challengeInstanceId", challengeInstanceId)
                .SetDurable(true)
                .Send((response) =>
                {
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

        private void RemoveGame(string challengeInstanceId)
        {
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

        public void CreateTurnBasedGame(string oppponentID, Area selectedArea, Action<LogEventResponse> successCallback, Action<LogEventResponse> errorCallback)
        {
            new LogEventRequest().SetEventKey("createTurnBased")
                // .SetDurable(true)
                .SetMaxResponseTimeInMillis(15000)
                .SetEventAttribute("opponentId", oppponentID)
                .SetEventAttribute("area", (long)selectedArea)
                .Send(successCallback, errorCallback);
        }

        public void SetViewedCompletedGame(string challengeInstanceId)
        {
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

        public void StartMatchmaking()
        {
            new LogEventRequest().SetEventKey("startMatchmaking")
                .SetEventAttribute("matchShortCode", "matchRanked")
                .Send((response) =>
                    {
                        if (response.HasErrors)
                        {
                            Debug.Log(response.Errors.JSON);
                        }
                    });
        }

        public void FindRandomChallenge(Action<FindChallengeResponse> successCallback, Action<FindChallengeResponse> errorCallback)
        {
            //tokenBoard = null;

            //new FindChallengeRequest()
            //    .SetAccessType("PUBLIC")
            //    .SetCount(50)
            //    .SetMaxResponseTimeInMillis(17000)
            //    .Send(successCallback, errorCallback);
        }

        public void JoinChallenge(string challengeInstanceId, Action<JoinChallengeResponse> successCallback, Action<JoinChallengeResponse> errorCallback)
        {
            new JoinChallengeRequest()
                .SetChallengeInstanceId(challengeInstanceId)
                .SetMaxResponseTimeInMillis(17000)
                .Send(successCallback, errorCallback);
        }

        public void OpenNewRealtimeGame(int firstPlayerPeerId, int seed)
        {
            //bool isFirstPlayer = false;
            //if (RealtimeManager.Instance.GetRTSession().PeerId == firstPlayerPeerId)
            //{
            //    isFirstPlayer = true;
            //}

            //string opponentId = string.Empty;
            //string opponentName = string.Empty;

            //for (int playerIndex = 0; playerIndex < RealtimeManager.Instance.GetSessionInfo().GetPlayerList().Count; playerIndex++)
            //{ // loop through all players
            //    // If the player's peerId is the same as this player's peerId then we know this is the player and we can setup players and opponents //
            //    if (RealtimeManager.Instance.GetSessionInfo().GetPlayerList()[playerIndex].peerId == RealtimeManager.Instance.GetRTSession().PeerId)
            //    {
            //        // Current player
            //        string playerId = RealtimeManager.Instance.GetSessionInfo().GetPlayerList()[playerIndex].id;
            //        string playerName = RealtimeManager.Instance.GetSessionInfo().GetPlayerList()[playerIndex].displayName;
            //    }
            //    else
            //    {
            //        // The opponent
            //        opponentId = RealtimeManager.Instance.GetSessionInfo().GetPlayerList()[playerIndex].id;
            //        opponentName = RealtimeManager.Instance.GetSessionInfo().GetPlayerList()[playerIndex].displayName;
            //    }
            //}

            //GetGamePiece(opponentId, GetGamePieceIdSuccess, GetGamePieceIdFailure);
            //Opponent opponent = new Opponent(opponentId, opponentName, null);

            //// tokenBoard = TokenBoardLoader.Instance.GetRandomTokenBoardByIndex(tokenBoardIndex);
            //tokenBoard = TokenBoardLoader.Instance.GetRandomTokenBoard(GameType.REALTIME, seed);

            //GameState gameState = new GameState(Constants.numRows, Constants.numColumns, GameType.REALTIME, true, isFirstPlayer, tokenBoard, tokenBoard.initialGameBoard, false, null, seed);

            //Game game = new Game(null, gameState, isFirstPlayer, false, false, opponent, ChallengeState.NONE, ChallengeType.STANDARD, UserManager.Instance.gamePieceID.ToString(), "0", null, null, null, null, true);
            //GameManager.Instance.activeGame = game;


            //Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            //customAttributes.Add("PlayerName", UserManager.Instance.userName);
            //customAttributes.Add("TokenBoardId", tokenBoard.id);
            //customAttributes.Add("TokenBoardName", tokenBoard.name);
            //AnalyticsManager.LogCustom("open_new_realtime_game", customAttributes);
        }

        private void GetGamePieceIdSuccess(LogEventResponse response)
        {
            //Debug.Log("GetGamePieceIdSuccess: " + response.ScriptData);
            //int gamePieceId = 0;

            //if (response.ScriptData != null)
            //{
            //    var gamePieceIdString = response.ScriptData.GetString("gamePieceId");
            //    gamePieceId = int.Parse(gamePieceIdString);
            //}

            //GameManager.Instance.activeGame.opponent.gamePieceId = gamePieceId;
            //GameManager.Instance.OpenGame(GameManager.Instance.activeGame);
        }

        private void GetGamePieceIdFailure(LogEventResponse response)
        {
            //Debug.Log("***** Error getting player gamepiece: " + response.Errors.JSON);
            //AnalyticsManager.LogError("get_player_gamepiece_error", response.Errors.JSON);
            //GameManager.Instance.OpenGame(GameManager.Instance.activeGame);
        }

        public void OpenNewMultiplayerGame()
        {
            //Debug.Log("Open New Multiplayer Game");

            //if (tokenBoard == null)
            //{
            //    TokenBoard randomTokenBoard = TokenBoardLoader.Instance.GetRandomTokenBoard(GameType.RANDOM);
            //    tokenBoard = randomTokenBoard;
            //}

            //GameState gameState = new GameState(Constants.numRows, Constants.numColumns, GameType.RANDOM, true, true, tokenBoard, tokenBoard.initialGameBoard, false, null);
            //Game game = new Game(null, gameState, true, false, false, null, ChallengeState.NONE, ChallengeType.STANDARD, UserManager.Instance.gamePieceID.ToString(), "0", null, null, null, null, true);
            //GameManager.Instance.OpenGame(game);

            //Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            //customAttributes.Add("PlayerName", UserManager.Instance.userName);
            //customAttributes.Add("TokenBoardId", tokenBoard.id);
            //customAttributes.Add("TokenBoardName", tokenBoard.name);
            //AnalyticsManager.LogCustom("open_new_multiplayer_game", customAttributes);
        }

        public void OpenJoinedMultiplayerGame()
        {
            //Debug.Log("Open Joined Multiplayer Game");

            ////GameManager.Instance.opponentProfilePicture.sprite = Sprite.Create(defaultProfilePicture,
            ////    new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height),
            ////    new Vector2(0.5f, 0.5f));

            //string challengerGamePieceId = challenge.ScriptData.GetString("challengerGamePieceId");
            //string challengedGamePieceId = UserManager.Instance.gamePieceID.ToString();
            //string winnerName = challenge.ScriptData.GetString("winnerName");

            //string tokenBoardId = challenge.ScriptData.GetString("tokenBoardId");
            //string tokenBoardName = challenge.ScriptData.GetString("tokenBoardName");

            //int[] lastGameboard = new int[0];
            //if (challenge.ScriptData.GetIntList("lastGameBoard") != null)
            //{
            //    lastGameboard = challenge.ScriptData.GetIntList("lastGameBoard").ToArray();
            //}

            //int[] tokenBoardArray = Enumerable.Repeat(0, 64).ToArray();
            //tokenBoardArray = challenge.ScriptData.GetIntList("tokenBoard").ToArray();
            //tokenBoard = new TokenBoard(tokenBoardArray, tokenBoardId, tokenBoardName, null, null, true);

            //List<GSData> moveList = challenge.ScriptData.GetGSDataList("moveList");

            //int currentPlayerMove = challenge.ScriptData.GetInt("currentPlayerMove").GetValueOrDefault();
            //bool isPlayerOneTurn = currentPlayerMove == (int)Piece.BLUE ? true : false;

            //GameState gameState = new GameState(Constants.numRows, Constants.numColumns, GameType.RANDOM, isPlayerOneTurn, true, tokenBoard, lastGameboard, false, ConvertMoveList(moveList));

            //Opponent opponent = new Opponent(challenge.Challenger.Id, challenge.Challenger.Name, challenge.Challenger.ExternalIds.GetString("FB"));

            //Game game = new Game(challenge.ChallengeId, gameState, false, false, false, opponent, ChallengeState.RUNNING, ChallengeType.STANDARD, challengerGamePieceId, challengedGamePieceId, null, winnerName, null, null, true);
            //GameManager.Instance.AddGame(game);
            //GameManager.Instance.OpenGame(game);

            //Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            //customAttributes.Add("ChallengeId", challenge.ChallengeId);
            //customAttributes.Add("PlayerName", UserManager.Instance.userName);
            //customAttributes.Add("OpponentName", challenge.Challenger.Name);
            //customAttributes.Add("TokenBoardId", tokenBoardId);
            //customAttributes.Add("TokenBoardName", tokenBoardName);
            //AnalyticsManager.LogCustom("joined_multiplayer_game", customAttributes);
        }

        public void GetChallengeRequest(string challengeInstanceId, Action<GetChallengeResponse> successCallback, Action<GetChallengeResponse> errorCallback)
        {
            new GetChallengeRequest()
                .SetChallengeInstanceId(challengeInstanceId)
                .SetMaxResponseTimeInMillis(17000)
                .Send(successCallback, errorCallback);
        }

        public void GetChallengeRequest(string challengeInstanceId)
        {
            new GetChallengeRequest()
                .SetChallengeInstanceId(challengeInstanceId)
                .SetMaxResponseTimeInMillis(17000)
                .Send((response) =>
                {
                    CheckChallenge(response.Challenge.BaseData);
                }, 
                (_response) =>
                {
                    Debug.Log("***** Error Challenge Request: " + _response.Errors.JSON);
                });
        }

        public void GetChallengesRequest()
        {
            if (gettingChallenges || !NetworkAccess.ACCESS)
                return;

            gettingChallenges = true;

            List<string> challengeStates = new List<string> { "RUNNING" };
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

            //We send a ListChallenge Request with the shortcode of our challenge, we set this in our GameSparks Portal
            new ListChallengeRequest()
                .SetShortCode("chalRanked")
                .SetMaxResponseTimeInMillis(20000)
                .SetStates(challengeStates)
                .SetEntryCount(Constants.running_challenges_count)
                .Send(((response) =>
                {
                    if (response.HasErrors)
                    {
                        gettingChallenges = false;

                        Debug.Log("***** Error Listing Challenges Request: " + response.Errors.JSON);
                        AnalyticsManager.LogError("list_challenge_request_error", response.Errors.JSON);

                        GetChallengesRequest();
                        return;
                    }

                    List<ChallengeData> _challenges = new List<ChallengeData>();

                    Debug.Log($"Challenges found: {response.ChallengeInstances.Count()}");

                    foreach (var gsChallenge in response.ChallengeInstances)
                    {
                        if (!ChallengeData.ValidateGSData(gsChallenge.BaseData)) continue;

                        ChallengeData newChallenge = new ChallengeData(gsChallenge.BaseData);

                        if (newChallenge.Validate()) _challenges.Add(newChallenge);
                    }

                    gettingChallenges = false;

                    stopwatch.Stop();
                    Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                    customAttributes.Add("endtime", stopwatch.Elapsed);
                    AnalyticsManager.LogCustom("ListChallengeRequest_response_endtime", customAttributes);

                    stopwatch.Reset();

                    AddCompleteChallenges(_challenges);
                }));
        }

        private void AddCompleteChallenges(List<ChallengeData> runningChallenges)
        {
            if (gettingChallenges || !NetworkAccess.ACCESS)
                return;

            gettingChallenges = true;

            List<string> challengeStates = new List<string> { "COMPLETE" };
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

            //We send a ListChallenge Request with the shortcode of our challenge, we set this in our GameSparks Portal
            new ListChallengeRequest()
                .SetShortCode("chalRanked")
                .SetMaxResponseTimeInMillis(20000)
                .SetStates(challengeStates)
                .SetEntryCount(Constants.complete_challenges_count)
                .Send(((response) =>
                {
                    if (response.HasErrors)
                    {
                        gettingChallenges = false;

                        Debug.Log("***** Error Listing Complete Challenges Request: " + response.Errors.JSON);
                        AnalyticsManager.LogError("list_challenge_request_error", response.Errors.JSON);

                        //start over again
                        GetChallengesRequest();
                        return;
                    }

                    Debug.Log($"Complete challenges found: {response.ChallengeInstances.Count()}");

                    foreach (var gsChallenge in response.ChallengeInstances)
                    {
                        if (!ChallengeData.ValidateGSData(gsChallenge.BaseData)) continue;

                        ChallengeData completeChallenge = new ChallengeData(gsChallenge.BaseData);

                        if (completeChallenge.Validate()) runningChallenges.Add(completeChallenge);
                    }

                    Challenges = runningChallenges;

                    gettingChallenges = false;

                    stopwatch.Stop();
                    Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                    customAttributes.Add("endtime", stopwatch.Elapsed);
                    AnalyticsManager.LogCustom("ListChallengeRequest_response_endtime", customAttributes);

                    stopwatch.Reset();
                }));
        }

        private void _OnChallengeTurnTaken(ChallengeTurnTakenMessage message)
        {
            CheckChallenge(message.Challenge.BaseData);

            OnChallengeTakeTurn?.Invoke(message);
        }

        private void _OnChallengeJoined(ChallengeJoinedMessage message)
        {
            CheckChallenge(message.Challenge.BaseData);

            OnChallengeJoined?.Invoke(message);
        }

        private void _OnChallengeWon(ChallengeWonMessage message)
        {
            CheckChallenge(message.Challenge.BaseData);

            OnChallengeWon?.Invoke(message);
        }

        private void _OnChallengeLost(ChallengeLostMessage message)
        {
            CheckChallenge(message.Challenge.BaseData);

            OnChallengeLost?.Invoke(message);
        }

        private void _OnChallengeDrawn(ChallengeDrawnMessage message)
        {
            CheckChallenge(message.Challenge.BaseData);

            OnChallengeDrawn?.Invoke(message);
        }

        private void _OnChallengeStarted(ChallengeStartedMessage message)
        {
            CheckChallenge(message.Challenge.BaseData);

            OnChallengeStarted?.Invoke(message);
        }

        private void _OnChallengeIssued(ChallengeIssuedMessage message)
        {
            OnChallengeIssued?.Invoke(message);
        }

        private void CheckChallenge(GSData baseData)
        {
            //update game
            ChallengeData _challenge = new ChallengeData(baseData);
            ChallengeData toModify = challenges.Find(__challenge => __challenge.challengeInstanceId == _challenge.challengeInstanceId);

            //if for some reason this challenge is not on a local challenges list
            if (toModify == null)
            {
                challenges.Add(_challenge);

                OnChallengesUpdate?.Invoke(challenges);
            }
            else
            {
                challenges[challenges.IndexOf(toModify)] = _challenge;

                //check if updated game finished
                //if so, check if active game is the one that got update, so it can be set as viewed
                if (GameManager.Instance.activeGame != null && GameManager.Instance.activeGame._Type == GameType.TURN_BASED)
                {
                    if (GameManager.Instance.activeGame.GameID == _challenge.challengeInstanceId && _challenge.lastTurnGame.isOver)
                        PlayerPrefsWrapper.SetGameViewed(_challenge.challengeInstanceId);
                }

                OnChallengeUpdate?.Invoke(_challenge);
            }
        }
    }
}