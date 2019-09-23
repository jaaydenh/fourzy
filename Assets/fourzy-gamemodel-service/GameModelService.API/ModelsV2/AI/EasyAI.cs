
using System;
using System.Linq;

namespace FourzyGameModel.Model
{
    //EASY AI PROFILE
    //Do not always make winning move
    //Do not always make the best move, by evaluating top 10 moves and picking one at random.

    public class EasyAI : AIPlayer
    {
        private GameState EvalState { get; set; }
        private int InitialChanceToWin = 0;
        private int ChanceToWinPerPiece = 2;
        private int InitialBlockChance = 90;
        private int ChanceToBlockPerPiece = -2;
        private int NumberOfMovesToConsider = 7;

        public EasyAI(GameState State)
        {
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            AITurnEvaluator AI = new AITurnEvaluator(EvalState);
            if (AI.WinningTurns.Count > 0)
            {
                if (EvalState.Board.Random.Chance(Math.Min(100, (EvalState.Board.PieceCount - 8) * 2)))
                    return AI.WinningTurns.First();
            }

            SimpleMove Move = null;
            if (EvalState.Board.Random.Chance(InitialBlockChance))
            {
                Move = AI.GetRandomOkMove(NumberOfMovesToConsider);
            }
            else
            {
                Move = AI.GetTopMoveNoRestriction(NumberOfMovesToConsider);
            }

            //If for any reason, no good moves, get best lost cause move.
            if (Move == null) return new PlayerTurn(AI.GetBestLostCauseMove());

            return new PlayerTurn(Move);
        }
    }
    
}