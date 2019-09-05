//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Tools;
using System.Collections.Generic;
using System.Linq;

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
            rewards.Add(new Reward("", UnityEngine.Random.Range(5, 10) + UserManager.Instance.level, CurrencyType.COINS));

            //maybe add gems
            //60%
            if (UnityEngine.Random.value > .4f)
                rewards.Add(new Reward("", UnityEngine.Random.Range(1, 3), CurrencyType.GEMS));

            //add 1 or 2 character progress
            int quantity = UnityEngine.Random.value > .49f ? 1 : 2;
            for (int characterCycle = 0; characterCycle < quantity; characterCycle++)
            {
                //get random character from selected area
                rewards.Add(new GamePieceReward("", UnityEngine.Random.Range(10, 20), CurrencyType.GAME_PIECE, GameContentManager.Instance.currentTheme.gamepieces.list.Random().prefab.pieceData.ID));
            }

            return rewards;
        }

        public static List<Reward> FakeRewards()
        {
            return new List<Reward>()
            {
                new Reward("Reward 1", 90, CurrencyType.XP),
                new Reward("Reward 2", 420, CurrencyType.XP),
                new Reward("Reward 3", 330, CurrencyType.PORTAL_POINTS),
                new Reward("Reward 4", 440, CurrencyType.PORTAL_POINTS),
                new Reward("Reward 5", 550, CurrencyType.XP),
            };
        }

        public static List<Reward> FakePortalRewards()
        {
            return new List<Reward>()
            {
                new Reward("", 1, CurrencyType.COINS),
                new Reward("", 2, CurrencyType.COINS),
                new Reward("", 3, CurrencyType.COINS),
                new GamePieceReward("", 20, CurrencyType.GAME_PIECE, 19),
                new GamePieceReward("", 20, CurrencyType.GAME_PIECE, 1),
                new Reward("", 5, CurrencyType.COINS),
                new Reward("", 6, CurrencyType.COINS),
                new Reward("", 7, CurrencyType.COINS),
            };
        }

        public static void AssignRewards(this List<Reward> rewards)
        {
            UserManager.Instance.xp += rewards.Where(reward => reward.rewardType == CurrencyType.XP).Sum(reward => reward.quantity);
            UserManager.Instance.portalPoints += rewards.Where(reward => reward.rewardType == CurrencyType.PORTAL_POINTS).Sum(reward => reward.quantity);
            UserManager.Instance.rarePortalPoints += rewards.Where(reward => reward.rewardType == CurrencyType.RARE_PORTAL_POINTS).Sum(reward => reward.quantity);
            UserManager.Instance.coins += rewards.Where(reward => reward.rewardType == CurrencyType.COINS).Sum(reward => reward.quantity);
            UserManager.Instance.tickets += rewards.Where(reward => reward.rewardType == CurrencyType.TICKETS).Sum(reward => reward.quantity);
            UserManager.Instance.gems += rewards.Where(reward => reward.rewardType == CurrencyType.GEMS).Sum(reward => reward.quantity);

            //add gamepieces
            rewards.Where(reward => reward.rewardType == CurrencyType.GAME_PIECE).ToList().ForEach(reward =>
            {
                GameContentManager.Instance.piecesDataHolder.GetGamePieceData((reward as GamePieceReward).gamePieceID).AddPieces(reward.quantity);
            });
        }

        #region Rewards Filters
        //victory reward check
        public class VictoryRewardCheck : IRewardFilter
        {
            public List<Reward> Check(IClientFourzy game)
            {
                if (game.IsWinner())
                    return new List<Reward>() { new Reward("Victory", 10, CurrencyType.XP), new Reward("Victory", 10, CurrencyType.PORTAL_POINTS), };
                else
                    return new List<Reward>() { new Reward("Loss", 3, CurrencyType.XP), };
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
                                new Reward("8 in a Row", 1000, CurrencyType.XP),
                                new Reward("8 in a Row", 1000, CurrencyType.PORTAL_POINTS),
                            };

                        case 7:
                            return new List<Reward>() {
                                new Reward("7 in a Row", 200, CurrencyType.XP),
                                new Reward("7 in a Row", 200, CurrencyType.PORTAL_POINTS),
                            };

                        case 6:
                            return new List<Reward>() {
                                new Reward("6 in a Row", 50, CurrencyType.XP),
                                new Reward("6 in a Row", 50, CurrencyType.PORTAL_POINTS),
                            };

                        case 5:
                            return new List<Reward>() {
                                new Reward("5 in a Row", 20, CurrencyType.XP),
                                new Reward("5 in a Row", 20, CurrencyType.PORTAL_POINTS),
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
                            _rewards.Add(new Reward("Fast Play", stage.Value, CurrencyType.XP));
                            break;
                        }

                    foreach (KeyValuePair<float, int> stage in DURATION_PORTAL_POINTS_STAGES)
                        if (game.gameDuration <= stage.Key)
                        {
                            _rewards.Add(new Reward("Fast Play", stage.Value, CurrencyType.PORTAL_POINTS));
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
            public string name;
            public CurrencyType rewardType;
            public int quantity;

            public Reward(string name, int quantity, CurrencyType rewardType)
            {
                this.rewardType = rewardType;
                this.name = name;
                this.quantity = quantity;
            }

            public GameContentManager.PrefabType asPrefabType
            {
                get
                {
                    switch (rewardType)
                    {
                        case CurrencyType.COINS:
                            return GameContentManager.PrefabType.REWARDS_COIN;

                        case CurrencyType.TICKETS:
                            return GameContentManager.PrefabType.REWARDS_TICKET;

                        case CurrencyType.GEMS:
                            return GameContentManager.PrefabType.REWARDS_GEM;

                        case CurrencyType.GAME_PIECE:
                            return GameContentManager.PrefabType.REWARDS_GAME_PIECE;

                        default:
                            return GameContentManager.PrefabType.NONE;
                    }
                }
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
        }

        [System.Serializable]
        public class GamePieceReward : Reward
        {
            public int gamePieceID;

            public GamePieceReward(string name, int quantity, CurrencyType rewardType, int gamePieceID) : base(name, quantity, rewardType)
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