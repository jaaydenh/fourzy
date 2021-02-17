using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class CornerPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }
        public List<BoardLocation> Locations { get; }

        public CornerPattern(GameBoard Board, BoardLocation Reference, int Width, int Height)
        {
            Locations = new List<BoardLocation>();
            Locations.Add(new BoardLocation(Reference.Row, Reference.Column));
            Locations.Add(new BoardLocation(Reference.Row + Height -1, Reference.Column));
            Locations.Add(new BoardLocation(Reference.Row, Reference.Column + Width -1));
            Locations.Add(new BoardLocation(Reference.Row + Height -1, Reference.Column + Width -1));
        }
    }
}
