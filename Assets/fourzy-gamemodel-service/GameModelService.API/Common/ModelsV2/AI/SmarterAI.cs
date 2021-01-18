using System.Linq;

namespace FourzyGameModel.Model
{
    public class SmartBotAI : AIPlayer
    {
        private GameState EvalState { get; set; }

        public SmartBotAI(GameState State)
        {
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            AITurnEvaluator AI = new AITurnEvaluator(EvalState);
            if (AI.WinningTurns.Count > 0) return AI.WinningTurns.First();

            AI.AIHeuristics.LookForSetups = true;
            //AI.AIHeuristics.IsAggressive = true;
            AI.AIHeuristics.LookForUnstoppable = true;
            SimpleMove Move = AI.GetRandomOkMove();
            if (Move == null) return new PlayerTurn(AI.GetBestLostCauseMove());

            return new PlayerTurn(Move);
        }
    }
}
