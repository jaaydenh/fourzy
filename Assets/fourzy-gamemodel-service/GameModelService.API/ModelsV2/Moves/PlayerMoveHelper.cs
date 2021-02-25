using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class PlayerMoveHelper
    {
        private AITurnEvaluator AITE { get; set; }

        public PlayerMoveHelper(GameState State)
        {
            this.AITE = new AITurnEvaluator(State);
        }

        public bool CanCurrentPlayerWin { get
            {
                return AITE.WinningTurns.Count > 0;
            }
        }

        public bool OpponentThreateningToWin
        { get
            {
                return AITE.IsAThreat();
            }
        }

        public PlayerTurn RecommendMove()
        {
            AIPlayer AI = AIPlayerFactory.Create(AITE.EvalState,AIProfile.SmartBot);
            return AI.GetTurn();
        }


        
    }
}
