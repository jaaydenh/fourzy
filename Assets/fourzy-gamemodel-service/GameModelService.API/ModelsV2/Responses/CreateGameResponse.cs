using Newtonsoft.Json;

namespace FourzyGameModel.Model
{
    public class CreateGameResponse
    {
        [JsonProperty("gameStateData")]
        public GameStateData GameStateData { get; set; }

        public CreateGameResponse(GameStateData gameStateData)
        {
            this.GameStateData = gameStateData;
        }
    }
}
