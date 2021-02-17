using FourzyGameModel.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FourzyConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> PuzzleIds = new List<string>();
            List<string> Dups = new List<string>();
            List<string> Failed = new List<string>();

 //           string FilePath = @"E:\projects\FourzyNov2019\fourzy\Assets\Fourzy\Resources\PuzzlePacks";
            string FilePath = @"E:\projects\FourzyDec2019\fourzy\Assets\Fourzy\Resources\PuzzlePacks\Event15_Tempo\puzzles";
            string Operation = "CompilePuzzle";

            switch (Operation)
            {

                case "PuzzleTests":
                    foreach (string f in System.IO.Directory.GetFiles(FilePath, "*.json", SearchOption.AllDirectories))
                    {
                        Console.WriteLine("evaluating f=" + f);
                        if (f.Contains("puzzlePack")) continue;
                        string puzzle_json = File.ReadAllText(f);
                        FourzyPuzzleData puzzle = JsonConvert.DeserializeObject<FourzyPuzzleData>(puzzle_json);
                        if (PuzzleIds.Contains(puzzle.ID)) {
                            Dups.Add(f);
                            Console.WriteLine("Dup in Puzzle:" + f);
                            puzzle.ID = Guid.NewGuid().ToString();
                        }
                        else PuzzleIds.Add(puzzle.ID);

                        if (puzzle.Solution != null)
                        if (!PuzzleGenerator.TestPuzzle(puzzle))
                        {
                            Failed.Add(f);
                            Console.WriteLine("Solution Failed:" + f);
                        }
                        puzzle_json = JsonConvert.SerializeObject(puzzle, Formatting.None);
                        System.IO.File.WriteAllText(f, puzzle_json);
                    }
                    break;

                case "PuzzleGenGarden":
                    Console.WriteLine("Find Garden Puzzles.");
                    PuzzleGenerator.FindPuzzles(Area.TRAINING_GARDEN, 3, 3, 30, 25, null, new List<TokenType>() { TokenType.BLOCKER, TokenType.ARROW });

                    //PuzzleGenerator.FindPuzzles(Area.TRAINING_GARDEN, 3, 3, 24, 25);
                    //PuzzleGenerator.FindPuzzles(Area.TRAINING_GARDEN, 3, 2, 24, 10, null, new List<TokenType>() { TokenType.BLOCKER });
                    //PuzzleGenerator.FindPuzzles(Area.TRAINING_GARDEN, 3, 2, 24, 10, null, new List<TokenType>() { TokenType.ARROW });
                    //PuzzleGenerator.FindPuzzles(Area.TRAINING_GARDEN, 3, 2, 24, 10, null, new List<TokenType>() { TokenType.BLOCKER, TokenType.ARROW });
                    //PuzzleGenerator.FindPuzzles(Area.TRAINING_GARDEN, 2, 1, 24, 50, null, new List<TokenType>() { TokenType.STICKY, TokenType.BLOCKER });
                    //PuzzleGenerator.FindPuzzles(Area.TRAINING_GARDEN, 3, 1, 24, 50, null, new List<TokenType>() { TokenType.STICKY, TokenType.BLOCKER });
                    //PuzzleGenerator.FindPuzzles(Area.TRAINING_GARDEN, 2, 1, 24, 50, null, new List<TokenType>() { TokenType.STICKY, TokenType.ARROW, TokenType.BLOCKER });
                    //PuzzleGenerator.FindPuzzles(Area.TRAINING_GARDEN, 3, 1, 24, 50, null, new List<TokenType>() { TokenType.FRUIT, TokenType.ARROW, TokenType.BLOCKER });
                    //PuzzleGenerator.FindPuzzles(Area.TRAINING_GARDEN, 3, 1, 24, 50, null, new List<TokenType>() { TokenType.ARROW, TokenType.BLOCKER });

                    break;

                case "CompilePuzzle":
                    Console.WriteLine("CompileAIMoves.");
                    foreach (string f in System.IO.Directory.GetFiles(FilePath, "*.json", SearchOption.AllDirectories))
                    {
                        Console.WriteLine("evaluating f=" + f);
                        if (f.Contains("puzzlePack")) continue;
                        string puzzle_json = File.ReadAllText(f);
                        FourzyPuzzleData puzzle = JsonConvert.DeserializeObject<FourzyPuzzleData>(puzzle_json);
                        puzzle = PuzzleGenerator.GeneratePuzzleMoveData(puzzle);

                        puzzle_json = JsonConvert.SerializeObject(puzzle, Formatting.None);
                        System.IO.File.WriteAllText(f, puzzle_json);
                    }
                    break;

            }
                                 

            int MinMoves = 1;
            int MaxMoves = 3;
            //Test Solutions:
            //foreach (string f in System.IO.Directory.GetFiles(@"E:\projects\FourzyPuzzles", "*.json"))

            //foreach (string f in System.IO.Directory.GetFiles(@"E:\projects\FourzyPuzzles\SolveThese", "*.json"))
            //{
            //    Console.WriteLine("evaluating: " + f);
            //    string board_json = File.ReadAllText(f);
            //    GameBoardDefinition Board = JsonConvert.DeserializeObject<GameBoardDefinition>(board_json);
            //    bool success = PuzzleGenerator.ConvertBoardToPuzzle(Board);
            //    Console.WriteLine(f + ":" + success.ToString());
            //}

            //foreach (string f in System.IO.Directory.GetFiles(@"E:\projects\FourzyPuzzles\SolveThese", "*.json", SearchOption.AllDirectories))
            //{
            //    Console.WriteLine("evaluating: " + f);
            //    string board_json = File.ReadAllText(f);
            //    GameBoardDefinition Board = JsonConvert.DeserializeObject<GameBoardDefinition>(board_json);
            //    PuzzleGenerator.FindPuzzleInBoard(Board, 3, 1, -1, 1, f);
            //    Console.WriteLine(f + " Complete.");
            //}

            //foreach (string f in System.IO.Directory.GetFiles(@"E:\projects\FourzyPuzzles\PuzzlePlayBoards", "*.json", SearchOption.AllDirectories))
            //{
            //    Console.WriteLine("evaluating: " + f);
            //    string board_json = File.ReadAllText(f);
            //    GameBoardDefinition Board = JsonConvert.DeserializeObject<GameBoardDefinition>(board_json);
            //    bool success = PuzzleGenerator.ConvertBoardToPuzzle(Board, 4, 1, -1, 1, f);
            //    Console.WriteLine(f + ":" + success.ToString());
            //}
                       


            //foreach (string f in System.IO.Directory.GetFiles(@"E:\projects\FourzyPuzzles\SolveThese", "*.json"))
            //    {
            //        string puzzle_json = File.ReadAllText(f);
            //        FourzyPuzzleData puzzle = JsonConvert.DeserializeObject<FourzyPuzzleData>(puzzle_json);
            //        GameState GS = new GameState(puzzle.GameBoard, puzzle.FirstTurn);
            //        if (PuzzleIds.Contains(puzzle.ID)) { Dups.Add(puzzle.ID + " in " + f); }
            //        else PuzzleIds.Add(puzzle.ID);

            //        bool solution = PuzzleGenerator.TestSolution(GS, puzzle.Solution);

            //        if (!solution)
            //        {
            //            puzzle.Solution.Reverse();
            //            solution = PuzzleGenerator.TestSolution(GS, puzzle.Solution);
            //            if (solution)
            //            {
            //                Console.WriteLine("ReversedSolution.");
            //                puzzle_json = JsonConvert.SerializeObject(puzzle, Formatting.None);
            //                System.IO.File.WriteAllText(f, puzzle_json);
            //            }
            //        }

            //        bool TryToFix = false;
            //        if (!solution && TryToFix)
            //        {
            //            Console.WriteLine("Resolving...");
            //            PuzzleTestResults R = PuzzleTestTools.EvaluateState(GS, MaxMoves);
            //            Console.WriteLine("Solved: Depth=" + R.VictoryDepth + " Paths=" + R.NumberOfVictoryPaths);
            //            if (R.NumberOfVictoryPaths > 0 && R.NumberOfVictoryPaths <= R.VictoryDepth)
            //            {
            //                if (R.VictoryDepth <= MaxMoves && R.VictoryDepth >= MinMoves)
            //                {
            //                    puzzle.Solution.Clear();
            //                    foreach (PlayerTurn t in R.Solution)
            //                    {
            //                        puzzle.Solution.Add(t);
            //                    }
            //                    puzzle.MoveLimit = R.VictoryDepth;
            //                    puzzle.SolutionStateData = R.VictoryStates[0].Board.SerializeData();

            //                    solution = PuzzleGenerator.TestSolution(GS, puzzle.Solution);

            //                    if (solution)
            //                    {
            //                        Console.WriteLine("Resolved Puzzle.");
            //                        puzzle_json = JsonConvert.SerializeObject(puzzle, Formatting.None);
            //                        System.IO.File.WriteAllText(f, puzzle_json);
            //                    }

            //                }
            //            }

            //        }

            //        bool DeleteBroken = true;
            //        Console.WriteLine(f + ":" + solution.ToString());
            //        if (!solution && DeleteBroken)
            //        {
            //            Console.WriteLine("Deleting broken puzzle:" + f);
            //            File.Delete(f);
            //        }

            //        if (Dups.Count > 0)
            //        {
            //            Console.WriteLine("Found Dups " + Dups.Count.ToString());
            //            foreach (string s in Dups)
            //            {
            //                Console.WriteLine(s);
            //            }
            //        }

            //    }

            //Create puzzles
            //PuzzleGenerator.FindPuzzles(Area.SANDY_ISLAND, 2, 1, 24, 50);
            //PuzzleGenerator.FindPuzzles(Area.ICE_PALACE, 2, 1, 24, 50);
            //PuzzleGenerator.FindPuzzles(Area.TRAINING_GARDEN, 2, 1, 24, 50);
            //PuzzleGenerator.FindPuzzles(Area.ENCHANTED_FOREST, 2, 1, 24, 50);
            //Console.WriteLine("Ice Puzzles.");
            //PuzzleGenerator.FindPuzzles(Area.ICE_PALACE, 2, 1, 24, 10);
            //Console.WriteLine("Ice Puzzles.");
            //PuzzleGenerator.FindPuzzles(Area.ICE_PALACE, 2, 1, 24, 10);
            //Console.WriteLine("Ice Puzzles.");
            //PuzzleGenerator.FindPuzzles(Area.ICE_PALACE, 2, 1, 24, 10);
            //Console.WriteLine("Ice Puzzles.");
            //PuzzleGenerator.FindPuzzles(Area.ICE_PALACE, 2, 1, 24, 10);



            //for (int i = 0; i < 10; i++)
            //{
            //    //Console.WriteLine("Ice Puzzles.");
            //    //PuzzleGenerator.FindPuzzles(Area.ICE_PALACE, 2, 1, 24, 10);

            //    //Console.WriteLine("Sandy Puzzles.");
            //    //PuzzleGenerator.FindPuzzles(Area.SANDY_ISLAND, 2, 1, 24, 10);

            //    Console.WriteLine("Garden Puzzles.");
            //    PuzzleGenerator.FindPuzzles(Area.TRAINING_GARDEN, 2, 1, 24, 10);

            //    //Console.WriteLine("Forest Puzzles.");
            //    //PuzzleGenerator.FindPuzzles(Area.ENCHANTED_FOREST, 2, 1, 24, 10);
            //}

            //Add new Guid
            //foreach (string f in System.IO.Directory.GetFiles(@"E:\projects\FourzyPuzzles"))
            //{
            //    string puzzle_json = File.ReadAllText(f);
            //    FourzyPuzzleData puzzle = JsonConvert.DeserializeObject<FourzyPuzzleData>(puzzle_json);
            //    puzzle.ID = Guid.NewGuid().ToString();
            //    puzzle_json = JsonConvert.SerializeObject(puzzle, Formatting.None);
            //    System.IO.File.WriteAllText(f, puzzle_json);
            //}

            //foreach (string f in System.IO.Directory.GetFiles(@"E:\projects\FourzyPuzzles"))
            //{
            //    string puzzle_json = File.ReadAllText(f);
            //    FourzyPuzzleData puzzle = JsonConvert.DeserializeObject<FourzyPuzzleData>(puzzle_json);
            //    puzzle = PuzzleGenerator.FixPuzzle(puzzle);
            //    puzzle_json = JsonConvert.SerializeObject(puzzle, Formatting.None);
            //    System.IO.File.WriteAllText(f, puzzle_json);
            //}
            Console.WriteLine("Complete.");
            Console.Read();

        }
    }
}
