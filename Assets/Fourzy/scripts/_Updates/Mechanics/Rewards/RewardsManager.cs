//@vadym udod

using FourzyGameModel.Model;
using System.Collections.Generic;

namespace Fourzy._Updates.Mechanics.Rewards
{
    public class RewardsManager
    {
        public static Dictionary<RewardType, List<IRewardFilter>> REWARDS_FILTERS = new Dictionary<RewardType, List<IRewardFilter>>()
        {
            [RewardType.XP] = new List<IRewardFilter>(),
            [RewardType.PORTAL_POINTS] = new List<IRewardFilter>(),
            [RewardType.FIRST_COMBINED] = new List<IRewardFilter>()
            {
                new VictoryRewardCheck(),
                new InARowRewardCheck(),
                new FastPlayRewardCheck(),
            },
        };

        public static List<Reward> DoCheck(RewardType targetType, RewardTestSubject subject)
        {
            List<Reward> result = new List<Reward>();

            foreach (KeyValuePair<RewardType, List<IRewardFilter>> filters in REWARDS_FILTERS)
                if (filters.Key.HasFlag(targetType))
                    filters.Value.ForEach(filter =>
                    {
                        Reward reward = filter.Check(targetType, subject);

                        if (reward != null) result.Add(reward);
                    });

            return result;
        }

        public static List<Reward> FakeRewards()
        {
            return new List<Reward>()
            {
                new Reward("Reward 1", 90),
                new Reward("Reward 2", 420),
                new Reward("Reward 3", 330),
                new Reward("Reward 4", 440),
                new Reward("Reward 5", 550),
            };
        }

        #region XP Rewards

        #endregion

        #region Portal Points Rewards

        #endregion

        #region Combined Rewards
        //victory reward check
        public class VictoryRewardCheck : IRewardFilter
        {
            public RewardType rewardType => RewardType.FIRST_COMBINED;

            public Reward Check(RewardType targetType, RewardTestSubject subject)
            {
                if (subject.state.WinnerId == subject.testedPlayerID)
                {
                    switch (targetType)
                    {
                        case RewardType.XP:
                            return new Reward("Victory", 10);

                        case RewardType.PORTAL_POINTS:
                            return new Reward("Victory", 10);
                    }
                }

                return null;
            }
        }

        //# in a row Reward
        public class InARowRewardCheck : IRewardFilter
        {
            public RewardType rewardType => RewardType.FIRST_COMBINED;

            public Reward Check(RewardType targetType, RewardTestSubject subject)
            {
                if (subject.state.WinningLocations != null)
                {
                    switch (targetType)
                    {
                        case RewardType.XP:
                            switch (subject.state.WinningLocations.Count)
                            {
                                case 8:
                                    return new Reward("8 in a Row", 1000);

                                case 7:
                                    return new Reward("7 in a Row", 200);

                                case 6:
                                    return new Reward("6 in a Row", 50);

                                case 5:
                                    return new Reward("5 in a Row", 20);

                                default:
                                    return null;
                            }

                        case RewardType.PORTAL_POINTS:
                            switch (subject.state.WinningLocations.Count)
                            {
                                case 8:
                                    return new Reward("8 in a Row", 1000);

                                case 7:
                                    return new Reward("7 in a Row", 200);

                                case 6:
                                    return new Reward("6 in a Row", 50);

                                case 5:
                                    return new Reward("5 in a Row", 20);

                                default:
                                    return null;
                            }
                    }
                }

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

            public RewardType rewardType => RewardType.FIRST_COMBINED;

            public Reward Check(RewardType targetType, RewardTestSubject subject)
            {
                if (subject.state.WinningLocations != null)
                {
                    switch (targetType)
                    {
                        case RewardType.XP:
                            foreach (KeyValuePair<float, int> stage in DURATION_XP_STAGES)
                                if (subject.gameDuration <= stage.Key)
                                    return new Reward("Fast Play", stage.Value);
                            break;

                        case RewardType.PORTAL_POINTS:
                            foreach (KeyValuePair<float, int> stage in DURATION_PORTAL_POINTS_STAGES)
                                if (subject.gameDuration <= stage.Key)
                                    return new Reward("Fast Play", stage.Value);
                            break;
                    }
                }

                return null;
            }
        }
        #endregion

        public class RewardTestSubject
        {
            public RewardTestSubject(string gameID, GameState state, List<CollectedItemReward> collectedItems, float gameDuration, int testedPlayerID)
            {
                this.gameID = gameID;
                this.state = state;
                this.collectedItems = collectedItems;
                this.gameDuration = gameDuration;
                this.testedPlayerID = testedPlayerID;
            }

            public string gameID;
            public GameState state;
            public List<CollectedItemReward> collectedItems;
            public float gameDuration;
            public int testedPlayerID;
        }

        public interface IRewardFilter
        {
            RewardType rewardType { get; }
            Reward Check(RewardType targetType, RewardTestSubject subject);
        }

        public interface IRewardTestSubject
        {
            RewardTestSubject asSubject { get; }
        }

        public class CollectedItemReward : Reward
        {
            public CollectedItemType type;

            public GameContentManager.PrefabType asPrefabType
            {
                get
                {
                    switch (type)
                    {
                        case CollectedItemType.COINS:
                            return GameContentManager.PrefabType.REWARDS_COLLECTABLE_COIN;

                        case CollectedItemType.TICKETS:
                            return GameContentManager.PrefabType.REWARDS_COLLECTABLE_TICKET;

                        default:
                            return GameContentManager.PrefabType.NONE;
                    }
                }
            }

            public CollectedItemReward(string name, int quantity, CollectedItemType type) : base(name, quantity)
            {
                this.type = type;
            }
        }

        public class Reward
        {
            public Reward(string name, int quantity)
            {
                this.name = name;
                this.quantity = quantity;
            }

            public string name;
            public int quantity;
        }

        public enum RewardType
        {
            XP = 0,
            PORTAL_POINTS = 1,

            FIRST_COMBINED = XP | PORTAL_POINTS,
        }

        public enum CollectedItemType
        {
            COINS = 0,
            TICKETS = 1,
        }
    }
}