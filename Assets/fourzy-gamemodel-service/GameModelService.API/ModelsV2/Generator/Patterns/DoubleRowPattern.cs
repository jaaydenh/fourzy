using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class DoubleColumnPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        public DoubleColumnPattern(GameBoard Board, BoardLocation Reference, LineType Line, int Spacing=2)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();

            int Width = 4;
            int Height = 4;
            Locations = new List<BoardLocation>();

            //Build four corners in center

            BoardLocation UL = new BoardLocation(Reference.Row, Reference.Column);
            BoardLocation LL = new BoardLocation(Reference.Row + Height -1, Reference.Column);
            BoardLocation UR = new BoardLocation(Reference.Row, Reference.Column + Width -1);
            BoardLocation LR = new BoardLocation(Reference.Row + Height -1, Reference.Column + Width -1);
            Locations.Add(UL);
            Locations.Add(LL);
            Locations.Add(UR);
            Locations.Add(LR);


            //Build Add rest of the columns outward.  Just one for now, but we can add more for larger boards.

            if (Line == LineType.VERTICAL || Line == LineType.NONE)
            {

                Locations.Add(UR.Neighbor(Direction.UP, Spacing));
                Locations.Add(UL.Neighbor(Direction.UP, Spacing));
                Locations.Add(LL.Neighbor(Direction.DOWN, Spacing));
                Locations.Add(LR.Neighbor(Direction.DOWN, Spacing));

            }

            if (Line == LineType.HORIZONTAL || Line == LineType.NONE)
            {
                Locations.Add(UR.Neighbor(Direction.RIGHT, Spacing));
                Locations.Add(UL.Neighbor(Direction.LEFT, Spacing));
                Locations.Add(LL.Neighbor(Direction.LEFT, Spacing));
                Locations.Add(LR.Neighbor(Direction.RIGHT, Spacing));
            }

        }
    }
}
