using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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