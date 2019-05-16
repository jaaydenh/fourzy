using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class DotPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        //create a patter

        public DotPattern(GameBoard Board, BoardLocation Reference, int Width, int Height)
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
                    if ((l.OddRow && OddRow && l.OddColumn && OddColumn)
                    || (l.OddRow && OddRow && l.EvenColumn && !OddColumn)
                    || (l.EvenRow && !OddRow && l.OddColumn && OddColumn)
                    || (l.EvenRow && !OddRow && l.EvenColumn && !OddColumn))
                        Locations.Add(l);
                }
            }


        }
    }
}
