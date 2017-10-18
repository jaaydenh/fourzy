using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy
{
    [System.Serializable]
    public class PuzzleMoveInfo
    {
        public List<int> BoardState = new List<int>();
        public int Location;
        public int Direction;
    }
}