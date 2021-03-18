namespace FourzyGameModel.Model
{
    public interface GameAction
    {
        GameActionType Type { get; }
        GameActionTiming Timing { get; }
        string Print();
    }
}
