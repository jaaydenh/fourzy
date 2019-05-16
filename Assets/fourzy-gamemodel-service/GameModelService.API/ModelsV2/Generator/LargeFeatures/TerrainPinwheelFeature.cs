using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class TerrainPinwheelFeature : IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public TokenType Token { get; set; }
        public string Pattern { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }

        public TerrainPinwheelFeature(TokenType Token, string Pattern, int Min=-1, int Max=-1)
        {
            this.Name = "Terrain Pinwheel";
            this.Type = IngredientType.LARGEFEATURE;
            this.Token = Token;
            this.Pattern = Pattern;
        }

        public void Build(GameBoard Board)
        {
            Board.AddToken(TokenFactory.Create(Token), new PinWheelPattern(Board,Pattern,Min,Max).Locations, 0, true);
        }
    }
}
