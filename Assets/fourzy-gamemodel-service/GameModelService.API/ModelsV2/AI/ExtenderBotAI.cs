namespace FourzyGameModel.Model
{
    public class ExtenderBotAI : AIPlayer
    {
        private GameState EvalState { get; set; }

        public ExtenderBotAI(GameState State)
        {
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            AITurnEvaluator AI = new AITurnEvaluator(EvalState);
            //if (AI.WinningTurns.Count > 0) return AI.WinningTurns.First();

            SimpleMove Move = AI.GetBestMoveWithoutWinning();
            if (Move == null) return new PlayerTurn(AI.GetBestLostCauseMove());

            return new PlayerTurn(Move);
        }
    }
}
