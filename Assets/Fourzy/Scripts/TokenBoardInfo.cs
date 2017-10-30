using System.Collections.Generic;

namespace Fourzy
{
    [System.Serializable]
    public class TokenBoardInfo
    {
        public string ID;
        public string Name;
        public bool Enabled  = false;
        public List<int> TokenData = new List<int>();
    }
}