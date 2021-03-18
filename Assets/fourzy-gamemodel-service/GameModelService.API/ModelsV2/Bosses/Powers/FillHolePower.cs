using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class FillHolePower : IBossPower, IMove
    {
        public string Name { get { return "Fill Hole"; } }
        public BossPowerType PowerType { get { return BossPowerType.FillHole; } }
        public MoveType MoveType { get { return MoveType.BOSSPOWER; } }

        public string Notation { get { return Name; } }

        public BoardLocation PitLocation { get; set; }

        public FillHolePower()
        {
            this.PitLocation = new BoardLocation(0, 0);
        }

        public FillHolePower(BoardLocation Location)
        {
            this.PitLocation = Location;
        }

        public bool Activate(GameState State)
        {
            List<IToken> Holes = State.Board.ContentsAt(PitLocation).FindTokens(TokenType.PIT);
            foreach (IToken t in Holes)
            {
                PitToken Hole = (PitToken)t;
                Hole.Space.Parent.RecordGameAction(new GameActionTokenRemove(Hole.Space.Location, TransitionType.BOSS_POWER, Hole));
            }
            return true;
        }

        public bool IsAvailable(GameState State, bool IsDesparate = false)
        {
            //Filling a hole won't help
            if (IsDesparate) return false;

            int HoleCount = State.Board.FindTokens(TokenType.PIT).Count;
            if (HoleCount >= State.Board.Rows / 2) return true;
            return false;
        }

        public List<IMove> GetPossibleActivations(GameState State, bool IsDesparate = false)
        {
            List<IMove> Powers = new List<IMove>();
            List<IToken> Holes = State.Board.FindTokens(TokenType.PIT);
            foreach (IToken t in Holes)
            {
                PitToken Hole = (PitToken)t;
                Powers.Add(new FillHolePower(Hole.Space.Location));
            }

            return Powers;
        }

    }
}