//@vadym udod

using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.Tools;
using FourzyGameModel.Model;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.ClientModel
{
    [System.Serializable]
    public class ClientPuzzleData : PuzzleData
    {
        public BasicPuzzlePack pack;

        public GameBoardDefinition gameBoardDefinition;
        public AIProfile aiProfile;
        public BossType aiBoss;
        public int firstTurn;
        public string herdID = "1";
        public string aiPlayerName = "";
        public SpellId[] availableSpells;
        public RewardsManager.Reward[] rewards;

        public int Complexity = -1;
        public int startingMagic = 0;

        public ResourceItem resource;

        public bool initialized = false;

        private Sprite _progressionIconEmpty;
        private Sprite _progressionIconSet;

        public Sprite progressionIconEmpty
        {
            get
            {
                if (!_progressionIconEmpty)
                    _progressionIconEmpty = GameContentManager.Instance.miscGameDataHolder.GetIcon("puzzleProgressionIconEmpty").sprite;

                return _progressionIconEmpty;
            }

            set => _progressionIconEmpty = value;
        }

        public Sprite progressionIconSet
        {
            get
            {
                if (!_progressionIconSet)
                    _progressionIconSet = GameContentManager.Instance.miscGameDataHolder.GetIcon("puzzleProgressionIconSet").sprite;

                return _progressionIconSet;
            }

            set => _progressionIconSet = value;
        }

        public ClientPuzzleData()
        {
            availableSpells = new SpellId[0];
            rewards = new RewardsManager.Reward[0];
        }

        public ClientPuzzleData(string ID, ResourceItem resource) : this()
        {
            this.resource = resource;
            this.ID = ID;

            PuzzlePlayer = new Player(2, "AI", AIProfile.PuzzleAI);
        }

        public void ParseJObject(JObject jObject)
        {
            if (string.IsNullOrEmpty(ID)) ID = jObject["ID"].ToObject<string>();
            Enabled = jObject["Enabled"].ToObject<bool>();
            gameBoardDefinition = jObject["GameBoard"].ToObject<GameBoardDefinition>();
            InitialGameBoard = gameBoardDefinition.ToGameBoardData();
            aiProfile = (AIProfile)jObject["Profile"].ToObject<int>();
            aiBoss = (BossType)jObject["Boss"].ToObject<int>();
            firstTurn = jObject["FirstTurn"].ToObject<int>();
            MoveLimit = jObject["MoveLimit"].ToObject<int>();
            Complexity = jObject["Complexity"].ToObject<int>();
            Solution = jObject["Solution"].ToObject<List<PlayerTurn>>();
            SolutionState = jObject["SolutionStateData"].ToObject<GameState>();
            GoalType = jObject["GoalType"].ToObject<PuzzleGoalType>();

            JToken _startingMagic = jObject["StartingMagic"];

            if (_startingMagic != null)
                startingMagic = _startingMagic.ToObject<int>();
            else
                startingMagic = FourzyGameModel.Model.Constants.PlayerStartingMagic;

            if (aiProfile != AIProfile.Player)
            {
                if (string.IsNullOrEmpty(aiPlayerName)) aiPlayerName = "AI";
                if (string.IsNullOrEmpty(herdID)) herdID = "1";

                if (pack)
                {
                    aiPlayerName = string.IsNullOrEmpty(pack.aiPlayerName) ? aiPlayerName : pack.aiPlayerName;
                    herdID = string.IsNullOrEmpty(pack.herdID) ? herdID : pack.herdID;
                }

                PuzzlePlayer = new Player(2, aiPlayerName, aiProfile) { HerdId = herdID };

            }

            GetInstructions();
        }

        public void AssignPuzzleRewards()
        {
            if (rewards.Length == 0) return;

            //filter rewards
            rewards.Where(reward => !PlayerPrefsWrapper.GetEventRewarded(GetRewardID(reward))).AssignRewards();

            //set events as rewarded
            foreach (RewardsManager.Reward reward in rewards)
            {
                switch (reward.rewardType)
                {
                    //player assigns these 2 manually
                    case RewardType.OPEN_PORTAL:
                    case RewardType.OPEN_RARE_PORTAL:

                        break;

                    default:
                        PlayerPrefsWrapper.SetEventRewarded(GetRewardID(reward), true);

                        break;
                }
            }
        }

        public ClientPuzzleData Initialize()
        {
            if (!initialized)
            {
                initialized = true;

                if (resource != null) ParseJObject(JObject.Parse(resource.Load<TextAsset>().text));
            }

            return this;
        }

        public string GetRewardID(RewardsManager.Reward reward) => ID + "_" + reward.rewardType.ToString();

        /// <summary>
        /// Only after rest was assigned
        /// </summary>
        public void GetInstructions()
        {
            Instructions = GoalType.ToString();
            switch (GoalType)
            {
                case PuzzleGoalType.SURVIVE:
                    Instructions += " for ";

                    break;

                default:
                    Instructions += " in ";

                    break;
            }
            Instructions += (MoveLimit + " ");
            if (MoveLimit > 1)
                Instructions += "moves!";
            else
                Instructions += "move!";
        }

        public static implicit operator bool(ClientPuzzleData data) => data != null;
    }
}