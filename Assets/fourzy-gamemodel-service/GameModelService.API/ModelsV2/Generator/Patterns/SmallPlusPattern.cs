using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class SmallPlusPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        public SmallPlusPattern(GameBoard Board, BoardLocation Reference, bool CenterOn = true)
        {
            this.Reference = Reference;
            Locations = Reference.GetOrthogonals(Board);

            if (CenterOn) Locations.Add(Reference);
        }
    }
}
