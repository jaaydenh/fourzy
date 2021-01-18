using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class HexSpell : ISpell, IMove
    {
        public string Name { get { return "HEX"; } }
        public SpellId SpellId { get { return SpellId.HEX; } }
        public SpellType SpellType { get { return SpellType.MOVE; } }

        public int Cost { get; set; }
        public MoveType MoveType { get { return MoveType.SPELL; } }
        public string Notation { get { return "HEX SPELL at Location=x,y"; } }

        public BoardLocation Location { get; set;  }
        public int Duration { get; set; }
        public int PlayerId { get; set; }
        public bool RequiresLocation { get; set; }

        //need an empty constructor for MoveConverter
        public HexSpell() { }

        public HexSpell(int PlayerId, BoardLocation Location, int Duration = SpellConstants.DefaultHexDuration)
        {
            this.Location = new BoardLocation(Location);
            this.Duration = Duration;
            this.Cost = SpellConstants.CostHexSpell;
            this.PlayerId = PlayerId;
            this.RequiresLocation = true;
        }

        public List<BoardLocation> GetValidSpellLocations(GameBoard Board)
        {
            List<BoardLocation> Locations = new List<BoardLocation>() { };

            foreach (BoardSpace s in Board.Contents)
            {
                if (!s.ContainsSpell
                        && !Board.Corners.Contains(s.Location)
                        && !s.ContainsPiece)
                    Locations.Add(s.Location);
            }

            return Locations;
        }

        public bool Cast(GameState State)
        {
            if (!State.Board.ContentsAt(Location).ContainsPiece)
            {
                State.Board.ContentsAt(Location).AddToken(new HexSpellToken(PlayerId, Duration));
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
