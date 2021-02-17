using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class FourPlusPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        //create a patter

        public FourPlusPattern(GameBoard Board, BoardLocation Reference, int Separation)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();
            Locations.Add(Reference);

            BoardLocation Bottom = Reference.Neighbor(Direction.DOWN, Separation * 2 + 2);
            if (Bottom.OnBoard(Board)) Locations.Add(Bottom);

            BoardLocation Right = new BoardLocation(Reference.Row + Separation + 1, Reference.Column + Separation + 1);
            if (Right.OnBoard(Board)) Locations.Add(Right);

            BoardLocation Left = Right.Neighbor(Direction.LEFT, Separation *2 +2 );
            if (Left.OnBoard(Board)) Locations.Add(Left);

        }
    }
}
