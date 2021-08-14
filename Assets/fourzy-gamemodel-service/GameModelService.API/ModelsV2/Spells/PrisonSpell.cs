using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class PrisonSpell : ISpell, IMove
    {
        public string Name { get { return "HOLD"; } }
        public SpellId SpellId { get { return SpellId.PRISON; } }
        public SpellType SpellType { get { return SpellType.MOVE; } }

        public int Cost { get; set; }
        public MoveType MoveType { get { return MoveType.SPELL; } }
        public string Notation { get { return "HOLD SPELL at Location=x,y"; } }

        public BoardLocation Location { get; set; }
        public int Duration { get; set; }
        public int PlayerId { get; set; }
        public bool RequiresLocation { get; set; }

        //need an empty constructor for MoveConverter
        public PrisonSpell() { }

        public PrisonSpell(int PlayerId, BoardLocation Location, int Duration = SpellConstants.DefaultPrisonDuration, int Cost = SpellConstants.DefaultPrisonCost)
        {
            this.Location = new BoardLocation(Location);
            this.Duration = Duration;
            this.Cost = Cost;
            this.PlayerId = PlayerId;
            this.RequiresLocation = true;
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
                    || !s.ContainsPiece) return false;
            return true;

        }

        public bool Cast(GameState State, out List<IToken> tokens)
        {
            tokens = new List<IToken>();

            BoardSpace s = State.Board.ContentsAt(Location);
            if (s.ContainsSpell
                || !s.ContainsPiece) return false;

            State.Board.RecordGameAction(new GameActionTokenDrop(new PrisonToken(PlayerId, Duration), TransitionType.SPELL_CAST, Location, Location));

            PrisonToken _token = new PrisonToken(PlayerId, Duration);
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
