using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class PuzzleTurnEvaluator
    {
        public List<GameAction> ResultActions;
        public PlayerTurn PuzzleTurn;
        private PuzzleTurnInstructions Instructions;
        public TurnEvaluator Evaluator;

        public PuzzleTurnEvaluator(GameState State, PuzzleTurnInstructions Instructions)
        {
            this.Instructions = Instructions;
            Evaluator = new TurnEvaluator(State);
        }

        public GameState Process()
        {
            if (Instructions != null)
            {
                //If there are instructions, use them.
                if (Instructions.Turn != null)
                {
                    PuzzleTurn = Instructions.Turn;
                    Evaluator.EvaluateTurn(Instructions.Turn);
                }
            }
            else
            {
                foreach (SimpleMove m in Evaluator.GetAvailableSimpleMoves())
                {
                    Evaluator.EvaluateTurn(new PlayerTurn(m));
                    if (Evaluator.DidPlayerWinRevised(Evaluator.OriginalState.ActivePlayerId))
                    {
                        Evaluator.DidPlayerWinAndFindWinningLocations(Evaluator.OriginalState.ActivePlayerId);
                        break;                            
                    }
                }
            }

            return Evaluator.EvalState;
        }
    }
}
