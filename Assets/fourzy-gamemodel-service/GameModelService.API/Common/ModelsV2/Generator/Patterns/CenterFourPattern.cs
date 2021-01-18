using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class CenterFourPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }
        public List<BoardLocation> Locations { get; }

        public CenterFourPattern(GameBoard Board)
        {
            Reference = new BoardLocation(Board.Rows / 2-1, Board.Columns / 2-1);
            Locations = new List<BoardLocation>();
            Locations.Add(new BoardLocation(Reference.Row, Reference.Column));
            Locations.Add(new BoardLocation(Reference.Row + 1, Reference.Column));
            Locations.Add(new BoardLocation(Reference.Row, Reference.Column + 1));
            Locations.Add(new BoardLocation(Reference.Row + 1, Reference.Column + 1));
        }
    }
}
