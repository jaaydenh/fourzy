using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class SymmetricEdgeBlockPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        public SymmetricEdgeBlockPattern(GameBoard Board, int BlockedPerSide=1, string BlockPattern="")
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();
            if (BlockPattern.Length == 0)
            {
                int Count = 0;
                int Blocked = Board.Random.RandomInteger(0, Math.Min(Board.Rows, Board.Columns) - 1);
                for (int i=0; i<Math.Min(Board.Rows, Board.Columns); i++)
                {
                    if (i == Blocked && Count < BlockedPerSide)
                    {
                        BlockPattern += "1";
                        Count++;
                        Blocked = Board.Random.RandomInteger(i + 1, Math.Min(Board.Rows, Board.Columns));
                        continue;
                    }
                    BlockPattern += "0";
                }
            }

            for (int c = 0; c < Board.Columns; c++) if ( BlockPattern[c]=='1') Locations.Add(new BoardLocation(0, c));
            for (int c = 0; c < Board.Columns; c++) if (BlockPattern[c] == '1') Locations.Add(new BoardLocation(Board.Rows - 1, Board.Columns - c -1));
            for (int r = 0; r < Board.Rows; r++) if (BlockPattern[Board.Rows - r - 1] == '1') Locations.Add(new BoardLocation(r, 0));
            for (int r = 0; r < Board.Rows; r++) if (BlockPattern[Board.Rows - r - 1] == '1') Locations.Add(new BoardLocation(Board.Rows - r -1, Board.Columns - 1));

        }
    }
}
