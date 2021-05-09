//@vadym udod

using Newtonsoft.Json;
using System.Linq;

namespace Fourzy._Updates.ClientModel
{
    public class AreaProgression
    {
        public AreaProgressionEntry[] progression;
        [JsonIgnore]
        public string jsonString;

        public AreaProgressionEntry GetNext(int games)
        {
            return progression.FirstOrDefault(_entry => _entry.gamesNumber > games);
        }

        public AreaProgressionEntry GetCurrent(int games)
        {
            return progression.LastOrDefault(_entry => _entry.gamesNumber <= games);
        }

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