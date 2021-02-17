using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class CheckerPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        public CheckerPattern(GameBoard Board, BoardLocation Reference, int Width, int Height, bool TopLeftOn = true)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();

            for (int r = 0; r < Height; r++)
            {
                for (int c = 0; c < Width; c++)
                {
                    if ((r + c) % 2 == 0)
                    {
                        if (((r + c) % 2 == 0) && TopLeftOn) Locations.Add(new BoardLocation(Reference.Row + r, Reference.Column + c));
                        else if (((r + c + 1) % 2 == 0) && TopLeftOn) Locations.Add(new BoardLocation(Reference.Row + r, Reference.Column + c));
                    }
                }
            }
        }
    }
}
