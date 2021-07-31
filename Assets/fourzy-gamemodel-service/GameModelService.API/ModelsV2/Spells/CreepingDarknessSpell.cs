using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class CreepingDarknessSpell : ISpell, IMove
    {
        public string Name { get { return "CREEPING DARKNESS"; } }
        public SpellId SpellId { get { return SpellId.CREEPING_DARKNESS; } }
        public SpellType SpellType { get { return SpellType.MOVE; } }

        public int Cost { get; set; }
        public MoveType MoveType { get { return MoveType.SPELL; } }
        public string Notation { get { return "CREEPING DARKNESS SPELL at Location=x,y"; } }

        public List<BoardLocation> Locations { get; set; }
        public int PlayerId { get; set; }
        public bool RequiresLocation { get; set; }

        //need an empty constructor for MoveConverter
        public CreepingDarknessSpell() { }

        public CreepingDarknessSpell(int PlayerId, List<BoardLocation> Locations, int Cost = SpellConstants.DefaultDarknessCost)
        {
            this.Locations = new List<BoardLocation>() { };
            foreach (BoardLocation l in Locations)
                this.Locations.Add(l);
            this.Cost = Cost;
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

        public bool Cast(GameState State, out List<IToken> tokens)
        {
            tokens = new List<IToken>();
            //if (!State.Board.ContentsAt(Location).ContainsPiece)
            //{
            //    State.Board.ContentsAt(Location).AddToken(new DarknessToken(PlayerId, Duration));
            //    return true;
            //}
            //return false;

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
