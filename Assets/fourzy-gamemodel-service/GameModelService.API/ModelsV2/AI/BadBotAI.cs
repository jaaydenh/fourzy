
using System;
using System.Collections.Generic;
using System.Linq;

namespace FourzyGameModel.Model
{
    //BAD AI PROFILE

    public class BadBotAI : AIPlayer
    {
        private GameState EvalState { get; set; }
        private int InitialChanceToWin = 0;
        private int ChanceToWinPerPiece = 2;
        private int InitialBlockChance = 90;
        private int ChanceToBlockPerPiece = -2;
        private int NumberOfMovesToConsider = 7;

        public BadBotAI(GameState State)
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

            //Can the opponent win next turn?
            List<SimpleMove> OkMoves = AI.GetOkMoves();
            if (OkMoves == null) return new PlayerTurn(AI.GetBestLostCauseMove());
            OkMoves = OkMoves.OrderBy(a => Guid.NewGuid()).Take(5).ToList();

            AI = new AITurnEvaluator(EvalState, OkMoves);

            SimpleMove Move = null;
            //Look at a couple moves and take the worst one.
            Move = AI.GetBadMove(3);

            //If for any reason, no good moves, get best lost cause move.
            if (Move == null) return new PlayerTurn(AI.GetBestLostCauseMove());

            return new PlayerTurn(Move);
        }
    }

}