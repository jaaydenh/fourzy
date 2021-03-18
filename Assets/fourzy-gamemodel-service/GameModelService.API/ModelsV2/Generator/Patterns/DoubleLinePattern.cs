using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class DoubleLinePattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        public DoubleLinePattern(GameBoard Board, BoardLocation Reference, LineType Line)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();

            foreach (BoardLocation l in Reference.GetLine(Board, Line)) Locations.Add(l);

            switch (Line)
            {
                case LineType.HORIZONTAL:
                case LineType.DIAGONAL:
                    foreach (BoardLocation l in Reference.Neighbor(Direction.DOWN).GetLine(Board, Line)) Locations.Add(l);
                    break;

                case LineType.VERTICAL:
                    foreach (BoardLocation l in Reference.Neighbor(Direction.RIGHT).GetLine(Board, Line)) Locations.Add(l);
                    break;
            }

        }
    }
}
