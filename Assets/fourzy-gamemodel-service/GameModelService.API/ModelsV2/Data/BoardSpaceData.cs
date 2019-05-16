using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    [Serializable]
    public class BoardSpaceData
    {
        // Piece
        public string P { get; set; }

        // Tokens
        public List<string> T { get; set; }
    }
}
