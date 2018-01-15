using System.Collections.Generic;

namespace Fourzy
{
    [System.Serializable]
    public class TokenBoardData
    {
        public string ID;
        public string Name;
        public bool Enabled;
        public List<int> TokenData = new List<int>();
    }
}