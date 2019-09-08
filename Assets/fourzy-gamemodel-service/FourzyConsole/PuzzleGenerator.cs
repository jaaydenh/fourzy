using FourzyGameModel.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FourzyConsole
{
    static class PuzzleGenerator
    {
        const string OutputFolder = @"E:\projects\FourzyPuzzles\";

        public static void FindPuzzles(Area TargetArea, int MaxMoves = 4, int MinMoves = 1, int MaxPieces = -1, int PuzzlesCount=100)
        {
            int found = 0;          
            DateTime Start = DateTime.Now;
            Dictionary<GameState, PuzzleTestResults> NewPuzzles = new Dictionary<GameState, PuzzleTestResults>();

            int Iterations = PuzzlesCount * 10;
            for (int g = 0; g < Iterations; g++)
            {
                int movecount = 0;
                Player player1 = new Player(1, "player 1");
                Player player2 = new Player(2, "player 2");
                FourzyGame Game = new FourzyGame(TargetArea, player1, player2);
                List<GameState> States = new List<GameState>();

                int pieces = 12;
                Game.State.ActivePlayerId = 1;
                while (Game.State.WinnerId < 0)
                {
                    movecount++;
                    SimpleAI AI = new SimpleAI(Game.State);
                    PlayerTurn Turn = AI.GetTurn();
                    States.Add(Game.TakeTurn(Turn).GameState);
                }

                if (Game.State.WinnerId == 1 && Game.State.Board.PieceCount <= MaxPieces)
                {
                    for (int v= MaxMoves; v>0; v--)
                    {
                        GameState GS = States[States.Count - (v * 2)];
                        PuzzleTestResults R = PuzzleTestTools.EvaluateState(GS, MaxMoves);
                        if (R.NumberOfVictoryPaths == 0 || R.NumberOfVictoryPaths > R.VictoryDepth)
                        {
                            continue;                            
                        }
                        
                        if (R.VictoryDepth <= MaxMoves && R.VictoryDepth >= MinMoves)
                        {
                            found++;
                            RecordPuzzle(found, TargetArea, GS, R);
                            break;
                        }
                    }

                    if (found >= PuzzlesCount) break;
                }

            }

        }

        public static void RecordPuzzle(int ID, Area TargetArea, GameState State, PuzzleTestResults Results)
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
            puzzle.ID = new Guid().ToString();
            puzzle.GameBoard = definition;
            //puzzle.InitialMoves = new List<PlayerTurn>();
            //puzzle.Instructions = TargetArea.ToString() + " Win In " + Results.VictoryDepth.ToString();
            puzzle.MoveLimit = Results.VictoryDepth;
            //puzzle.Name = new Guid().ToString();
            //puzzle.PackID = "RandomPuzzles";
            //puzzle.PackLevel = 0;
            //puzzle.PuzzlePlayer = new Player(1,Constants.GenerateName());
            puzzle.Complexity = PuzzleTestTools.PuzzleComplexity(State.Board, Results.VictoryDepth);

            string board_filename = OutputFolder + TargetArea.ToString() + "_Board_" + ID.ToString() + "_WinIn" + Results.VictoryDepth.ToString() + ".json";
            string puzzle_filename = OutputFolder + TargetArea.ToString() + "_Puzzle_" + ID.ToString() + "_WinIn" + Results.VictoryDepth.ToString() + ".json";

            string board_json = JsonConvert.SerializeObject(definition, Formatting.None);
            System.IO.File.WriteAllText(board_filename, board_json);

            string puzzle_json = JsonConvert.SerializeObject(puzzle, Formatting.None);
            System.IO.File.WriteAllText(puzzle_filename, puzzle_json);

        }
    }
}
