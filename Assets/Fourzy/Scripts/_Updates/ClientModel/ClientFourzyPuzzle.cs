//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.Serialized;
using FourzyGameModel.Model;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.ClientModel
{
    public class ClientFourzyPuzzle : FourzyPuzzle, IClientFourzy
    {
        public Action<int, int> onMagic { get; set; }

        public GameState _State
        {
            get => State;
            set => State = value;
        }
        public GameState _FirstState { get; set; }

        public List<PlayerTurn> _playerTurnRecord => playerTurnRecord;
        public List<PlayerTurn> _allTurnRecord { get; set; }
        public Dictionary<int, int> magic { get; set; }

        public Area _Area
        {
            get
            {
                if (State.Board.Area != Area.NONE)
                {
                    return State.Board.Area;
                }
                else
                {
                    return Area.TRAINING_GARDEN;
                }
            }

            set { }
        }

        public RealtimeGameStateData toGameStateData
        {
            get
            {
                return new RealtimeGameStateData(State.SerializeData());
            }
        }

        public GameType _Type
        {
            get => GameType.PUZZLE;
            set { }
        }

        public GameMode _Mode
        {
            get
            {
                if (puzzleData.pack)
                {
                    return GameMode.PUZZLE_PACK;
                }
                else
                {
                    return GameMode.PUZZLE_FAST;
                }
            }

            set { }
        }

        public string BoardID
        {
            get => puzzleData.gameBoardDefinition.ID;

            set { }
        }
        public bool isFourzyPuzzle => true;
        public List<Creature> myMembers => State.Herds[me.PlayerId].Members;

        public float initializedTime { get; set; }

        public GamePiecePrefabData playerOnePrefabData { get; set; }
        public GamePiecePrefabData playerTwoPrefabData { get; set; }

        public List<PlayerTurn> InitialTurns => puzzleData.InitialMoves;

        public List<RewardsManager.Reward> collectedItems { get; set; }

        public GamePieceView playerOneGamepiece
        {
            get
            {
                ClientFourzyHelper.AssignPrefabs(this);

                return playerOnePrefabData.player1Prefab;
            }
        }

        public GamePieceView playerTwoGamepiece
        {
            get
            {
                ClientFourzyHelper.AssignPrefabs(this);

                return playerOnePrefabData.data.ID == playerTwoPrefabData.data.ID ? playerTwoPrefabData.player2Prefab : playerTwoPrefabData.player1Prefab;
            }
        }

        public int Rows => State.Board.Rows;

        public int Columns => State.Board.Columns;

        public int BossMoves { get; set; }

        public int LoseStreak { get; set; }

        public bool isOver =>
            Status == PuzzleStatus.SUCCESS ||
            Status == PuzzleStatus.FAILED ||
            playerTurnRecord.Count == puzzleData.MoveLimit ||
            _State.WinnerId > 0;

        public bool draw { get; set; }

        public bool hideOpponent { get; set; } = true;

        public Piece activePlayerPiece => new Piece(
            State.ActivePlayerId,
            string.IsNullOrEmpty(activePlayer.HerdId) ? 1 : int.Parse(activePlayer.HerdId));

        public Piece playerPiece
        {
            get
            {
                Player _player = me;
                return new Piece(_player.PlayerId, string.IsNullOrEmpty(_player.HerdId) ? 1 : int.Parse(_player.HerdId));
            }
        }

        public Piece opponentPiece
        {
            get
            {
                Player _player = opponent;
                return new Piece(_player.PlayerId, string.IsNullOrEmpty(_player.HerdId) ? 1 : int.Parse(_player.HerdId));
            }
        }

        public ClientPuzzleData puzzleData { get; set; }

        public List<BoardSpace> boardContent => ClientFourzyHelper.BoardContent(this);

        public GamePieceView myGamePiece => me.PlayerId == 1 ? playerOneGamepiece : playerTwoGamepiece;

        public GamePieceView opponentGamePiece =>
            opponent.PlayerId == 1 ? playerOneGamepiece : playerTwoGamepiece;

        public GamePieceView activePlayerGamePiece =>
            State.ActivePlayerId == 1 ? playerOneGamepiece : playerTwoGamepiece;

        public Player me => State.Players[1].PlayerString == UserManager.Instance.userId ?
                    State.Players[1] : State.Players[2];

        public Player opponent => State.Players[(PlayerEnum)me.PlayerId == PlayerEnum.ONE ? 2 : 1];

        public Player activePlayer => isMyTurn ? me : opponent;

        public Player unactivePlayer => isMyTurn ? opponent : me;

        public Player player1 => State.Players[(_FirstState == null ? _State : _FirstState).ActivePlayerId];

        public Player player2 => ClientFourzyHelper.Other(this, player1);

        public bool isMyTurn => me.PlayerId == State.ActivePlayerId;

        public float gameDuration => Time.time - initializedTime;

        public TurnEvaluator turnEvaluator => new TurnEvaluator(State);

        public ClientFourzyGame asFourzyGame => null;

        public ClientFourzyPuzzle asFourzyPuzzle => this;

        public ClientFourzyPuzzle(ClientPuzzleData _puzzleData) : base(_puzzleData, UserManager.Instance.meAsPlayer)
        {
            puzzleData = _puzzleData;
            State.ActivePlayerId = _puzzleData.firstTurn < 1 ? me.PlayerId : _puzzleData.firstTurn;

            Initialize();
        }

        public bool IsWinner(Player player) => State.WinnerId == player.PlayerId;

        public bool IsWinner() => me.PlayerId == State.WinnerId;

        public override PlayerTurnResult TakeTurn(
            PlayerTurn Turn,
            bool ReturnStartOfNextTurn = false,
            bool TriggerEndOfTurn = true)
        {
            if (State.Players.ContainsKey(State.ActivePlayerId))
            {
                Turn.PlayerString = State.Players[State.ActivePlayerId].PlayerString ?? "";
            }
            else
            {
                Turn.PlayerString = "Null";
                Debug.Log("Active PlayerId is not present in Player Dictionary: " +
                    State.ActivePlayerId + " GameSeed: " + State.GameSeed);
            }

            _allTurnRecord.Add(Turn);

            return base.TakeTurn(Turn, ReturnStartOfNextTurn);
        }

        public override PlayerTurnResult TakeAITurn(bool ReturnStartOfNextTurn = false)
        {
            PlayerTurnResult result = base.TakeAITurn(ReturnStartOfNextTurn);

            _allTurnRecord.Add(result.Turn);

            return result;
        }

        public void SetInitialTime(float value) => ClientFourzyHelper.SetInitialTime(this, value);

        public void AddPlayerMagic(int playerId, int value) =>
            ClientFourzyHelper.AddPlayerMagic(this, playerId, value);

        public void OnVictory() => ClientFourzyHelper.OnVictory(this);

        public void OnDraw() => ClientFourzyHelper.OnDraw(this);

        public void CheckLost() => ClientFourzyHelper.CheckLost(this);

        public void RemoveMember() => ClientFourzyHelper.RemoveMember(this);

        public void AddMembers(int count) => ClientFourzyHelper.AddMembers(this, count);

        public IClientFourzy Next()
        {
            if (puzzleData.pack)
            {
                return puzzleData.pack.Next(this);
            }
            else
            {
                return GameContentManager.Instance.GetNextFastPuzzle(puzzleData.ID);
            }
        }

        public PlayerTurnResult StartTurn() => StartTurn(_State);

        public GameState _Reset(bool resetMembers = false)
        {
            State = new GameState(_FirstState);

            Initialize(false);

            playerTurnRecord = new List<PlayerTurn>();
            Status = PuzzleStatus.ACTIVE;

            LoseStreak++;

            return State;
        }

        private void Initialize(bool resetFirstState = true)
        {
            if (resetFirstState)
            {
                _FirstState = new GameState(State);
            }

            collectedItems = new List<RewardsManager.Reward>();
            _allTurnRecord = new List<PlayerTurn>();

            if (puzzleData.pack)
            {
                hideOpponent = puzzleData.pack.packType == PackType.PUZZLE_PACK;
            }
            else
            {
                hideOpponent = true;
            }

            draw = false;
            magic = new Dictionary<int, int>();
            //assign magic values
            foreach (KeyValuePair<int, Player> player in State.Players)
            {
                magic.Add(player.Key, player.Value.Magic);
            }
        }
    }
}