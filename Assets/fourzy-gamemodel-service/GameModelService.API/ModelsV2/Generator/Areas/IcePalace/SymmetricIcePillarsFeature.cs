using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class SymmetricIcePillarsFeature : IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public TokenType Token { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        //public LargeFeatureType Feature { get { return LargeFeatureType.BIGSTEPS; } }

        private Dictionary<string, int> Patterns;

        public SymmetricIcePillarsFeature(TokenType Token = TokenType.FRUIT_TREE)
        {
            this.Name = "Ice Pillars";
            this.Type = IngredientType.SMALLFEATURE;
            this.Token = Token;

            this.Patterns = new Dictionary<string, int>();
            this.Patterns.Add("SymmetricOutsideBlock", 5);
            this.Patterns.Add("DoubleRowVertical", 5);
            this.Patterns.Add("DoubleRowHorizontal", 5);
            this.Patterns.Add("DoubleRowPlus", 5);
            this.Patterns.Add("Corners", 10);

        }

        public void Build(GameBoard Board)
        {
            string Pattern = Board.Random.RandomWeightedItem(Patterns);
            List<BoardLocation> ColumnLocations = new List<BoardLocation>();
            BoardLocation Insert = Board.Random.RandomLocation(new BoardLocation(1, 1), Board.Rows / 2, Board.Columns / 2);

            switch (Pattern)
            {
                case "DoubleRowPlus":
                    ColumnLocations.AddRange(new DoubleColumnPattern(Board, Insert, LineType.NONE).Locations);

                    break;

                case "DoubleRowHorizontal":
                    ColumnLocations.AddRange(new DoubleColumnPattern(Board, Insert, LineType.HORIZONTAL).Locations);

                    break;

                case "DoubleRowVertical":
                    ColumnLocations.AddRange(new DoubleColumnPattern(Board, Insert, LineType.VERTICAL).Locations);

                    break;

                case "Corners":
                    ColumnLocations.AddRange(new CornerPattern(Board, Insert, 4, 4).Locations);
                    break;

            }

            Board.AddToken(new IceBlockToken(), ColumnLocations, AddTokenMethod.ONLY_TERRAIN, true);

        }
    }
}
