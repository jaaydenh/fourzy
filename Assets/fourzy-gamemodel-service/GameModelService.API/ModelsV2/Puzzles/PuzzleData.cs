using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class PuzzleData
    {
        // Unique Identifier for the Puzzle
        public string ID { get; set; }

        // Human readable name to display to the player
        public string Name { get; set; }

        // Puzzle packs are sorted in the Puzzle screen using PackID in ascending order.
        // Multiple puzzles can belong to the same pack
        public string PackID { get; set; }

        // This is the order the puzzle is presented to the Player within the pack
        public int PackLevel { get; set; }

        public bool Enabled { get; set; }

        public PuzzleGoalType GoalType { get; set; }

        public int MoveLimit { get; set; }

        public Player PuzzlePlayer { get; set; }

        public string Instructions { get; set; } //optional

        public GameBoardData InitialGameBoard { get; set; }

        public List<PlayerTurn> InitialMoves = new List<PlayerTurn>();

        //Direction on how to move
        //A Library on a gamestatestring as a key
        // and instructions for Puzzle AI on how to move.
        public Dictionary<string, PlayerTurn> AILibrary { get; set; }

        public List<PuzzleHint> AvailableHints = new List<PuzzleHint>();
    }
}
