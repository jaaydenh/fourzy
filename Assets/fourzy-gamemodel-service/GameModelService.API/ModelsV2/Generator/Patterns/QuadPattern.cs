using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class QuadPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }
        public List<BoardLocation> Locations { get; }

        public QuadPattern(GameBoard Board, int Count=1, int Pattern=-1)
        {
            List<string> Quads = new List<string>() { "NE", "NW", "SE", "SW" };
            Locations = new List<BoardLocation>();
            for (int i=0; i< Count; i++)
            {
                string q = Board.Random.RandomItem(Quads);
                Quads.Remove(q);
                switch (q)
                {
                    case "NW":
                        for (int r=0; r<Board.Rows/2; r++)
                        {
                            for (int c = 0; c < Board.Columns/ 2;c++)
                            {
                                Locations.Add(new BoardLocation(r, c));
                            }
                        }
                        break;
                    case "NE":
                        for (int r = 0; r < Board.Rows / 2; r++)
                        {
                            for (int c = Board.Columns / 2; c < Board.Columns ; c++)
                            {
                                Locations.Add(new BoardLocation(r, c));
                            }
                        }

                        break;
                    case "SW":
                        for (int r = Board.Rows/2; r < Board.Rows ; r++)
                        {
                            for (int c = 0; c < Board.Columns / 2; c++)
                            {
                                Locations.Add(new BoardLocation(r, c));
                            }
                        }

                        break;
                    case "SE":
                        for (int r = Board.Rows / 2; r < Board.Rows; r++)
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
}
