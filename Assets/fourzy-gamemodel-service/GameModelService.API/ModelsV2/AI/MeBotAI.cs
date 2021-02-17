using System.Linq;

namespace FourzyGameModel.Model
{
    public class MeBotAI : AIPlayer
    {
        private GameState EvalState { get; set; }

        public MeBotAI(GameState State)
        {
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            AITurnEvaluator AI = new AITurnEvaluator(EvalState);
            if (AI.WinningTurns.Count > 0) return AI.WinningTurns.First();

            //Only consider the active player pieces.
            AI.AIHeuristics.ConsiderOpponentPieces = false;

            SimpleMove Move = AI.GetRandomOkMove();
            if (Move == null) return new PlayerTurn(AI.GetBestLostCauseMove());

            return new PlayerTurn(Move);
        }
    }
}
