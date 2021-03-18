using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class TerrainIngredient : IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public List<TokenType> Tokens { get; }


        public IBoardPattern Pattern { get; set; }
        public IToken TokenPattern { get; set; }

        public TerrainIngredient(IToken TokenPattern)
        {
            this.Name = "Terrain";
            this.Type = IngredientType.TERRAIN;
            this.Tokens = new List<TokenType>() { TokenPattern.Type };

            this.TokenPattern = TokenPattern;
            this.Pattern = Pattern;
        }

        public void Build(GameBoard Board)
        {
            Board.AddTokenToAllSpaces(TokenPattern, AddTokenMethod.NO_TERRAIN);
        }
    }
}
