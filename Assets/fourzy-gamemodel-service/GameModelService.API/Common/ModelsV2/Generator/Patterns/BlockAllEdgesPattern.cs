using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class BlockAllEdgePattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        public BlockAllEdgePattern(GameBoard Board)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();

            for (int c = 0; c < Board.Columns; c++) Locations.Add(new BoardLocation(0, c));
            for (int c = 0; c < Board.Columns; c++) Locations.Add(new BoardLocation(Board.Rows - 1, c));
            for (int r = 0; r < Board.Rows; r++) Locations.Add(new BoardLocation(r, 0));
            for (int r = 0; r < Board.Rows; r++) Locations.Add(new BoardLocation(r, Board.Columns - 1));
        }
    }
}
