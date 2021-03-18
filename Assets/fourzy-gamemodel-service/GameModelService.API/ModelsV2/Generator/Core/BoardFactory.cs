using System;
using System.Collections.Generic;
using System.Linq;

namespace FourzyGameModel.Model
{
    public static class BoardFactory
    {
        public const int DynamicConstant = 25;
        public const int MoveableConstant = 5;

        //Maybe we can move these to a new spot
        public const int LowestRating = 800;
        public const int HighestRating = 2000;

        public static GameBoard CreateDefaultBoard(int Rows, int Columns)
        {
            GameBoard board = new GameBoard(Rows, Columns);
            foreach (BoardLocation loc in board.Corners)
            {
                board.AddToken(new BlockerToken(), loc, AddTokenMethod.CORNERS);
            }
            return board;
        }

        public static GameBoard CreateDefaultBoard(GameOptions Options)
        {
            return CreateDefaultBoard(Options.Rows, Options.Columns);
        }

        //public static GameBoard CreateRandomBoard(GameOptions Options, Area BoardArea, int MaxComplexity, List<TokenType> AllowedTokens, string Recipe="")
        //{

        //    return CreateGameBoard( BoardArea, Options, Perferences, Recipe);
        //}

        //public static GameBoard CreateRandomBoard(GameOptions Options, Player A, Player B, Area PreferredArea = Area.NONE, BoardGenerationPreferences Preferences = null)
        //{
        //    Area matchSetting;

        //    if (PreferredArea != Area.NONE)
        //    {
        //        matchSetting = PreferredArea;
        //    }
        //    else
        //    {
        //        matchSetting = RandomMatchingArea(A.Experience.UnlockedAreas, B.Experience.UnlockedAreas);
        //    }

        //    List<TokenType> allowedTokens = IntersectTokens(A.Experience.AllowedTokens, B.Experience.AllowedTokens);

        //    return CreateGameBoard(Options, matchSetting, allowedTokens, Preferences);
        //}


        //Look at the areas and tokens allowed and build a random board based on experience of both players.

        //public static GameBoard CreateRandomBoard(GameOptions Options, Player A, Player B, Area PreferredArea = Area.NONE, BoardGenerationPreferences Preferences = null, string Recipe="")
        //{
        //    Area SelectedArea = Area.NONE;

        //    if (PreferredArea != Area.NONE)
        //    {
        //        SelectedArea = PreferredArea;
        //    }
        //    else
        //    {
        //        SelectedArea = RandomMatchingArea(A.Experience.UnlockedAreas, B.Experience.UnlockedAreas);
        //    }

        //    List<TokenType> allowedTokens = IntersectTokens(A.Experience.AllowedTokens, B.Experience.AllowedTokens);

        //    return CreateGameBoard(SelectedArea, Options, Preferences, Recipe);
        //}

        //CREATE GAME BOARD

        //public static GameBoard CreateGameBoard(GameOptions Options, Area SelectedArea, BoardGenerationPreferences Preferences = null, string Recipe="")
        //{
        //    switch (SelectedArea)
        //    {
        //        case Area.TRAINING_GARDEN:
        //            return CreateGameBoard(new BeginnerGardenRandomGenerator(Recipe), Options, Preferences);
        //        case Area.ICE_PALACE:
        //            return CreateGameBoard(new IcePalaceRandomGenerator(Recipe), Options, Preferences);
        //        case Area.ENCHANTED_FOREST:
        //            return CreateGameBoard(new ForestRandomGenerator(Recipe), Options, Preferences);
        //        case Area.SANDY_ISLAND:
        //            return CreateGameBoard(new IslandRandomGenerator(Recipe), Options, Preferences);
        //        case Area.ARENA:
        //            break;
        //    }

        //    return CreateDefaultBoard(Options);
        //}

        //Create a gameboard 
        //public static GameBoard CreateGameBoard(BoardGenerator Generator, GameOptions Options)
        //{
        //    return Generator.GenerateBoard(Options.Rows, Options.Columns);
        //}

        public static GameBoard CreateGameBoard(Area BoardArea, GameOptions Options = null, BoardGenerationPreferences Preferences = null, string RecipeName = "")
        {
            if (Options == null) Options = new GameOptions();
            if (Preferences == null) Preferences = new BoardGenerationPreferences();
            if (RecipeName.Length > 0) Preferences.RequestedRecipe = RecipeName;

            GameBoard Board = null;
            BoardGenerator Generator = null;

            List<Area> LiveAreas = new List<Area>() { Area.TRAINING_GARDEN, Area.ENCHANTED_FOREST, Area.ICE_PALACE, Area.SANDY_ISLAND };
            if (BoardArea == Area.NONE) BoardArea = RandomArea(LiveAreas);
            
            switch (BoardArea)
            {
                case Area.TRAINING_GARDEN:
                    Generator = new BeginnerGardenRandomGenerator("", Options, Preferences);
                    break;
                case Area.ICE_PALACE:
                    Generator = new IcePalaceRandomGenerator("",Options, Preferences);
                    break;
                case Area.ENCHANTED_FOREST:
                    Generator = new ForestRandomGenerator("", Options, Preferences);
                    break;
                case Area.SANDY_ISLAND:
                    Generator = new IslandRandomGenerator("", Options, Preferences);
                    break;

                case Area.ARENA:
                    break;
            }

            Board = Generator.GenerateBoard();
            int Score = BoardComplexityScore(Board);
            int attempts = 0;
            
            //Check for complexity requirement.
            if ((Preferences.TargetComplexityHigh > 0 && Preferences.TargetComplexityLow > 0))
            {
                int BestMatchScore = -1;
                int TargetComplexityLow = -1;
                int TargetComplexityHigh = -1;
                GameBoard BestMatchBoard = null;
                int TargetComplexityScore = -1;

                //if (Preferences.DesiredComplexityPercentage >= 0)
                //{
                //    Tuple<int, int> DesiredComplexity = NormalizeComplexity(BoardArea, Preferences.DesiredComplexityPercentage);
                //    TargetComplexityLow = DesiredComplexity.Item1;
                //    TargetComplexityHigh = DesiredComplexity.Item2;
                //    TargetComplexityScore = ConvertPercentageToTargetScore(BoardArea, Preferences.DesiredComplexityPercentage);
                //}

                if (Preferences.TargetComplexityHigh > 0 && Preferences.TargetComplexityLow > 0)            
                {
                    TargetComplexityLow = Preferences.TargetComplexityLow;
                    TargetComplexityHigh = Preferences.TargetComplexityHigh;
                    TargetComplexityScore = (TargetComplexityHigh - TargetComplexityLow) / 2;
                }

                while (attempts++ < BoardGeneratorConstants.MAX_GENERATOR_ATTEMPTS)
                {
                    Board = Generator.GenerateBoard();
                    Score = BoardComplexityScore(Board);

                    if (Score > TargetComplexityLow && Score < TargetComplexityHigh) break;
                    else
                    {
                        if (BestMatchScore<0 || Math.Abs(BestMatchScore-TargetComplexityScore) > Math.Abs(Score - TargetComplexityScore))
                        {
                            BestMatchBoard = Board;
                            BestMatchScore = Score;
                        }
                    }
                }

                if (attempts >= BoardGeneratorConstants.MAX_GENERATOR_ATTEMPTS) Board = BestMatchBoard;
            }
            return Board;
        }


        //COMPLEXITY FACTORS:
        //Sum of all tokens
        //Number of different tokens
        //Has Dynamic Elements
        //Terrain allows pushing

        public static int BoardComplexityScore(GameBoard Board)
        {
            int score = 0;
            int numberDynamic = 0;
            int numberMoveable = 0;
            List<TokenType> TokenTypes = new List<TokenType>();
            int uniquetokenscore = 0;
            int totaltokencomplexity = 0;

            //Number of Tokens
            //Number of different types of tokens

            //Dynamic Tokens (token has a countdown)
            //Transforming Tokens (token will transform when bumped, passed over)
            //Moveable Tokens (a piece can be pushed on it)


            foreach (BoardSpace s in Board.Contents)
            {
                foreach (IToken t in s.Tokens.Values)
                {

                if (t.isMoveable) numberMoveable++;
                if (t.HasDynamicFeature) numberDynamic++;
                totaltokencomplexity += t.Complexity;
                
                if (!TokenTypes.Contains(t.Type)) {
                    TokenTypes.Add(t.Type);
                    uniquetokenscore += t.Complexity;
                    }
                }
            }

            score += totaltokencomplexity;
            score += uniquetokenscore * Board.Rows * Board.Columns;
            //Increase score for dynamic tokens
            if (numberDynamic > 0)
            {
                score += numberDynamic * uniquetokenscore * DynamicConstant;
            }

            //Increase score for number of tokens that allow pushing
            if (numberMoveable > 0)
            {
                score += numberMoveable * MoveableConstant;
            }

            return score;
        }

        public static int BoardComplexityScoreOld(GameBoard Board)
        {
            int score = 0;
            int numberDynamic = 0;
            int numberMoveable = 0;
            List<TokenType> TokenTypes = new List<TokenType>();
            int uniquetokenscore = 0;

            foreach (BoardSpace s in Board.Contents)
            {
                foreach (IToken t in s.Tokens.Values)
                {

                    if (t.isMoveable) numberMoveable++;
                    if (t.HasDynamicFeature) numberDynamic++;
                    score += t.Complexity;
                    if (!TokenTypes.Contains(t.Type))
                    {
                        TokenTypes.Add(t.Type);
                        uniquetokenscore += t.Complexity;
                    }
                }
            }

            if (numberDynamic > 0)
            {
                score += numberDynamic * DynamicConstant;
            }

            if (numberMoveable > 0)
            {
                score += numberMoveable * MoveableConstant;
            }

            score += uniquetokenscore * Board.Rows * Board.Columns;

            return score;
        }

        public static int BoardComplexityPercentage(GameBoard Board)
        {
            int score = BoardComplexityScore(Board);

            BoardGenerator Gen = BoardGeneratorFactory.CreateGenerator(Board.Area);
            int Min = Gen.MinComplexity;
            int Max = Gen.MaxComplexity;

            score = 100 * (Math.Max(score,Min) - Min)/(Max - Min) ;

            return score;
        }

        public static List<TokenType> IntersectTokens(List<TokenType> A, List<TokenType> B)
        {
            List<TokenType> intersect = new List<TokenType>();
            foreach (TokenType t in A)
            {
                if (B.Contains(t)) intersect.Add(t);
            }

            return intersect;
        }

        public static Area RandomMatchingArea(List<Area> A, List<Area> B)
        {
            List<Area> intersect = new List<Area>();
            foreach (Area a in A)
            {
                if (B.Contains(a)) intersect.Add(a);
            }

            if (intersect.Count == 0) return AreaConstants.DefaultArea;
            if (intersect.Count == 1) return intersect.First();

            return RandomArea(intersect);
        }


        public static Area RandomArea(List<Area> Areas)
        {
            RandomTools r = new RandomTools();

            return Areas[r.Range(0, Areas.Count)];
        }

        public static Tuple<int, int> NormalizeComplexity(Area TargetArea, int TargetPercentage)
        {
            int PercentBand = 20;
            BoardGenerator Gen = BoardGeneratorFactory.CreateGenerator(TargetArea);
            int Min = Gen.MinComplexity;
            int Max = Gen.MaxComplexity;


            Min = Min + (Max - Min) * Math.Max(TargetPercentage - PercentBand/2, 0)/100;
            Max = Min + (Max - Min) * Math.Min(TargetPercentage + PercentBand/2, 100)/100;

            return new Tuple<int, int>(Min, Max);
        }

        public static int ConvertPercentageToTargetScore(Area TargetArea, int TargetPercentage)
        {
            BoardGenerator Gen = BoardGeneratorFactory.CreateGenerator(TargetArea);
            int Min = Gen.MinComplexity;
            int Max = Gen.MaxComplexity;

            return Min + (Max-Min) * TargetPercentage/100;
        }

        public static List<string> GetRecipeListForComplexityRange(Area TargetArea, int TargetPercentage)
        {
            List<string> RecipeList = new List<string>();
            BoardGenerator Generator = null;

            BoardGenerationPreferences Preferences = new BoardGenerationPreferences(TargetArea, TargetPercentage);
           
            Generator = BoardGeneratorFactory.CreateGenerator(TargetArea);
            
            foreach (BoardRecipe r in Generator.GetRecipesInComplexityRange(Preferences.TargetComplexityLow, Preferences.TargetComplexityHigh))
            {
                RecipeList.Add(r.Name);
            }

            return RecipeList; 
        }

    }
}
