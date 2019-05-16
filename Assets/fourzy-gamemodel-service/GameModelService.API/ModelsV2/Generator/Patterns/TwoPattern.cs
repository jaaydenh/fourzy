using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class TwoPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        public TwoPattern(GameBoard Board, BoardLocation Reference, CompassDirection Direction, int Separation)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();
            Locations.Add(Reference);

            if (Reference.Row == 0 && Reference.Column==0) Reference = Board.Random.RandomLocation(new BoardLocation(1, 1), Board.Rows - 5, Board.Columns - 5);
            if (Direction == CompassDirection.NONE) Direction = Board.Random.RandomCompassDirection(new List<CompassDirection>() { CompassDirection.E, CompassDirection.S, CompassDirection.SE });
            if (Separation < 0) Separation = Board.Random.RandomInteger(0, 4);

            BoardLocation Two = Reference.Neighbor(Direction, Separation);
            if (Two.OnBoard(Board)) Locations.Add(Two);
          
        }
    }
}
