
using System.Collections.Generic;
using System.Linq;

namespace FourzyGameModel.Model
{
    public class BlindBotAI : AIPlayer
    {
        private GameState EvalState { get; set; }

        public BlindBotAI(GameState State)
        {
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            TurnEvaluator TE = new TurnEvaluator(EvalState);

            List<SimpleMove> Moves = new List<SimpleMove>();
            foreach (Direction d in TokenConstants.GetDirections())
            {
                List<SimpleMove> DirectionMoves = TE.GetAvailableSimpleMoves(d);
                DirectionMoves = DirectionMoves.OrderBy(a => System.Guid.NewGuid()).ToList();
                Moves.AddRange(DirectionMoves.Take(EvalState.Board.Rows - 2));
            }

            AITurnEvaluator AI = new AITurnEvaluator(EvalState, Moves);
            if (AI.WinningTurns.Count > 0) return AI.WinningTurns.First();

            //Get the 4th-10th best scoring move.
            SimpleMove Move = AI.GetRandomOkMove();
            if (Move == null) return new PlayerTurn(AI.GetBestLostCauseMove());

            return new PlayerTurn(Move);
        }
    }
}