using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class ApplyPatternFeature : IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public TokenType Token { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        IBoardPattern Pattern { get; set; }

        public ApplyPatternFeature(TokenType Token, IBoardPattern Pattern)
        {
            this.Name = Pattern.ToString();
            this.Type = IngredientType.LARGEFEATURE;
            this.Token = Token;

            this.Pattern = Pattern;
        }

        public void Build(GameBoard Board)
        {
            Board.AddToken(TokenFactory.Create(Token), Pattern.Locations, 0, true);
        }
    }
}
