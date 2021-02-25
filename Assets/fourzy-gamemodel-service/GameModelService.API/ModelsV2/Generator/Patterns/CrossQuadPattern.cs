using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class CrossQuadPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }
        public List<BoardLocation> Locations { get; }

        public CrossQuadPattern(GameBoard Board)
        {
            List<string> Quads = new List<string>() { "NE-SW", "NW-SE"};
            Locations = new List<BoardLocation>();
                string q = Board.Random.RandomItem(Quads);
                switch (q)
                {
                    case "NW-SE":
                        for (int r = 0; r < Board.Rows / 2; r++)
                        {
                            for (int c = 0; c < Board.Columns / 2; c++)
                            {
                                Locations.Add(new BoardLocation(r, c));
                            }
                        }
                        for (int r = Board.Rows / 2; r < Board.Rows; r++)
                        {
                            for (int c = Board.Columns / 2; c < Board.Columns; c++)
                             {
                                Locations.Add(new BoardLocation(r, c));
                            }
                        }
                         break;

                    case "NE-SW":
                        for (int r = 0; r < Board.Rows / 2; r++)
                        {
                            for (int c = Board.Columns / 2; c < Board.Columns; c++)
                            {
                                Locations.Add(new BoardLocation(r, c));
                            }
                        }
                        for (int r = Board.Rows / 2; r < Board.Rows; r++)
                        {
                            for (int c = 0; c < Board.Columns / 2; c++)
                             {
                                 Locations.Add(new BoardLocation(r, c));
                            }
                        }
                        break;
                }
        }
    }
}
