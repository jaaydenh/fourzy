using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class RainbowSpell : ISpell, IMove
    {
        public string Name { get { return "Rainbow"; } }
        public SpellId SpellId { get { return SpellId.RAINBOW; } }
        public SpellType SpellType { get { return SpellType.MOVE; } }

        public int Cost { get; set; }
        public MoveType MoveType { get { return MoveType.SPELL; } }
        public string Notation { get { return "Rainbow SPELL at Location=x,y "; } }

        public BoardLocation Location { get; set; }
        public int Duration { get; set; }
        public int PlayerId { get; set; }
        public bool RequiresLocation { get; set; }

        //need an empty constructor for MoveConverter
        public RainbowSpell() { }

        public RainbowSpell(int PlayerId, BoardLocation Location, int Cost = SpellConstants.DefaultRainbowCost)
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
                  if (ValidLocationTarget(s))
                        Locations.Add(s.Location);
            }

            return Locations;
        }

        public bool ValidLocationTarget(BoardSpace s)
        {
            if (s.ContainsSpell
            || s.Parent.Corners.Contains(s.Location)
            || s.ContainsPiece
            || !s.ContainsOnlyTerrain
            || !s.TokensAllowEndHere) return false;
            return true;
        }


        public bool Cast(GameState State)
        {
            BoardSpace s = State.Board.ContentsAt(Location);
            if (!ValidLocationTarget(s))
            {
                return false;
            }

            State.Board.RecordGameAction(new GameActionTokenDrop(new RainbowToken(), TransitionType.SPELL_CAST, Location, Location));
            State.Board.ContentsAt(Location).AddToken(new RainbowToken());
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
