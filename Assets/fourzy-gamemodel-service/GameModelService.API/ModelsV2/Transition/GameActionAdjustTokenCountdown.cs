using System;

namespace FourzyGameModel.Model
{
    public class GameActionAdjustTokenCountdown: GameAction
    {
        public GameActionType Type { get { return GameActionType.TOKEN_COUNTDOWN; } }
        public GameActionTiming Timing { get { return GameActionTiming.AFTER_MOVE; } }

        //Fizzle is a generic failure type
        public int CountdownDelta { get; set; }
        public IToken Token { get; set; }

        public GameActionAdjustTokenCountdown(IToken Token, int DurationDelta)
        {
            this.Token = Token;
            this.CountdownDelta = CountdownDelta;
        }

        public string Print()
        {
            throw new NotImplementedException();
        }
    }
}
