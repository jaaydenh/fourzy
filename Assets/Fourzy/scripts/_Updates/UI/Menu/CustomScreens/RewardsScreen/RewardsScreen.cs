//@vadym udod

using Fourzy._Updates.Mechanics.Rewards;
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
        public RectTransform xpTab;
        public RewardsScreenWidget xpWidgetPrefab;
        public CurrencyWidget xpWidget;

        public RectTransform portalKeysTab;
        public RewardsScreenWidget portalKeysWidgetPrefab;
        public RewardsScreenWidget totalPortalKeysWidgetPrefab;

        public RectTransform collectedItemsTab;

        private AlphaTween xpTabAlphaTween;
        private AlphaTween portalKeysTabAlphaTween;
        private AlphaTween collectedItemsTabAlphaTween;

        private List<RewardsManager.Reward> xpRewards;
        private List<RewardsManager.Reward> portalKeysRewards;

        private List<RewardsScreenWidget> xpWidgets = new List<RewardsScreenWidget>();
        private List<RewardsScreenWidget> portalKeysWidgets = new List<RewardsScreenWidget>();
        private List<RewardsScreenWidget> collectedItemsWidgets = new List<RewardsScreenWidget>();

        private ScrollRect scrollRect;

        private bool animationFinished;
        private int previousXP;

        private RewardsManager.RewardTestSubject subject;

        protected override void Awake()
        {
            base.Awake();

            xpTabAlphaTween = xpTab.GetComponent<AlphaTween>();
            portalKeysTabAlphaTween = portalKeysTab.GetComponent<AlphaTween>();
            collectedItemsTabAlphaTween = collectedItemsTab.GetComponent<AlphaTween>();
            scrollRect = GetComponentInChildren<ScrollRect>();
        }

        public void SetData(RewardsManager.RewardTestSubject subject)
        {
            title.text = GameContentManager.Instance.themesDataHolder.GetTheme(subject.state.Board.Area).name;
            this.subject = subject;

            //assign all rewards
            xpRewards = RewardsManager.FakeRewards();
            //xpRewards = RewardsManager.DoCheck(RewardsManager.RewardType.XP, subject);
            portalKeysRewards = RewardsManager.FakeRewards();
            //portalKeysRewards = RewardsManager.DoCheck(RewardsManager.RewardType.XP, subject);

            previousXP = UserManager.Instance.xp;
            //add xp to user manager
            UserManager.Instance.xp += xpRewards.Sum(reward => reward.quantity);

            //add portal keys to user manager
            UserManager.Instance.portalPoints += portalKeysRewards.Sum(reward => reward.quantity);

            subject.collectedItems.ForEach(collectedItemReward =>
            {
                switch (collectedItemReward.type)
                {
                    //assign collected items
                    case RewardsManager.CollectedItemType.COINS:
                        UserManager.Instance.coins += collectedItemReward.quantity;
                        break;

                    case RewardsManager.CollectedItemType.TICKETS:
                        UserManager.Instance.tickets += collectedItemReward.quantity;
                        break;
                }
            });

            PlayerPrefsWrapper.SetGameRewarded(subject.gameID);

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

            StartRoutine("show", 3f, () => Open());
        }

        public override void Open()
        {
            base.Open();
            
            StartRoutine("xpTab", XPWidgetsRoutine(), () =>
            {
                xpTabAlphaTween.AtProgress(1f);
                int xpWidgetsCount = xpWidgets.Count;
                for (int index = xpWidgetsCount; index < xpRewards.Count; index++)
                    xpWidgets.Add(AddWidget(xpRewards[index], xpWidgetPrefab, xpTab));

                previousXP = UserManager.Instance.xp;
                xpWidget.SetTo(UserManager.Instance.xp, false);

                scrollRect.normalizedPosition = Vector2.zero;

                StartRoutine("portalKeysTab", PortalKeysWidgetsRoutine(), () =>
                {
                    portalKeysTabAlphaTween.AtProgress(1f);
                    int portalKeysCount = portalKeysWidgets.Count;
                    for (int index = portalKeysWidgets.Count; index < portalKeysRewards.Count; index++)
                        portalKeysWidgets.Add(AddWidget(portalKeysRewards[index], portalKeysWidgetPrefab, portalKeysTab));

                    //add total if needed
                    if (portalKeysWidgets.Count == portalKeysRewards.Count)
                        portalKeysWidgets.Add(AddWidget(new RewardsManager.Reward("Total", portalKeysRewards.Sum(reward => reward.quantity)), totalPortalKeysWidgetPrefab, portalKeysTab));

                    scrollRect.normalizedPosition = Vector2.zero;

                    StartRoutine("collectedItemsTab", CollectedItemsWidgetsRoutine(), () =>
                    {
                        collectedItemsTabAlphaTween.AtProgress(1f);
                        int collectedItemsCount = collectedItemsWidgets.Count;
                        for (int index = collectedItemsWidgets.Count; index < subject.collectedItems.Count; index++)
                            collectedItemsWidgets.Add(
                                AddWidget(subject.collectedItems[index], GameContentManager.GetPrefab<RewardsScreenWidget>(subject.collectedItems[index].asPrefabType), collectedItemsTab));

                        scrollRect.normalizedPosition = Vector2.zero;
                        OpenFinished();
                    });
                });
            });
        }

        public override void OnBack()
        {
            //cant close till display is over
            if (!animationFinished)
            {
                Next();
                return;
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
                xpWidgets.Add(AddWidget(reward, xpWidgetPrefab, xpTab));

                previousXP += reward.quantity;
                xpWidget.SetTo(previousXP, true);

                scrollRect.normalizedPosition = Vector2.zero;

                yield return new WaitForSeconds(.35f);
            }
        }

        private IEnumerator PortalKeysWidgetsRoutine()
        {
            yield return new WaitForSeconds(.5f);

            if (portalKeysRewards.Count > 0)
            {
                portalKeysTab.gameObject.SetActive(true);
                portalKeysTabAlphaTween.PlayForward(true);
            }
            else
            {
                portalKeysTab.gameObject.SetActive(false);
                yield break;
            }
            
            foreach (RewardsManager.Reward reward in portalKeysRewards)
            {
                portalKeysWidgets.Add(AddWidget(reward, portalKeysWidgetPrefab, portalKeysTab));

                scrollRect.normalizedPosition = Vector2.zero;

                yield return new WaitForSeconds(.35f);
            }

            if (portalKeysWidgets.Count == 0) yield break;
            
            //add total
            portalKeysWidgets.Add(AddWidget(new RewardsManager.Reward("Total", portalKeysRewards.Sum(reward => reward.quantity)), totalPortalKeysWidgetPrefab, portalKeysTab));

            scrollRect.normalizedPosition = Vector2.zero;
        }

        //collected items tab
        private IEnumerator CollectedItemsWidgetsRoutine()
        {
            yield return new WaitForSeconds(.5f);

            if (subject.collectedItems.Count > 0)
            {
                collectedItemsTab.gameObject.SetActive(true);
                collectedItemsTabAlphaTween.PlayForward(true);
            }
            else
            {
                collectedItemsTab.gameObject.SetActive(false);
                yield break;
            }
            
            foreach (RewardsManager.CollectedItemReward reward in subject.collectedItems)
            {
                collectedItemsWidgets.Add(AddWidget(reward, GameContentManager.GetPrefab<RewardsScreenWidget>(reward.asPrefabType), collectedItemsTab));

                scrollRect.normalizedPosition = Vector2.zero;

                yield return new WaitForSeconds(.35f);
            }
        }
    }
}
