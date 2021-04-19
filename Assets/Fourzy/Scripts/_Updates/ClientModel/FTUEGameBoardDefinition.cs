//@vadym udod

using FourzyGameModel.Model;
using Newtonsoft.Json;

namespace Fourzy._Updates.ClientModel
{
    public class FTUEGameBoardDefinition : GameBoardDefinition
    {
        [JsonProperty("aiProfile")]
        public int aiProfile = -1;

        public AIProfile AIProfile => aiProfile <= 0 ? AIProfile.Player : (AIProfile)aiProfile;
    }
}