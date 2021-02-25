using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class CenterOnePattern : IBoardPattern
    {
        public BoardLocation Reference { get; }
        public List<BoardLocation> Locations { get; }

        public CenterOnePattern(GameBoard Board)
        {
            Reference = new BoardLocation(Board.Rows / 2 - 1, Board.Columns / 2 - 1);
            Locations = new List<BoardLocation>();

            List<BoardLocation> PossibleLocations = new List<BoardLocation>();

            int Height = 2;
            int Width = 2;
            //code for odd sized boards.
            if (Board.Rows % 2 == 1) Height = 1;
            if (Board.Columns % 2 == 1) Width = 1;

            PossibleLocations.Add(new BoardLocation(Reference.Row, Reference.Column));
            if (Height > 0)
                PossibleLocations.Add(new BoardLocation(Reference.Row + 1, Reference.Column));
            if (Width > 0)
                PossibleLocations.Add(new BoardLocation(Reference.Row, Reference.Column + 1));
            if (Height > 0 & Width > 0)
                PossibleLocations.Add(new BoardLocation(Reference.Row + 1, Reference.Column + 1));

            //Add One Location
            Locations.Add(Board.Random.RandomLocation(PossibleLocations));
        }
    }
}
