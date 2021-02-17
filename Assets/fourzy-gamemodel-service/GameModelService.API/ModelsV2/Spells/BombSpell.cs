 using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class BombSpell : ISpell, IMove
    {
        public string Name { get { return "BOMB"; } }
        public SpellId SpellId { get { return SpellId.THROW_BOMB; } }
        public SpellType SpellType { get { return SpellType.MOVE; } }

        public int Cost { get; set; }
        public MoveType MoveType { get { return MoveType.SPELL; } }
        public string Notation { get { return "BOMB SPELL at Location=x,y "; } }

        public BoardLocation Location { get; set; }
        public int Duration { get; set; }
        public int PlayerId { get; set; }
        public bool RequiresLocation { get; set; }
        
        //need an empty constructor for MoveConverter
        public BombSpell() { }

        public BombSpell(int PlayerId, BoardLocation Location, int Cost = SpellConstants.DefaultBombCost, int Fuse = SpellConstants.DefaultBombFuse)
        {
            this.Location = new BoardLocation(Location);
            this.Duration = Fuse;
            this.Cost = Cost;
            this.RequiresLocation = true;
            this.PlayerId = PlayerId;
        }

        public List<BoardLocation> GetValidSpellLocations(GameBoard Board)
        {
            List<BoardLocation> Locations = new List<BoardLocation>() { };

            foreach (BoardSpace s in Board.Contents)
            {
                    if (!ValidLocationTarget(s)) continue;
                    //Is space next to a controlled space?
                    foreach (BoardLocation l in s.Location.GetOrthogonals(Board))
                    {
                        if (Board.ContentsAt(l).Control == PlayerId) { Locations.Add(s.Location); break; }
                    }

            }

            return Locations;
        }

        public bool ValidLocationTarget(BoardSpace s)
        {
            if (s.ContainsSpell
                    || s.Parent.Corners.Contains(s.Location)
                    || s.ContainsPiece
                    || !s.ContainsOnlyTerrain
                    || !s.TokensAllowEndHere) return false;
            return true;

        }

    public bool Cast(GameState State)
        {
            BoardSpace s = State.Board.ContentsAt(Location);
            if (!ValidLocationTarget(s))
            {
                return false;
            }

            State.Board.RecordGameAction(new GameActionTokenDrop(new CrossBombToken(Duration), TransitionType.SPELL_CAST, s.Location, s.Location));
            State.Board.ContentsAt(Location).AddToken(new CrossBombToken(Duration));
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
