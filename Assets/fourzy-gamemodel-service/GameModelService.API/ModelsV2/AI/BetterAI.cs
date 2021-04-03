﻿using System.Linq;

namespace FourzyGameModel.Model
{
    public class BetterAI : AIPlayer
    {
        private GameState EvalState { get; set; }
        public BetterAI(GameState State)
        {
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            AITurnEvaluator AI = new AITurnEvaluator(EvalState);
            if (AI.WinningTurns.Count > 0) return AI.WinningTurns.First();

            //SimpleMove Move = AI.GetRandomBetterMove(3,12,6);
            SimpleMove Move = AI.GetRandomOkMove(4);
            if (Move == null) Move = AI.GetBestLostCauseMove();

            return new PlayerTurn(Move);
        }

    }
}