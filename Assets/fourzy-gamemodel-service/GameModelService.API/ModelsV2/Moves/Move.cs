using Newtonsoft.Json;

namespace FourzyGameModel.Model
{
    public interface IMove
    {
        [JsonProperty("moveType")]
        MoveType MoveType { get; }

        [JsonProperty("notation")]
        string Notation { get; }
    }
}
