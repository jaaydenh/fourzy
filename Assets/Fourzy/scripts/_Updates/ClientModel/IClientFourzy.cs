//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.Serialized;
using FourzyGameModel.Model;
using System;
using System.Collections.Generic;
using GameType = Fourzy.GameType;

namespace Fourzy._Updates.ClientModel
{
    public interface IClientFourzy
    {
        Action<int, int> onMagic { get; set; }
        GameState _State { get; }
        GameType _Type { get; set; }
        Area _Area { get; set; }
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
        Piece activePlayerPiece { get; }
        TurnEvaluator turnEvaluator { get; }
        List<BoardSpace> boardContent { get; }
        List<PlayerTurn> _playerTurnRecord { get; }

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

        ClientFourzyGame asFourzyGame { get; }
        ClientFourzyPuzzle asFourzyPuzzle { get; }
        RewardsManager.RewardTestSubject asSubject { get; }

        //taketurn
        PlayerTurnResult TakeTurn(PlayerTurn playerTurn, params object[] extraValues);
        PlayerTurnResult TakeTurn(Direction direction, int location, params object[] extraValues);
        PlayerTurnResult TakeAITurn(bool ReturnStartOfNextTurn = false);

        GameState Reset();
    }
}
