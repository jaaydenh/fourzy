//@vadym udod

using Fourzy._Updates.Mechanics.GameplayScene;
using FourzyGameModel.Model;
using Newtonsoft.Json;
using System;

namespace Fourzy._Updates.ClientModel
{
    [Serializable]
    public class RealtimeGameStateData : GameStateData
    {
        [JsonProperty("createdEpoch")]
        public long createdEpoch;
        [JsonProperty("player1Timer")]
        public float player1Timer;
        [JsonProperty("player2Timer")]
        public float player2Timer;
        [JsonProperty("player1Magic")]
        public int player1Magic;
        [JsonProperty("player2Magic")]
        public int player2Magic;

        public RealtimeGameStateData() { }

        public RealtimeGameStateData(GameStateData data)
        {
            GameSeed = data.GameSeed;
            Herds = data.Herds;
            Players = data.Players;
            WinnerId = data.WinnerId;
            WinningLocations = data.WinningLocations;
            GameBoardData = data.GameBoardData;
            GameEffects = data.GameEffects;
            ActivePlayerId = data.ActivePlayerId;
        }

        public void UpdateWithGameplayData(GamePlayManager gameplayManager, bool both)
        {
            //update timer data
            //update magic data
            if (gameplayManager.Game.me == gameplayManager.Game.player1 || both)
            {
                player1Timer = gameplayManager.GameplayScreen.Player1TimeLeft;
                player1Magic = gameplayManager.GameplayScreen.Player1Magic;
            }
            
            if (gameplayManager.Game.me == gameplayManager.Game.player2 || both)
            {
                player2Timer = gameplayManager.GameplayScreen.Player2TimeLeft;
                player2Magic = gameplayManager.GameplayScreen.Player2Magic;
            }
        }
    }
}