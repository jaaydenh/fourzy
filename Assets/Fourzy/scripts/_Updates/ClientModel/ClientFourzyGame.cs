//@vadym udod

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
    public class ClientFourzyGame : FourzyGame, RewardsManager.IRewardTestSubject, IClientFourzy
    {
        public Action<int, int> onMagic { get; set; }

        public GameState _State => State;
        public GameState FirstState { get; private set; }

        public List<PlayerTurn> _playerTurnRecord => playerTurnRecord;

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
        public GameType _Type { get; set; }
        public float initializedTime { get; set; }
        public string GameID { get; set; }

        public GamePiecePrefabData playerOnePrefabData { get; set; }
        public GamePiecePrefabData playerTwoPrefabData { get; set; }

        public GameBoardDefinition gameboardDefinition { get; private set; }

        public List<PlayerTurn> InitialTurns
        {
            get
            {
                List<PlayerTurn> result = new List<PlayerTurn>();

                foreach (SimpleMove move in gameboardDefinition.InitialMoves)
                    result.Add(new PlayerTurn(move));

                return result;
            }
        }

        //turn base data
        public ChallengeData challengeData { get; set; }

        public RewardsManager.RewardTestSubject asSubject
        {
            get => new RewardsManager.RewardTestSubject(
                GameID,
                State,
                new List<RewardsManager.CollectedItemReward>()
                {
                    new RewardsManager.CollectedItemReward("3", 300, RewardsManager.CollectedItemType.COINS),
                    new RewardsManager.CollectedItemReward("1", 4, RewardsManager.CollectedItemType.TICKETS),
                },
                gameDuration,
                me.PlayerId);
        }

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

        public Piece activePlayerPiece => new Piece(State.ActivePlayerId);

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
        public Player me => State.Players.Values.ToList().Find(_player => _player.PlayerString == UserManager.Instance.userId) ?? State.Players[1];

        //opposite of "me"
        public Player opponent => State.Players[(PlayerEnum)me.PlayerId == PlayerEnum.ONE ? (int)PlayerEnum.TWO : (int)PlayerEnum.ONE];

        public Player activePlayer => isMyTurn ? me : opponent;

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
            //area = gameStateData.GameBoardData.Area;

            Initialize();
        }

        public ClientFourzyGame(Player Player1, Player Player2, GameOptions Options = null) : base(Player1, Player2, Options)
        {
            Initialize();
        }

        public ClientFourzyGame(Area Area, Player Player1, Player Player2, int FirstPlayerId, GameOptions Options = null) : base(Area, Player1, Player2, FirstPlayerId, Options)
        {
            //area = Area;

            Initialize();
        }

        public ClientFourzyGame(GameBoardDefinition definition, Player Player1, Player Player2) : base(definition, Player1, Player2)
        {
            gameboardDefinition = definition;
            //area = definition.Area;

            Initialize();
        }

        public ClientFourzyGame(string boardJson) : base(boardJson)
        {
            Initialize();
        }

        public bool IsWinner(Player player) => State.WinnerId == player.PlayerId;

        public bool IsWinner() => me.PlayerId == State.WinnerId;

        public PlayerTurnResult TakeTurn(PlayerTurn playerTurn, params object[] extraValues)
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

            PlayerTurnResult turnResult = base.TakeTurn(playerTurn, true);

            if (!(bool)extraValues[0])
            {
                //if its a turn-based game, update server with PlayerTurn data
                switch (_Type)
                {
                    case Fourzy.GameType.TURN_BASED:
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
                                    AnalyticsManager.LogError("take_turn_error", response.Errors.JSON);

                                    MenuController.GetMenu("GameSceneCanvas").GetScreen<PromptScreen>().Prompt("Move failed!", response.Errors.JSON, null, "OK", null);
                                }
                                else
                                {
                                    Debug.Log("Take Turn Success");
                                }
                            });


                        //if user just made a turn, manually update this game without waiting for server response
                        challengeData.playerTurnRecord.Add(playerTurn);
                        challengeData.UpdateLastTurnGame();

                        ChallengeManager.OnChallengeUpdateLocal.Invoke(challengeData);
                        break;
                }
            }

            return turnResult;
        }

        public PlayerTurnResult TakeTurn(Direction direction, int location, params object[] extraValues) => TakeTurn(new PlayerTurn(State.ActivePlayerId, direction, location), extraValues);

        public void SetInitialTime(float value)
        {
            initializedTime = value;
        }

        public void AddPlayerMagic(int playerId, int value)
        {
            State.Players[playerId].Magic += value;

            onMagic?.Invoke(playerId, State.Players[playerId].Magic);
        }

        public GameState Reset()
        {
            State = new GameState(FirstState);

            return State;
        }

        private void Initialize()
        {
            FirstState = new GameState(State);

            //adjust active player ID
            switch (_Type)
            {
                case Fourzy.GameType.PASSANDPLAY:
                case Fourzy.GameType.AI:
                    State.ActivePlayerId = UnityEngine.Random.value > .5f ? 1 : 2;
                    break;
            }
        }

        private void AssignPrefabs()
        {
            if (playerOnePrefabData == null)
            {
                int player1HerdId = 0;
                if (State.Players[1].HerdId != null)
                {
                    player1HerdId = (int)float.Parse(State.Players[1].HerdId);
                }
                if (State.Players.ContainsKey(1) && !string.IsNullOrEmpty(State.Players[1].HerdId))
                    playerOnePrefabData = GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(player1HerdId);
                else
                    playerOnePrefabData = GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(UserManager.Instance.gamePieceID);
            }

            if (playerTwoPrefabData == null)
            {
                int player2HerdId = 0;
                if (State.Players[2].HerdId != null)
                {
                    player2HerdId = (int)float.Parse(State.Players[2].HerdId);
                }

                if (State.Players.ContainsKey(2) && !string.IsNullOrEmpty(State.Players[2].HerdId))
                    playerTwoPrefabData = GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(player2HerdId);
                else
                    playerTwoPrefabData = GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(UserManager.Instance.gamePieceID);
            }
        }
    }
}
