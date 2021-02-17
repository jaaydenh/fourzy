using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class PinWheelPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        public PinWheelPattern(GameBoard Board, string pattern = "", int MinPixels=-1, int MaxPixels=-1)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();

            int Width = Board.Columns / 2;
            int Height = Board.Rows / 2;
            BoardLocation Q1 = new BoardLocation(0, 0);
            BoardLocation Q2 = new BoardLocation(0, Width);
            BoardLocation Q3 = new BoardLocation(Height, 0);
            BoardLocation Q4 = new BoardLocation(Height, Width);

            if (pattern.Length < (Width * Height))
            {
                while (pattern.Length < (Width * Height) || (MinPixels >= 0 && MinPixels > pattern.Count(s => s == '1')) || (MaxPixels > 0 && MaxPixels < pattern.Count(s => s == '1')))
                    pattern = Convert.ToString(Board.Random.RandomInteger(0, (int)Math.Pow(2, Width * Height)), 2).PadLeft(Width * Height, '0');
            }

            for (int r = 0; r < Height; r++)
            {
                for (int c = 0; c < Width; c++)
                {
                    if (pattern.Substring(r*Width+c,1) == "1")
                    {
                        Locations.Add(new BoardLocation(Q1.Row + r, Q1.Column + c));
                        Locations.Add(new BoardLocation(Q2.Row + c, Q2.Column + Width - r-1));
                        Locations.Add(new BoardLocation(Q3.Row + Height -c-1, Q3.Column + r));
                        Locations.Add(new BoardLocation(Q4.Row + Height -r-1, Q4.Column + Width -c-1));

                    }
                }
            }
            
        }
    }
}
