using System.Linq;

namespace FourzyGameModel.Model
{
    public class WaitBotAI : AIPlayer
    {
        private GameState EvalState { get; set; }
        private int SleepTurns { get; set; }

        public WaitBotAI(GameState State, int SleepTurns = 2)
        {
            this.SleepTurns = SleepTurns;
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            if (EvalState.Board.FindPieces(EvalState.Opponent(EvalState.ActivePlayerId)).Count < SleepTurns)
            {
                return new PlayerTurn(EvalState.ActivePlayerId, new PassMove());
            }

            AITurnEvaluator AI = new AITurnEvaluator(EvalState);
            if (AI.WinningTurns.Count > 0) return AI.WinningTurns.First();

            SimpleMove Move = AI.GetRandomOkMove();
            if (Move == null) return new PlayerTurn(AI.GetBestLostCauseMove());

            return new PlayerTurn(Move);
        }
    }
}
