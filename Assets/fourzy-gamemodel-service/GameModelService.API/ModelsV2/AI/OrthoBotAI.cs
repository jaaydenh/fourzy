
using System.Linq;

namespace FourzyGameModel.Model
{
    public class OrthoBotAI : AIPlayer
    {
        private GameState EvalState { get; set; }

        public OrthoBotAI(GameState State)
        {
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            AIHeuristicWeight AIWeight = new AIHeuristicWeight();
            AIWeight.ConsiderDiagonals = false;

            AITurnEvaluator AI = new AITurnEvaluator(EvalState, AIWeight);
            if (AI.WinningTurns.Count > 0) return AI.WinningTurns.First();

            //Get the 4th-10th best scoring move.
            SimpleMove Move = AI.GetLowerScoringMove(8, 6);
            if (Move == null) return new PlayerTurn(AI.GetBestLostCauseMove());

            return new PlayerTurn(Move);
        }
    }
}