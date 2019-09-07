//@vadym udod

using Fourzy._Updates.Mechanics.Rewards;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.ClientModel
{
    [System.Serializable]
    public class JsonPuzzlePack : BasicPuzzlePack
    {
        public UnlockRequirementsEnum unlockRequirement;
        public string herdID;
        public string playerName;
        public RawardIndexed[] rewards;

        public JsonPuzzlePack(ResourceItem resourceItem)
        {
            JObject jObject = JObject.Parse(resourceItem.Load<TextAsset>().text);

            name = jObject["name"].ToObject<string>();
            packType = (PackType)jObject["type"].ToObject<int>();
            packID = jObject["id"].ToObject<string>();
            unlockRequirement = (UnlockRequirementsEnum)jObject["unlockRequirement"].ToObject<int>();
            herdID = jObject["herdID"].ToObject<string>();
            playerName = jObject["playerName"].ToObject<string>();
            rewards = jObject["rewards"].ToObject<RawardIndexed[]>();
            allRewards = rewards.Cast<RewardsManager.Reward>().ToList();

            Initialize();
        }
    }

    [System.Serializable]
    public class RawardIndexed : RewardsManager.Reward
    {
        public int levelIndex;

        public RawardIndexed(int levelIndex, string name, int quantity, RewardType rewardType) : base(name, quantity, rewardType)
        {
            this.levelIndex = levelIndex;
        }
    }
}