using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class FourSlantedDiamondPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        //create a patter

        public FourSlantedDiamondPattern(GameBoard Board, BoardLocation Reference, int Separation)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();
            Locations.Add(Reference);

            BoardLocation BottomLeft = new BoardLocation(Reference.Row + Separation + 1, Reference.Column + 1);
            if (BottomLeft.OnBoard(Board)) Locations.Add(BottomLeft);

            BoardLocation TopRight = new BoardLocation(Reference.Row, Reference.Column + Separation + 1);
            if (TopRight.OnBoard(Board)) Locations.Add(TopRight);

            BoardLocation BottomRight = BottomLeft.Neighbor(Direction.RIGHT, Separation +1);
            if (BottomRight.OnBoard(Board)) Locations.Add(BottomRight);

        }
    }
}
