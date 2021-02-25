using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class BubbleEdgePattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        //create a patter

        public BubbleEdgePattern(GameBoard Board, Direction Direction)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();

            switch (Direction)
            {
                case Direction.UP:
                    for (int c = 1; c < Board.Columns-1; c++) Locations.Add(new BoardLocation(0, c));
                    for (int c = 2; c < Board.Columns-2; c++) Locations.Add(new BoardLocation(1, c));
                    for (int c = 3; c < Board.Columns-3; c++) Locations.Add(new BoardLocation(2, c));
                    break;
                case Direction.DOWN:
                    for (int c = 1; c < Board.Columns-1; c++) Locations.Add(new BoardLocation(Board.Rows - 1, c));
                    for (int c = 2; c < Board.Columns-2; c++) Locations.Add(new BoardLocation(Board.Rows - 2, c));
                    for (int c = 3; c < Board.Columns-3; c++) Locations.Add(new BoardLocation(Board.Rows - 3, c));
                    break;
                case Direction.LEFT:
                    for (int r = 1; r < Board.Rows-1; r++) Locations.Add(new BoardLocation(r, 0));
                    for (int r = 2; r < Board.Rows-2; r++) Locations.Add(new BoardLocation(r, 1));
                    for (int r = 3; r < Board.Rows-3; r++) Locations.Add(new BoardLocation(r, 2));
                    break;
                case Direction.RIGHT:
                    for (int r = 1; r < Board.Rows-1; r++) Locations.Add(new BoardLocation(r, Board.Columns - 1));
                    for (int r = 2; r < Board.Rows-2; r++) Locations.Add(new BoardLocation(r, Board.Columns - 2));
                    for (int r = 3; r < Board.Rows-3; r++) Locations.Add(new BoardLocation(r, Board.Columns - 3));
                    break;
            }

        }
    }
}
