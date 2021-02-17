using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class WallOfGoopPower : IBossPower, IMove
    {
        public string Name { get { return "Wall Of Goop"; } }

        public BossPowerType PowerType { get { return BossPowerType.WallOfGoop; } }
        public MoveType MoveType { get { return MoveType.BOSSPOWER; } }

        public string Notation { get { return Name; } }

        public Direction BlockOrientation { get; set; }

        public WallOfGoopPower()
        {
            this.BlockOrientation = Direction.NONE;
        }

        public WallOfGoopPower(Direction BlockOrientation)
        {
            this.BlockOrientation = BlockOrientation;
        }

        public bool Activate(GameState State)
        {
            List<BoardLocation> Locations = new List<BoardLocation>();
            switch (BlockOrientation)
            {
                case Direction.UP:
                    for (int c = 1; c < State.Board.Columns - 2; c++)
                    {
                        Locations.Add(new BoardLocation(0, c));
                    }
                    break;
                case Direction.DOWN:
                    for (int c = 1; c < State.Board.Columns - 2; c++)
                    {
                        Locations.Add(new BoardLocation(State.Board.Rows - 1, c));
                    }
                    break;
                case Direction.LEFT:
                    for (int r = 1; r < State.Board.Rows - 2; r++)
                    {
                        Locations.Add(new BoardLocation(r, 0));
                    }
                    break;
                case Direction.RIGHT:
                    for (int r = 1; r < State.Board.Rows - 2; r++)
                    {
                        Locations.Add(new BoardLocation(r, State.Board.Columns - 1));
                    }
                    break;
            }
            foreach (BoardLocation l in Locations)
            {
                if (State.Board.ContentsAt(l).TokensAllowEnter && !State.Board.ContentsAt(l).TokensAllowPushing)
                {
                    State.Board.AddToken(new StickyToken(), l);
                    State.Board.RecordGameAction(new GameActionTokenDrop(new StickyToken(), TransitionType.BOSS_POWER, l, l));
                }
            }
            State.Board.RecordGameAction(new GameActionBossPower(this));

            BossAIHelper.GetBoss(State).UseSpecialAbility();
            return true;
        }

        public bool IsAvailable(GameState State, bool IsDesparate = false)
        {
            if (BossAIHelper.GetBoss(State).SpecialAbilityCount > 0 && IsDesparate) return true;
            return false;
        }

        public List<IMove> GetPossibleActivations(GameState State, bool IsDesparate = false)
        {
            List<IMove> Powers = new List<IMove>();
            if (BossAIHelper.GetBoss(State).SpecialAbilityCount == 0) return Powers;

            foreach (Direction d in TokenConstants.GetDirections())
            {
                Powers.Add(new WallOfGoopPower(d));
            }
            return Powers;
        }
    }

}