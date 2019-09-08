using FourzyGameModel.Model;
using System;

namespace FourzyConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //PuzzleGenerator.FindPuzzles(Area.ICE_PALACE, 3, 2, 24, 100);
            PuzzleGenerator.FindPuzzles(Area.ENCHANTED_FOREST, 3, 2, 24, 100);
            PuzzleGenerator.FindPuzzles(Area.SANDY_ISLAND, 3, 2, 24, 100);
            PuzzleGenerator.FindPuzzles(Area.TRAINING_GARDEN, 3, 2, 24, 100);
        }
    }
}
