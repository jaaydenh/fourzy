using FourzyGameModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class BoardGeneratorTester
    {
        public Dictionary<string, List<GameBoard>> GeneratedBoards;
        public Dictionary<string, List<double>> ComplexityScores;
        public Dictionary<string, GeneratedBoardStatistics> RecipeStats;
        public Dictionary<string, string> Errors;
        public const int MaxDeadSpaces = 12;
        public const int MinNewBoardScore = 50;
        public const int NumberTestAttempts = 50;
        public int FailureCount { get; set; }

        public BoardGeneratorTester()
        {
            RecipeStats = new Dictionary<string, GeneratedBoardStatistics>();
            ComplexityScores = new Dictionary<string, List<double>>();
            GeneratedBoards = new Dictionary<string, List<GameBoard>>();
            Errors = new Dictionary<string, string>();
            FailureCount = 0;
        }

        public bool RunTests(Area TestArea, List<string> RecipeList, int NumberAttempts = NumberTestAttempts, int FailureTolerance = -1)
        {
            if (FailureTolerance < 0) FailureTolerance = NumberAttempts / 10;
            FailureCount = 0;
            foreach (string r in RecipeList)
            {
                if (!RunTests(TestArea, r, NumberAttempts)) FailureCount++;
                if (FailureCount > FailureTolerance) return false;
            }
            return true;
        }

        public bool RunTests(Area TestArea, string Recipe, int NumberAttempts = NumberTestAttempts)
        {
            GeneratedBoards.Add(Recipe, new List<GameBoard>());
            for (int i = 0; i < NumberAttempts; i++)
            {
                try
                {
                    GameBoard GB = CreateBoardForTest(TestArea, Recipe);
                    if (GB == null) continue;

                    //Have we already looked at this board?
                    if (GeneratedBoards[Recipe].Where(g => g.ContentString == GB.ContentString).Count() > 0) continue;

                    GeneratedBoards[Recipe].Add(GB);
                    if (!TestGeneratedBoard(GB)) return false;
                    int Score = BoardFactory.BoardComplexityScore(GB);
                    if (!ComplexityScores.ContainsKey(Recipe)) ComplexityScores.Add(Recipe, new List<double>() { });
                    ComplexityScores[Recipe].Add(Score);
                }
                catch 
                {
                    
                    return false;
                }
            }
            return true;
        }

        public void CalculateComplexityStats()
        {
            foreach(string recipe in ComplexityScores.Keys)
            {
                GeneratedBoardStatistics Stats = new GeneratedBoardStatistics();
                Stats.MaxComplexity = (int)ComplexityScores[recipe].Max();
                Stats.MinComplexity = (int)ComplexityScores[recipe].Min();
                Stats.AverageComplexity = (int)ComplexityScores[recipe].Average();
                Stats.StandardDeviation = (int)ComplexityScores[recipe].StdDev();
                Stats.UniqueBoards = GeneratedBoards[recipe].Count();

                int count = ComplexityScores[recipe].Count();

                if (count % 2 == 0)
                    Stats.MedianComplexity = (int)ComplexityScores[recipe].OrderBy(x => x).Skip((count / 2) - 1).Take(2).Average();
                else
                    Stats.MedianComplexity = (int)ComplexityScores[recipe].OrderBy(x => x).ElementAt(count / 2);

                RecipeStats.Add(recipe, Stats);
            }
        }

        public GameBoard CreateBoardForTest(Area TestArea, string Recipe)
        {
            Player A = new Player(1, "A");
//            A.Experience = new PlayerExperience();

            Player B = new Player(2, "B");
  //          B.Experience = new PlayerExperience();

            GameBoard board = BoardFactory.CreateGameBoard(TestArea, new GameOptions(), new BoardGenerationPreferences(), Recipe);
            return board;
        }

        public bool TestGeneratedBoard(GameBoard Board)
        {
            if (Errors == null) Errors = new Dictionary<string, string>();
            
            //it's possible that the generate created the same board more than once.
            if (Errors.Keys.Contains(Board.ContentString)) return false;

            foreach (BoardLocation l in Board.Corners)
                if (Board.ContentsAt(l).ContainsPiece
                    || !Board.ContentsAt(l).ContainsTokenType(TokenType.BLOCKER)
                    || Board.ContentsAt(l).TokenCount > 1)
                {
                    Errors.Add(Board.ContentString, "Corners");
                    return false;
                }

            int Dead = AIScoreEvaluator.CountTheDead(Board);
            if (Dead > MaxDeadSpaces)
            {
                if (!Errors.Keys.Contains(Board.ContentString))
                {
                    Errors.Add(Board.ContentString, "TooManyDead=" + Dead);
                    return false;
                }
            }


                //int Score = AITurnEvaluator.ScoreFours(new GameState(Board, new GameOptions()),1);
                //if (Score < MinNewBoardScore) { Errors.Add(Board.ContentString, "ScoreTooLow=" + Score); return false; }

                foreach (BoardSpace s in Board.Contents)
                {

                    //  More than two of the same classification.
                    if (s.CountTokenClass(TokenClassification.TERRAIN) > 1) { Errors.Add(Board.ContentString, "Terrain"); return false; }
                    if (s.CountTokenClass(TokenClassification.INSTRUCTION) > 1) { Errors.Add(Board.ContentString, "Instructions"); return false; }
                    if (s.CountTokenClass(TokenClassification.BLOCKER) > 1) { Errors.Add(Board.ContentString, "Blockers"); return false; }
                    if (s.CountTokenClass(TokenClassification.ITEM) > 1) { Errors.Add(Board.ContentString, "Item"); return false; }

                    //Do not have like things:
                    //  Ice and Ice Block
                    //  Sticky and Fruit
                    if (s.ContainsTokenType(TokenType.STICKY) && s.ContainsTokenType(TokenType.ARROW)) { Errors.Add(Board.ContentString, "Sticky and Arrow"); return false; }
                    if (s.ContainsTokenType(TokenType.STICKY) && s.ContainsTokenType(TokenType.FRUIT)) { Errors.Add(Board.ContentString, "Sticky and Fruit"); return false; }
                    if (s.ContainsTokenType(TokenType.ICE) && s.ContainsTokenType(TokenType.ICE_BLOCK)) { Errors.Add(Board.ContentString, "Ice and Ice Block"); return false; }

                }
                return true;
            }
    }
}


