using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class LargeCheckerPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        public LargeCheckerPattern(GameBoard Board, int RowOffset =-1, int ColumnOffset=-1)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();
            if (RowOffset < 0) RowOffset = Board.Random.RandomInteger(0, 3);
            if (ColumnOffset < 0) ColumnOffset = Board.Random.RandomInteger(0, 3);


            for (int r = 0; r < Board.Rows; r++)
            {
                for (int c = 0; c < Board.Columns; c++)
                {
                    if ((((c+ColumnOffset)%4)<2 && ((r+RowOffset)%4)<2) 
                        || (((c+ColumnOffset) % 4) > 1 && ((r+RowOffset) % 4) > 1))
                        Locations.Add(new BoardLocation(Reference.Row + r, Reference.Column + c));
                }
            }
        }
    }
}
