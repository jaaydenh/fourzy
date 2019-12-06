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
        public string DisplayName { get; set; }

        [JsonProperty("profile")]
        public AIProfile Profile { get; set; }

        [JsonProperty("bossType")]
        public BossType BossType { get; set; }

        [JsonProperty("herdId")]
        public string HerdId { get; set; }

        [JsonProperty("herdCount")]
        public int HerdCount { get; set; }

        [JsonProperty("magic")]
        public int Magic { get; set; }

        [JsonProperty("special")]
        public int SpecialAbilityCount { get; set; }

        [JsonProperty("selectedArea")]
        public Area SelectedArea { get; set; }

        [JsonProperty("experience")]
        public PlayerExperience Experience { get; set; }

        [JsonConstructor]
        public Player(int PlayerId, string DisplayName, AIProfile Profile = AIProfile.Player)
        {
            this.PlayerId = PlayerId;
            this.DisplayName = DisplayName;
            this.Magic = Constants.PlayerStartingMagic;
            this.SelectedArea = Area.NONE;
            this.Experience = new PlayerExperience();
            this.Experience.UnlockedAreas.Add(Area.TRAINING_GARDEN);
            this.Profile = Profile;
            this.SpecialAbilityCount = 0;
            this.BossType = BossType.None;
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
            this.Profile = original.Profile;
            this.SpecialAbilityCount = original.SpecialAbilityCount;
            this.BossType = original.BossType;
            this.HerdCount = original.HerdCount;
        }

        public void UseSpecialAbility()
        {
            this.SpecialAbilityCount--;
        }

        public void GainSpecialAbility()
        {
            this.SpecialAbilityCount++;
        }

        public void AddMagic(int Magic)
        {
            this.Magic += Magic;
        }

        public void LoseMagic(int Magic)
        {
            this.Magic -= Magic;
        }


    }
}
