using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class GlobalArrowChangePower : IBossPower, IMove
    {
        public string Name { get { return "Global Direction Change"; } }

        public BossPowerType PowerType { get { return BossPowerType.GlobalArrowChange; } }
        public MoveType MoveType { get { return MoveType.BOSSPOWER; } }

        public string Notation { get { return Name; } }

        public Direction Orientation{ get; set; }

        public GlobalArrowChangePower()
        {
            this.Orientation = Direction.NONE;
        }

        public GlobalArrowChangePower(Direction Orientation)
        {
            this.Orientation = Orientation;
        }

        public bool Activate(GameState State)
        {
            List<IToken> Arrows = State.Board.FindTokens(TokenType.ARROW);
            foreach (IToken t in Arrows)
            {
                ArrowToken Arrow = (ArrowToken)t;
                if (Arrow.Orientation != Orientation )
                {
                    Direction OrigOrientation = Arrow.Orientation;
                    Arrow.Orientation = Orientation;
                    Arrow.Space.Parent.RecordGameAction(new GameActionTokenRotation(Arrow, TransitionType.BOSS_POWER, Rotation.CLOCKWISE, OrigOrientation, Arrow.Orientation));
                }
            }
            BossAIHelper.GetBoss(State).UseSpecialAbility();
            return true;
        }

        public bool IsAvailable(GameState State, bool IsDesparate = false)
        {
            if (BossAIHelper.GetBoss(State).SpecialAbilityCount > 0) return true;
            return false;
        }

        public List<IMove> GetPossibleActivations(GameState State, bool IsDesparate = false)
        {
            List<IMove> Powers = new List<IMove>();

            foreach (Direction d in Enum.GetValues(typeof(Direction)))
            {
                if (d != Direction.NONE)
                    Powers.Add(new GlobalArrowChangePower(d));
            }
            return Powers;
        }
    }

}