using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class HalfPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }
        public List<BoardLocation> Locations { get; }

        public HalfPattern(GameBoard Board, Direction Side = Direction.NONE)
        {
                if (Side == Direction.NONE) Side = Board.Random.RandomDirection();
            Locations = new List<BoardLocation>();

            switch (Side)
                {
                    case Direction.UP:
                        for (int r = 0; r < Board.Rows / 2; r++)
                        {
                            for (int c = 0; c < Board.Columns; c++)
                            {
                                Locations.Add(new BoardLocation(r, c));
                            }
                        }
                        break;
                    case Direction.DOWN:
                    for (int r = Board.Rows/2; r < Board.Rows; r++)
                        {
                            for (int c = 0; c < Board.Columns; c++)
                            {
                                Locations.Add(new BoardLocation(r, c));
                            }
                        }

                        break;
                    case Direction.LEFT:
                    for (int r = 0; r < Board.Rows; r++)
                        {
                            for (int c = 0; c < Board.Columns / 2; c++)
                            {
                                Locations.Add(new BoardLocation(r, c));
                            }
                        }

                        break;
                    case Direction.RIGHT:
                    for (int r = 0; r < Board.Rows; r++)
                        {
                            for (int c = Board.Columns / 2; c < Board.Columns; c++)
                            {
                                Locations.Add(new BoardLocation(r, c));
                            }
                        }

                        break;
                }
            }
        }
    }

