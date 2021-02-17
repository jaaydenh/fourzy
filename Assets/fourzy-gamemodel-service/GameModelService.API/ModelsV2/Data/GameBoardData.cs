using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FourzyGameModel.Model
{
    [Serializable]
    public class GameBoardData
    {
        [JsonProperty("rows")]
        public int Rows { get; set; }

        [JsonProperty("columns")]
        public int Columns { get; set; }

        [JsonProperty("area")]
        public Area Area { get; set; }

        [JsonProperty("boardSpaceData")]
        public List<BoardSpaceData> BoardSpaceData = new List<BoardSpaceData>();
    }
}