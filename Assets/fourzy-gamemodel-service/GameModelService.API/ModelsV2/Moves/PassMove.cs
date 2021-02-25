using Newtonsoft.Json;

namespace FourzyGameModel.Model
{
    public class PassMove : IMove
    {
        [JsonProperty("moveType")]
        public MoveType MoveType { get { return MoveType.PASS; } }

        public string Notation
        {
            get
            {
                return string.Format("Pass");
            }
        }

        public PassMove()
        {

        }
    }
}
