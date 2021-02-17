using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class CrossBoardFourTurnPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        Direction Start { get; set; }
        Direction End { get; set; }
        int LocationStart { get; set; }
        int LocationEnd { get; set; }

        public CrossBoardFourTurnPattern(GameBoard Board, Direction Start = Direction.NONE, int LocationStart = -1, int LocationTurn1 = -1, int LocationBend=-1, int LocationTurn2=-1, int LocationEnd = -1)
        {
            this.Locations = new List<BoardLocation>();

            if (Start == Direction.NONE)
                Start = Board.Random.RandomDirection(new List<Direction>() { Direction.LEFT, Direction.UP });

            if (LocationStart < 1) LocationStart = Board.Random.RandomInteger(1, Board.Rows - 2);

            if (LocationBend< 1)
            {
                LocationBend = Board.Random.RandomInteger(1, Board.Columns - 2);
                while (LocationStart == LocationBend)
                    LocationBend = Board.Random.RandomInteger(1, Board.Columns - 2);
            }

            if (LocationEnd < 1)
            {
                LocationEnd = Board.Random.RandomInteger(1, Board.Columns - 2);
                while (LocationEnd == LocationBend)
                    LocationEnd = Board.Random.RandomInteger(1, Board.Columns - 2);
            }

            if (LocationTurn1 < 1) LocationTurn1 = Board.Random.RandomInteger(3, Board.Rows - 4);
            if (LocationTurn2 < 1) LocationTurn2 = Board.Random.RandomInteger(LocationTurn1+1, Board.Rows - 2);

            switch (Start)
            {
                case Direction.LEFT:
                    for (int c = 0; c < LocationTurn1; c++)
                    {
                        Locations.Add(new BoardLocation(LocationStart, c));
                    }
                    for (int r = Math.Min(LocationStart, LocationBend); r <= Math.Max(LocationStart, LocationBend); r++)
                    {
                        if (!Locations.Contains(new BoardLocation(r, LocationTurn1)))
                            Locations.Add(new BoardLocation(r, LocationTurn1));
                    }
                    for (int c = LocationTurn1; c < LocationTurn2; c++)
                    {
                        Locations.Add(new BoardLocation(LocationBend, c));
                    }
                    for (int r = Math.Min(LocationBend, LocationEnd); r <= Math.Max(LocationBend, LocationEnd); r++)
                    {
                        if (!Locations.Contains(new BoardLocation(r, LocationTurn2)))
                            Locations.Add(new BoardLocation(r, LocationTurn2));
                    }
                    for (int c = LocationTurn2; c < Board.Columns; c++)
                    {
                        Locations.Add(new BoardLocation(LocationEnd, c));
                    }

                    break;

                case Direction.UP:
                    for (int r = 0; r < LocationTurn1; r++)
                    {
                        Locations.Add(new BoardLocation(r, LocationStart));
                    }

                    for (int c = Math.Min(LocationStart, LocationBend); c <= Math.Max(LocationStart, LocationBend); c++)
                    {
                        if (!Locations.Contains(new BoardLocation(LocationTurn1, c)))
                            Locations.Add(new BoardLocation(LocationTurn1, c));
                    }

                    for (int r = LocationTurn1; r < LocationTurn2; r++)
                    {
                        Locations.Add(new BoardLocation(r, LocationBend));
                    }

                    for (int c = Math.Min(LocationBend, LocationEnd); c <= Math.Max(LocationBend, LocationEnd); c++)
                    {
                        if (!Locations.Contains(new BoardLocation(LocationTurn2, c)))
                            Locations.Add(new BoardLocation(LocationTurn2, c));
                    }

                    for (int r = LocationTurn2; r < Board.Rows; r++)
                    {
                        Locations.Add(new BoardLocation(r, LocationEnd));
                    }

                    break;
            }

        }

    }
}
