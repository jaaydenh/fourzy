using System;
using System.Collections.Generic;
using System.Linq;

namespace FourzyGameModel.Model
{
    //Apprenctice AI PROFILE
    //Make winning moves only after 1-n turns
    //Make blocking moves until 1-b turns

    public class LesserBlockAI : AIPlayer
    {
        private GameState EvalState { get; set; }
        private int NumberOfMovesToConsider = 4;
        private int MinNumberTurnsBeforeWinning = 30;
        private int MaxNumberTurnsBeforeWinning = 40;

        public LesserBlockAI(GameState State)
        {
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {

            List<SimpleMove> Moves = new List<SimpleMove>() { };
            AITurnEvaluatorRevised AI = new AITurnEvaluatorRevised(EvalState);
            Moves = AI.AvailableSimpleMoves;
            if (AI.WinningTurns.Count > 0)
            {
                int NumberTurnsBeforeWinning = MinNumberTurnsBeforeWinning + (MaxNumberTurnsBeforeWinning - MinNumberTurnsBeforeWinning) * EvalState.Board.Random.RandomInteger(0, 100) / 100;

                //If beyond the turn limit, start making winning moves
                if (EvalState.TurnCount > NumberTurnsBeforeWinning)
                    return AI.WinningTurns.First();

                //Otherwise, remove these moves from consideration, even if they block.
                foreach (PlayerTurn t in AI.WinningTurns)
                {
                    SimpleMove m = (SimpleMove)t.Moves[0];
                    Moves.Remove(m);
                }
            }

            //If there are no moves that do not win, make the first one.
            if (Moves.Count == 0)
                return new PlayerTurn(AI.AvailableSimpleMoves.First());

            //Create a new evaluator minus the winning moves.
            AI = new AITurnEvaluatorRevised(EvalState, Moves);

            SimpleMove Move = null;
            List<SimpleMove> PotentialMoves = AI.TopScoringMoves(AI.AvailableSimpleMoves, NumberOfMovesToConsider);
            PotentialMoves.Reverse();

            foreach (SimpleMove m in PotentialMoves)
            {
                if (AI.IsMoveOk(m)) { Move = m; break; }
            }

            if (Move == null)
            {
                PotentialMoves.Reverse();
                foreach (SimpleMove m in PotentialMoves)
                {
                    TurnEvaluator TE = new TurnEvaluator(EvalState);
                    GameState TestState = TE.EvaluateTurn(m);
                    AITurnEvaluator AIEval = new AITurnEvaluator(TestState);
                    if (AIEval.WinningTurns.Count == 0) { Move = m; break; }
                }
            }

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