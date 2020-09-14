using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class IceWallSpell : ISpell, IMove
    {
        public string Name { get { return "ICE_WALL"; } }
        public SpellId SpellId { get { return SpellId.ICE_WALL; } }
        public SpellType SpellType { get { return SpellType.MOVE; } }

        public int Cost { get; set; }
        public MoveType MoveType { get { return MoveType.SPELL; } }
        public string Notation { get { return "ICE WALL SPELL at Location=x,y"; } }

        public int PlayerId { get; set; }

        public BoardLocation Location { get; set; }
        public bool RequiresLocation { get; set; }

        public IceWallSpell(BoardLocation Location)
        {
            this.Location = new BoardLocation(Location);
            this.Cost = SpellConstants.CostIceWallSpell;
            this.PlayerId = PlayerId;
            this.RequiresLocation = true;
        }
        
        // Not another spell
        // Not a corner
        // Does not contain a piece
        // Only terrain.  No arrows.
        // No fire.  (Not implemented yet)

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
                || s.ContainsOnlyTerrain
                || s.Parent.Corners.Contains(s.Location)
                || s.ContainsPiece) return false;
            return true;
        }

        public bool Cast(GameState State)
        {
            BoardSpace s = State.Board.ContentsAt(Location);
            if (ValidLocationTarget(s))
            {
                State.Board.ContentsAt(Location).AddToken(new IceBlockToken());
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
