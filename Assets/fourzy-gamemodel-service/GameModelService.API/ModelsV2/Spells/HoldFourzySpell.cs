using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class HoldFourzySpell : ISpell, IMove
    {
        public string Name { get { return "HOLD"; } }
        public SpellId SpellId { get { return SpellId.HOLD_FOURZY; } }
        public SpellType SpellType { get { return SpellType.MOVE; } }

        public int Cost { get; set; }
        public MoveType MoveType { get { return MoveType.SPELL; } }
        public string Notation { get { return "HOLD SPELL at Location=x,y"; } }

        public BoardLocation Location { get; set; }
        public int Duration { get; set; }
        public int PlayerId { get; set; }
        public bool RequiresLocation { get; set; }

        //need an empty constructor for MoveConverter
        public HoldFourzySpell() { }

        public HoldFourzySpell(int PlayerId, BoardLocation Location, int Duration = SpellConstants.DefaultHoldDuration, int Cost = SpellConstants.DefaultHoldCost)
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
                if (!s.ContainsSpell
                        && s.ContainsPiece)
                    Locations.Add(s.Location);
            }

            return Locations;
        }

        public bool Cast(GameState State, out List<IToken> tokens)
        {
            tokens = new List<IToken>();

            if (State.Board.ContentsAt(Location).ContainsPiece)
            {
                State.Board.RecordGameAction(new GameActionTokenDrop(new HoldSpellToken(PlayerId, Duration), TransitionType.SPELL_CAST, Location, Location));

                HoldSpellToken _token = new HoldSpellToken(PlayerId, Duration);
                State.Board.ContentsAt(Location).AddToken(_token);
                tokens.Add(_token);

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
