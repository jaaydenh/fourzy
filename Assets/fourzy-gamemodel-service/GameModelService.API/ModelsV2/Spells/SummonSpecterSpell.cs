using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class SpecterSpell : ISpell, IMove
    {
        public string Name { get { return "LURE"; } }
        public SpellId SpellId { get { return SpellId.SUMMON_SPECTER; } }
        public SpellType SpellType { get { return SpellType.MOVE; } }

        public int Cost { get; set; }
        public MoveType MoveType { get { return MoveType.SPELL; } }
        public string Notation { get { return "SPECTER SPELL at Location=x,y "; } }

        public BoardLocation Location { get; set; }
        public int Duration { get; set; }
        public int PlayerId { get; set; }
        public bool RequiresLocation { get; set; }

        //need an empty constructor for MoveConverter
        public SpecterSpell() { }

        public SpecterSpell(int PlayerId, BoardLocation Location, int Cost = SpellConstants.DefaultSpecterCost)
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
                        && !Board.Corners.Contains(s.Location)
                        && !s.ContainsPiece
                        && s.TokensAllowEndHere)
                    Locations.Add(s.Location);
            }

            return Locations;
        }

        public bool Cast(GameState State, out List<IToken> tokens)
        {
            tokens = new List<IToken>();

            BoardSpace s = State.Board.ContentsAt(Location);
            if (s.ContainsPiece
                || s.ContainsSpell
                || !s.TokensAllowEndHere)
            {
                return false;
            }

            SpecterToken _token = new SpecterToken();
            State.Board.ContentsAt(Location).AddToken(_token);
            tokens.Add(_token);

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
