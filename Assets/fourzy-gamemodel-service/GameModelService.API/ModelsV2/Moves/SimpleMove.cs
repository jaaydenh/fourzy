using Newtonsoft.Json;

namespace FourzyGameModel.Model
{
    public class SimpleMove : IMove
    {
        [JsonProperty("piece")]
        public Piece Piece {get; set;}

        [JsonProperty("direction")]
        public Direction Direction { get; set; }

        [JsonProperty("location")]
        public int Location { get; set; }

        [JsonProperty("moveType")]
        public MoveType MoveType { get { return MoveType.SIMPLE; } }

        public string Notation
        {
            get
            {
                return string.Format("Move:{0}{1}", Direction.ToString(), Location.ToString());
            }
        }

        public SimpleMove()
        {

        }

        public SimpleMove(Piece Piece, Direction Direction, int Location)
        {
            this.Piece = Piece;
            this.Direction = Direction;
            this.Location = Location;
        }
    }
}
