using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class RandomEdgeLocation: IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }


        public RandomEdgeLocation(GameBoard Board, Direction Side, int Count = 1)
        {
            this.Locations = new List<BoardLocation>();

            List<BoardLocation> possible = new List<BoardLocation>();
            switch (Side)
            {
                case Direction.UP:
                    for (int i = 1; i < Board.Columns - 1; i++) {
                        BoardSpace target = Board.ContentsAt(new BoardLocation(0, i));
                        if (target.Empty || target.ContainsOnlyTerrain) possible.Add(new BoardLocation(0, i));
                    }
                    break;

                case Direction.DOWN:
                    for (int i = 1; i < Board.Columns - 1; i++)
                    {
                        BoardSpace target = Board.ContentsAt(new BoardLocation(0, Board.Rows - 1));
                        if (target.Empty || target.ContainsOnlyTerrain) possible.Add(new BoardLocation(0, Board.Rows - 1));
                    }

                    break;

                case Direction.LEFT:
                    for (int i = 1; i < Board.Rows - 1; i++)
                    {
                        BoardSpace target = Board.ContentsAt(new BoardLocation(i, 0));
                        if (target.Empty || target.ContainsOnlyTerrain) possible.Add(new BoardLocation(i, 0));
                    }

                    break;

                case Direction.RIGHT:
                    for (int i = 1; i < Board.Rows - 1; i++)
                    {
                        BoardSpace target = Board.ContentsAt(new BoardLocation(i, Board.Columns - 1));
                        if (target.Empty || target.ContainsOnlyTerrain) possible.Add(new BoardLocation(i, Board.Columns - 1));
                    }

                    break;
            }
            if (possible.Count > 0)
            for (int i=0; i< Count; i++)
            {
                BoardLocation l = Board.Random.RandomLocation(possible);
                Locations.Add(l);
                possible.Remove(l);
            }
        }

    }
}
