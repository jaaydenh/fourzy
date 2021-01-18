using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class LifeSpell : ISpell, IMove
    {
        public string Name { get { return "Life"; } }
        public SpellId SpellId { get { return SpellId.LIFE; } }
        public SpellType SpellType { get { return SpellType.MOVE; } }

        public int Cost { get; set; }
        public MoveType MoveType { get { return MoveType.SPELL; } }
        public string Notation { get { return "LIFE SPELL at Location=x,y "; } }

        public BoardLocation Location { get; set; }
        public int Duration { get; set; }
        public int PlayerId { get; set; }
        public bool RequiresLocation { get; set; }

        //need an empty constructor for MoveConverter
        public LifeSpell() { }

        public LifeSpell(int PlayerId, BoardLocation Location, int Cost = SpellConstants.DefaultLifeCost)
        {
            this.Location = new BoardLocation(Location);
            this.Duration = -1;
            this.Cost = Cost;
            this.RequiresLocation = true;
            this.PlayerId = PlayerId;
        }

        public List<BoardLocation> GetValidSpellLocations(GameBoard Board)
        {
            List<BoardLocation> Locations = new List<BoardLocation>() { };

            foreach (BoardSpace s in Board.Contents)
            {
                if (!s.ContainsSpell
                        && !Board.Corners.Contains(s.Location))
                    Locations.Add(s.Location);
            }

            return Locations;
        }

        public bool Cast(GameState State)
        {
            BoardSpace s = State.Board.ContentsAt(Location);
            if (s.ContainsSpell)
            {
                return false;
            }

            s.ApplyElement(ElementType.LIFE);
            if (!s.ContainsTerrain) s.AddToken(new SproutToken());
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
