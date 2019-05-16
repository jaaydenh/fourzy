//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.Serialized;
using FourzyGameModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Fourzy._Updates.Serialized.PuzzlePacksDataHolder;
using GameType = Fourzy.GameType;

namespace Fourzy._Updates.ClientModel
{
    public class ClientFourzyPuzzle : FourzyPuzzle, IClientFourzy
    {
        public Action<int, int> onMagic { get; set; }

        public GameState _State => State;

        public List<PlayerTurn> _playerTurnRecord => playerTurnRecord;

        public Area _Area
        {
            get
            {
                if (State.Board.Area != Area.NONE)
                    return State.Board.Area;
                else
                    return Area.TRAINING_GARDEN;
            }

            set { }
        }

        public GameType _Type
        {
            get => GameType.PUZZLE;
            set { }
        }

        public string GameID
        {
            get => _data.ID;
            set { }
        }

        public float initializedTime { get; set; }

        public GamePiecePrefabData playerOnePrefabData { get; set; }
        public GamePiecePrefabData playerTwoPrefabData { get; set; }

        public List<PlayerTurn> InitialTurns => _data.InitialMoves;

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

        public PuzzleData _data { get; private set; }

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

        public bool isOver => Status == PuzzleStatus.SUCCESS || Status == PuzzleStatus.FAILED || playerTurnRecord.Count == _data.MoveLimit;

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

        public Player me => State.Players.Values.ToList().Find(_player => _player.PlayerString == UserManager.Instance.userId) ?? State.Players[1];

        public Player opponent => State.Players[(PlayerEnum)me.PlayerId == PlayerEnum.ONE ? (int)PlayerEnum.TWO : (int)PlayerEnum.ONE];

        public Player activePlayer => isMyTurn ? me : opponent;

        public Player unactivePlayer => isMyTurn ? opponent : me;

        public bool isMyTurn => me.PlayerId == State.ActivePlayerId;

        public float gameDuration => Time.time - initializedTime;

        public TurnEvaluator turnEvaluator => new TurnEvaluator(State);

        public ClientFourzyGame asFourzyGame => null;

        public ClientFourzyPuzzle asFourzyPuzzle => this as ClientFourzyPuzzle;

        public ClientFourzyPuzzle(PuzzleData Data) : base(Data, UserManager.Instance.meAsPlayer)
        {
            _data = Data;
            GameID = _data.ID;
        }

        public bool IsWinner(Player player) => State.WinnerId == player.PlayerId;

        public bool IsWinner() => me.PlayerId == State.WinnerId;

        public PuzzlePack puzzlePack { get; set; }

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

        public ClientFourzyPuzzle Next() => puzzlePack.Next(this);

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