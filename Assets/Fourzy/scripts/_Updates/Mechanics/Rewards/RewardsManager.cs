//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Tools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Rewards
{
    public static class RewardsManager
    {
        public static List<IRewardFilter> DEFAULT_FILTERS = new List<IRewardFilter>()
        {
            new VictoryRewardCheck(),
            new InARowRewardCheck(),
            new FastPlayRewardCheck(),
        };

        public static List<Reward> DoCheck(IClientFourzy game)
        {
            List<Reward> result = new List<Reward>();

            foreach (IRewardFilter filter in DEFAULT_FILTERS)
            {
                List<Reward> rewards = filter.Check(game);

                if (rewards != null) result.AddRange(rewards);
            }

            return result;
        }

        public static List<Reward> GetPortalRewards()
        {
            List<Reward> rewards = new List<Reward>();

            //100% give some coins
            rewards.Add(new Reward(UnityEngine.Random.Range(5, 10) + UserManager.Instance.level, RewardType.COINS));

            //some hints
            rewards.Add(new Reward(UnityEngine.Random.Range(2, 4), RewardType.HINTS));

            //maybe add gems
            //60%
            if (UnityEngine.Random.value > .4f)
                rewards.Add(new Reward(UnityEngine.Random.Range(1, 3), RewardType.GEMS));

            //add 1 or 2 character progress
            int quantity = UnityEngine.Random.value > .49f ? 1 : 2;
            for (int characterCycle = 0; characterCycle < quantity; characterCycle++)
            {
                //get random character from selected area
                rewards.Add(new GamePieceReward(
                    UnityEngine.Random.Range(10, 20), 
                    RewardType.GAME_PIECE, 
                    GameContentManager.Instance.currentArea.gamepieces.Random().pieceData.ID));
            }

            return rewards;
        }

        public static List<Reward> FakeRewards()
        {
            return new List<Reward>()
            {
                new Reward(90, RewardType.XP),
                new Reward(420, RewardType.XP),
                new Reward(330, RewardType.PORTAL_POINTS),
                new Reward(440, RewardType.PORTAL_POINTS),
                new Reward(550, RewardType.XP),
            };
        }

        public static List<Reward> FakePortalRewards()
        {
            return new List<Reward>()
            {
                new Reward(1, RewardType.COINS),
                new Reward(2, RewardType.COINS),
                new Reward(3, RewardType.COINS),
                new GamePieceReward(20, RewardType.GAME_PIECE, "19"),
                new GamePieceReward(20, RewardType.GAME_PIECE, "1"),
                new Reward(5, RewardType.COINS),
                new Reward(6, RewardType.COINS),
                new Reward(7, RewardType.COINS),
            };
        }

        public static void AssignRewards(this IEnumerable<Reward> rewards)
        {
            UserManager.Instance.xp += rewards.Where(reward => reward.rewardType == RewardType.XP).Sum(reward => reward.quantity);
            UserManager.Instance.portalPoints += rewards.Where(reward => reward.rewardType == RewardType.PORTAL_POINTS).Sum(reward => reward.quantity);
            UserManager.Instance.rarePortalPoints += rewards.Where(reward => reward.rewardType == RewardType.RARE_PORTAL_POINTS).Sum(reward => reward.quantity);
            UserManager.Instance.coins += rewards.Where(reward => reward.rewardType == RewardType.COINS).Sum(reward => reward.quantity);
            UserManager.Instance.tickets += rewards.Where(reward => reward.rewardType == RewardType.TICKETS).Sum(reward => reward.quantity);
            UserManager.Instance.hints += rewards.Where(reward => reward.rewardType == RewardType.HINTS).Sum(reward => reward.quantity);
            UserManager.Instance.gems += rewards.Where(reward => reward.rewardType == RewardType.GEMS).Sum(reward => reward.quantity);

            //add gamepieces
            rewards.Where(reward => reward.rewardType == RewardType.GAME_PIECE).ToList().ForEach(reward => 
                GameContentManager.Instance.piecesDataHolder.GetGamePieceData((reward as GamePieceReward).gamePieceID).AddPieces(reward.quantity));
        }

        #region Rewards Filters
        //victory reward check
        public class VictoryRewardCheck : IRewardFilter
        {
            public List<Reward> Check(IClientFourzy game)
            {
                if (game.IsWinner())
                    return new List<Reward>() { new Reward(10, RewardType.XP, "Victory"), new Reward(10, RewardType.PORTAL_POINTS, "Victory"), };
                else
                    return new List<Reward>() { new Reward(3, RewardType.XP, "Loss"), };
            }

            public List<Reward> CustomRewardsCheck(IClientFourzy game, List<Reward> rewards)
            {
                if (game.IsWinner())
                    return rewards;

                return null;
            }
        }

        //# in a row Reward
        public class InARowRewardCheck : IRewardFilter
        {
            public List<Reward> Check(IClientFourzy game)
            {
                if (game.IsWinner())
                {
                    switch (game._State.WinningLocations.Count)
                    {
                        case 8:
                            return new List<Reward>() {
                                new Reward(1000, RewardType.XP, "8 in a Row"),
                                new Reward(1000, RewardType.PORTAL_POINTS, "8 in a Row"),
                            };

                        case 7:
                            return new List<Reward>() {
                                new Reward(200, RewardType.XP, "7 in a Row"),
                                new Reward(200, RewardType.PORTAL_POINTS, "7 in a Row"),
                            };

                        case 6:
                            return new List<Reward>() {
                                new Reward(50, RewardType.XP, "6 in a Row"),
                                new Reward(50, RewardType.PORTAL_POINTS, "6 in a Row"),
                            };

                        case 5:
                            return new List<Reward>() {
                                new Reward(20, RewardType.XP, "5 in a Row"),
                                new Reward(20, RewardType.PORTAL_POINTS, "5 in a Row"),
                            };

                        default:
                            return null;
                    }
                }

                return null;
            }


            public List<Reward> CustomRewardsCheck(IClientFourzy game, List<Reward> rewards)
            {
                if (game.IsWinner() && game._State.WinningLocations.Count >= 4)
                    return rewards;

                return null;
            }
        }

        //fast play reward 
        public class FastPlayRewardCheck : IRewardFilter
        {
            public static Dictionary<float, int> DURATION_XP_STAGES = new Dictionary<float, int>()
            {
                //seconds - points
                [20f] = 70,
                [30f] = 50,
                [40f] = 30,
            };

            public static Dictionary<float, int> DURATION_PORTAL_POINTS_STAGES = new Dictionary<float, int>()
            {
                //seconds - points
                [20f] = 70,
                [30f] = 50,
                [40f] = 30,
            };

            public List<Reward> Check(IClientFourzy game)
            {
                if (game.IsWinner())
                {
                    List<Reward> _rewards = new List<Reward>();

                    foreach (KeyValuePair<float, int> stage in DURATION_XP_STAGES)
                        if (game.gameDuration <= stage.Key)
                        {
                            _rewards.Add(new Reward(stage.Value, RewardType.XP, "Fast Play"));
                            break;
                        }

                    foreach (KeyValuePair<float, int> stage in DURATION_PORTAL_POINTS_STAGES)
                        if (game.gameDuration <= stage.Key)
                        {
                            _rewards.Add(new Reward(stage.Value, RewardType.PORTAL_POINTS, "Fast Play"));
                            break;
                        }

                    return _rewards;
                }

                return null;
            }

            public List<Reward> CustomRewardsCheck(IClientFourzy game, List<Reward> rewards)
            {
                return rewards;
            }
        }
        #endregion

        public interface IRewardFilter
        {
            List<Reward> Check(IClientFourzy game);
            List<Reward> CustomRewardsCheck(IClientFourzy game, List<Reward> rewards);
        }

        [System.Serializable]
        public class Reward
        {
            public RewardType rewardType;
            public int quantity;
            public string customName;

            public string name
            {
                get
                {
                    if (!string.IsNullOrEmpty(customName)) return customName;
                    else
                    {
                        switch (rewardType)
                        {
                            case RewardType.COINS:
                                return LocalizationManager.Value("coins");

                            case RewardType.GAME_PIECE:
                                return LocalizationManager.Value("gamepieces");

                            case RewardType.GEMS:
                                return LocalizationManager.Value("gems");

                            case RewardType.HINTS:
                                return LocalizationManager.Value("hints");

                            case RewardType.MAGIC:
                                return LocalizationManager.Value("magic");

                            case RewardType.OPEN_PORTAL:
                                return LocalizationManager.Value("openportal");

                            case RewardType.OPEN_RARE_PORTAL:
                                return LocalizationManager.Value("openrareportal");

                            case RewardType.PORTAL_POINTS:
                                return LocalizationManager.Value("portalpoints");

                            case RewardType.RARE_PORTAL_POINTS:
                                return LocalizationManager.Value("rareportalpoints");

                            case RewardType.TICKETS:
                                return LocalizationManager.Value("tickets");

                            case RewardType.XP:
                                return LocalizationManager.Value("xp");

                            default:
                                return customName;
                        }
                    }
                }
            }

            public string GetID(string prefix)
            {
                switch (rewardType)
                {
                    case RewardType.CUSTOM:
                        return name;

                    default:
                        return prefix + "_" + rewardType.ToString();
                }
            }

            public Reward(int quantity, RewardType rewardType, string customName = "")
            {
                this.rewardType = rewardType;
                this.quantity = quantity;
                this.customName = customName;
            }

            public override string ToString()
            {
                switch (rewardType)
                {
                    //case CurrencyType.COINS:
                    //    return (quantity / 10) + "";

                    default:
                        return quantity + "";
                }
            }

            public GamePieceReward asGamePieceReward => this as GamePieceReward;
        }

        [System.Serializable]
        public class GamePieceReward : Reward
        {
            public string gamePieceID;

            public GamePieceReward(int quantity, RewardType rewardType, string gamePieceID, string customName = "") : base(quantity, rewardType, customName)
            {
                this.gamePieceID = gamePieceID;
            }
        }

        public enum PortalType
        {
            SIMPLE = 0,
            RARE = 1,
            //..
        }
    }
}