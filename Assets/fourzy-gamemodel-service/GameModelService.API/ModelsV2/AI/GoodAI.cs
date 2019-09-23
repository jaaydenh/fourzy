using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class SomewhatGoodAI : AIPlayer
    {
        private GameState EvalState { get; set; }
        public SomewhatGoodAI(GameState State)
        {
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            AITurnEvaluator AI = new AITurnEvaluator(EvalState);
            if (AI.WinningTurns.Count > 0) return AI.WinningTurns.First();

            //SimpleMove Move = AI.GetGoodMove(4);
            SimpleMove Move = AI.GetRandomOkMove(4);
            if (Move == null) Move = AI.GetBestLostCauseMove();

            return new PlayerTurn(Move);
        }

    }
}
