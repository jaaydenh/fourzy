using System.Linq;

namespace FourzyGameModel.Model
{
    public class PanicBotAI : AIPlayer
    {
        private GameState EvalState { get; set; }

        public PanicBotAI(GameState State)
        {
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            AITurnEvaluator AI = new AITurnEvaluator(EvalState);
            if (AI.WinningTurns.Count > 0) return AI.WinningTurns.First();
        

            bool Panic = false;
            SimpleMove Move = AI.GetRandomOkMove(2);
            if (Move == null)
            {
                Panic = true;
            }
            if (Move == null) return new PlayerTurn(AI.GetBestLostCauseMove());

            // If you need to block, block the move.
            if (Panic) return new PlayerTurn(Move);

            // If your score is behind opponent, make a move.
            AIScoreEvaluator AISE = new AIScoreEvaluator(EvalState);
            if (AISE.Score(EvalState.ActivePlayerId) < 100 ) return new PlayerTurn(Move);

            // If your score is higher, then pass.
            return new PlayerTurn(EvalState.ActivePlayerId, new PassMove());
        }
    }
}
