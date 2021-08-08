using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class SlurpSpell : ISpell, IMove
    {
        public string Name { get { return "SLURP"; } }
        public SpellId SpellId { get { return SpellId.SLURP; } }
        public SpellType SpellType { get { return SpellType.MOVE; } }

        public int Cost { get; set; }
        public MoveType MoveType { get { return MoveType.SPELL; } }
        public string Notation { get { return "SLURP SPELL at Location=x,y "; } }

        public BoardLocation Location { get; set; }
        public int Duration { get; set; }
        public int PlayerId { get; set; }

        public bool RequiresLocation { get; set; }

        //need an empty constructor for MoveConverter
        public SlurpSpell() { }

        public SlurpSpell(int PlayerId, BoardLocation Location, int Cost = SpellConstants.DefaultSlurpCost)
        {
            this.Location = new BoardLocation(Location);
            this.Duration = -1;
            this.Cost = Cost;
            this.PlayerId = PlayerId;
            this.RequiresLocation = true;
        }

        public List<BoardLocation> GetValidSpellLocations(GameBoard Board)
        {
            List<BoardLocation> Locations = Board.FindTokenLocations(TokenType.WATER);
            Locations.AddRange(Board.FindTokenLocations(TokenType.STICKY));

            foreach (BoardSpace s in Board.Contents)
            {
                if (Board.Corners.Contains(s.Location)) continue;
                if (s.ContainsSpell) continue;
 
                foreach(BoardLocation l in s.Location.GetOrthogonals(Board))
                {
                    if (Board.ContentsAt(l).Control == PlayerId) Locations.Add(s.Location);
                }
            }

            return Locations;
        }

        public bool Cast(GameState State)
        {
            if (!State.Board.ContentsAt(Location).ContainsTokenType(TokenType.WATER)
                && !State.Board.ContentsAt(Location).ContainsTokenType(TokenType.STICKY)) return false;

            bool valid = false;
            foreach (BoardLocation l in Location.GetOrthogonals(State.Board))
            {
                if (State.Board.ContentsAt(l).Control == PlayerId) { valid = true; break; }
            }
            if (valid)
                {
                    State.Board.ContentsAt(Location).RemoveTokens(TokenType.WATER);
                    State.Board.ContentsAt(Location).RemoveTokens(TokenType.STICKY);
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
