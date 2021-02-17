using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class BlockEdgePattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        //create a patter

        public BlockEdgePattern(GameBoard Board, Direction Direction)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();

            switch(Direction)
            {
                case Direction.UP:
                    for (int c = 1; c < Board.Columns-1; c++) Locations.Add(new BoardLocation(0, c));
                    break;
                case Direction.DOWN:
                    for (int c = 1; c < Board.Columns-1; c++) Locations.Add(new BoardLocation(Board.Rows-1, c));
                    break;
                case Direction.LEFT:
                    for (int r = 1; r < Board.Rows-1; r++) Locations.Add(new BoardLocation(r, 0));
                    break;
                case Direction.RIGHT:
                    for (int r = 1; r < Board.Rows-1; r++) Locations.Add(new BoardLocation(r, Board.Columns -1));
                    break;
            }

        }
    }
}
