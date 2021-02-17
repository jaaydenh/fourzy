using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class BlockRandomEdgePattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        public BlockRandomEdgePattern(GameBoard Board, int TotalBlocked = 4)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();
            List<int> Blocked = new List<int>();

            int Total = 2 * Board.Columns + 2 * Board.Columns;
            while (Blocked.Count < TotalBlocked)
            {
                int New = Board.Random.RandomInteger(0, Total);
                if (!Blocked.Contains(New)) Blocked.Add(New);
            }

            int count = 0;
            for (int c = 1; c < Board.Columns-1; c++) if (Blocked.Contains(count++)) Locations.Add(new BoardLocation(0, c));
            for (int c = 1; c < Board.Columns-1; c++) if (Blocked.Contains(count++)) Locations.Add(new BoardLocation(Board.Rows - 1, Board.Columns - c - 1));
            for (int r = 1; r < Board.Rows-1; r++) if (Blocked.Contains(count++)) Locations.Add(new BoardLocation(r, 0));
            for (int r = 1; r < Board.Rows-1; r++) if (Blocked.Contains(count++)) Locations.Add(new BoardLocation(Board.Rows - r - 1, Board.Columns - 1));

        }
    }
}
