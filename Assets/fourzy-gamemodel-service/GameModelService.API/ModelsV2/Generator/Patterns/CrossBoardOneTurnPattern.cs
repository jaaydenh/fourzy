using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class CrossBoardOneTurnPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        Direction Start { get; set; }
        Direction End { get; set; }
        int LocationStart { get; set; }
        int LocationEnd { get; set; }

        public CrossBoardOneTurnPattern(GameBoard Board, Direction Start = Direction.NONE, int LocationStart = -1, Direction End = Direction.NONE, int LocationEnd = -1)
        {
            this.Locations = new List<BoardLocation>();

            if (Start == Direction.NONE)
                Start = Board.Random.RandomDirection(new List<Direction>() { Direction.LEFT, Direction.RIGHT });
            if (End == Direction.NONE)
                End = Board.Random.RandomDirection(new List<Direction>() { Direction.UP, Direction.DOWN });

            if (LocationStart < 1) LocationStart = Board.Random.RandomInteger(2, Board.Rows - 3);
            if (LocationEnd < 1) LocationEnd = Board.Random.RandomInteger(2, Board.Columns - 3);

            switch (Start)
            {
                case Direction.LEFT:
                    for (int c = 0; c <= LocationEnd; c++)
                    {
                        Locations.Add(new BoardLocation(LocationStart, c));
                    }
                    break;

                case Direction.RIGHT:
                    for (int c = LocationEnd; c < Board.Columns; c++)
                    {
                        Locations.Add(new BoardLocation(LocationStart, c));
                    }
                    break;
            }


            switch (End)
            {
                case Direction.UP:
                    for (int r = 0; r <= LocationStart; r++)
                    {
                        Locations.Add(new BoardLocation(r, LocationEnd));
                    }

                    break;
                case Direction.DOWN:
                    for (int r = LocationStart; r < Board.Rows; r++)
                    {
                        Locations.Add(new BoardLocation(r, LocationEnd));
                    }
                    break;
            }

        }

    }
}
