using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class TreesDropFruitFeature : IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public SmallFeatureType Feature { get { return SmallFeatureType.TREES_DROP_FRUIT; } }
        public TokenType Token { get; }

        private Dictionary<string, int> Patterns;
        private int NumberPatterns { get; set; }

        public TreesDropFruitFeature(int NumberPatterns = 1)
        {
            this.Name = "Trees Drop Some Fruit";
            this.Type = IngredientType.ENHANCEMENT;
            this.Token = TokenType.FRUIT_TREE;

            this.NumberPatterns = NumberPatterns;
            this.Patterns = new Dictionary<string, int>();
            this.Patterns.Add("SurroundWithSticky", 10);
            this.Patterns.Add("SurroundWithFruit", 10);
            this.Patterns.Add("FourSidesSticky", 10);
            this.Patterns.Add("FourSidesFruit", 10);
            this.Patterns.Add("ACoupleRandomFruit", 10);
            this.Patterns.Add("ACoupleRandomSticky", 10);
            this.Patterns.Add("DottedHLineOfFruit", 10);
            this.Patterns.Add("DottedHLineOfSticky", 10);
            this.Patterns.Add("DottedVLineOfFruit", 10);
            this.Patterns.Add("DottedVLineOfSticky", 10);
            this.Patterns.Add("CheckeredSticky", 10);
            this.Patterns.Add("CheckeredFruit", 10);
            this.Patterns.Add("DottedSticky", 10);
            this.Patterns.Add("DottedFruit", 10);
        }

        public void Build(GameBoard Board)
        {
            List<BoardLocation> TreeLocations = Board.FindTokenLocations(TokenType.FRUIT_TREE);
            //BoardLocation Insert = Board.Random.RandomLocation(new BoardLocation(1, 1), Board.Rows / 2, Board.Columns / 2);

            if (TreeLocations.Count == 0) return;

            int count = 0;
            while(count < NumberPatterns)
            {
                BoardLocation Insert = Board.Random.RandomLocation(TreeLocations);
                List<BoardLocation> InsertLocatons = new List<BoardLocation>();

            switch (Board.Random.RandomWeightedItem(Patterns))
            {
                case "SurroundWithSticky":
                        InsertLocatons = new SurroundPattern(Board, Insert).Locations;
                        Board.AddToken(new StickyToken(), InsertLocatons, AddTokenMethod.ONLY_TERRAIN, true);
                        break;

                case "SurroundWithFruit":
                        InsertLocatons = new SurroundPattern(Board, Insert).Locations;
                        Board.AddToken(new FruitToken(), InsertLocatons, AddTokenMethod.ONLY_TERRAIN, true);
                        break;

                case "FourSidesFruit":
                        InsertLocatons = new  FourSidesPattern(Board, Insert).Locations;
                        Board.AddToken(new FruitToken(), InsertLocatons, AddTokenMethod.ONLY_TERRAIN, true);
                        break;

                case "FourSidesSticky":
                        InsertLocatons = new FourSidesPattern(Board, Insert).Locations;
                        Board.AddToken(new StickyToken(), InsertLocatons, AddTokenMethod.ONLY_TERRAIN, true);
                        break;

                    case "ACoupleRandomFruit":
                        InsertLocatons = new RandomPattern(Board, new BoardLocation(1,1),6,Board.Rows-2, Board.Columns-2).Locations;
                        Board.AddToken(new FruitToken(), InsertLocatons, AddTokenMethod.ONLY_TERRAIN, true);
                        break;

                    case "ACoupleRandomSticky":
                        InsertLocatons = new RandomPattern(Board, new BoardLocation(1, 1), 6, Board.Rows - 2, Board.Columns - 2).Locations;
                        Board.AddToken(new StickyToken(), InsertLocatons, AddTokenMethod.ONLY_TERRAIN, true);
                        break;

                    case "DottedHLineOfFruit":
                        Insert = Board.Random.RandomLocation(new BoardLocation(1,1),Board.Rows-2, Board.Columns-2);
                        InsertLocatons = new DottedLinePattern(Board, Insert, LineType.HORIZONTAL,Board.Random.Chance(50)).Locations;
                        Board.AddToken(new FruitToken(), InsertLocatons, AddTokenMethod.ONLY_TERRAIN, true);
                        break;

                    case "DottedVLineOfFruit":
                        Insert = Board.Random.RandomLocation(new BoardLocation(1, 1), Board.Rows - 2, Board.Columns - 2);
                        InsertLocatons = new DottedLinePattern(Board, Insert, LineType.HORIZONTAL, Board.Random.Chance(50)).Locations;
                        Board.AddToken(new FruitToken(), InsertLocatons, AddTokenMethod.ONLY_TERRAIN, true);
                        break;

                    case "DottedHLineOfSticky":
                        Insert = Board.Random.RandomLocation(new BoardLocation(1, 1), Board.Rows - 2, Board.Columns - 2);
                        InsertLocatons = new DottedLinePattern(Board, Insert, LineType.HORIZONTAL, Board.Random.Chance(50)).Locations;
                        Board.AddToken(new StickyToken(), InsertLocatons, AddTokenMethod.ONLY_TERRAIN, true);
                        break;

                    case "DottedVLineOfSticky":
                        Insert = Board.Random.RandomLocation(new BoardLocation(1, 1), Board.Rows - 2, Board.Columns - 2);
                        InsertLocatons = new DottedLinePattern(Board, Insert, LineType.HORIZONTAL, Board.Random.Chance(50)).Locations;
                        Board.AddToken(new StickyToken(), InsertLocatons, AddTokenMethod.ONLY_TERRAIN, true);
                        break;

                    case "CheckeredSticky":
                        Insert = Board.Random.RandomLocation(new BoardLocation(1, 1), Board.Rows/2, Board.Columns/2);
                        InsertLocatons = new CheckerPattern(Board, Insert,4,3).Locations;
                        Board.AddToken(new StickyToken(), InsertLocatons, AddTokenMethod.ONLY_TERRAIN, true);
                        break;

                    case "CheckeredFruit":
                        Insert = Board.Random.RandomLocation(new BoardLocation(1, 1), Board.Rows / 2, Board.Columns / 2);
                        InsertLocatons = new CheckerPattern(Board, Insert, 4, 3).Locations;
                        Board.AddToken(new FruitToken(), InsertLocatons, AddTokenMethod.ONLY_TERRAIN, true);
                        break;

                    case "DottedSticky":
                        Insert = Board.Random.RandomLocation(new BoardLocation(1, 1), Board.Rows / 2, Board.Columns / 2);
                        InsertLocatons = new DotPattern(Board, Insert, 4,3).Locations;
                        Board.AddToken(new StickyToken(), InsertLocatons, AddTokenMethod.ONLY_TERRAIN, true);
                        break;

                    case "DottedFruit":
                        Insert = Board.Random.RandomLocation(new BoardLocation(1, 1), Board.Rows / 2, Board.Columns / 2);
                        InsertLocatons = new DotPattern(Board, Insert, 4, 3).Locations;
                        Board.AddToken(new FruitToken(), InsertLocatons, AddTokenMethod.ONLY_TERRAIN, true);
                        break;

                    default:
                        count++;
                        break;
                    }

                count++;
                
            }

        }



    }
    
}
