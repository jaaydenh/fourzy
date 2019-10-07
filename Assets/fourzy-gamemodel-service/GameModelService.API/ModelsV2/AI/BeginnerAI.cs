
using System.Linq;

namespace FourzyGameModel.Model
{
    public class BeginnerAI : AIPlayer
    {
        private GameState EvalState { get; set; }

        public BeginnerAI(GameState State)
        {
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            AITurnEvaluator AI = new AITurnEvaluator(EvalState);
            if (AI.WinningTurns.Count > 0) return AI.WinningTurns.First();

            //Get the 4th-10th best scoring move.
            SimpleMove Move = AI.GetLowerScoringMove(8,6);
            if (Move == null) return new PlayerTurn(AI.GetBestLostCauseMove());

            return new PlayerTurn(Move);
        }
    }
}