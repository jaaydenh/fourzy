
using System;
using System.Collections.Generic;
using System.Linq;

namespace FourzyGameModel.Model
{
    //EASY AI PROFILE
    //Do not always make winning move
    //Do not always make the best move, by evaluating top 10 moves and picking one at random.

    public class RotatorBotAI : AIPlayer
    {
        private GameState EvalState { get; set; }
        private int InitialChanceToWin = 0;
        private int ChanceToWinPerPiece = 2;
        private int InitialBlockChance = 90;
        private int ChanceToBlockPerPiece = -2;
        private int NumberOfMovesToConsider = 7;

        public RotatorBotAI(GameState State)
        {
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            TurnEvaluator TE = new TurnEvaluator(EvalState);
            Direction CurrentDirection = (Direction) (EvalState.Board.FindPieces(EvalState.ActivePlayerId).Count % 4);
            List<SimpleMove> SimpleMoves = TE.GetAvailableSimpleMoves(CurrentDirection);
            int i = 0;
            while (SimpleMoves.Count == 0 && i<4)
            {
                i++;
                CurrentDirection = (Direction)((EvalState.Board.FindPieces(EvalState.ActivePlayerId).Count +i) % 4);
                SimpleMoves = TE.GetAvailableSimpleMoves(CurrentDirection);
            }

            AITurnEvaluator AI = null;
            if (SimpleMoves.Count > 0) AI = new AITurnEvaluator(EvalState, SimpleMoves);
            else AI = new AITurnEvaluator(EvalState);

            if (AI.WinningTurns.Count > 0)
            {
                foreach (PlayerTurn t in AI.WinningTurns)
                {
                    foreach (IMove m in t.Moves)
                    {
                        if (m.MoveType == MoveType.SIMPLE)
                        {
                            SimpleMove M = (SimpleMove)m;
                            if (M.Direction == CurrentDirection) return t;
                        }
                    }
                }
            }

            //Get the top scoring move. AI will still only consider moves from the top.
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