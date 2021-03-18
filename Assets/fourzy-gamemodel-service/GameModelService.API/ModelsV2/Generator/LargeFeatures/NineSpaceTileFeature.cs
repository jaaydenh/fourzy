using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class TileFeature : IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public List<TokenType> Tokens { get; }

        public IToken TokenTemplate { get; set; }
        public string Pattern { get; set; }
        public int MinPixels { get; set; }
        public int MaxPixels { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }


        public TileFeature(IToken TokenTemplate, int Width=3, int Height=3, string Pattern = "", int Min = 3, int Max = 5)
        {
            this.Name = "Tiles";
            this.Type = IngredientType.LARGEFEATURE;
            this.Tokens = new List<TokenType>() { TokenTemplate.Type };
            this.TokenTemplate = TokenTemplate;
            this.Pattern = Pattern;
            this.MinPixels = Min;
            this.MaxPixels = Max;
            this.Width = Width;
            this.Height = Height;
        }

        public void Build(GameBoard Board)
        {
            if (Width < 0) Width = Board.Random.RandomInteger(3, 5);
            if (Height < 0) Height = Board.Random.RandomInteger(3, 5);
            if (MinPixels < 0) MinPixels = Board.Random.RandomInteger(1, Math.Min(Width,Height));
            if (MaxPixels < 0) MaxPixels = Board.Random.RandomInteger(Math.Min(Width, Height), 1 + Math.Min(Width, Height) + Math.Max(Width, Height)/2 );
            
            if (Pattern.Length < (Width * Height))
            {
                while (Pattern.Length < (Width * Height) 
                    || (MinPixels >= 0 && MinPixels > Pattern.Count(s => s == '1')) 
                    || (MaxPixels > 0 && MaxPixels < Pattern.Count(s => s == '1')))
                  Pattern = Convert.ToString(Board.Random.RandomInteger(0, (int)Math.Pow(2, Width * Height)), 2).PadLeft(Width * Height, '0');
            }

            for(int r= 0; r<Board.Rows; r++)
              for (int c=0; c<Board.Columns; c++)
                {
                    int Index = (r % Height) * Width + (c % Width);
                    if (Pattern[Index] == '1')
                       Board.AddToken(TokenTemplate, new BoardLocation(r,c), AddTokenMethod.ALWAYS, true);
                }

        }
    }
}
