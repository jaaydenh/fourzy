using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class DigSpell : ISpell, IMove
    {
        public string Name { get { return "DIG"; } }
        public SpellId SpellId { get { return SpellId.DIG; } }
        public SpellType SpellType { get { return SpellType.MOVE; } }

        public int Cost { get; set; }
        public MoveType MoveType { get { return MoveType.SPELL; } }
        public string Notation { get { return "DIG SPELL at Location=x,y "; } }

        public BoardLocation Location { get; set; }
        public int Duration { get; set; }
        public int PlayerId { get; set; }
        public bool RequiresLocation { get; set; }

        //need an empty constructor for MoveConverter
        public DigSpell() { }

        public DigSpell(int PlayerId) {
            this.Location = new BoardLocation(0,0);
            this.Duration = -1;
            this.Cost = SpellConstants.DefaultDigCost;
            this.RequiresLocation = true;
            this.PlayerId = PlayerId;
        }

        public DigSpell(int PlayerId, BoardLocation Location, int Cost = SpellConstants.DefaultDigCost)
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
                if (!GoodForDigging(s)) continue;
                
                //Is space next to a controlled space?
                foreach (BoardLocation l in s.Location.GetOrthogonals(Board))
                {
                    if (Board.ContentsAt(l).Control == PlayerId) { Locations.Add(s.Location); break; }
                }

            }

            return Locations;
        }

        private bool GoodForDigging(BoardSpace Space)
        {
            if (Space.Parent.Corners.Contains(Space.Location)) return false;
            if (!Space.ContainsOnlyTerrain) return false;
            if (Space.ContainsSpell) return false;
            if (Space.ContainsPiece) return false;
            if (!Space.TokensAllowEndHere) return false;
            if (Space.ContainsTokenType(TokenType.PIT)) return false;
            return true;
        }

        public bool Cast(GameState State, out List<IToken> tokens)
        {
            tokens = new List<IToken>();

            BoardSpace s = State.Board.ContentsAt(Location);
            if (!GoodForDigging(s)) return false;

            State.Board.RecordGameAction(new GameActionTokenDrop(new PitToken(), TransitionType.SPELL_CAST, s.Location, s.Location));
            PitToken _token = new PitToken();
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
