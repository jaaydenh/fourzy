using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class CornerThreePattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        public CornerThreePattern(GameBoard Board)
        {
            Locations = new List<BoardLocation>();
            Reference = new BoardLocation(1, 1);
            int Width = Board.Columns - 2;
            int Height = Board.Rows - 2;

            Locations.Add(new BoardLocation(Reference));
            Locations.Add(new BoardLocation(Reference.Row, Reference.Column - 1));
            Locations.Add(new BoardLocation(Reference.Row - 1, Reference.Column));

            Locations.Add(new BoardLocation(Reference.Row, Reference.Column + Width - 1));
            Locations.Add(new BoardLocation(Reference.Row - 1, Reference.Column + Width - 1));
            Locations.Add(new BoardLocation(Reference.Row, Reference.Column + Width - 1 + 1));

            Locations.Add(new BoardLocation(Reference.Row + Height - 1, Reference.Column));
            Locations.Add(new BoardLocation(Reference.Row + Height - 1 + 1, Reference.Column));
            Locations.Add(new BoardLocation(Reference.Row + Height - 1, Reference.Column - 1));

            Locations.Add(new BoardLocation(Reference.Row + Height - 1, Reference.Column + Width - 1));
            Locations.Add(new BoardLocation(Reference.Row + Height - 1, Reference.Column + Width - 1 + 1));
            Locations.Add(new BoardLocation(Reference.Row + Height - 1 + 1, Reference.Column + Width - 1));

        }

        public CornerThreePattern(GameBoard Board, BoardLocation Reference, int Width, int Height)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();

            if (Width < 2) Width = Board.Random.RandomInteger(2, Board.Columns - 2);
            if (Height < 2) Height = Board.Random.RandomInteger(2, Board.Rows - 2);
            if (Reference.Row == 0 && Reference.Column == 0) Reference = Board.Random.RandomLocation(new BoardLocation(1, 1), Board.Rows - 2-Height, Board.Columns - 2-Width);

            Locations.Add(new BoardLocation(Reference));
            Locations.Add(new BoardLocation(Reference.Row, Reference.Column - 1));
            Locations.Add(new BoardLocation(Reference.Row - 1, Reference.Column));

            Locations.Add(new BoardLocation(Reference.Row, Reference.Column + Width-1));
            Locations.Add(new BoardLocation(Reference.Row - 1, Reference.Column + Width-1));
            Locations.Add(new BoardLocation(Reference.Row, Reference.Column + Width -1+1));

            Locations.Add(new BoardLocation(Reference.Row + Height-1, Reference.Column));
            Locations.Add(new BoardLocation(Reference.Row + Height-1 +1, Reference.Column ));
            Locations.Add(new BoardLocation(Reference.Row + Height-1, Reference.Column - 1));

            Locations.Add(new BoardLocation(Reference.Row + Height-1, Reference.Column + Width-1));
            Locations.Add(new BoardLocation(Reference.Row + Height-1, Reference.Column + Width-1+ 1));
            Locations.Add(new BoardLocation(Reference.Row + Height-1 + 1, Reference.Column + Width-1));

        }
    }
}
