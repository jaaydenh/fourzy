using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class SurroundPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        public SurroundPattern(GameBoard Board, BoardLocation Reference)
        {
            this.Reference = Reference;
            Locations = Reference.GetAdjacent(Board);
        }
    }
}
