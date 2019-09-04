//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.Serialized;
using FourzyGameModel.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using GameType = Fourzy.GameType;

namespace Fourzy._Updates.ClientModel
{
    public interface IClientFourzy
    {
        Action<int, int> onMagic { get; set; }
        GameState _FirstState { get; set; }
        GameState _State { get; }
        GameType _Type { get; set; }
        Area _Area { get; set; }
        GameStateDataEpoch toGameStateData { get; }
        string GameID { get; set; }

        List<PlayerTurn> InitialTurns { get; }

        GamePiecePrefabData playerOnePrefabData { get; set; }
        GamePiecePrefabData playerTwoPrefabData { get; set; }

        GamePieceView playerOneGamepiece { get; }
        GamePieceView playerTwoGamepiece { get; }

        GamePieceView playerGamePiece { get; }
        GamePieceView opponentGamePiece { get; }

        int Rows { get; }
        int Columns { get; }
        bool isOver { get; }
        bool draw { get; set; }
        bool hideOpponent { get; set; }
        Piece activePlayerPiece { get; }
        
        ClientPuzzleData puzzleData { get; set; }

        TurnEvaluator turnEvaluator { get; }
        List<BoardSpace> boardContent { get; }
        List<PlayerTurn> _playerTurnRecord { get; }
        List<PlayerTurn> _allTurnRecord { get; set; }
        Dictionary<int, int> magic { get; set; }

        List<RewardsManager.Reward> collectedItems { get; set; }

        Player me { get; }
        Player opponent { get; }
        Player activePlayer { get; }
        Player unactivePlayer { get; }

        bool isMyTurn { get; }
        float gameDuration { get; }
        float initializedTime { get; set; }

        bool IsWinner(Player player);
        bool IsWinner();
        void SetInitialTime(float value);
        void AddPlayerMagic(int playerId, int value);
        void OnVictory();
        void OnDraw();
        IClientFourzy Next();
        PlayerTurnResult StartTurn(GameState gameState);
        PlayerTurnResult StartTurn();

        ClientFourzyGame asFourzyGame { get; }
        ClientFourzyPuzzle asFourzyPuzzle { get; }

        //taketurn
        PlayerTurnResult TakeTurn(PlayerTurn playerTurn, bool local, bool returnStartOfNextTurn);
        PlayerTurnResult TakeAITurn(bool ReturnStartOfNextTurn = false);

        GameState Reset();
    }
}

namespace FourzyGameModel.Model
{
    [Serializable]
    public class GameStateDataEpoch : GameStateData
    {
        [JsonProperty("realtimeData")]
        public RealtimeData realtimeData;
    }
}
