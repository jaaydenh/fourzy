using System.Linq;

namespace FourzyGameModel.Model
{
    public class ScoreBotAI : AIPlayer
    {
        private GameState EvalState { get; set; }

        public ScoreBotAI(GameState State)
        {
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            AITurnEvaluator AI = new AITurnEvaluator(EvalState);
            if (AI.WinningTurns.Count > 0) return AI.WinningTurns.First();

             SimpleMove Move = AI.GetBestOkMovesWithLeastOppScore();
            if (Move == null) return AI.GetRandomTurn();

            return new PlayerTurn(Move);
        }
    }
}
