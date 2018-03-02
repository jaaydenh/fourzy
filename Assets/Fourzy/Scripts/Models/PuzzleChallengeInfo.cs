namespace Fourzy
{
    using System.Collections.Generic;

    [System.Serializable]
    public class PuzzleChallengeInfo
    {
        public string ID;
        public string Name;
        public int Level;
        public bool Enabled  = false;
        public List<int> InitialGameBoard = new List<int>();
        public int MoveGoal;
        public int FirstMove;
        public List<int> InitialTokenBoard = new List<int>();
        public List<PuzzleRandomMoveInfo> RandomMoves = new List<PuzzleRandomMoveInfo>();
        public List<PuzzleMoveInfo> Moves = new List<PuzzleMoveInfo>();
    }
}