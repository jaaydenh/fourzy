//@vadym udod

using Newtonsoft.Json;

namespace Fourzy._Updates.ClientModel
{
    [System.Serializable]
    public class RealtimeData
    {
        [JsonProperty("createdEpoch")]
        public long createdEpoch;
    }
}
