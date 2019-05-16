using System.Collections.Generic;
using System.Linq;

namespace FourzyGameModel.Model
{
    public static class BoardFactory
    {
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


        //Look at the areas and tokens allowed and build a random board based on experience of both players.
        public static GameBoard CreateRandomBoard(GameOptions Options, Player A, Player B, Area PreferredArea = Area.NONE, string Recipe="")
        {
            Area matchSetting;

            if (PreferredArea != Area.NONE)
            {
                matchSetting = PreferredArea;
            }
            else
            {
                matchSetting = RandomMatchingArea(A.Experience.UnlockedAreas, B.Experience.UnlockedAreas);
            }

            List<TokenType> allowedTokens = IntersectTokens(A.Experience.AllowedTokens, B.Experience.AllowedTokens);

            return CreateGameBoard(Options, matchSetting, allowedTokens, Recipe);
        }

        public static GameBoard CreateGameBoard(GameOptions Options, Area MatchSetting, List<TokenType> AllowedTokens, string Recipe="")
        {
            switch (MatchSetting)
            {
                case Area.TRAINING_GARDEN:
                    return CreateGameBoard(new BeginnerGardenRandomGenerator(Recipe), Options);
                case Area.ICE_PALACE:
                    return CreateGameBoard(new IcePalaceRandomGenerator(Recipe), Options);
                case Area.ENCHANTED_FOREST:
                    return CreateGameBoard(new ForestRandomGenerator(Recipe), Options);
                case Area.SANDY_ISLAND:
                    return CreateGameBoard(new IslandRandomGenerator(Recipe), Options);
                case Area.ARENA:
                    break;
            }

            return CreateDefaultBoard(Options);
        }

        public static GameBoard CreateGameBoard(BoardGenerator Generator, GameOptions Options)
        {
            return Generator.GenerateBoard(Options.Rows, Options.Columns);
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

    }
}
