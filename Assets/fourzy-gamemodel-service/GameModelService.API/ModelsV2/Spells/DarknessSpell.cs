using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class DarknessSpell : ISpell, IMove
    {
        public string Name { get { return "DARKNESS"; } }
        public SpellId SpellId { get { return SpellId.DARKNESS; } }
        public SpellType SpellType { get { return SpellType.MOVE; } }

        public int Cost { get; set; }
        public MoveType MoveType { get { return MoveType.SPELL; } }
        public string Notation { get { return "DARKNESS SPELL at Location=x,y"; } }

        public BoardLocation Location { get; set; }
        public int Duration { get; set; }
        public int PlayerId { get; set; }
        public bool RequiresLocation { get; set; }

        //need an empty constructor for MoveConverter
        public DarknessSpell() { }

        public DarknessSpell(int PlayerId, BoardLocation Location, int Duration = SpellConstants.DefaultDarknessDuration, int Cost = SpellConstants.DefaultDarknessCost)
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
            if (s.ContainsTokenType(TokenType.DARKNESS)) return true;
            if (s.ContainsSpell
                || s.Parent.Corners.Contains(s.Location)) return false;
            return true;
        }

        public bool Cast(GameState State, out List<IToken> tokens)
        {
            tokens = new List<IToken>();

            BoardSpace s = State.Board.ContentsAt(Location);
            if (ValidTargetLocation(s))
            {

                if (State.Board.ContentsAt(Location).ContainsTokenType(TokenType.DARKNESS))
                {
                    List<IToken> dark = State.Board.ContentsAt(Location).FindTokens(TokenType.DARKNESS);
                    ((DarknessToken)dark[0]).Countdown += Duration;
                    State.Board.RecordGameAction(new GameActionAdjustTokenCountdown((DarknessToken)dark[0], Duration));
                }
                else
                {
                    DarknessToken _token = new DarknessToken(PlayerId, Duration);

                    tokens.Add(_token);
                    State.Board.ContentsAt(Location).AddToken(_token);
                }
                State.Board.RecordGameAction(new GameActionTokenDrop(new DarknessToken(PlayerId,Duration), TransitionType.SPELL_CAST, Location, Location));

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
