using FourzyGameModel.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FourzyConsole
{
    static class PuzzleGenerator
    {
        const string OutputFolder = @"E:\projects\FourzyPuzzles\";

        public static void FindPuzzles(Area TargetArea, int MaxMoves = 4, int MinMoves = 1, int MaxPieces = 24, int PuzzlesCount = 100, List<BoardRecipe> Recipes = null, List<TokenType> AvailableTokens = null)
        {
            int found = 0;
            DateTime Start = DateTime.Now;
            Dictionary<GameState, PuzzleTestResults> NewPuzzles = new Dictionary<GameState, PuzzleTestResults>();
            BoardGenerator Generator = new BeginnerGardenRandomGenerator();

            if (AvailableTokens == null)
                AvailableTokens = new List<TokenType>() { TokenType.BLOCKER, TokenType.STICKY, TokenType.FRUIT, TokenType.ARROW };

            if (Recipes == null )
                Recipes = Generator.GetRecipes(AvailableTokens);
            
            int exception_count = 0;
            List<Exception> E_List = new List<Exception>();
            int Iterations = PuzzlesCount * 10;
            for (int g = 0; g < Iterations; g++)
            {
                try
                {
                    foreach (BoardRecipe recipe in Recipes)
                    {

                        int movecount = 0;
                        Player player1 = new Player(1, "player 1");
                        Player player2 = new Player(2, "player 2");

                        GameBoard Board = BoardFactory.CreateGameBoard(TargetArea, new GameOptions(), new BoardGenerationPreferences(), recipe.Name);

                        //GameBoard Board = BoardFactory.CreateRandomBoard(new GameOptions(), TargetArea, -1, new List<TokenType>() { }, recipe.Name);
                        FourzyGame Game = new FourzyGame(Board, player1, player2);

                        //FourzyGame Game = new FourzyGame(TargetArea, player1, player2);
                        List<GameState> States = new List<GameState>();

                        AIProfile Player1 = AIProfile.PuzzleAI;
                        AIProfile Player2 = AIProfile.PuzzleAI;

                        //int pieces = 12;
                        Game.State.ActivePlayerId = 1;
                        while (Game.State.WinnerId < 0)
                        {
                            AIPlayer AI = null;
                            if (movecount % 2 == 0) AI = AIPlayerFactory.Create(Game.State, Player1);
                            else AI = AIPlayerFactory.Create(Game.State, Player2);

                            movecount++;
                            PlayerTurn Turn = AI.GetTurn();
                            States.Add(Game.TakeTurn(Turn).GameState);
                        }


                        if (Game.State.WinnerId > 0 && Game.State.Board.PieceCount <= MaxPieces)
                        {
                            //for (int v= MaxMoves; v>0; v--)
                            //{


                            int v = MaxMoves;
                            GameState GS = States[States.Count - (v * 2)];

                            if (Game.State.WinnerId != 1)
                            {
                                GS.Board.SwapPieces();
                                GS.ActivePlayerId = GS.Opponent(GS.ActivePlayerId);
                            }

                            PuzzleTestResults R = PuzzleTestTools.EvaluateState(GS, MaxMoves);
                            if (R.NumberOfVictoryPaths == 0 || R.NumberOfVictoryPaths > R.VictoryDepth)
                            {
                                continue;
                            }

                            bool Tested = TestSolution(GS, R.Solution);

                            if (!Tested)
                            {
                                continue;
                            }


                            if (R.VictoryDepth <= MaxMoves && R.VictoryDepth >= MinMoves)
                            {
                                found++;

                                RecordPuzzle(found, TargetArea, GS, R);
                                //break;
                            }
                            //}
                        }
                    }

                }
                catch (Exception ex)
                {
                    exception_count++;
                    E_List.Add(ex);
                }

                if (found >= PuzzlesCount) break;


            }

        }

        public static void FindPuzzleInBoard(GameBoardDefinition Board, int MaxMoves = 4, int MinMoves = 1, int MaxPieces = 24, int PuzzlesCount = 1, string name="")
        {
            int found = 0;
            DateTime Start = DateTime.Now;

            int exception_count = 0;
            List<Exception> E_List = new List<Exception>();
            int Iterations = PuzzlesCount * 10;
            for (int g = 0; g < Iterations; g++)
            {
                try
                {

                    int movecount = 0;
                    Player player1 = new Player(1, "player 1");
                    Player player2 = new Player(2, "player 2");

                    FourzyGame Game = new FourzyGame(Board, player1, player2);
                    List<GameState> States = new List<GameState>();

                    AIProfile Player1 = AIProfile.SimpleAI;
                    AIProfile Player2 = AIProfile.PuzzleAI;

                    //int pieces = 12;
                    Game.State.ActivePlayerId = 1;
                    while (Game.State.WinnerId < 0)
                    {
                        AIPlayer AI = null;
                        if (movecount % 2 == 0) AI = AIPlayerFactory.Create(Game.State, Player1);
                        else AI = AIPlayerFactory.Create(Game.State, Player2);

                        movecount++;
                        PlayerTurn Turn = AI.GetTurn();
                        States.Add(Game.TakeTurn(Turn).GameState);
                    }


                    if (Game.State.WinnerId > 0 && (Game.State.Board.PieceCount <= MaxPieces || MaxPieces <1))
                    {
                        int v = MaxMoves;
                        GameState GS = States[States.Count - (v * 2)];

                        if (Game.State.WinnerId != 1)
                        {
                            GS.Board.SwapPieces();
                            GS.ActivePlayerId = GS.Opponent(GS.ActivePlayerId);
                        }

                        PuzzleTestResults R = PuzzleTestTools.EvaluateState(GS, MaxMoves);
                        if (R.NumberOfVictoryPaths == 0 || R.NumberOfVictoryPaths > R.VictoryDepth *2+1)
                        {
                            continue;
                        }

                        bool Tested = TestSolution(GS, R.Solution);

                        if (!Tested)
                        {
                            continue;
                        }

                        if (R.VictoryDepth <= MaxMoves && R.VictoryDepth >= MinMoves)
                        {
                            found++;

                            RecordPuzzle(found, Area.TRAINING_GARDEN, GS, R, name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("Error: " + ex.Message);
                    exception_count++;
                    E_List.Add(ex);
                }

                if (found >= PuzzlesCount) break;
            }
        }

        public static bool ConvertBoardToPuzzle(GameBoardDefinition Board, int MaxMoves = 4, int MinMoves = 1, int MaxPieces = -1, int PuzzlesCount = 1, string Name="")
        {

            GameState GS = new GameState(Board);
            GS.Players.Add(1, new Player(1, "Player1"));
            GS.Players.Add(2, new Player(2, "Player2"));


            PuzzleTestResults R = PuzzleTestTools.EvaluateState(GS, MaxMoves);
            if (R.NumberOfVictoryPaths == 0)
            {
                return false;
            }

//            if (R.NumberOfVictoryPaths > R.VictoryDepth) return false;


            bool Tested = TestSolution(GS, R.Solution);

            if (!Tested)
            {
                return false;
            }

            if (R.VictoryDepth <= MaxMoves && R.VictoryDepth >= MinMoves)
            {
                RecordPuzzle(1, Area.TRAINING_GARDEN, GS, R, Name);
                return true;
            }

            return false;
        }
         public static FourzyPuzzleData GeneratePuzzle(Area TargetArea, int MinMoves, int MaxMoves, int MaxPieces=24)
        {
                bool found = false;
                int attempts = 0;
                Dictionary<GameState, PuzzleTestResults> NewPuzzles = new Dictionary<GameState, PuzzleTestResults>();

                while (!found || attempts++ <= 100)
                {

                    int movecount = 0;
                    Player player1 = new Player(1, "player 1");
                    Player player2 = new Player(2, "player 2");
                    FourzyGame Game = new FourzyGame(TargetArea, player1, player2);
                    List<GameState> States = new List<GameState>();

                    Game.State.ActivePlayerId = 1;
                    while (Game.State.WinnerId < 0)
                    {
                        movecount++;
                        SimpleAI AI = new SimpleAI(Game.State);
                        PlayerTurn Turn = AI.GetTurn();
                        States.Add(Game.TakeTurn(Turn).GameState);
                    }

                    if (Game.State.WinnerId == 1 && (Game.State.Board.PieceCount <= MaxPieces || MaxPieces <0) )
                    {
                        for (int v = MaxMoves; v > 0; v--)
                        {
                            GameState GS = States[States.Count - (v * 2)];
                            PuzzleTestResults R = PuzzleTestTools.EvaluateState(GS, MaxMoves);
                            if (R.NumberOfVictoryPaths == 0 || R.NumberOfVictoryPaths > R.VictoryDepth)
                            {
                                continue;
                            }

                            if (R.VictoryDepth <= MaxMoves && R.VictoryDepth >= MinMoves)
                            {
                                found = true;
                                return GeneratePuzzleData(TargetArea, GS, R);
                            }
                        }

                    }
                }
            return null; 
        }

        public static FourzyPuzzleData GeneratePuzzleMoveData(FourzyPuzzleData PuzzleData)
        {
            GameState GSEval = new GameState(PuzzleData.GameBoard, PuzzleData.FirstTurn);

            Dictionary<int, List<GameState>> StateBucket = new Dictionary<int, List<GameState>>();
            Dictionary<string, PuzzleTurnTransition> TurnBucket = new Dictionary<string, PuzzleTurnTransition>();

            TurnEvaluator TE = new TurnEvaluator(GSEval);
            StateBucket[0] = new List<GameState>();
            StateBucket[0].Add(GSEval);

            for (int d = 1; d <= PuzzleData.MoveLimit; d++)
            {
                StateBucket[d] = new List<GameState>();

                int evaluated_states = 0;
                foreach (GameState GS in StateBucket[d-1])
                {
                    //What moves will the player make
                    TE = new TurnEvaluator(GS);
                    foreach (SimpleMove m in TE.GetAvailableSimpleMoves())
                    {

                        //For each move, evaluate the move
                        GameState GSTest = TE.EvaluateTurn(m);
                        
                        //If either player wins, don't worry about it.  AI will handle
                        if (GSTest.WinnerId < 0)
                        {
                            //Get the AI to make the best move.
                            AIPlayer AI = new PuzzleAI(GSTest);
                            PlayerTurn AITurn = AI.GetTurn();
                            TurnEvaluator AITE = new TurnEvaluator(GSTest);
                            GameState GSAI = AITE.EvaluateTurn(AITurn);

                            //If there is not a winning move, then record the game state
                            if (GSAI.WinnerId < 0)
                                {
                                    StateBucket[d].Add(GSAI);
                                    if (!TurnBucket.ContainsKey(GSAI.UniqueId))
                                        TurnBucket.Add(GSAI.UniqueId, new PuzzleTurnTransition(GS, new PlayerTurn(m), GSAI));
                                }
                            }

                        }
                    }

                    evaluated_states++;
                }

            PuzzleData.AILibrary = new Dictionary<string, string>();
            foreach (string gsid in TurnBucket.Keys)
            {
                //PuzzleData.AILibrary.Add(gsid, new PuzzleTurnInstructions(TurnBucket[gsid].Turn));
                PuzzleData.AILibrary.Add(gsid, TurnBucket[gsid].Turn.Notation);
            }

            return PuzzleData;
        }



        //returns a serializable puzzle data object.
        public static FourzyPuzzleData GeneratePuzzleData(Area TargetArea, GameState State, PuzzleTestResults Results)
{
        GameBoardData board = State.Board.SerializeData();
        GameBoardDefinition definition = new GameBoardDefinition();
        definition.Area = TargetArea;
        definition.BoardName = TargetArea.ToString() + " Win In " + Results.VictoryDepth.ToString();
        definition.EnabledRealtime = false;
        definition.Rows = board.Rows;
        definition.Columns = board.Columns;
        definition.ID = State.UniqueId;
        definition.Enabled = true;
        definition.BoardSpaceData = board.BoardSpaceData;
        definition.InitialMoves = new List<SimpleMove>();
        FourzyPuzzleData puzzle = new FourzyPuzzleData();
        puzzle.SolutionStateData = Results.VictoryStates[0].Board.SerializeData();
        foreach (PlayerTurn t in Results.Solution)
            puzzle.Solution.Add(t);
        puzzle.Enabled = true;
        puzzle.GoalType = PuzzleGoalType.WIN;
        puzzle.ID = Guid.NewGuid().ToString();
        puzzle.GameBoard = definition;
        puzzle.MoveLimit = Results.VictoryDepth;
        puzzle.Complexity = PuzzleTestTools.PuzzleComplexity(State.Board, Results.VictoryDepth);
            if (puzzle.AvailableSpells == null) puzzle.AvailableSpells = new List<SpellId>();
            if (puzzle.AvailableSpells.Count == 0) puzzle.StartingMagic = 0;
            else puzzle.StartingMagic = 100;

            return puzzle;
    }

        public static void RecordPuzzle(int ID, Area TargetArea, GameState State, PuzzleTestResults Results, string Name="")
        {
            GameBoardData board = State.Board.SerializeData();
            GameBoardDefinition definition = new GameBoardDefinition();
            definition.Area = TargetArea;
            definition.BoardName = TargetArea.ToString() + " Win In " + Results.VictoryDepth.ToString();
            definition.EnabledRealtime = false;
            definition.Rows = board.Rows;
            definition.Columns = board.Columns;
            definition.ID = State.UniqueId;
            definition.Enabled = true;
            definition.BoardSpaceData = board.BoardSpaceData;
            definition.InitialMoves = new List<SimpleMove>();

            FourzyPuzzleData puzzle = new FourzyPuzzleData();
            puzzle.SolutionStateData = Results.VictoryStates[0].Board.SerializeData();
            foreach (PlayerTurn t in Results.Solution)
                puzzle.Solution.Add(t);
            puzzle.Enabled = true;
            puzzle.GoalType = PuzzleGoalType.WIN;
            puzzle.ID = Guid.NewGuid().ToString();
            puzzle.GameBoard = definition;
            puzzle.MoveLimit = Results.VictoryDepth;
            //puzzle.PuzzlePlayer = new Player(1,Constants.GenerateName());
            puzzle.Complexity = PuzzleTestTools.PuzzleComplexity(State.Board, Results.VictoryDepth);
            if (puzzle.AvailableSpells == null) puzzle.AvailableSpells = new List<SpellId>();
            if (puzzle.AvailableSpells.Count == 0) puzzle.StartingMagic = 0;
            else puzzle.StartingMagic = 100;


            string puzzle_filename = "";

            if (Name == "") puzzle_filename = OutputFolder + TargetArea.ToString() + "_Puzzle_" + ID.ToString() + "_WinIn" + Results.VictoryDepth.ToString() + ".json";
            else puzzle_filename = OutputFolder + System.IO.Path.GetFileNameWithoutExtension(Name) + ".json"; 
                    
            //string puzzle_filename = OutputFolder + TargetArea.ToString() + "_Puzzle_" + ID.ToString() + "_WinIn" + Results.VictoryDepth.ToString() + ".json";

            if (System.IO.File.Exists(puzzle_filename))
            {
               // bool good_name = false;

                if (Name == "") puzzle_filename = OutputFolder + TargetArea.ToString() + "_Puzzle_" + (++ID).ToString() + "_WinIn" + Results.VictoryDepth.ToString() + ".json";
                else puzzle_filename = OutputFolder + System.IO.Path.GetFileNameWithoutExtension(Name) + "_" + (++ID).ToString() +  ".json";
                
                //while (!good_name)
                //{
                //    puzzle_filename = OutputFolder + TargetArea.ToString() + "_Puzzle_" + (++ID).ToString() + "_WinIn" + Results.VictoryDepth.ToString() + ".json";
                //    if (!System.IO.File.Exists(puzzle_filename)) good_name = true;
                //}
            }

            //string board_json = JsonConvert.SerializeObject(definition, Formatting.None);
            //System.IO.File.WriteAllText(board_filename, board_json);

            string puzzle_json = JsonConvert.SerializeObject(puzzle, Formatting.None);
            System.IO.File.WriteAllText(puzzle_filename, puzzle_json);

        }

        public static FourzyPuzzleData FixPuzzle(FourzyPuzzleData PuzzleData)
        {
            if (PuzzleData.ID == null) PuzzleData.ID = Guid.NewGuid().ToString();
            if (PuzzleData.AvailableSpells == null) PuzzleData.AvailableSpells = new List<SpellId>();
            if (PuzzleData.AvailableSpells.Count == 0) PuzzleData.StartingMagic = 0;
            else PuzzleData.StartingMagic = 100;

            GameState GS = new GameState(PuzzleData.GameBoard, PuzzleData.FirstTurn);
            if (!TestSolution(GS, PuzzleData.Solution))
            {
                PuzzleData = GeneratePuzzle(PuzzleData.GameBoard.Area, PuzzleData.Solution.Count, PuzzleData.Solution.Count);
            }

            return PuzzleData;
        }

        public static bool TestSolution(GameState State, List<PlayerTurn> Solution, int PlayerId = 1)
        {
            GameState GS = new GameState(State);
            if (GS.Players.Count == 0)
            {
                GS.Players.Add(1, new Player(1, "ONE"));
                GS.Players.Add(2, new Player(2, "TWO"));
            }

            GS.ActivePlayerId = PlayerId;
            for(int i=0;i<Solution.Count;i++)
            {
                PlayerTurn t = Solution[i];
                //PlayerTurn t = Solution[Solution.Count -i -1];

                TurnEvaluator TE = new TurnEvaluator(GS);
                GS = TE.EvaluateTurn(t);

                //if wrong player or draw.
                if (GS.WinnerId > 0 && GS.WinnerId != PlayerId) return false;

                //Don't make AI move on last player turn.
                if (i == Solution.Count - 1) break;

                if (GS.WinnerId < 0)
                {
                    AIPlayer AI = AIPlayerFactory.Create(GS, AIProfile.PuzzleAI);
                    PlayerTurn Turn = AI.GetTurn();

                    TE = new TurnEvaluator(GS);
                    GS = TE.EvaluateTurn(Turn);

                    //if wrong player or draw.
                    if (GS.WinnerId > 0 && GS.WinnerId != PlayerId) return false;
                }

            }

            //If not winner, or Wrong player/draw
            if (GS.WinnerId < 0 || GS.WinnerId != PlayerId) return false;

            return true;
        }

        public static bool TestPuzzle(FourzyPuzzleData PuzzleData)
        {
            GameState GS = new GameState(PuzzleData.GameBoard, PuzzleData.FirstTurn);
            return TestSolution(GS, PuzzleData.Solution);
        }


    }
}
