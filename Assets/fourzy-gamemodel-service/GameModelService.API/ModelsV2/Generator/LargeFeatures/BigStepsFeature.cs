using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class BigStepsFeature : IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public List<TokenType> Tokens { get; }


        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        public LargeFeatureType Feature { get {return LargeFeatureType.BIGSTEPS;} }

        public BigStepsFeature(TokenType Token, int Height = -1, int Width =-1)
        {
            this.Name = "Big Steps";
            this.Type = IngredientType.LARGEFEATURE;
            this.Tokens = new List<TokenType>() { Token };
            this.Height = Height;
            this.Width = Width;
        }

        public void Build(GameBoard Board)
        {

            if (Height < 0)
                Height = Board.Rows / 2;
            if (Width < 0)
                Width = Board.Columns / 2;

            Insert = Board.Random.RandomLocation(new BoardLocation(1,1), Board.Rows- Height-2, Board.Columns - Width -2);

            for (int r = 0; r < Height; r++)
            {
                for (int c = 0; c <Width; c++)
                {
                    if (c>=r)
                        Board.AddToken(TokenFactory.Create(Tokens.First()), 
                            new BoardLocation(Insert.Row + r, Insert.Column + c));
                }
            }

        }
    }
}
