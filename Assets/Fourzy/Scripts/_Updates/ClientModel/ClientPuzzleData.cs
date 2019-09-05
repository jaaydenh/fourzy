//@vadym udod

using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
using FourzyGameModel.Model;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.ClientModel
{
    [System.Serializable]
    public class ClientPuzzleData : PuzzleData
    {
        public PuzzlePacksDataHolder.BasicPuzzlePack pack;

        public GameBoardDefinition gameBoardDefinition;
        public AIProfile aiProfile;
        public BossType aiBoss;
        public int firstTurn;
        public SpellId[] availableSpells;
        public RewardsManager.Reward[] rewards;
        public Sprite progressionIconEmpty;
        public Sprite progressionIconSet;

        public int Complexity = -1;
        public string puzzleFilePath;

        public bool initialized = false;

        public ClientPuzzleData()
        {
            availableSpells = new SpellId[0];
            rewards = new RewardsManager.Reward[0];
        }

        public ClientPuzzleData(string ID, string puzzleFilePath) : this()
        {
            this.puzzleFilePath = puzzleFilePath;
            this.ID = ID;

            PuzzlePlayer = new Player(2, "AI", /*(AIProfile)Profile*/AIProfile.PuzzleAI);
        }

        public ClientPuzzleData(JObject jObject) : this()
        {
            ParseJObject(jObject);
            Initialize();

            PuzzlePlayer = new Player(2, "AI", /*(AIProfile)Profile*/AIProfile.PuzzleAI);
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

        public virtual void Initialize()
        {
            if (!initialized)
            {
                initialized = true;

                if (!string.IsNullOrEmpty(puzzleFilePath)) ParseJObject(JObject.Parse(File.ReadAllText(puzzleFilePath)));
            }
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