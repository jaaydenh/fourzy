using System.Linq;

namespace FourzyGameModel.Model
{
    public class AggressiveAI : AIPlayer
    {
        private GameState EvalState { get; set; }

        public AggressiveAI(GameState State)
        {
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            AITurnEvaluator AI = new AITurnEvaluator(EvalState);
            if (AI.WinningTurns.Count > 0) return AI.WinningTurns.First();

            AI.AIHeuristics.LookForSetups = true;
            AI.AIHeuristics.IsAggressive = true;
            SimpleMove Move = AI.GetRandomOkMove();
            if (Move == null) return new PlayerTurn(AI.GetBestLostCauseMove());

            return new PlayerTurn(Move);
        }
    }
}
