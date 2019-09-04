using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class TreeCharmPower : IBossPower, IMove
    {
        public string Name { get { return "Tree Charm"; } }
        public BossPowerType PowerType { get { return BossPowerType.TreeCharm; } }
        public MoveType MoveType { get { return MoveType.BOSSPOWER; } }

        public string Notation { get { return Name; } }

        public BoardLocation TreeLocation { get; set; }
        public Direction MoveDirection { get; set; }

        public TreeCharmPower()
        {
            this.TreeLocation = new BoardLocation(0, 0);
            this.MoveDirection = Direction.NONE;
        }

        public TreeCharmPower(BoardLocation Location, Direction MoveDirection)
        {
            this.TreeLocation = Location;
            this.MoveDirection = MoveDirection;
        }

        public bool Activate(GameState State)
        {
            List<IToken> Trees = State.Board.ContentsAt(TreeLocation).FindTokens(TokenType.FRUIT_TREE);
            foreach (IToken t in Trees)
            {
                FruitTreeToken Tree = (FruitTreeToken)t;
                BoardLocation Target = Tree.Space.Location.Neighbor(MoveDirection);
                if (!Target.OnBoard(State.Board)) return false;
                if (State.Board.ContentsAt(Target).ContainsPiece) return false;
                if (!State.Board.ContentsAt(Target).TokensAllowEndHere) return false;

                State.Board.ContentsAt(Tree.Space.Location).RemoveTokens(TokenType.FRUIT_TREE);
                State.Board.ContentsAt(Target).AddToken(new FruitTreeToken());

                State.RecordGameAction(new GameActionTokenMovement(Tree,TransitionType.BOSS_POWER, Tree.Space.Location, Target));
            }
            return true;
        }

        public bool IsAvailable(GameState State, bool IsDesparate)
        {
            return true;
        }

        public List<IMove> GetPossibleActivations(GameState State, bool IsDesparate = false)
        {
            List<IMove> Powers = new List<IMove>();
            List<IToken> Trees  = State.Board.FindTokens(TokenType.FRUIT_TREE);
            foreach (IToken t in Trees)
            {
                FruitTreeToken Tree = (FruitTreeToken)t;
                foreach (Direction d in TokenConstants.GetDirections())
                {
                    BoardLocation Target = Tree.Space.Location.Neighbor(d);
                    if (!Target.OnBoard(State.Board)) continue;
                    if (State.Board.ContentsAt(Target).ContainsPiece) continue;
                    if (State.Board.ContentsAt(Target).ContainsTokenType(TokenType.FRUIT)) continue;
                    if (!State.Board.ContentsAt(Target).TokensAllowEndHere) continue;

                    Powers.Add(new TreeCharmPower(Tree.Space.Location, d));
                }

            }

            return Powers;
        }

    }
}