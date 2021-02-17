using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class TriggerAllBombsPower : IBossPower, IMove
    {
        public string Name { get { return "TriggerAllBombs"; } }
        public BossPowerType PowerType { get { return BossPowerType.ArrowCharm; } }
        public MoveType MoveType { get { return MoveType.BOSSPOWER; } }

        public string Notation { get { return Name; } }

        public List<BoardLocation> Locations { get; set; }

        public TriggerAllBombsPower()
        {

        }

        public bool Activate(GameState State)
        {
            if (State.Board.FindTokens(TokenType.CIRCLE_BOMB).Count == 0) return false;
            if (BossAIHelper.GetBoss(State).SpecialAbilityCount == 0) return false;
            BossAIHelper.GetBoss(State).UseSpecialAbility();

            foreach (CircleBombToken t in State.Board.FindTokens(TokenType.CIRCLE_BOMB))
            {
                t.Explode();
            }                

            return true;
        }

        public bool IsAvailable(GameState State, bool IsDesparate = false)
        {
            if (!IsDesparate) return false;
            if (State.Board.FindTokens(TokenType.CIRCLE_BOMB).Count == 0) return false;
            if (BossAIHelper.GetBoss(State).SpecialAbilityCount > 0) return true;
            return false;
        }

        public List<IMove> GetPossibleActivations(GameState State, bool IsDesparate = false)
        {

            List<IMove> Powers = new List<IMove>();
            Powers.Add(new TriggerAllBombsPower());

            return Powers;

        }

    }
}