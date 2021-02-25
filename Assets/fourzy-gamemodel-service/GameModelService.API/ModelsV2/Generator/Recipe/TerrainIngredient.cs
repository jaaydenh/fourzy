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
        public TokenType Token { get; set; }

        public IBoardPattern Pattern { get; set; }
        public IToken TokenPattern { get; set; }

        public TerrainIngredient(IToken TokenPattern)
        {
            this.Name = "Terrain";
            this.Type = IngredientType.TERRAIN;
            this.Token = TokenPattern.Type;
            this.TokenPattern = TokenPattern;
            this.Pattern = Pattern;
        }

        public void Build(GameBoard Board)
        {
            Board.AddTokenToAllSpaces(TokenPattern, AddTokenMethod.NO_TERRAIN);
        }
    }
}
