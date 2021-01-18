using System.Linq;

namespace FourzyGameModel.Model
{
    //First Turn Is Random
    //   

    public class UnevenBotAI : AIPlayer
    {
        const int RandomMoveScoreThreshold = 500;
        private GameState EvalState { get; set; }

        public UnevenBotAI(GameState State)
        {
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            AITurnEvaluator AI = new AITurnEvaluator(EvalState);
            if (AI.WinningTurns.Count > 0) return AI.WinningTurns.First();

            if (EvalState.Board.FindPieces(EvalState.Opponent(EvalState.ActivePlayerId)).Count <= 1)
                return AI.GetRandomTurn();

            bool Panic = false;
            SimpleMove Move = AI.GetRandomOkMove(2);
            if (Move == null)
            {
                Panic = true;
                return new PlayerTurn(AI.GetBestLostCauseMove());
            }

            // If your score is behind opponent, make a move.
            AIScoreEvaluator AISE = new AIScoreEvaluator(EvalState);
            if (AISE.Score(EvalState.ActivePlayerId) < RandomMoveScoreThreshold) return new PlayerTurn(Move);
                        
            // If your score is higher, then make a random move.
            return AI.GetRandomTurn();
        }
    }
}
