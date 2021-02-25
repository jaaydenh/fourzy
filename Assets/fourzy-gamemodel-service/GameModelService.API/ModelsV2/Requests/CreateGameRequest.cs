using Newtonsoft.Json;

namespace FourzyGameModel.Model
{
    public class CreateGameRequest
    {
        [JsonProperty("player")]
        public Player Player { get; set; }

        [JsonProperty("opponent")]
        public Player Opponent { get; set; }
    }
}
