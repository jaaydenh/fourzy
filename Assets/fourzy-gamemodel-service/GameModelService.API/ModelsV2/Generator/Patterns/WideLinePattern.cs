using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class WideLinePattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        public WideLinePattern(GameBoard Board, LineType Line = LineType.NONE, int Width = 4)
        {
            Locations = new List<BoardLocation>();

            int InsertLocation = Board.Random.RandomInteger(1, Math.Max(Board.Rows, Board.Columns) / 2);
            if (Line == LineType.NONE) Line = (LineType)Board.Random.RandomInteger(0, 1);


            switch (Line)
            {
                case LineType.DIAGONAL:
                case LineType.HORIZONTAL:
                    for (int i = 0; i < Width; i++)
                        for (int c = 0; c < Board.Columns; c++)
                            Locations.Add(new BoardLocation(InsertLocation + i, c));
                            break;

                case LineType.VERTICAL:
                    for (int i = 0; i < Width; i++)
                        for (int r=0; r< Board.Rows; r++)
                        Locations.Add(new BoardLocation(r,InsertLocation + i));
                    break;
            }

        }
    }
}
