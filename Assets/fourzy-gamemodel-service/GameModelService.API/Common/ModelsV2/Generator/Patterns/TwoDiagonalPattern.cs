using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class TwoDiagonalPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        //create a patter

        public TwoDiagonalPattern(GameBoard Board, BoardLocation Reference, CompassDirection Direction, int Separation)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();
            Locations.Add(Reference);
            BoardLocation Two = new BoardLocation(Reference);

            switch (Direction)
            {
                case CompassDirection.NE:
                    Two = new BoardLocation(Reference.Row - Separation,Reference.Column + Separation);
                    break;
                case CompassDirection.NW:
                    Two = new BoardLocation(Reference.Row - Separation, Reference.Column - Separation);
                    break;
                case CompassDirection.SE:
                    Two = new BoardLocation(Reference.Row + Separation, Reference.Column + Separation);
                    break;
                case CompassDirection.SW:
                    Two = new BoardLocation(Reference.Row + Separation, Reference.Column - Separation);
                    break;
            }
            if (Two.OnBoard(Board)) Locations.Add(Two);

        }
    }
}
