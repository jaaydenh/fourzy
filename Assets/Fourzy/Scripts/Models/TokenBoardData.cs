using System.Collections.Generic;

namespace Fourzy
{
    [System.Serializable]
    public class TokenBoardData
    {
        public string ID;
        public string Name;
        public bool Enabled;
        public bool EnabledGallery;
        public bool EnabledRealtime;
        public List<int> TokenData = new List<int>();
        public List<int> InitialGameBoard = new List<int>();
        public List<MoveInfo> InitialMoves;
    }
}