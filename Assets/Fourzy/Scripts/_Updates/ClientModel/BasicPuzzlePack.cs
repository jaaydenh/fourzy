//@vadym udod

using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Toasts;
using FourzyGameModel.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.ClientModel
{
    [System.Serializable]
    public class BasicPuzzlePack
    {
        public string name;
        public PackType packType = PackType.PUZZLE_PACK;
        public string packID;
        public string aiPlayerName;
        public string herdID;
        public UnlockRequirementsEnum unlockRequirement;
        public string description;

        public int gauntletLevels { get; private set; } = -1;
        public GauntletStatus gauntletStatus { get; private set; }
        public List<ClientPuzzleData> puzzlesData { get; set; }
        public List<ClientPuzzleData> enabledPuzzlesData { get; set; }
        public List<ClientPuzzleData> rewardPuzzles { get; set; }
        public Player puzzlePlayer { get; set; }
        public Dictionary<int, List<RewardsManager.Reward>> allRewards { get; set; }

        public List<ClientPuzzleData> puzzlesComplete => enabledPuzzlesData.Where(puzzle => PlayerPrefsWrapper.GetPuzzleChallengeComplete(puzzle.ID)).ToList();

        public bool complete => puzzlesComplete.Count == enabledPuzzlesData.Count;

        public bool justFinished { get; set; }

        public BasicPuzzlePack() { }

        public BasicPuzzlePack(ResourceItem resourceItem)
        {
            Initialize();

            JObject jObject = JObject.Parse(resourceItem.Load<TextAsset>().text);

            name = jObject["name"].ToObject<string>();
            packType = (PackType)jObject["type"].ToObject<int>();
            packID = jObject["id"].ToObject<string>();
            unlockRequirement = (UnlockRequirementsEnum)jObject["unlockRequirement"].ToObject<int>();
            herdID = jObject["herdID"].ToObject<string>();
            aiPlayerName = jObject["playerName"].ToObject<string>();
            JToken descriptionToken = jObject["description"];
            if (descriptionToken != null) description = descriptionToken.ToObject<string>();
            puzzlePlayer = new Player(2, aiPlayerName) { HerdId = herdID };

            RewardIndexed[] _rewards = jObject["rewards"].ToObject<RewardIndexed[]>();

            foreach (RewardIndexed reward in _rewards)
            {
                if (!allRewards.ContainsKey(reward.levelIndex)) allRewards.Add(reward.levelIndex, new List<RewardsManager.Reward>());

                allRewards[reward.levelIndex].Add(reward);
            }
        }

        public BasicPuzzlePack(int gauntletLevels)
        {
            Initialize();

            this.gauntletLevels = gauntletLevels;
            gauntletStatus = new GauntletStatus();

            name = "Beat The Bot";
            packType = PackType.AI_PACK;
            packID = Guid.NewGuid().ToString();
            unlockRequirement = UnlockRequirementsEnum.NONE;
            herdID = "1";
            aiPlayerName = "The Bot";
            puzzlePlayer = new Player(2, aiPlayerName) { HerdId = herdID };

            for (int levelIndex = 0; levelIndex < gauntletLevels; levelIndex++)
            {
                ClientPuzzleData puzzleData = new ClientPuzzleData();
                puzzleData.pack = this;
                puzzleData.ID = packID + "_level_" + levelIndex;
                puzzleData.aiPlayerName = aiPlayerName;
                puzzleData.PuzzlePlayer = puzzlePlayer;
                puzzleData.startingMagic = FourzyGameModel.Model.Constants.PlayerStartingMagic;

                puzzlesData.Add(puzzleData);
                enabledPuzzlesData.Add(puzzleData);

                PlayerPrefsWrapper.SetPuzzleChallengeComplete(puzzleData.ID, false);
            }
        }

        public virtual void Initialize()
        {
            puzzlesData = new List<ClientPuzzleData>();
            enabledPuzzlesData = new List<ClientPuzzleData>();
            rewardPuzzles = new List<ClientPuzzleData>();
            allRewards = new Dictionary<int, List<RewardsManager.Reward>>();
        }

        public ClientPuzzleData nextUnsolvedData
        {
            get
            {
                if (enabledPuzzlesData.Count == 0) return null;

                for (int puzzleIndex = 0; puzzleIndex < enabledPuzzlesData.Count; puzzleIndex++)
                {
                    if (puzzleIndex == 0 && !PlayerPrefsWrapper.GetPuzzleChallengeComplete(enabledPuzzlesData[puzzleIndex].ID))
                        return enabledPuzzlesData[puzzleIndex];
                    else if (puzzleIndex > 0 && PlayerPrefsWrapper.GetPuzzleChallengeComplete(enabledPuzzlesData[puzzleIndex - 1].ID)
                        && !PlayerPrefsWrapper.GetPuzzleChallengeComplete(enabledPuzzlesData[puzzleIndex].ID))
                        return enabledPuzzlesData[puzzleIndex];
                }

                return enabledPuzzlesData[0];
            }
        }

        public string getUnlockRewardID
        {
            get
            {
                //find unlock reward
                RewardsManager.Reward reward = null;

                foreach (var _set in allRewards)
                {
                    reward = _set.Value.Find(_reward => _reward.rewardType == RewardType.PACK_COMPLETE);

                    if (reward != null) return puzzlesData[_set.Key].GetRewardID(reward);
                }

                return "";
            }
        }

        public IClientFourzy Next(IClientFourzy current)
        {
            ClientPuzzleData data = enabledPuzzlesData.Next(current.puzzleData);

            data.Initialize();

            switch (data.pack.packType)
            {
                case PackType.PUZZLE_PACK: return new ClientFourzyPuzzle(data);

                default: return ClientFourzyGame.FromPuzzleData(data, current);
            }
        }

        public void StartNextUnsolvedPuzzle(IClientFourzy current = null)
        {
            IClientFourzy game = null;

            if (current != null)
                game = Next(current);
            else
            {
                ClientPuzzleData data = nextUnsolvedData;

                if (data)
                {
                    data.Initialize();

                    switch (data.pack.packType)
                    {
                        case PackType.PUZZLE_PACK: game = new ClientFourzyPuzzle(data); break;

                        default: game = ClientFourzyGame.FromPuzzleData(data, GameManager.Instance.activeGame); break;
                    }
                }
            }

            if (game == null)
            {
                GamesToastsController.ShowTopToast("Empty puzzle pack");
                return;
            }

            GameManager.Instance.currentPuzzlePack = this;
            GameManager.Instance.StartGame(game, GameTypeLocal.LOCAL_GAME);
        }

        public void ResetPlayerPrefs()
        {
            PlayerPrefsWrapper.SetPuzzlePackUnlocked(packID, false);
            PlayerPrefsWrapper.SetPuzzlePackOpened(packID, false);

            if (puzzlesData == null) Debug.Log(name);
            puzzlesData.ForEach(_data =>
            {
                PlayerPrefsWrapper.SetPuzzleChallengeComplete(_data.ID, false);
                PlayerPrefsWrapper.SetGameRewarded(_data.ID, false);

                //clear rewards
                foreach (RewardsManager.Reward reward in _data.rewards)
                {
                    PlayerPrefsWrapper.SetRewardRewarded(_data.GetRewardID(reward), false);
                }
            });
        }

        public string GetHerdID(int levelIndex = -1)
        {
            if (levelIndex == -1)
                return herdID;
            else
                return puzzlesData[levelIndex].herdID;
        }

        //public void RemoveHerdMember()
        //{
        //    gauntletStatus.FourzyCount = Mathf.Clamp(gauntletStatus.FourzyCount - 1, 0, int.MaxValue);
        //}

        public static implicit operator bool(BasicPuzzlePack pack) => pack != null;
    }

    [System.Serializable]
    public class RewardIndexed : RewardsManager.Reward
    {
        public int levelIndex;

        public RewardIndexed(int levelIndex, int quantity, RewardType rewardType, string name) : base(quantity, rewardType, name)
        {
            this.levelIndex = levelIndex;
        }
    }

    public enum UnlockRequirementsEnum
    {
        NONE = 0,
        STARS = 1,
        COINS = 2,
        GEMS = 3,
    }

    public enum PackType
    {
        PUZZLE_PACK,
        AI_PACK,
        BOSS_AI_PACK,
    }
}