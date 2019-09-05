﻿//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using FourzyGameModel.Model;
using GameSparks.Api.Requests;
using GameSparks.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.ClientModel
{
    public class ClientFourzyGame : FourzyGame, IClientFourzy
    {
        public Action<int, int> onMagic { get; set; }

        public GameState _State => State;
        public GameState _FirstState { get; set; }

        public List<PlayerTurn> _playerTurnRecord => playerTurnRecord;
        public List<PlayerTurn> _allTurnRecord { get; set; }
        public Dictionary<int, int> magic { get; set; }

        public bool isBoardRandom { get; private set; } = false;

        public Area _Area
        {
            get
            {
                if ((int)State.Board.Area > 1)
                    return State.Board.Area;
                else
                    return Area.TRAINING_GARDEN;
            }

            set { }
        }

        public GameStateDataEpoch toGameStateData
        {
            get
            {
                GameStateDataEpoch result = new GameStateDataEpoch();

                result.GameBoardData = State.Board.SerializeData();
                //get board
                result.Players = new Dictionary<int, Player>();
                //get players
                foreach (var player in State.Players) result.Players.Add(player.Key, new Player(player.Value));

                result.WinnerId = State.WinnerId;
                result.ActivePlayerId = 1;
                result.GameSeed = _Type.ToString();

                result.GameEffects = new List<GameEffect>();
                result.Herds = new Dictionary<int, Herd>();

                if (State.GameEffects != null) result.GameEffects = new List<GameEffect>(State.GameEffects);
                if (State.WinningLocations != null)
                {
                    result.WinningLocations = new List<BoardLocation>();
                    result.WinningLocations = new List<BoardLocation>(State.WinningLocations);
                }
                if (State.Herds != null) foreach (var herd in State.Herds) result.Herds.Add(herd.Key, herd.Value);

                return result;
            }
        }
        public GameType _Type
        {
            get => _type;

            set
            {
                _type = value;

                //force modify opponent HerdID for passplay games
                switch (_Type)
                {
                    case Fourzy.GameType.PASSANDPLAY:
                    case Fourzy.GameType.PRESENTATION:
                        switch (GameManager.Instance.characterType)
                        {
                            case GameManager.PassPlayCharactersType.SELECTED_RANDOM:
                                opponent.HerdId = GameContentManager.Instance.piecesDataHolder.random.data.ID;

                                break;

                            case GameManager.PassPlayCharactersType.RANDOM:
                                me.HerdId = GameContentManager.Instance.piecesDataHolder.random.data.ID;
                                opponent.HerdId = GameContentManager.Instance.piecesDataHolder.random.data.ID;

                                break;
                        }

                        break;
                }

                //adjust active player ID
                switch (_Type)
                {
                    case Fourzy.GameType.PASSANDPLAY:
                        State.ActivePlayerId = UnityEngine.Random.value > .5f ? 1 : 2;

                        break;
                }
            }
        }
        public float initializedTime { get; set; }
        public string GameID
        {
            get
            {
                if (string.IsNullOrEmpty(gameID)) gameID = Guid.NewGuid().ToString();

                return gameID;
            }

            set => gameID = value;
        }

        public GamePiecePrefabData playerOnePrefabData { get; set; }
        public GamePiecePrefabData playerTwoPrefabData { get; set; }

        private string gameID;
        private GameType _type;

        public List<SimpleMove> initialMoves { get; private set; } = new List<SimpleMove>();

        public List<PlayerTurn> InitialTurns
        {
            get
            {
                List<PlayerTurn> result = new List<PlayerTurn>();

                foreach (SimpleMove move in initialMoves) result.Add(new PlayerTurn(move));

                return result;
            }
        }

        //turn base data
        public ChallengeData challengeData { get; set; }

        //realtime data
        public RealtimeData realtimeData { get; set; }

        public List<RewardsManager.Reward> collectedItems { get; set; }

        public GamePieceView playerOneGamepiece
        {
            get
            {
                AssignPrefabs();

                return playerOnePrefabData.player1Prefab;
            }
        }

        public GamePieceView playerTwoGamepiece
        {
            get
            {
                AssignPrefabs();

                return playerOnePrefabData.data.ID == playerTwoPrefabData.data.ID ? playerTwoPrefabData.player2Prefab : playerTwoPrefabData.player1Prefab;
            }
        }

        public int Rows => State.Board.Rows;

        public int Columns => State.Board.Columns;

        public bool isOver => State.WinningLocations != null;

        public bool draw { get; set; }

        public bool hideOpponent { get; set; } = false;

        public Piece activePlayerPiece => new Piece(State.ActivePlayerId, string.IsNullOrEmpty(activePlayer.HerdId) ? 1 : int.Parse(activePlayer.HerdId));

        public List<BoardSpace> boardContent
        {
            get
            {
                List<BoardSpace> result = new List<BoardSpace>();

                for (int row = 0; row < Rows; row++)
                    for (int col = 0; col < Columns; col++)
                        result.Add(State.Board.ContentsAt(row, col));

                return result;
            }
        }

        public GamePieceView playerGamePiece => me.PlayerId == 1 ? playerOneGamepiece : playerTwoGamepiece;

        public GamePieceView opponentGamePiece => opponent.PlayerId == 1 ? playerOneGamepiece : playerTwoGamepiece;

        //will try to return Player with same playerID as this user, otherwise first player
        public Player me
        {
            get
            {
                switch (_Type)
                {
                    case Fourzy.GameType.REALTIME:
                        return State.Players.Values.ToList().Find(_player => _player.PlayerString == (PhotonNetwork.isMasterClient ? "1" : "2")) ?? State.Players[1];

                    default:
                        return State.Players.Values.ToList().Find(_player => _player.PlayerString == UserManager.Instance.userId) ?? State.Players[1];
                }
            }
        }

        //opposite of "me"
        public Player opponent => State.Players[(PlayerEnum)me.PlayerId == PlayerEnum.ONE ? (int)PlayerEnum.TWO : (int)PlayerEnum.ONE];

        public Player activePlayer => isMyTurn ? me : opponent;

        public ClientPuzzleData puzzleData { get; set; }

        public Player unactivePlayer => isMyTurn ? opponent : me;

        public bool isMyTurn => me.PlayerId == State.ActivePlayerId;

        public bool haveMoves => State.TurnCount > 0;

        public TurnEvaluator turnEvaluator => new TurnEvaluator(State);

        public float gameDuration
        {
            get
            {
                switch (_Type)
                {
                    case Fourzy.GameType.TURN_BASED:
                        return 0f;

                    case Fourzy.GameType.PASSANDPLAY:
                    case Fourzy.GameType.PUZZLE:
                    case Fourzy.GameType.AI:
                        return Time.time - initializedTime;

                    default:
                        return 0f;
                }
            }
        }

        public ClientFourzyGame asFourzyGame => this as ClientFourzyGame;

        public ClientFourzyPuzzle asFourzyPuzzle => null;

        public ClientFourzyGame() : base()
        {
            Initialize();
        }

        public ClientFourzyGame(GameState gameState) : base(gameState)
        {
            Initialize();
        }

        public ClientFourzyGame(GameStateData gameStateData) : base(gameStateData)
        {
            Initialize();

            //gamestateepoch means its realtime game
            if (gameStateData.GetType() == typeof(GameStateDataEpoch))
            {
                realtimeData = (gameStateData as GameStateDataEpoch).realtimeData;
                _Type = Fourzy.GameType.REALTIME;
            }
        }

        public ClientFourzyGame(Player Player1, Player Player2, GameOptions Options = null) : base(Player1, Player2, Options)
        {
            Initialize();

            isBoardRandom = true;
        }

        public ClientFourzyGame(Area Area, Player Player1, Player Player2, int FirstPlayerId, GameOptions Options = null) : base(Area, Player1, Player2, FirstPlayerId, Options)
        {
            Initialize();

            isBoardRandom = true;
        }

        public ClientFourzyGame(GameBoardDefinition definition, Player Player1, Player Player2) : base(definition, Player1, Player2)
        {
            initialMoves = new List<SimpleMove>(definition.InitialMoves);
            GameID = definition.ID;

            Initialize();
        }

        public ClientFourzyGame(string boardJson) : base(boardJson)
        {
            Initialize();
        }

        public ClientFourzyGame(Area Area, BossType Boss, Player Player) : base(Area, Boss, Player)
        {
            Initialize();

            isBoardRandom = true;
        }

        public ClientFourzyGame(GameBoardDefinition definition, BossType aiBoss, Player player) : base(definition, aiBoss, player)
        {
            Initialize();

            _Type = Fourzy.GameType.AI;
        }

        public ClientFourzyGame(GameBoardDefinition definition, AIProfile aiProfile, Player player) : base(definition, aiProfile, player)
        {
            Initialize();

            _Type = Fourzy.GameType.AI;
        }

        public bool IsWinner(Player player) => State.WinnerId == player.PlayerId;

        public bool IsWinner() => me.PlayerId == State.WinnerId;

        public PlayerTurnResult TakeTurn(PlayerTurn playerTurn, bool local, bool returnStartOfNextTurn)
        {
            if (State.Players.ContainsKey(State.ActivePlayerId))
            {
                playerTurn.PlayerString = State.Players[State.ActivePlayerId].PlayerString ?? "";
            }
            else
            {
                playerTurn.PlayerString = "Null";
                Debug.Log("Active PlayerId is not present in Player Dictionary: " + State.ActivePlayerId + " GameSeed: " + State.GameSeed);
            }

            PlayerTurnResult turnResult = TakeTurn(playerTurn, returnStartOfNextTurn);

            if (!local)
            {
                //if its a turn-based game, update server with PlayerTurn data
                switch (_Type)
                {
                    case Fourzy.GameType.TURN_BASED:
                        Debug.Log(JsonConvert.SerializeObject(playerTurn));
                        new LogChallengeEventRequest().SetEventKey("takeTurnNew")
                            .SetChallengeInstanceId(challengeData.challengeInstanceId)
                            .SetEventAttribute("playerTurn", new GSRequestData(JsonConvert.SerializeObject(playerTurn)))
                            .SetDurable(true)
                            .Send(response =>
                            {
                                if (response.HasErrors)
                                {
                                    Debug.Log("***** Error taking turn: " + response.Errors.JSON);
                                    Debug.Log("Player Turn: " + JsonConvert.SerializeObject(playerTurn));
                                    Debug.Log("GameStateData: " + JsonConvert.SerializeObject(State.SerializeData()));

                                    AnalyticsManager.Instance.LogError(response.Errors.JSON, AnalyticsManager.AnalyticsErrorType.turn_based);

                                    MenuController.GetMenu("GameSceneCanvas").GetScreen<PromptScreen>().Prompt("Move failed!", response.Errors.JSON, null, "OK", null);
                                }
                                else
                                {
                                    Debug.Log("Take Turn Success");

                                    AnalyticsManager.Instance.LogGameEvent(AnalyticsManager.AnalyticsGameEvents.TAKE_TURN, this);
                                }
                            });


                        //if user just made a turn, manually update this game without waiting for server response
                        challengeData.playerTurnRecord.Add(playerTurn);
                        challengeData.UpdateLastTurnGame();

                        ChallengeManager.OnChallengeUpdateLocal.Invoke(challengeData);

                        break;

                    case Fourzy.GameType.REALTIME:
                        //notify other about this turn
                        PhotonNetwork.RaiseEvent(Constants.TAKE_TURN, JsonConvert.SerializeObject(playerTurn), true, new RaiseEventOptions() { Receivers = ReceiverGroup.Others });

                        break;
                }
            }

            return turnResult;
        }

        public override PlayerTurnResult TakeTurn(PlayerTurn Turn, bool ReturnStartOfNextTurn = false)
        {
            _allTurnRecord.Add(Turn);

            return base.TakeTurn(Turn, ReturnStartOfNextTurn);
        }

        public override PlayerTurnResult TakeAITurn(bool ReturnStartOfNextTurn = false)
        {
            PlayerTurnResult result = base.TakeAITurn(ReturnStartOfNextTurn);

            _allTurnRecord.Add(result.Turn);

            switch (_Type)
            {
                //AI turn played on realtime mode it means that players timer is empty, send this turn to another player too
                case Fourzy.GameType.REALTIME:
                    //notify other about this turn
                    PhotonNetwork.RaiseEvent(Constants.TAKE_TURN, JsonConvert.SerializeObject(result.Turn), true, new RaiseEventOptions() { Receivers = ReceiverGroup.Others });

                    break;
            }

            return result;
        }

        public void SetInitialTime(float value)
        {
            initializedTime = value;
        }

        public void AddPlayerMagic(int playerId, int value)
        {
            magic[playerId] += value;

            onMagic?.Invoke(playerId, magic[playerId]);
        }

        public void OnVictory()
        {
            if (puzzleData && IsWinner())
            {
                if (puzzleData.pack)
                {
                    bool _complete = puzzleData.pack.complete;

                    PlayerPrefsWrapper.SetPuzzleChallengeComplete(GameID, true);

                    if (puzzleData.pack.complete && !_complete) puzzleData.pack.justFinished = true;
                }

                //assign rewards if any
                puzzleData.AssignPuzzleRewards();
            }
        }

        public void OnDraw()
        {
            draw = true;
        }

        /// <summary>
        /// Only work with passplay boards for now
        /// </summary>
        /// <returns></returns>
        public IClientFourzy Next()
        {
            if (puzzleData)
            {
                if (puzzleData.pack)
                    return puzzleData.pack.Next(this);
                else
                    return GameContentManager.Instance.GetFastPuzzle(puzzleData.ID);
            }

            switch (_Type)
            {
                case Fourzy.GameType.PASSANDPLAY:
                    GameBoardDefinition _gameBoardDefinition = null;

                    //get next board
                    int index = GameContentManager.Instance.passAndPlayDataHolder.gameboards.IndexOf(GameContentManager.Instance.passAndPlayDataHolder.gameboards.Find(board => board.ID == GameID));

                    if (index < GameContentManager.Instance.passAndPlayDataHolder.gameboards.Count && index >= 0)
                        _gameBoardDefinition = GameContentManager.Instance.passAndPlayDataHolder.gameboards[index + 1];
                    else
                        _gameBoardDefinition = GameContentManager.Instance.passAndPlayDataHolder.gameboards[0];

                    return new ClientFourzyGame(_gameBoardDefinition, UserManager.Instance.meAsPlayer, new Player(2, "Player Two")) { _Type = Fourzy.GameType.PASSANDPLAY };
            }

            return null;
        }

        public PlayerTurnResult StartTurn() => StartTurn(_State);

        public GameState Reset()
        {
            State = new GameState(_FirstState);

            Initialize(false);

            return State;
        }

        public static ClientFourzyGame FromPuzzleData(ClientPuzzleData puzzleData)
        {
            ClientFourzyGame game = null;
            switch (puzzleData.pack.packType)
            {
                case PuzzlePacksDataHolder.PackType.AI_PACK:
                    game = new ClientFourzyGame(puzzleData.gameBoardDefinition, puzzleData.aiProfile, UserManager.Instance.meAsPlayer);

                    break;

                default:
                    game = new ClientFourzyGame(puzzleData.gameBoardDefinition, puzzleData.aiBoss, UserManager.Instance.meAsPlayer);

                    break;
            }

            game.puzzleData = puzzleData;
            game.GameID = puzzleData.ID;
            game._State.ActivePlayerId = puzzleData.firstTurn < 1 ? game.me.PlayerId : puzzleData.firstTurn; 

            game.opponent.HerdId = puzzleData.PuzzlePlayer.HerdId;

            return game;
        }

        private void Initialize(bool resetFirstState = true)
        {
            if (resetFirstState) _FirstState = new GameState(State);

            collectedItems = new List<RewardsManager.Reward>();
            _allTurnRecord = new List<PlayerTurn>();

            //assign magic values
            magic = new Dictionary<int, int>();
            foreach (KeyValuePair<int, Player> player in State.Players) magic.Add(player.Key, player.Value.Magic);
        }

        private void AssignPrefabs()
        {
            if (playerOnePrefabData == null)
            {
                string player1HerdId = "1";
                if (State.Players[1].HerdId != null) player1HerdId = State.Players[1].HerdId;

                if (State.Players.ContainsKey(1) && !string.IsNullOrEmpty(State.Players[1].HerdId))
                    playerOnePrefabData = GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(player1HerdId);
                else
                    playerOnePrefabData = GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(UserManager.Instance.gamePieceID);
            }

            if (playerTwoPrefabData == null)
            {
                string player2HerdId = "1";
                if (State.Players[2].HerdId != null) player2HerdId = State.Players[2].HerdId;

                if (State.Players.ContainsKey(2) && !string.IsNullOrEmpty(State.Players[2].HerdId))
                    playerTwoPrefabData = GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(player2HerdId);
                else
                    playerTwoPrefabData = GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(UserManager.Instance.gamePieceID);
            }
        }
    }
}