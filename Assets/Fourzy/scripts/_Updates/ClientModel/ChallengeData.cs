//@vadym udod

using FourzyGameModel.Model;
// using GameSparks.Core;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.ClientModel
{
    [System.Serializable]
    public class ChallengeData
    {
        public string challengeInstanceId;
        public GameStateData originalGameState;

        public List<PlayerTurn> playerTurnRecord;
        public ClientFourzyGame lastTurnGame { get; private set; }

        public PlayerTurn lastTurn => playerTurnRecord.Count == 0 ? null : playerTurnRecord[playerTurnRecord.Count - 1];
        public bool canBeNext => lastTurnGame.isMyTurn || (lastTurnGame.isOver && !PlayerPrefsWrapper.GetGameViewed(challengeInstanceId));
        public bool haveMoves => lastTurnGame.haveMoves || playerTurnRecord.Count > 0;

        //constructor for when game is created by server
        public ChallengeData(GameStateData originalGameState, string challengeInstanceId)
        {
            this.originalGameState = originalGameState;
            this.challengeInstanceId = challengeInstanceId;

            playerTurnRecord = new List<PlayerTurn>();

            //if by some mistake area is set to 0, set it to Training Garden
            if (this.originalGameState.GameBoardData.Area == Area.NONE)
                this.originalGameState.GameBoardData.Area = Area.TRAINING_GARDEN;
        }

        //constructor for when game is received as challenge from server
        // public ChallengeData(GSData challengeData) : 
        //     this(JsonConvert.DeserializeObject<GameStateData>(challengeData.GetGSData("scriptData").GetGSData("gameStateData").JSON), challengeData.GetString("challengeId"))
        // {
        //     foreach (GSData turnRecord in challengeData.GetGSData("scriptData").GetGSDataList("playerTurnRecord"))
        //         playerTurnRecord.Add(JsonConvert.DeserializeObject<PlayerTurn>(turnRecord.JSON));

        //     originalGameState.ActivePlayerId = challengeData.GetGSData("scriptData").GetInt("firstPlayerId").GetValueOrDefault(-1);
        //     UpdateLastTurnGame();
        // }

        public ClientFourzyGame GetGameForMove(PlayerTurn targetMove)
        {
            ClientFourzyGame game = new ClientFourzyGame(originalGameState);
            game.BoardID = challengeInstanceId;
            game._Type = GameType.TURN_BASED;
            game.challengeData = this;

            if (targetMove == null)
                return game;

            foreach (PlayerTurn turn in playerTurnRecord)
            {
                game.TakeTurn(turn, true);

                if (turn == targetMove) break;
            }

            return game;
        }

        public ClientFourzyGame GetGameForPreviousMove()
        {
            if (playerTurnRecord.Count < 2)
                return GetGameForMove(null);

            return GetGameForMove(playerTurnRecord[playerTurnRecord.Count - 2]);
        }

        public ClientFourzyGame GetGameForLastMove()
        {
            if (playerTurnRecord.Count == 0)
                return GetGameForMove(null);

            return GetGameForMove(lastTurn);
        }

        public void UpdateLastTurnGame() => lastTurnGame = GetGameForLastMove();

        public bool Validate() => !string.IsNullOrEmpty(challengeInstanceId) && originalGameState.Players.Count > 1;

        // public static bool ValidateGSData(GSData data) => data.GetGSData("scriptData").ContainsKey("gameStateData") && !string.IsNullOrEmpty(data.GetString("challengeId"));
    }
}