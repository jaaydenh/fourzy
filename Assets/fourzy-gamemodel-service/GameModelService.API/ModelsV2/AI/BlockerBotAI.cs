using System;
using System.Collections.Generic;
using System.Linq;

namespace FourzyGameModel.Model
{
    //Apprenctice AI PROFILE
    //Make winning moves only after 1-n turns
    //Make blocking moves until 1-b turns

    public class BlockerBotAI : AIPlayer
    {
        private GameState EvalState { get; set; }
        private int NumberOfMovesToConsider = 6;
        private int NumberOfMovesToLookForSetups = 12;
        private int MinNumberOfMovesBeforeStopBlocking = 10;
        private int MaxNumberOfMovesBeforeStopBlocking = 16;
        private int MinNumberTurnsBeforeWinning = 8;
        private int MaxNumberTurnsBeforeWinning = 16;

        public EventuallyBot(GameState State)
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
                int NumberTurnsBeforeWinning = MinNumberTurnsBeforeWinning + (MaxNumberOfMovesBeforeStopBlocking - MinNumberTurnsBeforeWinning) * EvalState.Board.Random.RandomInteger(0, 100) / 100;

                //If beyond the turn limit, start making winning moves
                if (EvalState.TurnCount > NumberTurnsBeforeWinning)
                    return AI.WinningTurns.First();

                //Otherwise, remove these moves from consideration, even if they block.
                foreach (PlayerTurn t in AI.WinningTurns)
                {
                    Moves.Remove((SimpleMove)t.Moves[0]);
                }
            }
            
            //If there are no moves that do not win, make the first one.
            if (Moves.Count == 0)
                return new PlayerTurn(AI.AvailableSimpleMoves.First());

            //Create a new evaluator minus the winning moves.
            AI = new AITurnEvaluator(EvalState, Moves);

            if (Moves.Count < NumberOfMovesToLookForSetups)
               AI.AIHeuristics.LookForSetups = true;
                        
            SimpleMove Move = null;

            int NumberOfMovesBeforeStopBlocking = MinNumberOfMovesBeforeStopBlocking + (MaxNumberOfMovesBeforeStopBlocking - MinNumberOfMovesBeforeStopBlocking) * EvalState.Board.Random.RandomInteger(0, 100) / 100;

            //Only make blocking moves for a couple of turns, but eventually make a mistake.
            if (EvalState.TurnCount > NumberOfMovesBeforeStopBlocking)
                Move = AI.GetRandomTopAvailableMove(NumberOfMovesToConsider);
            else
                Move = AI.GetRandomOkMove(NumberOfMovesToConsider);

            //If there are no ok moves, then make the best move possible.
            if (Move == null)
            {
                return new PlayerTurn(AI.GetBestMoveWithoutWinning());
            }

            //return new PlayerTurn(AI.GetBestLostCauseMove());

            return new PlayerTurn(Move);
        }
    }

}