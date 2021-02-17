using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class WideCrossPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        public WideCrossPattern(GameBoard Board, int Width = 4)
        {
            Locations = new List<BoardLocation>();

            Reference = Board.Random.RandomLocation(new BoardLocation(1,1), Board.Rows/2, Board.Columns/2);
            for (int r = 0; r < Board.Rows; r++)
            {
                for (int c = 0; c < Board.Columns; c++)
                {
                    if ((r>=Reference.Row && r<Reference.Row + Width) || (c>=Reference.Column && c<Reference.Column+Width))
                    {
                        Locations.Add(new BoardLocation(r,c));
                    }
                }
            }
        }

    }
}
