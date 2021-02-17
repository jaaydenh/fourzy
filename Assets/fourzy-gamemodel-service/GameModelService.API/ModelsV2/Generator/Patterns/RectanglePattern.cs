using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class RectanglePattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        //create a patter

        public RectanglePattern(GameBoard Board, BoardLocation Reference, int Height, int Width)
        {
            if (Height < 0) Height = Board.Random.RandomInteger(2, Board.Rows / 2);
            if (Width < 0) Width = Board.Random.RandomInteger(2, Board.Columns / 2);
            if (Reference.Row == 0 && Reference.Column == 0)
                Reference = Board.Random.RandomLocation(new BoardLocation(1, 1), Board.Rows - Height, Board.Columns - Width);
                
            this.Reference = Reference;
            Locations = new List<BoardLocation>();
            
            for (int r = 0; r < Height; r++)
            {
                for (int c = 0; c < Width; c++)
                {
                        Locations.Add(new BoardLocation(Reference.Row + r, Reference.Column + c));
                }
            }

        }
    }
}
