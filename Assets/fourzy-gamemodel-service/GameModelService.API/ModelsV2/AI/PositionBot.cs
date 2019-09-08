using System.Linq;

namespace FourzyGameModel.Model
{
    public class PositionBot : AIPlayer
    {
        private GameState EvalState { get; set; }

        public PositionBot(GameState State)
        {
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            AITurnEvaluator AI = new AITurnEvaluator(EvalState);
            if (AI.WinningTurns.Count > 0) return AI.WinningTurns.First();

            AIHeuristicWeight AIWeight = new AIHeuristicWeight(25, 0, 100);

            SimpleMove Move = AI.GetOkMoveWithHueristics(AIWeight,5);
            if (Move == null) Move = AI.GetBestLostCauseMove();

            return new PlayerTurn(Move);
        }
    }
}
