using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class FruitSpell : ISpell, IMove
    {
        public string Name { get { return "HEX"; } }
        public SpellId SpellId { get { return SpellId.FRUIT; } }
        public SpellType SpellType { get { return SpellType.MOVE; } }

        public int Cost { get; set; }
        public MoveType MoveType { get { return MoveType.SPELL; } }
        public string Notation { get { return "FRUIT SPELL at Location=x,y "; } }

        public BoardLocation Location { get; set; }
        public int Duration { get; set; }
        public int PlayerId { get; set; }

        public bool RequiresLocation { get; set; }

        //need an empty constructor for MoveConverter
        public FruitSpell() { }

        public FruitSpell(int PlayerId, BoardLocation Location, int Cost = SpellConstants.DefaultFruitCost)
        {
            this.Location = new BoardLocation(Location);
            this.Duration = -1;
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
                        && !s.ContainsPiece
                        && !s.ContainsTokenType(TokenType.FRUIT)
                        && !s.ContainsTokenType(TokenType.STICKY)
                        && s.TokensAllowEndHere
                        )
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
                || s.ContainsTokenType(TokenType.FRUIT)
                || s.ContainsTokenType(TokenType.STICKY)
                || !s.TokensAllowEndHere )
            {
                return false;
            }

            State.Board.RecordGameAction(new GameActionTokenDrop(new FruitToken(), TransitionType.SPELL_CAST, s.Location, s.Location));
            FruitToken _token = new FruitToken();
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
