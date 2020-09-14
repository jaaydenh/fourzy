using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class PunchSpell : ISpell, IMove
    {
        public string Name { get { return "SLURP"; } }
        public SpellId SpellId { get { return SpellId.SLURP; } }
        public SpellType SpellType { get { return SpellType.MOVE; } }

        public int Cost { get; set; }
        public MoveType MoveType { get { return MoveType.SPELL; } }
        public string Notation { get { return "PUNCH SPELL at Location=x,y "; } }

        public BoardLocation Location { get; set; }
        public int Duration { get; set; }
        public int PlayerId { get; set; }

        public bool RequiresLocation { get; set; }

        //need an empty constructor for MoveConverter
        public PunchSpell() { }

        public PunchSpell(int PlayerId, BoardLocation Location, int Cost = SpellConstants.DefaultPunchCost)
        {
            this.Location = new BoardLocation(Location);
            this.Duration = -1;
            this.Cost = Cost;
            this.PlayerId = PlayerId;
            this.RequiresLocation = true;
        }

        public List<BoardLocation> GetValidSpellLocations(GameBoard Board)
        {
            List<BoardLocation> Possible = Board.FindPieceLocations(Board.Parent.Opponent(PlayerId));
            List<BoardLocation> Locations = new List<BoardLocation>() { };

            foreach (BoardLocation l in Locations)
            {
                BoardSpace s = Board.ContentsAt(l);    
                if (s.ContainsSpell) continue;

                foreach (BoardLocation l2 in l.GetOrthogonals(Board))
                {
                    if (Board.ContentsAt(l).Control == PlayerId) Locations.Add(s.Location);
                }
            }

            return Locations;
        }

        public bool Cast(GameState State)
        {
            BoardSpace s = State.Board.ContentsAt(Location);

            if (!s.ContainsPiece) return false;
            if (s.Control == PlayerId) return false;

            bool valid = false;
            foreach (BoardLocation l in Location.GetOrthogonals(State.Board))
            {
                if (State.Board.ContentsAt(l).Control == PlayerId) { valid = true; break; }
            }
            if (valid)
            {
                Piece p = State.Board.ContentsAt(Location).ActivePiece;
                State.Board.RecordGameAction(new GameActionPieceCondition(p,Location,PieceConditionType.STUNNED));
                p.Conditions.Add(PieceConditionType.STUNNED);
                return true;
            }

            return false;
        }

        public void StartOfTurn(int PlayerId)
        {

        }

        public string Export()
        {
            return "";
        }

    }
}
