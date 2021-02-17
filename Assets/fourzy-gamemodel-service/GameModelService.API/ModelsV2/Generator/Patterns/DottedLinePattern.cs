using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class DottedLinePattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        public DottedLinePattern(GameBoard Board, BoardLocation Reference, LineType Line, bool ReferencePointOn = true)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();

            //Determine On Off based on reference point.
            bool OddColumn = (ReferencePointOn && (Reference.OddColumn));
            bool OddRow= (ReferencePointOn && (Reference.OddRow));

            List<BoardLocation>LineLocations = Reference.GetLine(Board, Line);
            LineLocations.Add(Reference);
            foreach (BoardLocation l in LineLocations)
            {
                switch (Line)
                {
                    case LineType.VERTICAL:
                        if ((OddRow && l.OddRow) || (!OddRow && l.EvenRow)) Locations.Add(l);
                        break;

                    case LineType.HORIZONTAL:
                        if ((OddColumn && l.OddColumn)|| (!OddColumn && l.EvenColumn)) Locations.Add(l);
                        break;
                }
            }

        }
    }
}
