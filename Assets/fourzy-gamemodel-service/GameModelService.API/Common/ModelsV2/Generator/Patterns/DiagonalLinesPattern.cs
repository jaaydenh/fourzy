using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class DiagonalLinesPattern : IBoardPattern
    {
        public List<BoardLocation> Locations { get; }

        public DiagonalLinesPattern(GameBoard Board)
        {
            BoardLocation Reference = Board.Random.RandomLocation(new BoardLocation(0,0),2,2);
            int Height = Board.Rows - 1 - Reference.Row;
            int Width = Board.Columns - 1 - Reference.Column;

            Locations = new List<BoardLocation>();

            for (int r = 0; r < Height; r++)
            {
                for (int c = 0; c < Width; c++)
                {
                    if ((r + c) % 3 == 1)
                        Locations.Add(new BoardLocation(Reference.Row + r, Reference.Column + c));
                }
            }

        }

        public DiagonalLinesPattern(GameBoard Board, BoardLocation Reference, int Height, int Width)
        {
            Locations = new List<BoardLocation>();

            for (int r = 0; r < Height; r++)
            {
                for (int c = 0; c < Width; c++)
                {
                    if ((r + c) % 3 == 1)
                        Locations.Add(new BoardLocation(Reference.Row + r, Reference.Column + c));
                }
            }

        }
    }
}
