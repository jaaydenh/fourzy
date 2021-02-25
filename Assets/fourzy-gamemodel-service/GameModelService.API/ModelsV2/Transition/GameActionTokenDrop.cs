using System;

namespace FourzyGameModel.Model
{
    public class GameActionTokenDrop : GameAction
    {
        public GameActionType Type { get { return GameActionType.ADD_TOKEN; } }

        public GameActionTiming Timing { get { return GameActionTiming.MOVE; } }

        // Source and Destination could be used to show a token animating from the source location to the destination location
        public BoardLocation Source { get; set; } // Source is optional
        public BoardLocation Destination { get; set; } // The location the token should be 
        public  TransitionType Reason { get; set; }

        public IToken Token { get; set; }

        public GameActionTokenDrop(IToken Token, TransitionType Reason, BoardLocation Source, BoardLocation Destination)
        {
            this.Token = Token;
            this.Reason = Reason;
            this.Source = Source;
            this.Destination = Destination;
        }

        public string Print()
        {
            throw new NotImplementedException();
        }
    }
}
