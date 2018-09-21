using System.Collections.Generic;

namespace Fourzy
{
    [System.Serializable]
    public class TokenData
    {
        public int ID;
        public string Name;
        public string Arena;
        public string Description;
        public bool Enabled;
        public bool showBackgroundTile;
        public List<int> InGameTokenTypes;
        public string GameBoardInstructionID;
    }
}