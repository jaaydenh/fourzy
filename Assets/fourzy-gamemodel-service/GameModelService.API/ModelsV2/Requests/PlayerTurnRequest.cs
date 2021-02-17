using Newtonsoft.Json;

namespace FourzyGameModel.Model
{
    public class PlayerTurnRequest
    {
        [JsonProperty("gameStateData")]
        public GameStateData GameStateData { get; set; }

        [JsonProperty("playerTurn")]
        public PlayerTurn PlayerTurn { get; set; }
    }
}
