//@vadym udod

using Newtonsoft.Json;

namespace Fourzy._Updates.ClientModel
{
    public class AreaProgression
    {
        public AreaProgressionEntry[] progression;
        [JsonIgnore]
        public string jsonString;

        public static AreaProgression FromJsonString(string jsonString)
        {
            AreaProgression areaProgression = JsonConvert.DeserializeObject<AreaProgression>(jsonString);

            if (areaProgression != null)
            {
                areaProgression.jsonString = jsonString;
            }

            return areaProgression;
        }
    }
}