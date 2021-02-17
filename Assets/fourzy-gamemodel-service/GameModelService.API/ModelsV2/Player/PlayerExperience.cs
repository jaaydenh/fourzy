using System.Collections.Generic;
using Newtonsoft.Json;

namespace FourzyGameModel.Model
{
    public class PlayerExperience
    {
        [JsonProperty("allowedTokens")]
        public List<TokenType> AllowedTokens { get; set; }

        [JsonProperty("unlockedAreas")]
        public List<Area> UnlockedAreas{ get; set; }

        public PlayerExperience()
        {
            AllowedTokens = new List<TokenType>();
            UnlockedAreas = new List<Area>();
        }
    }
}
