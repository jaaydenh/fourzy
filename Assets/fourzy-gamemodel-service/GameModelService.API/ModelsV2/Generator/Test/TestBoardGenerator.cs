using FourzyGameModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class BoardGeneratorTester
    {
        public List<GameBoard> GeneratedBoards;
        public static Dictionary<string, string> Errors;
        public const int MaxDeadSpaces = 12;
        public const int MinNewBoardScore = 50;
        public int FailureCount { get; set; }

        public BoardGeneratorTester()
        {
            GeneratedBoards = new List<GameBoard>();
            Errors = new Dictionary<string, string>();
            FailureCount = 0;
        }

        public bool RunTests(Area TestArea, List<string> RecipeList, int NumberAttempts=25, int FailureTolerance=-1)
        {
            if (FailureTolerance<0) FailureTolerance = NumberAttempts / 10;
            FailureCount = 0;
            foreach(string r in RecipeList)
            {
                if (!RunTests(TestArea, r, NumberAttempts)) FailureCount++;
                if (FailureCount > FailureTolerance) return false;
            }
            return true;
        }

        public bool RunTests(Area TestArea, string BoardString, int NumberAttempts = 25)
        {
            for (int i = 0; i < NumberAttempts; i++)
            {
                try
                {
                    GameBoard GB = RunTest(TestArea, BoardString);
                    if (GB == null) return false;

                    GeneratedBoards.Add(GB);
                    if (!TestGeneratedBoard(GB)) return false;

                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return true;
        }

        public GameBoard RunTest(Area TestArea, string BoardString)
        {
            Player A = new Player(1, "A");
            A.Experience = new PlayerExperience();
            A.Experience.UnlockedAreas.Add(TestArea);

            Player B = new Player(2, "B");
            B.Experience = new PlayerExperience();
            B.Experience.UnlockedAreas.Add(TestArea);

            GameBoard board = BoardFactory.CreateGameBoard(TestArea, new GameOptions(), new BoardGenerationPreferences(), BoardString);
            return board;
        }

        public static bool TestGeneratedBoard(GameBoard Board)
        {
            if (Errors == null) Errors = new Dictionary<string, string>();
            foreach (BoardLocation l in Board.Corners)
                if (Board.ContentsAt(l).ContainsPiece
                    || !Board.ContentsAt(l).ContainsTokenType(TokenType.BLOCKER)
                    || Board.ContentsAt(l).TokenCount > 1)
                { Errors.Add(Board.ContentString, "Corners"); return false; }

            int Dead = AIScoreEvaluator.CountTheDead(Board);
            if (Dead > MaxDeadSpaces) { Errors.Add(Board.ContentString, "TooManyDead=" + Dead); return false; }

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


