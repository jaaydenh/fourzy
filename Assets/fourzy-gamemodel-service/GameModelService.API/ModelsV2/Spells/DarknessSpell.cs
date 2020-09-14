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
            if (s.ContainsSpell
                || s.Parent.Corners.Contains(s.Location)) return false;
            return true;
        }

        public bool Cast(GameState State)
        {
            BoardSpace s = State.Board.ContentsAt(Location);
            if (ValidTargetLocation(s))
            {
                State.Board.ContentsAt(Location).AddToken(new DarknessToken(PlayerId, Duration));
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
