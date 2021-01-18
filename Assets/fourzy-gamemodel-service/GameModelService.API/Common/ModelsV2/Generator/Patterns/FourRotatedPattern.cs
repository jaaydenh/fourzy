using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class FourRotatedPattern: IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        //create a patter

        public FourRotatedPattern(GameBoard Board, int Offset =2, int DistanceFromEdge=1)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();

            BoardLocation Top = new BoardLocation(DistanceFromEdge, DistanceFromEdge + Offset);
            BoardLocation Right = new BoardLocation(DistanceFromEdge + Offset, Board.Columns - DistanceFromEdge -1);
            BoardLocation Bottom = new BoardLocation(Board.Rows - DistanceFromEdge -1, Board.Columns - DistanceFromEdge - Offset -1);
            BoardLocation Left = new BoardLocation(Board.Rows - DistanceFromEdge - 1 - Offset, DistanceFromEdge);
            Locations.AddRange(new List<BoardLocation>() { Top, Right, Left, Bottom });

        }
    }
}
