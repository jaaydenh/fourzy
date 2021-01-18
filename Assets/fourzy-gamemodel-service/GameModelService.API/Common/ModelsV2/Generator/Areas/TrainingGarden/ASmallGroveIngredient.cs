using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class ASmallGroveFeature : IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public TokenType Token { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        //public LargeFeatureType Feature { get { return LargeFeatureType.; } }

        private Dictionary<string, int> Patterns;

        public ASmallGroveFeature(TokenType Token = TokenType.FRUIT_TREE)
        {
            this.Name = "A Small Grove";
            this.Type = IngredientType.SMALLFEATURE;
            this.Token = Token;

            this.Patterns = new Dictionary<string, int>();
            this.Patterns.Add("TwoPatternSep2", 5);
            this.Patterns.Add("TwoPatternSep3",5);
            this.Patterns.Add("ThreePattern", 10);
            this.Patterns.Add("Corners", 10);
            this.Patterns.Add("Staggered", 20);
//            this.Patterns.Add("StaggeredLarge", 10);
        }

        public void Build(GameBoard Board)
        {
            string Pattern = Board.Random.RandomWeightedItem(Patterns);
            List<BoardLocation> TreeLocations = new List<BoardLocation>();
            BoardLocation Insert = Board.Random.RandomLocation(new BoardLocation(1, 1), Board.Rows / 2, Board.Columns / 2);

            switch (Pattern)
            {
                case "TwoPatternSep2":
                    CompassDirection D1 = Board.Random.RandomCompassDirection(new List<CompassDirection>() { CompassDirection.E, CompassDirection.SE, CompassDirection.S });

                    TreeLocations.AddRange(new TwoPattern(Board, Insert, D1, 2).Locations);

                    break;

                case "TwoPatternSep3":
                    CompassDirection D2 = Board.Random.RandomCompassDirection(new List<CompassDirection>() { CompassDirection.E, CompassDirection.SE, CompassDirection.S });

                    TreeLocations.AddRange(new TwoPattern(Board, Insert, D2, 3).Locations);

                    break;

                case "ThreePattern":
                    CompassDirection D3 = Board.Random.RandomCompassDirection(new List<CompassDirection>() { CompassDirection.E, CompassDirection.SE, CompassDirection.S });

                    TreeLocations.AddRange(new ThreePattern(Board, Insert, D3).Locations);

                    break;


                case "Corners":

                    TreeLocations.AddRange(new CornerPattern(Board, Insert, 4, 4).Locations);
                    break;

                case "Staggered":

                    TreeLocations.AddRange(new StaggeredDotPattern(Board, Insert, 4, 4).Locations);
                    break;

                case "StaggeredLarge":

                    TreeLocations.AddRange(new StaggeredDotPattern(Board, new BoardLocation(1,1), 4, 6).Locations);
                    break;

            }

            Board.AddToken(new FruitTreeToken(), TreeLocations, AddTokenMethod.ONLY_TERRAIN, true);

        }
    }
}
