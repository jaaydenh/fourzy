using System.Collections.Generic;
using Newtonsoft.Json;

namespace FourzyGameModel.Model
{
    public class PlayerTurnResponse
    {
        [JsonProperty("gameStateData")]
        public GameStateData GameStateData { get; set; }

        [JsonProperty("activity")]
        public List<GameAction> Activity { get; set; }

        public PlayerTurnResponse(GameStateData GameStateData, List<GameAction> Activity)
        {
            this.GameStateData = GameStateData;
            this.Activity = Activity;
        }
    }
}
