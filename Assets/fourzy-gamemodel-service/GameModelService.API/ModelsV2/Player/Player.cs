using Newtonsoft.Json;

namespace FourzyGameModel.Model
{
    public class Player
    {
        [JsonProperty("playerId")]
        public int PlayerId { get; }

        [JsonProperty("playerString")]
        public string PlayerString { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; }

        [JsonProperty("herdId")]
        public string HerdId { get; set; }

        [JsonProperty("magic")]
        public int Magic { get; set; }

        [JsonProperty("selectedArea")]
        public Area SelectedArea { get; set; }

        [JsonProperty("experience")]
        public PlayerExperience Experience { get; set; }

        [JsonConstructor]
        public Player(int PlayerId, string DisplayName)
        {
            this.PlayerId = PlayerId;
            this.DisplayName = DisplayName;
            this.Magic = Constants.PlayerStartingMagic;
            this.SelectedArea = Area.NONE;
            this.Experience = new PlayerExperience();
            this.Experience.UnlockedAreas.Add(Area.TRAINING_GARDEN);
        }

        public Player(Player original)
        {
            this.PlayerId = original.PlayerId;
            this.PlayerString = original.PlayerString;
            this.DisplayName = original.DisplayName;
            this.HerdId = original.HerdId;
            this.Magic = original.Magic;
            this.SelectedArea = original.SelectedArea;

            //not sure if copy of experience instance is needed
            this.Experience = original.Experience;
        }

        public void AddMagic(int Magic)
        {
            this.Magic += Magic;
        }
    }
}
