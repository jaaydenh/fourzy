using FourzyGameModel.Model;
using Newtonsoft.Json;
using System;
using System.IO;

namespace FourzyConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //PuzzleGenerator.FindPuzzles(Area.ENCHANTED_FOREST, 3, 2, 24, 1);
            //PuzzleGenerator.FindPuzzles(Area.SANDY_ISLAND, 3, 2, 24, 1);
            //PuzzleGenerator.FindPuzzles(Area.TRAINING_GARDEN, 3, 2, 24, 1);
            //PuzzleGenerator.FindPuzzles(Area.ICE_PALACE, 3, 2, 24, 1);

            foreach (string f in System.IO.Directory.GetFiles(@"E:\projects\FourzyPuzzles"))
            {
                string puzzle_json = File.ReadAllText(f);
                FourzyPuzzleData puzzle = JsonConvert.DeserializeObject<FourzyPuzzleData>(puzzle_json);
                puzzle.ID = Guid.NewGuid().ToString();
                puzzle_json = JsonConvert.SerializeObject(puzzle, Formatting.None);
                System.IO.File.WriteAllText(f, puzzle_json);
            }

        }
    }
}
