using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class BossAI : AIPlayer
    {
        private GameState EvalState { get; set; }
        private BossType Type { get; set; }
        private IBoss Boss { get; set; }

        public BossAI(GameState State, BossType Type)
        {
            this.Type = Type;
            Boss = BossFactory.Create(Type);

            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            //Rule 1: Win if possible.
            AITurnEvaluator AI = new AITurnEvaluator(EvalState);
            if (AI.WinningTurns.Count > 0) return AI.WinningTurns.First();

            List<SimpleMove> OkMoves = AI.GetTopOkMoves(6);
            //Flag if no non-winning moves.
            bool Desparate = false;

            if (OkMoves == null)
            {
                Desparate = true;
            }

            if (!Desparate && Boss.UseCustomAI)
                return Boss.GetTurn(EvalState);

            //if (Desparate)
            //{
            //    OkMoves = new List<SimpleMove>();
            //    OkMoves.Add(AI.GetBestLostCauseMove());
            //}
            List<IMove> Powers = Boss.GetPossibleActivations(EvalState, Desparate);

            if (Desparate)
            {
                List<PlayerTurn> OkBossTurns = BossAIHelper.FindOkMove(EvalState, AI.AvailableSimpleMoves, Powers, 1);
                if (OkBossTurns.Count == 0) return new PlayerTurn(AI.GetBestLostCauseMove());
                if (OkBossTurns.Count >= 1) return OkBossTurns.First();
            }

            PlayerTurn TopBossTurn = BossAIHelper.GetBestBossTurn(EvalState, OkMoves, Powers);
            return TopBossTurn;
        }

    }
}
