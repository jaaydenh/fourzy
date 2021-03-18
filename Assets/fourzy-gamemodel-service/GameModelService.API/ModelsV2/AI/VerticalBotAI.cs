using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    //EASY AI PROFILE
    //Do not always make winning move
    //Do not always make the best move, by evaluating top 10 moves and picking one at random.

    public class VerticalBotAI : AIPlayer
    {
        private GameState EvalState { get; set; }
        //private int InitialChanceToWin = 0;
        //private int ChanceToWinPerPiece = 2;
        //private int InitialBlockChance = 90;
        //private int ChanceToBlockPerPiece = -2;
        private int NumberOfMovesToConsider = 7;

        public VerticalBotAI(GameState State)
        {
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            TurnEvaluator TE = new TurnEvaluator(EvalState);

            List<SimpleMove> HorizontalMoves = TE.GetAvailableSimpleMoves(Direction.UP);
            HorizontalMoves.AddRange(TE.GetAvailableSimpleMoves(Direction.DOWN));

            AITurnEvaluator AI = new AITurnEvaluator(EvalState, HorizontalMoves);
            if (AI.WinningTurns.Count > 0)
            {
                foreach (PlayerTurn t in AI.WinningTurns)
                {
                    foreach (IMove m in t.Moves)
                    {
                        if (m.MoveType == MoveType.SIMPLE)
                        {
                            SimpleMove M = (SimpleMove)m;
                            if (M.Direction == Direction.UP) return t;
                            if (M.Direction == Direction.DOWN) return t;
                        }
                    }
                }
            }

            //Get the top scoring move. AI will still only consider moves from the left or right.
            SimpleMove Move = AI.GetRandomOkMove();
            if (Move == null)
            {
                AI = new AITurnEvaluator(EvalState);
                Move = AI.GetRandomOkMove(1);
            }

            if (Move == null) Move = AI.GetTopMoveNoRestriction(NumberOfMovesToConsider);

            //If for any reason, no good moves, get best lost cause move.
            if (Move == null) return new PlayerTurn(AI.GetBestLostCauseMove());

            return new PlayerTurn(Move);
        }
    }

}