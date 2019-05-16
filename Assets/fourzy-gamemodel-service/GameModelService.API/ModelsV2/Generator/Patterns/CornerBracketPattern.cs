using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class CornerBracketPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        public CornerBracketPattern(GameBoard Board, BoardLocation Reference, int Width, int Height)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();

            Locations.Add(new BoardLocation(Reference));
            Locations.Add(new BoardLocation(Reference.Row, Reference.Column +1));
            Locations.Add(new BoardLocation(Reference.Row +1, Reference.Column));

            Locations.Add(new BoardLocation(Reference.Row, Reference.Column + Width -1));
            Locations.Add(new BoardLocation(Reference.Row, Reference.Column + Width));
            Locations.Add(new BoardLocation(Reference.Row +1, Reference.Column + Width));

            Locations.Add(new BoardLocation(Reference.Row + Height, Reference.Column));
            Locations.Add(new BoardLocation(Reference.Row + Height, Reference.Column + 1));
            Locations.Add(new BoardLocation(Reference.Row + Height -1, Reference.Column -1));

            Locations.Add(new BoardLocation(Reference.Row + Height, Reference.Column + Width));
            Locations.Add(new BoardLocation(Reference.Row + Height, Reference.Column - 1));
            Locations.Add(new BoardLocation(Reference.Row + Height -1, Reference.Column));

        }
    }
}
