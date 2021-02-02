//@vadym udod

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
    }
}