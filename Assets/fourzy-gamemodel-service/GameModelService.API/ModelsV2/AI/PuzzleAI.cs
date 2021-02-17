using System.Linq;

namespace FourzyGameModel.Model
{
    public class PuzzleAI : AIPlayer
    {
        private GameState EvalState { get; set; }

        public PuzzleAI(GameState State)
        {
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            AITurnEvaluator AI = new AITurnEvaluator(EvalState);
            if (AI.WinningTurns.Count > 0) return AI.WinningTurns.First();

            AI.AIHeuristics.LookForSetups = true;
            AI.AIHeuristics.AvoidSetups = true;
            AI.AIHeuristics.AvoidUnstoppable = true;
            AI.AIHeuristics.IsAggressive = true;

            SimpleMove Move = AI.GetRandomOkMove(1);
            if (Move == null) return new PlayerTurn(AI.GetBestLostCauseMove());

            return new PlayerTurn(Move);
        }
    }
}
