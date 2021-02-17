using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class DottedCrossPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        public DottedCrossPattern(GameBoard Board, BoardLocation Reference, bool Center = true)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();

            bool OddRow = (Reference.Row % 2 == 1) && Center;
            bool OddColumn = (Reference.Column % 2 == 1) && Center;

            if (Center) Locations.Add(Reference);

            foreach (BoardLocation l in Reference.GetColumn(Board))
            {
                if ((l.OddRow && OddRow) || (l.EvenRow && !OddRow))
                    Locations.Add(l);
            }

            foreach (BoardLocation l in Reference.GetRow(Board))
            {
                if ((l.OddColumn && OddColumn) || (l.EvenColumn && !OddColumn))
                    Locations.Add(l);
            }

        }
    }
}
