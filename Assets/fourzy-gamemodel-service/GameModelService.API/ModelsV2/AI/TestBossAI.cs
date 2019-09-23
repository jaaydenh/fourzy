using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class TestBossAI : AIPlayer
    {
        private GameState EvalState { get; set; }
        private BossType Type { get; set; }
        private IBoss Boss {get; set;}

        public TestBossAI(GameState State, BossType Type)
        {
            this.Type = Type;
            Boss = BossFactory.Create(Type);

            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            AITurnEvaluator AI = new AITurnEvaluator(EvalState);
            if (AI.WinningTurns.Count > 0) return AI.WinningTurns.First();

            bool Desparate = false;

            SimpleMove Move = AI.GetRandomOkMove();
            if (Move == null)
            {
                Desparate = true;
                Move = AI.GetTopMoveNoRestriction();
            }
            
            PlayerTurn TopBossTurn = new PlayerTurn(Move);
            //int TopScore = AITurnEvaluator.Score(EvalState, TopBossTurn);
            AITurnEvaluator AIBoss = new AITurnEvaluator(EvalState, TopBossTurn);
            int TopScore = AIBoss.Score(AIBoss.ActivePlayerId);

            List<IMove> Powers = Boss.GetPossibleActivations(EvalState, Desparate);

            if (Powers.Count > 0)
            {
                foreach(IBossPower p in Powers)
                {
                    PlayerTurn TestTurn = new PlayerTurn(Move);
                    TestTurn.Moves.Add((IMove)p);

                    AITurnEvaluator AITE = new AITurnEvaluator(EvalState, TestTurn);
                    if (AITE.Evaluator.GetFirstWinningMove() != null) continue;

                    int Score = AITE.Score(AITE.ActivePlayerId);
                    //int Score = AITurnEvaluator.Score(EvalState, TestTurn);
                    if (Score > TopScore)
                    {
                        TopBossTurn = TestTurn;
                    }
                }
            }

            return TopBossTurn;
        }

    }
}
