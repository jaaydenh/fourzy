using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class BlockASidePower : IBossPower, IMove
    {
        public string Name { get { return "Global Direction Change"; } }

        public BossPowerType PowerType { get { return BossPowerType.BlockASide; } }
        public MoveType MoveType { get { return MoveType.BOSSPOWER; } }

        public string Notation { get { return Name; } }

        public Direction BlockOrientation { get; set; }

        public BlockASidePower()
        {
            this.BlockOrientation = Direction.NONE;
        }

        public BlockASidePower(Direction BlockOrientation)
        {
            this.BlockOrientation = BlockOrientation;
        }

        public bool Activate(GameState State)
        {
            List<IToken> Blockers = State.Board.FindTokens(TokenType.MOVE_BLOCKER);
            foreach (BoardLocation l in State.Board.Edges)
            {
                if (State.Board.ContentsAt(l).ContainsTokenType(TokenType.MOVE_BLOCKER))
                {
                    State.Board.ContentsAt(l).RemoveTokens(TokenType.MOVE_BLOCKER);
                    State.Board.RecordGameAction(new GameActionTokenRemove(l, TransitionType.BOSS_POWER, null));
                }
            }

            List<BoardLocation> Locations = new List<BoardLocation>();
            switch (BlockOrientation)
            {
                case Direction.UP:
                    for (int c=1; c<State.Board.Columns-2; c++)
                    {
                        Locations.Add(new BoardLocation(0, c));
                    }
                    break;
                case Direction.DOWN:
                    for (int c = 1; c < State.Board.Columns - 2; c++)
                    {
                        Locations.Add(new BoardLocation(State.Board.Rows-1, c));
                    }
                    break;
                case Direction.LEFT:
                    for (int r = 1; r < State.Board.Rows - 2; r++)
                    {
                        Locations.Add(new BoardLocation(r,0));
                    }
                    break;
                case Direction.RIGHT:
                    for (int r = 1; r < State.Board.Rows - 2; r++)
                    {
                        Locations.Add(new BoardLocation(r, State.Board.Columns-1));
                    }
                    break;
            }
            foreach (BoardLocation BlockLocation in Locations)
            {
                State.Board.AddToken(new MoveBlockerToken(), BlockLocation);
                State.Board.RecordGameAction(new GameActionTokenDrop(new MoveBlockerToken(), TransitionType.BOSS_POWER, BlockLocation, BlockLocation));
            }
            State.Board.RecordGameAction(new GameActionBossPower(this));

            BossAIHelper.GetBoss(State).UseSpecialAbility();
            return true;
        }

        public bool IsAvailable(GameState State, bool IsDesparate=false)
        {
            if (BossAIHelper.GetBoss(State).SpecialAbilityCount > 0 && IsDesparate) return true;
            return false;
        }

        public List<IMove> GetPossibleActivations(GameState State, bool IsDesparate = false)
        {
            List<IMove> Powers = new List<IMove>();

            foreach (Direction d in TokenConstants.GetDirections())
            {
                    Powers.Add(new BlockASidePower(d));
            }
            return Powers;
        }
    }

}