using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class SwirlingArrowsFeature : ISmallFeature, IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public TokenType Token { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        public SmallFeatureType Feature { get { return SmallFeatureType.ARROWS_SwIRLING; } }
        public Rotation Rotation { get; set; }
        public string Pattern { get; set; }
        public int MinPixels { get; set; }
        public int MaxPixels { get; set; }

        public SwirlingArrowsFeature(Rotation Rotation = Rotation.CLOCKWISE, string Pattern = "", int MinPixels = -1, int MaxPixels = -1)
        {
            this.Rotation = Rotation;
            this.Insert = Insert;
            this.MinPixels = MinPixels;
            this.MaxPixels = MaxPixels;
            this.Pattern = Pattern;
        }


        public void Build(GameBoard Board)
        {

            int Width = Board.Columns / 2;
            int Height = Board.Rows / 2;
            BoardLocation Q1 = new BoardLocation(0, 0);
            BoardLocation Q2 = new BoardLocation(0, Width);
            BoardLocation Q3 = new BoardLocation(Height, 0);
            BoardLocation Q4 = new BoardLocation(Height, Width);

            if (Pattern.Length < (Board.Rows * Board.Columns))
            {
                while (Pattern.Length < (Width * Height) || (MinPixels >= 0 && MinPixels > Pattern.Count(s => s == '1')) || (MaxPixels > 0 && MaxPixels < Pattern.Count(s => s == '1')))
                    Pattern = Convert.ToString(Board.Random.RandomInteger(0, (int)Math.Pow(2, Width * Height)), 2).PadLeft(Width * Height, '0');
            }

            for (int r = 0; r < Height; r++)
            {
                for (int c = 0; c < Width; c++)
                {
                    if (Pattern.Substring(r * Width + c, 1) == "1")
                    {
                        Board.AddToken(new ArrowToken(Rotation == Rotation.CLOCKWISE ? Direction.RIGHT:Direction.LEFT),new BoardLocation(Q1.Row + r, Q1.Column + c));
                        Board.AddToken(new ArrowToken(Rotation == Rotation.CLOCKWISE ? Direction.DOWN: Direction.UP), new BoardLocation(Q2.Row + c, Q2.Column + Width - r - 1));
                        Board.AddToken(new ArrowToken(Rotation == Rotation.CLOCKWISE ? Direction.UP: Direction.DOWN), new BoardLocation(Q3.Row + Height - c - 1, Q3.Column + r));
                        Board.AddToken(new ArrowToken(Rotation == Rotation.CLOCKWISE ? Direction.LEFT: Direction.RIGHT), new BoardLocation(Q4.Row + Height - r - 1, Q4.Column + Width - c - 1));

                    }
                }
            }

        }
    }
}
