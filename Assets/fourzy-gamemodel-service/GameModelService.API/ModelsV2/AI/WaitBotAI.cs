using System.Linq;

namespace FourzyGameModel.Model
{
    public class WaitBotAI : AIPlayer
    {
        private GameState EvalState { get; set; }
        private int SleepTurns { get; set; }
        const int SleepScoreThreshold = 500;

        public WaitBotAI(GameState State, int SleepTurns = 3)
        {
            this.SleepTurns = SleepTurns;
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            //When to wake up??
            //  After x turns.
            //  If need to block
            //  If player is threatening to make the setup

            AITurnEvaluator AI = new AITurnEvaluator(EvalState);

            bool awake = false;
            if (EvalState.Board.FindPieces(EvalState.Opponent(EvalState.ActivePlayerId)).Count < SleepTurns)
            {
                if (AI.IsAThreat())
                {
                    awake = true;
                }
            }
            else
            {
                AIScoreEvaluator AISE = new AIScoreEvaluator(EvalState);
                if (AISE.Score(EvalState.ActivePlayerId) < SleepScoreThreshold) awake = true; 
            }

            if (!awake) return new PlayerTurn(EvalState.ActivePlayerId, new PassMove());
            
            if (AI.WinningTurns.Count > 0) return AI.WinningTurns.First();

            SimpleMove Move = AI.GetRandomOkMove();
            if (Move == null) return new PlayerTurn(AI.GetBestLostCauseMove());

            return new PlayerTurn(Move);
        }
    }
}
