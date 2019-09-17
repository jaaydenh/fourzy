//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.Serialized;
using FourzyGameModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameType = Fourzy.GameType;

namespace Fourzy._Updates.ClientModel
{
    public class ClientFourzyPuzzle : FourzyPuzzle, IClientFourzy
    {
        public Action<int, int> onMagic { get; set; }

        public GameState _State => State;
        public GameState _FirstState
        {
            get => FirstState;
            set { }
        }

        public List<PlayerTurn> _playerTurnRecord => playerTurnRecord;
        public List<PlayerTurn> _allTurnRecord { get; set; }
        public Dictionary<int, int> magic { get; set; }

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
                result.WinningLocations = new List<BoardLocation>();
                result.Herds = new Dictionary<int, Herd>();

                if (State.GameEffects != null) result.GameEffects = new List<GameEffect>(State.GameEffects);
                if (State.WinningLocations != null) result.WinningLocations = new List<BoardLocation>(State.WinningLocations);
                if (State.Herds != null) foreach (var herd in State.Herds) result.Herds.Add(herd.Key, herd.Value);

                return result;
            }
        }
        public GameType _Type
        {
            get => GameType.PUZZLE;
            set { }
        }

        public string GameID
        {
            get => puzzleData.ID;

            set { }
        }

        public float initializedTime { get; set; }

        public GamePiecePrefabData playerOnePrefabData { get; set; }
        public GamePiecePrefabData playerTwoPrefabData { get; set; }

        public List<PlayerTurn> InitialTurns => puzzleData.InitialMoves;

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

        public bool isOver => 
            Status == PuzzleStatus.SUCCESS || 
            Status == PuzzleStatus.FAILED || 
            playerTurnRecord.Count == puzzleData.MoveLimit ||
            _State.WinnerId > 0;

        public bool draw { get; set; }

        public bool hideOpponent { get; set; } = true;

        public Piece activePlayerPiece => new Piece(State.ActivePlayerId, string.IsNullOrEmpty(activePlayer.HerdId) ? 1 : int.Parse(activePlayer.HerdId));

        public ClientPuzzleData puzzleData { get; set; }

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

        public ClientFourzyPuzzle(ClientPuzzleData _puzzleData) : base(_puzzleData, UserManager.Instance.meAsPlayer)
        {
            puzzleData = _puzzleData;
            State.ActivePlayerId = _puzzleData.firstTurn < 1 ? me.PlayerId : _puzzleData.firstTurn;

            Initialize();
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
            if (puzzleData)
            {
                if (IsWinner())
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
        }

        public void OnDraw()
        {
            draw = true;
        }

        public IClientFourzy Next()
        {
            if (puzzleData.pack)
                return puzzleData.pack.Next(this);
            else
                return GameContentManager.Instance.GetFastPuzzle(puzzleData.ID);
        }

        public PlayerTurnResult StartTurn() => StartTurn(_State);

        public override GameState Reset()
        {
            Initialize();

            return base.Reset();
        }

        private void Initialize()
        {
            collectedItems = new List<RewardsManager.Reward>();
            _allTurnRecord = new List<PlayerTurn>();

            if (puzzleData.pack)
                hideOpponent = puzzleData.pack.packType == PackType.PUZZLE_PACK;
            else
                hideOpponent = true;

            magic = new Dictionary<int, int>();
            //assign magic values
            foreach (KeyValuePair<int, Player> player in State.Players)
                magic.Add(player.Key, player.Value.Magic);
        }

        private void AssignPrefabs()
        {
            if (playerOnePrefabData == null)
            {
                string player1HerdId = "0";
                if (State.Players[1].HerdId != null) player1HerdId = State.Players[1].HerdId;

                if (State.Players.ContainsKey(1) && !string.IsNullOrEmpty(State.Players[1].HerdId))
                    playerOnePrefabData = GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(player1HerdId);
                else
                    playerOnePrefabData = GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(UserManager.Instance.gamePieceID);
            }

            if (playerTwoPrefabData == null)
            {
                string player2HerdId = "0";
                if (State.Players[2].HerdId != null) player2HerdId = State.Players[2].HerdId;

                if (State.Players.ContainsKey(2) && !string.IsNullOrEmpty(State.Players[2].HerdId))
                    playerTwoPrefabData = GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(player2HerdId);
                else
                    playerTwoPrefabData = GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(UserManager.Instance.gamePieceID);
            }
        }
    }
}