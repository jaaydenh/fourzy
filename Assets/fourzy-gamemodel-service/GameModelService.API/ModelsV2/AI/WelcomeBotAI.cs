using System;
using System.Collections.Generic;
using System.Linq;

namespace FourzyGameModel.Model
{
    //EASY AI PROFILE
    //Do not always make winning move
    //Do not always make the best move, by evaluating top 10 moves and picking one at random.

    public class WelcomeBotAI : AIPlayer
    {
        private GameState EvalState { get; set; }
        private int NumberOfMovesToConsider = 4;
        private int NumberTurnsBeforeWinning = 16;


        public WelcomeBotAI(GameState State)
        {
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            List<SimpleMove> Moves = new List<SimpleMove>() { };
            AITurnEvaluator AI = new AITurnEvaluator(EvalState);
            Moves = AI.AvailableSimpleMoves;
            if (AI.WinningTurns.Count > 0)
            {

                //If beyond the turn limit, start making winning moves
                if (EvalState.TurnCount > NumberTurnsBeforeWinning)
                    return AI.WinningTurns.First();

                //Otherwise, remove these moves from consideration, even if they block.
                foreach (PlayerTurn t in AI.WinningTurns)
                {
                    Moves.Remove((SimpleMove)t.Moves[0]);
                }
            }

            //Create a new evaluator minus the winning moves.
            AI = new AITurnEvaluator(EvalState, Moves);

            SimpleMove Move = null;

            //Get the top four remaining moves, and choose one of them randomly.
            Move = AI.GetRandomOkMove(NumberOfMovesToConsider);

            //If there are no ok moves, then make the best move possible.
            if (Move == null) return new PlayerTurn(AI.GetBestLostCauseMove());

            return new PlayerTurn(Move);
        }
    }

}