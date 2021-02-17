using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class LureSpell : ISpell, IMove
    {
        public string Name { get { return "LURE"; } }
        public SpellId SpellId { get { return SpellId.PLACE_LURE; } }
        public SpellType SpellType { get { return SpellType.MOVE; } }

        public int Cost { get; set; }
        public MoveType MoveType { get { return MoveType.SPELL; } }
        public string Notation { get { return "LURE SPELL at Location=x,y "; } }

        public BoardLocation Location { get; set; }
        public int Duration { get; set; }
        public int PlayerId { get; set; }
        public bool RequiresLocation { get; set; }

        //need an empty constructor for MoveConverter
        public LureSpell() { }

        public LureSpell(int PlayerId, BoardLocation Location, int Cost = SpellConstants.DefaultLureCost)
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
                if (ValidTargetLocation(s))
                    Locations.Add(s.Location);
            }

            return Locations;
        }

        public bool ValidTargetLocation(BoardSpace s)
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
            if (!ValidTargetLocation(s))
            {
                return false;
            }

            State.Board.RecordGameAction(new GameActionTokenDrop(new LureToken(), TransitionType.SPELL_CAST, s.Location, s.Location));
            State.Board.ContentsAt(Location).AddToken(new LureToken());
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
