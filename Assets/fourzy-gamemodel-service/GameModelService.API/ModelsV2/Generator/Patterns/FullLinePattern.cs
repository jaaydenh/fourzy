using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class SolidFullLinePattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        public SolidFullLinePattern(GameBoard Board, BoardLocation Reference, LineType Line)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();
            foreach (BoardLocation l in Reference.GetLine(Board, Line)) Locations.Add(l);
            Locations.Add(Reference);
        }
    }
}
