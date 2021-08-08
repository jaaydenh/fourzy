using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class GrowlSpell : ISpell, IMove
    {
        public string Name { get { return "GROWL"; } }
        public SpellId SpellId { get { return SpellId.GROWL; } }
        public SpellType SpellType { get { return SpellType.MOVE; } }

        public int Cost { get; set; }
        public MoveType MoveType { get { return MoveType.SPELL; } }
        public string Notation { get { return "GROWL SPELL at Location=x,y "; } }

        public BoardLocation Location { get; set; }
        public int Duration { get; set; }
        public int PlayerId { get; set; }

        public bool RequiresLocation { get; set; }

        //need an empty constructor for MoveConverter
        public GrowlSpell() { }

        public GrowlSpell(int PlayerId, BoardLocation Location, int Cost = SpellConstants.DefaultGrowlCost)
        {
            this.Location = new BoardLocation(Location);
            this.Duration = -1;
            this.Cost = Cost;
            this.PlayerId = PlayerId;
            this.RequiresLocation = true;
        }

        public List<BoardLocation> GetValidSpellLocations(GameBoard Board)
        {
            List<BoardLocation> Possible = Board.FindPieceLocations(PlayerId);
            List<BoardLocation> Locations = new List<BoardLocation>() { };

            foreach (BoardLocation l in Possible)
            {
                BoardSpace s = Board.ContentsAt(l);
                if (s.ContainsSpell) continue;
                
                foreach (BoardLocation adj in s.Location.GetOrthogonals(Board))
                {
                    if (Board.ContentsAt(l).ContainsPiece) { Locations.Add(s.Location); break; }
                }
            }

            return Locations;
        }

        public bool Cast(GameState State, out List<IToken> tokens)
        {
            tokens = new List<IToken>();

            BoardSpace s = State.Board.ContentsAt(Location);
            if (!s.ContainsPiece || s.Control != PlayerId) return false;
            Growl(State, Direction.UP);
            Growl(State, Direction.DOWN);
            Growl(State, Direction.LEFT);
            Growl(State, Direction.RIGHT);

            //Check each adjacent space and try to move each piece.

            return true;
        }

        //l2 = the opponent Fourzy
        //l3 = the next space where it would move.
        private void Growl(GameState State, Direction GrowlDirection)
        {
            BoardLocation l2 = Location.Neighbor(GrowlDirection);
            BoardLocation l3 = Location.Neighbor(GrowlDirection, 2);

            //check if anything in the growl space.
            if (!l2.OnBoard(State.Board)) return;
            if (!l3.OnBoard(State.Board)) return;

            BoardSpace s1 = State.Board.ContentsAt(Location);
            BoardSpace s2 = State.Board.ContentsAt(l2);

            //Is there a piece.
            //Should the growl work only on other players?  I guess let's try this for now.
            if (!s2.ContainsPiece || s2.Control == PlayerId) return;

            //Is there a place to move to.
            BoardSpace s3 = State.Board.ContentsAt(l3);
            if (!s3.TokensAllowEndHere || !s3.TokensAllowEnter) return;

            Piece p = State.Board.ContentsAt(l2).RemovePieceFrom();
            State.Board.ContentsAt(l3).AddPiece(p);
            MovingPiece mp = new MovingPiece(p, l2, GrowlDirection, 1);
            State.Board.RecordGameAction(new GameActionMove(mp, l2, l3));

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
