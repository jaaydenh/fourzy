//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Widgets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class RewardsScreen : MenuScreen
    {
        public TMP_Text title;
        public CurrencyWidget xpWidget;
        public RectTransform xpTab;
        public RectTransform portalKeysTab;
        public RectTransform collectedItemsTab;

        private AlphaTween xpTabAlphaTween;
        private AlphaTween portalKeysTabAlphaTween;
        private AlphaTween collectedItemsTabAlphaTween;

        private List<RewardsManager.Reward> rewards;
        private List<RewardsManager.Reward> xpRewards;
        private List<RewardsManager.Reward> portalPointsRewards;

        private List<RewardsScreenWidget> xpWidgets = new List<RewardsScreenWidget>();
        private List<RewardsScreenWidget> portalKeysWidgets = new List<RewardsScreenWidget>();
        private List<RewardsScreenWidget> collectedItemsWidgets = new List<RewardsScreenWidget>();

        private ScrollRect scrollRect;

        private bool animationFinished;
        private int previousXP;

        private List<RewardsManager.Reward> collectedItems;

        protected override void Awake()
        {
            base.Awake();

            xpTabAlphaTween = xpTab.GetComponent<AlphaTween>();
            portalKeysTabAlphaTween = portalKeysTab.GetComponent<AlphaTween>();
            collectedItemsTabAlphaTween = collectedItemsTab.GetComponent<AlphaTween>();
            scrollRect = GetComponentInChildren<ScrollRect>();
        }

        public static bool WillDisplayRewardsScreen(IClientFourzy game)
        {
            if (game.puzzleData)
            {
                if (PlayerPrefsWrapper.GetRemoteSetting(Constants.KEY_REWARDS_PUZZLEPLAY) == "0") return false;
            }
            else
            {
                switch (game._Type)
                {
                    case GameType.TURN_BASED:
                        if (PlayerPrefsWrapper.GetRemoteSetting(Constants.KEY_REWARDS_TURNBASED) == "0") return false;

                        break;

                    case GameType.PASSANDPLAY:
                        if (PlayerPrefsWrapper.GetRemoteSetting(Constants.KEY_REWARDS_PASSPLAY) == "0") return false;

                        break;

                    case GameType.REALTIME:
                        if (PlayerPrefsWrapper.GetRemoteSetting(Constants.KEY_REWARDS_REALTIME) == "0") return false;

                        break;

                    default: return false;
                }
            }

            if (game.draw || !game.isOver) return false;

            if (game.puzzleData && game.puzzleData.pack)
                return game.puzzleData.pack.complete && !PlayerPrefsWrapper.GetRewardRewarded(game.puzzleData.pack.packID);
            else
                return !PlayerPrefsWrapper.GetGameRewarded(game.BoardID);
        }

        public void SetData(IClientFourzy game, bool addTimerRoutine = true)
        {
            //availability check
            if (!WillDisplayRewardsScreen(game)) return;

            rewards = new List<RewardsManager.Reward>();

            //collected items
            collectedItems = new List<RewardsManager.Reward>(game.collectedItems);
            rewards.AddRange(collectedItems);

            if (game.puzzleData != null) rewards.AddRange(game.puzzleData.rewards);
            else rewards.AddRange(RewardsManager.DoCheck(game));

            xpRewards = rewards.Where(reward => reward.rewardType == RewardType.XP).ToList();
            portalPointsRewards = rewards.Where(reward => reward.rewardType == RewardType.PORTAL_POINTS).ToList();

            //assign rewards
            rewards.AssignRewards();

            PlayerPrefsWrapper.SetGameRewarded(game.BoardID, true);

            previousXP = UserManager.Instance.xp;

            title.text = GameContentManager.Instance.themesDataHolder.GetTheme(game._Area).name;

            SetInteractable(true);
            canvasGroup.blocksRaycasts = true;

            xpTabAlphaTween.AtProgress(0f);
            portalKeysTabAlphaTween.AtProgress(0f);
            collectedItemsTabAlphaTween.AtProgress(0f);

            animationFinished = false;

            menuController.SetCurrentScreen(this);
            menuController.screensStack.Push(this);

            //remove old widgets
            foreach (RewardsScreenWidget widget in xpWidgets)
                Destroy(widget.gameObject);
            xpWidgets.Clear();

            foreach (RewardsScreenWidget widget in portalKeysWidgets)
                Destroy(widget.gameObject);
            portalKeysWidgets.Clear();

            foreach (RewardsScreenWidget widget in collectedItemsWidgets)
                Destroy(widget.gameObject);
            collectedItemsWidgets.Clear();

            StartRoutine("show", addTimerRoutine ? 3f : -1f, () => Open());
        }

        public override void Open()
        {
            base.Open();

            StartRoutine("xpTab", XPWidgetsRoutine(), () =>
            {
                if (xpRewards.Count > 0)
                {
                    xpTabAlphaTween.AtProgress(1f);
                    int xpWidgetsCount = xpWidgets.Count;
                    for (int index = xpWidgetsCount; index < xpRewards.Count; index++)
                        xpWidgets.Add(AddWidget(xpRewards[index], 
                            GameContentManager.GetPrefab<RewardsScreenWidget>(GameContentManager.PrefabType.REWARDS_XP), xpTab));

                    previousXP = UserManager.Instance.xp;
                    xpWidget.SetTo(UserManager.Instance.xp, false);

                    scrollRect.normalizedPosition = Vector2.zero;
                }

                StartRoutine("portalKeysTab", PortalKeysWidgetsRoutine(), () =>
                {
                    if (portalPointsRewards.Count > 0)
                    {
                        portalKeysTabAlphaTween.AtProgress(1f);
                        int portalKeysCount = portalKeysWidgets.Count;
                        for (int index = portalKeysWidgets.Count; index < portalPointsRewards.Count; index++)
                            portalKeysWidgets.Add(AddWidget(portalPointsRewards[index], GameContentManager.GetPrefab<RewardsScreenWidget>(GameContentManager.PrefabType.REWARDS_PORTAL_POINTS), portalKeysTab));

                        //add total if needed
                        if (portalKeysWidgets.Count == portalPointsRewards.Count)
                            portalKeysWidgets.Add(AddWidget(new RewardsManager.Reward(portalPointsRewards.Sum(reward => reward.quantity), RewardType.PORTAL_POINTS, "Total"), GameContentManager.GetPrefab<RewardsScreenWidget>(GameContentManager.PrefabType.REWARDS_PORTAL_POINTS), portalKeysTab));

                        scrollRect.normalizedPosition = Vector2.zero;
                    }

                    StartRoutine("collectedItemsTab", CollectedItemsWidgetsRoutine(), () =>
                    {
                        if (collectedItems.Count > 0)
                        {
                            collectedItemsTabAlphaTween.AtProgress(1f);
                            int collectedItemsCount = collectedItemsWidgets.Count;
                            for (int index = collectedItemsWidgets.Count; index < collectedItems.Count; index++)
                                collectedItemsWidgets.Add(
                                    AddWidget(collectedItems[index], GameContentManager.GetPrefab<RewardsScreenWidget>(collectedItems[index].rewardType.AsPrefabType()), collectedItemsTab));

                            scrollRect.normalizedPosition = Vector2.zero;
                        }

                        OpenFinished();
                    });
                });
            });
        }

        public override void OnBack()
        {
            if (!animationFinished)
            {
                Next(); return;
            }

            base.OnBack();
        }

        public void Next()
        {
            if (IsRoutineActive("show"))
            {
                CancelRoutine("show");
                return;
            }

            if (IsRoutineActive("xpTab"))
            {
                CancelRoutine("xpTab");
                return;
            }

            if (IsRoutineActive("portalKeysTab"))
            {
                CancelRoutine("portalKeysTab");
                return;
            }

            if (IsRoutineActive("collectedItemsTab"))
            {
                CancelRoutine("collectedItemsTab");
                return;
            }
        }

        private void OpenFinished()
        {
            animationFinished = true;
        }

        private RewardsScreenWidget AddWidget(RewardsManager.Reward reward, RewardsScreenWidget prefab, RectTransform parent)
        {
            RewardsScreenWidget widget = Instantiate(prefab, parent);
            widget.transform.localScale = Vector3.one;
            widget.SetData(reward);
            widget.Show(.5f);
            widget.ScaleTo(Vector3.one, .5f);

            return widget;
        }

        private IEnumerator XPWidgetsRoutine()
        {
            yield return new WaitForSeconds(.5f);

            if (xpRewards.Count > 0)
            {
                xpTab.gameObject.SetActive(true);
                xpTabAlphaTween.PlayForward(true);
            }
            else
            {
                xpTab.gameObject.SetActive(false);
                yield break;
            }

            foreach (RewardsManager.Reward reward in xpRewards)
            {
                xpWidgets.Add(AddWidget(reward, GameContentManager.GetPrefab<RewardsScreenWidget>(GameContentManager.PrefabType.REWARDS_XP), xpTab));

                previousXP += reward.quantity;
                xpWidget.SetTo(previousXP, true);

                scrollRect.normalizedPosition = Vector2.zero;

                yield return new WaitForSeconds(.35f);
            }
        }

        private IEnumerator PortalKeysWidgetsRoutine()
        {
            yield return new WaitForSeconds(.5f);

            if (portalPointsRewards.Count > 0)
            {
                portalKeysTab.gameObject.SetActive(true);
                portalKeysTabAlphaTween.PlayForward(true);
            }
            else
            {
                portalKeysTab.gameObject.SetActive(false);
                yield break;
            }

            foreach (RewardsManager.Reward reward in portalPointsRewards)
            {
                portalKeysWidgets.Add(AddWidget(reward, 
                    GameContentManager.GetPrefab<RewardsScreenWidget>(GameContentManager.PrefabType.REWARDS_PORTAL_POINTS), portalKeysTab));

                scrollRect.normalizedPosition = Vector2.zero;

                yield return new WaitForSeconds(.35f);
            }

            if (portalKeysWidgets.Count == 0) yield break;

            //add total
            portalKeysWidgets.Add(AddWidget(new RewardsManager.Reward(portalPointsRewards.Sum(reward => reward.quantity), RewardType.PORTAL_POINTS, "Total"), GameContentManager.GetPrefab<RewardsScreenWidget>(GameContentManager.PrefabType.REWARDS_PORTAL_POINTS), portalKeysTab));

            scrollRect.normalizedPosition = Vector2.zero;
        }

        //collected items tab
        private IEnumerator CollectedItemsWidgetsRoutine()
        {
            yield return new WaitForSeconds(.5f);

            if (collectedItems.Count > 0)
            {
                collectedItemsTab.gameObject.SetActive(true);
                collectedItemsTabAlphaTween.PlayForward(true);
            }
            else
            {
                collectedItemsTab.gameObject.SetActive(false);
                yield break;
            }

            foreach (RewardsManager.Reward reward in collectedItems)
            {
                collectedItemsWidgets.Add(AddWidget(reward, GameContentManager.GetPrefab<RewardsScreenWidget>(reward.rewardType.AsPrefabType()), collectedItemsTab));

                scrollRect.normalizedPosition = Vector2.zero;

                yield return new WaitForSeconds(.35f);
            }
        }
    }
}
