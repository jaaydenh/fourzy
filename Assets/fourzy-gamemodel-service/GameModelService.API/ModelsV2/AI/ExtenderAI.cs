using System.Linq;

namespace FourzyGameModel.Model
{
    public class ExtenderAI : AIPlayer
    {
        private GameState EvalState { get; set; }

        public ExtenderAI(GameState State)
        {
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            AITurnEvaluator AI = new AITurnEvaluator(EvalState);
            if (AI.WinningTurns.Count > 0) return AI.WinningTurns.First();

            SimpleMove Move = AI.GetRandomOkMove();
            if (Move == null) return new PlayerTurn(AI.GetBestLostCauseMove());

            return new PlayerTurn(Move);
        }
    }
}
