using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class FullLinesPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }
        public List<BoardLocation> Locations { get; }

        public FullLinesPattern(GameBoard Board, LineType Type = LineType.NONE, int Offset=-1)
        {
            if (Type == LineType.NONE) Type = (LineType)Board.Random.RandomInteger(0, 1);
            if (Offset < 0) Offset = Board.Random.RandomInteger(0, 1);

            Locations = new List<BoardLocation>();

            for (int r = 0; r < Board.Rows ; r++)
            {
                for (int c = 0; c < Board.Columns; c++)
                {
                    switch (Type)
                    {
                        case LineType.HORIZONTAL:
                            if ((r+Offset)%2==0) Locations.Add(new BoardLocation(r, c));
                            break;
                        case LineType.VERTICAL:
                            if ((c+Offset)% 2  == 0) Locations.Add(new BoardLocation(r, c));
                            break;
                    }
                    
                }
            }
            
        }
    }
}

