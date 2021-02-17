using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FourzyGameModel.Model
{
    [Serializable]
    public class GameBoardDefinition
    {
        [JsonProperty("rows")]
        public int Rows { get; set; }

        [JsonProperty("columns")]
        public int Columns { get; set; }

        [JsonProperty("area")]
        public Area Area { get; set; }

        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("boardName")]
        public string BoardName { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("enabledGallery")]
        public bool EnabledGallery { get; set; }

        [JsonProperty("enabledRealtime")]
        public bool EnabledRealtime { get; set; }

        // public GameBoardData definition { get; set; }

        [JsonProperty("boardSpaceData")]
        public List<BoardSpaceData> BoardSpaceData = new List<BoardSpaceData>();

        [JsonProperty("initialMoves")]
        public List<SimpleMove> InitialMoves = new List<SimpleMove>();
    }
}