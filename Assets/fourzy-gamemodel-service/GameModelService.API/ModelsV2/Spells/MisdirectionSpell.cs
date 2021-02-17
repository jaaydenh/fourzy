using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class MisdirectionSpell : ISpell, IMove
    {
        public string Name { get { return "HEX"; } }
        public SpellId SpellId { get { return SpellId.MISDIRECTION; } }
        public SpellType SpellType { get { return SpellType.MOVE; } }

        public int Cost { get; set; }
        public MoveType MoveType { get { return MoveType.SPELL; } }
        public string Notation { get { return "MISDIRECTION SPELL at Location=x,y Direction=z"; } }

        public BoardLocation Location { get; set; }
        public int Duration { get; set; }
        public Direction ArrowDirection { get; set; }
        public int PlayerId { get; set; }

        public bool RequiresLocation { get; set; }

        //need an empty constructor for MoveConverter
        public MisdirectionSpell() { }

        public MisdirectionSpell(int PlayerId, BoardLocation Location, Direction ArrowDirection, int Cost = SpellConstants.DefaultMisdirectionCost)
        {
            this.Location = new BoardLocation(Location);
            this.Duration = -1;
            this.ArrowDirection = ArrowDirection;
            this.Cost = Cost;
            this.PlayerId = PlayerId;
            this.RequiresLocation = true;
        }

        public List<BoardLocation> GetValidSpellLocations(GameBoard Board)
        {
            List<BoardLocation> Locations = new List<BoardLocation>() { };

            foreach (BoardSpace s in Board.Contents)
            {
                    if (ValidLocationTarget(s))
                        Locations.Add(s.Location);
            }

            return Locations;
        }

        public bool ValidLocationTarget(BoardSpace s)
        {
            if (s.ContainsSpell
                       || s.Parent.Corners.Contains(s.Location)
                       || !s.ContainsOnlyTerrain
                       || s.ContainsPiece) return false;
            return true;
        }

        public bool Cast(GameState State)
        {
            BoardSpace s = State.Board.ContentsAt(Location);
            if (!ValidLocationTarget(s))
            {
                return false;
            }

            State.Board.RecordGameAction(new GameActionTokenDrop(new ArrowOnceToken(this.ArrowDirection), TransitionType.SPELL_CAST, s.Location, s.Location));
            State.Board.ContentsAt(Location).AddToken(new ArrowOnceToken(this.ArrowDirection));
            return true;

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
