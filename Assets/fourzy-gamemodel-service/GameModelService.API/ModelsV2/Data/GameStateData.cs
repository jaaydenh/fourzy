using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FourzyGameModel.Model
{
    [Serializable]
    public class GameStateData
    {
        [JsonProperty("gameBoardData")]
        public GameBoardData GameBoardData { get; set; }

        [JsonProperty("players")]
        public Dictionary<int, Player> Players { get; set; }

        [JsonProperty("herds")]
        public Dictionary<int, Herd> Herds { get; set; }

        [JsonProperty("gameSeed")]
        public string GameSeed { get; set; }

        [JsonProperty("winnerId")]
        public int WinnerId { get; set; }

        [JsonProperty("winningLocations")]
        public List<BoardLocation> WinningLocations { get; set; }

        [JsonProperty("gameEffects")]
        public List<IGameEffect> GameEffects { get; set; }

        [JsonProperty("activePlayerId")]
        public int ActivePlayerId { get; set; }
    }
}
