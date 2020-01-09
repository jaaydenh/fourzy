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
        public bool hasAIOpponent = true;

        public ResourceItem resource;

        public bool initialized = false;

        private Sprite _progressionIconEmpty;
        private Sprite _progressionIconSet;

        public Sprite progressionIconEmpty
        {
            get
            {
                if (!_progressionIconEmpty)
                    _progressionIconEmpty = GameContentManager.Instance.miscGameDataHolder.GetIcon("ProgressionIcons", "puzzleProgressionIconEmpty").sprite;

                return _progressionIconEmpty;
            }

            set => _progressionIconEmpty = value;
        }

        public Sprite progressionIconSet
        {
            get
            {
                if (!_progressionIconSet)
                    _progressionIconSet = GameContentManager.Instance.miscGameDataHolder.GetIcon("ProgressionIcons", "puzzleProgressionIconSet").sprite;

                return _progressionIconSet;
            }

            set => _progressionIconSet = value;
        }

        public Sprite currentProgressionIcon
        {
            get
            {
                if (!pack) return progressionIconEmpty;
                else return pack.puzzlesComplete.Contains(this) ? progressionIconSet : progressionIconEmpty;
            }
        }

        public bool lastInPack
        {
            get
            {
                if (!pack) return true;
                else return pack.enabledPuzzlesData.IndexOf(this) == pack.enabledPuzzlesData.Count - 1;
            }
        }

        public GauntletStatus gauntletStatus
        {
            get
            {
                if (pack && pack.gauntletStatus != null) return pack.gauntletStatus;

                return null;
            }
        }

        public int puzzleIndex
        {
            get
            {
                if (pack) return pack.enabledPuzzlesData.IndexOf(this);

                return -1;
            }
        }

        public ClientPuzzleData()
        {
            availableSpells = new SpellId[0];
            rewards = new RewardsManager.Reward[0];
        }

        public ClientPuzzleData(ResourceItem resource) : this()
        {
            this.resource = resource;
            string textData = resource.Load<TextAsset>().text;
            ID = textData.GetIDFromPuzzleDataFile();
            Enabled = textData.GetEnabledFromPuzzleDataFile();

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

            //try get starting magic
            JToken _startingMagic = jObject["StartingMagic"];
            if (_startingMagic != null)
                startingMagic = _startingMagic.ToObject<int>();
            else
                startingMagic = FourzyGameModel.Model.Constants.PlayerStartingMagic;

            //try get hasAIOpponent
            JToken _hasAIOpponent = jObject["hasAIOpponent"];
            if (_hasAIOpponent != null) hasAIOpponent = _hasAIOpponent.ToObject<bool>();

            if (hasAIOpponent)
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
            rewards.Where(reward => !PlayerPrefsWrapper.GetRewardRewarded(GetRewardID(reward))).AssignRewards();

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
                        PlayerPrefsWrapper.SetRewardRewarded(GetRewardID(reward), true);

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

        public string GetRewardID(RewardsManager.Reward reward) => reward.GetID(ID);

        /// <summary>
        /// Only after rest was assigned
        /// </summary>
        public void GetInstructions()
        {
            Instructions = LocalizationManager.Value(GoalType.GoalTypeToKey());

            switch (GoalType)
            {
                case PuzzleGoalType.SURVIVE:
                    Instructions += $" {LocalizationManager.Value("for")} ";

                    break;

                default:
                    Instructions += $" {LocalizationManager.Value("in")} ";

                    break;
            }

            Instructions += MoveLimit;

            if (MoveLimit > 1) Instructions += $" {LocalizationManager.Value("moves")}";
            else Instructions += $" {LocalizationManager.Value("move")}";
        }

        public static implicit operator bool(ClientPuzzleData data) => data != null;
    }
}