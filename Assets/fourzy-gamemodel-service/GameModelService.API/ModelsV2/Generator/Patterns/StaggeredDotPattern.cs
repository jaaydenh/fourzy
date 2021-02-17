using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class StaggeredDotPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }


        public StaggeredDotPattern(GameBoard Board, BoardLocation Reference, int Width, int Height)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();

            bool OddRow = (Reference.Row % 2 == 1);
            bool OddColumn = (Reference.Column % 2 == 1);

            for (int r = 0; r < Height; r++)
            {
                for (int c = 0; c < Width; c++)
                {
                    BoardLocation l = new BoardLocation(Reference.Row + r, Reference.Column + c);
                    if (c % 2 == 0)
                        if (c % 4 == 0)
                        {
                            if (r % 2 == 0) Locations.Add(l);
                        }
                        else
                        {
                            if ((r + 1) % 2 == 0) Locations.Add(l);
                        }
                }
            }
        }
    }
}
