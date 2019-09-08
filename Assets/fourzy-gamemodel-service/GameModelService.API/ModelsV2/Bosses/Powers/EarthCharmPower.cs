using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class EarthCharmPower : IBossPower, IMove
    {
        public string Name { get { return "Earth Charm"; } }
        public BossPowerType PowerType { get { return BossPowerType.EarthCharm; } }
        public MoveType MoveType { get { return MoveType.BOSSPOWER; } }

        public string Notation { get { return Name; } }

        public BoardLocation HoleLocation { get; set; }
        public Direction MoveDirection { get; set; }

        public EarthCharmPower()
        {
            this.HoleLocation = new BoardLocation(0, 0);
            this.MoveDirection = Direction.NONE;
        }

        public EarthCharmPower(BoardLocation Location, Direction MoveDirection)
        {
            this.HoleLocation = Location;
            this.MoveDirection = MoveDirection;
        }

        public bool Activate(GameState State)
        {
            List<IToken> Holes = State.Board.ContentsAt(HoleLocation).FindTokens(TokenType.PIT);
            foreach (IToken t in Holes)
            {
                PitToken Hole = (PitToken)t;
                BoardLocation Target = Hole.Space.Location.Neighbor(MoveDirection);
                if (!Target.OnBoard(State.Board)) return false;
                if (State.Board.ContentsAt(Target).ContainsPiece) return false;
                if (!State.Board.ContentsAt(Target).TokensAllowEndHere) return false;

                State.Board.ContentsAt(Hole.Space.Location).RemoveTokens(TokenType.PIT);
                State.Board.ContentsAt(Target).AddToken(new PitToken());

                State.RecordGameAction(new GameActionTokenMovement(Hole, TransitionType.BOSS_POWER, Hole.Space.Location, Target));
            }
            return true;
        }

        public bool IsAvailable(GameState State, bool IsDesparate = false)
        {
            return true;
        }

        public List<IMove> GetPossibleActivations(GameState State, bool IsDesparate = false)
        {
            List<IMove> Powers = new List<IMove>();
            List<IToken> Holes = State.Board.FindTokens(TokenType.PIT);
            foreach (IToken t in Holes)
            {
                PitToken Hole = (PitToken)t;
                foreach (Direction d in Enum.GetValues(typeof(Direction)))
                {
                    if (d == Direction.NONE) continue;
                    BoardLocation Target = Hole.Space.Location.Neighbor(d);
                    if (!Target.OnBoard(State.Board)) continue;
                    if (State.Board.ContentsAt(Target).ContainsPiece) continue;
                    if (!State.Board.ContentsAt(Target).TokensAllowEndHere) continue;

                    Powers.Add(new EarthCharmPower(Hole.Space.Location, d));
                }

            }

            return Powers;
        }

    }
}