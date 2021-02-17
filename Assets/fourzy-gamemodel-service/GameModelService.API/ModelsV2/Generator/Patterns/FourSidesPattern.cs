using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class FourSidesPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        public FourSidesPattern(GameBoard Board, BoardLocation Reference)
        {
            this.Reference = Reference;
            Locations = Reference.GetOrthogonals(Board);
        }
    }
}
