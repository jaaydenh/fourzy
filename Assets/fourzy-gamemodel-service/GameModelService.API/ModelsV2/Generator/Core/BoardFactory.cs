using System;
using System.Collections.Generic;
using System.Linq;

namespace FourzyGameModel.Model
{
    public static class BoardFactory
    {
        public const int DynamicConstant = 25;
        public const int MoveableConstant = 5;


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

        public static GameBoard CreateGameBoard(Area BoardArea, GameOptions Options = null, BoardGenerationPreferences Preferences = null, string Recipe = "")
        {
            if (Options == null) Options = new GameOptions();
            if (Preferences == null) Preferences = new BoardGenerationPreferences();

            GameBoard Board = null;
            BoardGenerator Generator = null;

            List<Area> LiveAreas = new List<Area>() { Area.TRAINING_GARDEN, Area.ENCHANTED_FOREST, Area.ICE_PALACE, Area.SANDY_ISLAND };
            if (BoardArea == Area.NONE) BoardArea = RandomArea(LiveAreas);
            
            switch (BoardArea)
            {
                case Area.TRAINING_GARDEN:
                    Generator = new BeginnerGardenRandomGenerator(Recipe);
                    break;
                case Area.ICE_PALACE:
                    Generator = new IcePalaceRandomGenerator(Recipe);
                    break;
                case Area.ENCHANTED_FOREST:
                    Generator = new ForestRandomGenerator(Recipe);
                    break;
                case Area.SANDY_ISLAND:
                    Generator = new IslandRandomGenerator(Recipe);
                    break;

                case Area.ARENA:
                    break;
            }

            Board = Generator.GenerateBoard(Options.Rows, Options.Columns);
            int Score = BoardComplexityScore(Board);
            int attempts = 0;

            
            //Check for complexity requirement.
            if (Preferences.DesiredComplexityPercentage > 0)
            {
                Tuple<int, int> DesiredComplexity = NormalizeComplexity(BoardArea, Preferences.DesiredComplexityPercentage);
                while ((Score < DesiredComplexity.Item1 || Score > DesiredComplexity.Item2) && attempts < BoardGeneratorConstants.MAX_GENERATOR_ATTEMPTS)
                {
                    Board = Generator.GenerateBoard(Options.Rows, Options.Columns);
                    Score = BoardComplexityScore(Board);
                }
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
            
            foreach (BoardSpace s in Board.Contents)
            {
                foreach (IToken t in s.Tokens.Values)
                {

                if (t.isMoveable) numberMoveable++;
                if (t.HasDynamicFeature) numberDynamic++;
                score += t.Complexity;
                if (!TokenTypes.Contains(t.Type)) {
                    TokenTypes.Add(t.Type);
                    uniquetokenscore += t.Complexity;
                    }
                }
            }

            if (numberDynamic >0)
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
            int Min = 500;
            int Max = 1500;
            int PercentBand = 25;
            switch (TargetArea)
            {
                case Area.TRAINING_GARDEN:
                    Min = 250;
                    Max = 900;
                    break;
                case Area.ENCHANTED_FOREST:
                    Min = 270;
                    Max = 1200;
                    break;
                case Area.ICE_PALACE:
                    Min = 500;
                    Max = 1700;
                    break;
                case Area.SANDY_ISLAND:
                    Min = 350;
                    Max = 1800;
                    break;
            }

            Min = Min + (Max - Min) * Math.Max(TargetPercentage - PercentBand/2, 0)/100;
            Max = Min + (Max - Min) * Math.Min(TargetPercentage + PercentBand/2, 100)/100;

            return new Tuple<int, int>(Min, Max);
        }
    }
}
