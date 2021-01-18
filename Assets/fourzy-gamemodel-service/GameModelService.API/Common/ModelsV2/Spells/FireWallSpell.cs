using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class FireWallSpell : ISpell, IMove
    {
        public string Name { get { return "FIRE_WALL"; } }
        public SpellId SpellId { get { return SpellId.FIRE_WALL; } }
        public SpellType SpellType { get { return SpellType.MOVE; } }

        public int Cost { get; set; }
        public MoveType MoveType { get { return MoveType.SPELL; } }
        public string Notation { get { return "FIRE_WALL SPELL at Location=x,y"; } }

        public BoardLocation Location { get; set; }
        public int Duration { get; set; }
        public int PlayerId { get; set; }
        public bool RequiresLocation { get; set; }

        //need an empty constructor for MoveConverter
        public FireWallSpell() { }

        public FireWallSpell(int PlayerId, BoardLocation Location, int Duration = SpellConstants.DefaultFireWallDuration)
        {
            this.Location = new BoardLocation(Location);
            this.Duration = Duration;
            this.Cost = SpellConstants.CostFireWallSpell;
            this.PlayerId = PlayerId;
            this.RequiresLocation = true;
    }

        public List<BoardLocation> GetValidSpellLocations(GameBoard Board)
        {
            List<BoardLocation> Locations = new List<BoardLocation>() { };

            foreach (BoardSpace s in Board.Contents)
            {
                if (AllowedLocationTarget(s))
                    Locations.Add(s.Location);
            }

            return Locations;
        }

        public bool AllowedLocationTarget(BoardSpace s)
        {
            if (s.ContainsSpell
                      || s.Parent.Corners.Contains(s.Location)
                      || s.ContainsPiece) return false;
                return true;
        }

        public bool Cast(GameState State)
        {
            BoardSpace s = State.Board.ContentsAt(Location);
            if (AllowedLocationTarget(s))
            {
                State.Board.RecordGameAction(new GameActionTokenDrop(new MagicFireToken(PlayerId, Duration), TransitionType.SPELL_CAST, s.Location, s.Location));
                State.Board.ContentsAt(Location).AddToken(new MagicFireToken(PlayerId, Duration));
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
