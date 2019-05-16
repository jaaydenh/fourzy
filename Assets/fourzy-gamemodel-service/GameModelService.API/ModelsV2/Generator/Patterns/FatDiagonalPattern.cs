using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class FatDiagonalPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }
        public List<BoardLocation> Locations { get; }

        public FatDiagonalPattern(GameBoard Board, DiagonalType Type = DiagonalType.NONE)
        {
            if (Type == DiagonalType.NONE) Type = (DiagonalType) Board.Random.RandomInteger(0,1);

            Locations = new List<BoardLocation>();

            switch (Type)
            {
                case DiagonalType.HIGHLEFT:
                    foreach (BoardLocation l in new BoardLocation(0, 0).GetDiagonal(Board, Type))
                        Locations.Add(l);
                    foreach (BoardLocation l in new BoardLocation(0, 1).GetDiagonal(Board, Type))
                        Locations.Add(l);
                    foreach (BoardLocation l in new BoardLocation(1, 0).GetDiagonal(Board, Type))
                        Locations.Add(l);
                    break;
                case DiagonalType.HIGHRIGHT:
                    foreach (BoardLocation l in new BoardLocation(0, Board.Columns-1).GetDiagonal(Board, Type))
                        Locations.Add(l);
                    foreach (BoardLocation l in new BoardLocation(0, Board.Columns - 2).GetDiagonal(Board, Type))
                        Locations.Add(l);
                    foreach (BoardLocation l in new BoardLocation(1, Board.Columns - 1).GetDiagonal(Board, Type))
                        Locations.Add(l);
                    break;
            }
        }
    }
}

