using System.Linq;

namespace FourzyGameModel.Model
{
    public class DoctorBotAI : AIPlayer
    {
        private GameState EvalState { get; set; }

        public DoctorBotAI(GameState State)
        {
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            AITurnEvaluator AI = new AITurnEvaluator(EvalState);
            AI.AIHeuristics.PruneFiveSetup = true;
            AI.AIHeuristics.LookForSetups = true;
            AI.AIHeuristics.LookForUnstoppable = true;
            if (AI.WinningTurns.Count > 0) return AI.WinningTurns.First();

            SimpleMove Move = AI.GetBestOkMovesWithLeastOppScore();

            if (Move == null) return new PlayerTurn(AI.GetBestLostCauseMove());

            return new PlayerTurn(Move);
        }
    }
}
