using System;

namespace FourzyGameModel.Model
{
    public class GameActionCollect : GameAction
    {
        public GameActionType Type { get { return GameActionType.COLLECT; } }

        public GameActionTiming Timing { get { return GameActionTiming.AFTER_MOVE; } }

        Piece Piece { get; set; }
        BoardLocation End { get; set; }
        IToken CollectedToken { get; set; }

        public GameActionCollect(Piece Piece, BoardLocation End, IToken CollectedToken)
        {
            this.Piece = Piece;
            this.End = End;
            this.CollectedToken= CollectedToken;
        }

        public string Print()
        {
            throw new NotImplementedException();
        }
    }
}
