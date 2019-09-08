using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class CrossBoardTwoTurnPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        Direction Start { get; set; }
        Direction End { get; set; }
        int LocationStart { get; set; }
        int LocationEnd { get; set; }

        public CrossBoardTwoTurnPattern(GameBoard Board, Direction Start = Direction.NONE, int LocationStart = -1, int LocationTurn=-1, int LocationEnd = -1)
        {
            this.Locations = new List<BoardLocation>();

            if (Start == Direction.NONE)
                Start = Board.Random.RandomDirection(new List<Direction>() { Direction.LEFT, Direction.UP});

            if (LocationStart < 1) LocationStart = Board.Random.RandomInteger(1, Board.Rows - 2);
            if (LocationEnd < 1)
            {
                LocationEnd = Board.Random.RandomInteger(1, Board.Columns - 2);
                while (LocationStart == LocationEnd)
                    LocationEnd = Board.Random.RandomInteger(1, Board.Columns - 2);
            }


            if (LocationTurn < 1) LocationTurn = Board.Random.RandomInteger(3, Board.Columns - 3);

            switch (Start)
            {
                case Direction.LEFT:
                    for (int c = 0; c < LocationTurn; c++)
                    {
                        Locations.Add(new BoardLocation(LocationStart, c));
                    }
                    for (int r = Math.Min(LocationStart, LocationEnd); r <= Math.Max(LocationStart, LocationEnd); r++)
                    {
                        if (!Locations.Contains(new BoardLocation(r, LocationTurn)))
                            Locations.Add(new BoardLocation(r, LocationTurn));
                    }
                    for (int c = LocationTurn; c < Board.Columns; c++)
                    {
                        Locations.Add(new BoardLocation(LocationEnd, c));
                    }

                    break;

                case Direction.UP:
                    for (int r = 0; r < LocationTurn; r++)
                    {
                        Locations.Add(new BoardLocation(r, LocationStart));
                    }
                    for (int c = Math.Min(LocationStart, LocationEnd); c <= Math.Max(LocationStart,LocationEnd); c++)
                    {
                        if (!Locations.Contains(new BoardLocation(LocationTurn,c)))
                            Locations.Add(new BoardLocation(LocationTurn, c));
                    }
                    for (int r = LocationTurn; r < Board.Rows; r++)
                    {
                        Locations.Add(new BoardLocation(r, LocationEnd));
                    }

                    break;
            }

        }

    }
}
