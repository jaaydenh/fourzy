using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class SolidFullCrossPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }
        
        public SolidFullCrossPattern(GameBoard Board, BoardLocation Reference)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();

            Locations.Add(Reference);
            foreach (BoardLocation l in Reference.GetColumn(Board))
            {
                Locations.Add(l);
            }

            foreach (BoardLocation l in Reference.GetRow(Board))
            {
                Locations.Add(l);
            }

        }
    }
}
