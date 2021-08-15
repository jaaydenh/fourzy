//@vadym udod


using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class AreasProgressionScreen : MenuScreen
    {
        [SerializeField]
        private RectTransform areasHolder;
        [SerializeField]
        private RectTransform rewardsHolder;
        [SerializeField]
        private Badge unlockConditionLabel;
        [SerializeField]
        private Badge worldLabel;
        [SerializeField]
        private AreasProgressionScreen_Entry rewardPrefab;
        [SerializeField]
        private Image fillImage;
        [SerializeField]
        private Image fillParent;
        [SerializeField]
        private Badge progressMarker;

        private bool isPlayFabInitialized = false;
        private RectTransform progressMarkerRectTransform;

        private List<PracticeScreenAreaSelectWidget> areasWidgets = new List<PracticeScreenAreaSelectWidget>();
        private List<AreasProgressionScreen_Entry> entriesList = new List<AreasProgressionScreen_Entry>();
        private Dictionary<Area, List<(int number, CatalogItem item)>> progression;
        private PracticeScreenAreaSelectWidget currentAreaWidget;

        protected override void Awake()
        {
            base.Awake();

            progressMarkerRectTransform = progressMarker.GetComponent<RectTransform>();

            AreasDataHolder.onAreaUnlockChanged += OnAreaUnlockedChange;

            UserManager.onPlayfabValuesLoaded += OnPlayfabValueLoaded;
            UserManager.onAreaProgression += OnAreaProgression;
        }

        protected void OnDestroy()
        {
            AreasDataHolder.onAreaUnlockChanged -= OnAreaUnlockedChange;

            UserManager.onPlayfabValuesLoaded -= OnPlayfabValueLoaded;
            UserManager.onAreaProgression -= OnAreaProgression;
        }

        public override void Open()
        {
            base.Open();

            HeaderScreen.Instance.Close();
        }

        public void _Open(Area area)
        {
            SetArea(areasWidgets.Find(widget => widget.Area == area), false);
            menuController.OpenScreen(this);
        }

        private void OnAreaUnlockedChange(Area area, bool state)
        {
            if (!isPlayFabInitialized)
            {
                return;
            }

            PracticeScreenAreaSelectWidget widget = areasWidgets.Find(_widget => _widget.Area == area);

            widget.CheckArea(keepEnabled: true);
            OnAreaWidgetUpdate(widget);
        }

        private void SetArea(PracticeScreenAreaSelectWidget widget, bool updateSelectedArea)
        {
            if (!isPlayFabInitialized)
            {
                return;
            }

            if (currentAreaWidget && currentAreaWidget != widget)
            {
                currentAreaWidget.Deselect();
            }

            currentAreaWidget = widget;
            currentAreaWidget.Select();

            if (!currentAreaWidget.Disabled)
            {
                PlayerPrefsWrapper.SetCurrentArea((int)currentAreaWidget.Area);
            }

            OnAreaWidgetUpdate(widget);
        }

        private void OnAreaWidgetUpdate(PracticeScreenAreaSelectWidget widget)
        {
            if (!isPlayFabInitialized)
            {
                return;
            }

            if (currentAreaWidget.Disabled)
            {
                UpdateUnlockCondition();
                worldLabel.SetValue(0);
            }
            else
            {
                unlockConditionLabel.SetValue(0);
                worldLabel.SetValue($"WORLD {GameContentManager.Instance.areasDataHolder.areas.IndexOf(widget.AreaData) + 1}");
            }

            UpdateRewardsView();
        }

        private void UpdateUnlockCondition()
        {
            var unlockData = GetAreaUnlockItem(currentAreaWidget.Area);

            if (unlockData.area == Area.NONE)
            {
                unlockConditionLabel.SetValue($"CURRENTLY NOT AVAILABLE");
            }
            else
            {
                unlockConditionLabel.SetValue($"WIN {unlockData.entry.gamesNumber} GAMES IN <color=#{ColorUtility.ToHtmlStringRGB(Color.yellow)}>{GameContentManager.Instance.areasDataHolder[unlockData.area].Name}</color> TO UNLOCK");
            }
        }

        private void UpdateRewardsView()
        {
            foreach (AreasProgressionScreen_Entry entry in entriesList)
            {
                Destroy(entry.gameObject);
            }
            entriesList.Clear();

            int gamesNumber = UserManager.Instance.GetAreaProgression(currentAreaWidget.Area);
            bool useFillValue = true;
            int prevItemNumber = 0;
            foreach (var entry in progression[currentAreaWidget.Area])
            {
                AreasProgressionScreen_Entry newEntry = Instantiate(rewardPrefab, rewardsHolder);
                newEntry.gameObject.SetActive(true);
                newEntry.Initialize(entry.number, useFillValue ? (float)(Mathf.Min(gamesNumber, entry.number) - prevItemNumber) / (entry.number - prevItemNumber) : 0f, entry.item);

                if (useFillValue)
                {
                    useFillValue = gamesNumber > entry.number;
                    prevItemNumber = entry.number;
                }

                entriesList.Add(newEntry);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(rewardsHolder);

            //calc progressbar
            AreasProgressionScreen_Entry prev = null;
            float fillAmount = 0f;
            fillParent.rectTransform.sizeDelta = new Vector2(fillParent.rectTransform.sizeDelta.x, - entriesList.Last().rectTransform.rect.height * .5f);
            float totalHeight = fillParent.rectTransform.rect.height;
            for (int widgetIndex = 0; widgetIndex < entriesList.Count; widgetIndex++)
            {
                if (prev)
                {
                    fillAmount += (entriesList[widgetIndex].rectTransform.rect.height * .5f + prev.rectTransform.rect.height * .5f) * entriesList[widgetIndex].FillAmount / totalHeight;
                }
                else
                {
                    fillAmount += entriesList[widgetIndex].rectTransform.rect.height * .5f * entriesList[widgetIndex].FillAmount / totalHeight;
                }

                prev = entriesList[widgetIndex];
            }

            fillImage.fillAmount = fillAmount;
            progressMarker.SetValue(gamesNumber);
            progressMarkerRectTransform.anchoredPosition = new Vector2(progressMarkerRectTransform.anchoredPosition.x, totalHeight * fillAmount);
            progressMarker.transform.SetAsLastSibling();
        }

        private void OnPlayfabValueLoaded(PlayfabValuesLoaded value)
        {
            //init slider
            if (!isPlayFabInitialized && UserManager.Instance.IsPlayfabValueLoaded(PlayfabValuesLoaded.TITLE_DATA_RECEIVED, PlayfabValuesLoaded.PLAYER_STATS_RECEIVED, PlayfabValuesLoaded.CATALOG_INFO_RECEIVED))
            {
                progression = new Dictionary<Area, List<(int number, CatalogItem item)>>();

                progression.Add(Area.TRAINING_GARDEN, GetItemsFromProgression(InternalSettings.Current.TRAINING_GARDEN));
                progression.Add(Area.ENCHANTED_FOREST, GetItemsFromProgression(InternalSettings.Current.ENCHANTED_FOREST));
                progression.Add(Area.SANDY_ISLAND, GetItemsFromProgression(InternalSettings.Current.SANDY_ISLAND));
                progression.Add(Area.ICE_PALACE, GetItemsFromProgression(InternalSettings.Current.ICE_PALACE));

                isPlayFabInitialized = true;

                SetArea(areasWidgets[0], false);
            }

            List<(int number, CatalogItem item)> GetItemsFromProgression(AreaProgression areaProgression)
            {
                return areaProgression.progression.Select(item => (item.gamesNumber, GameContentManager.Instance.GetFirstInBundle(item.id))).ToList();
            }
        }

        private void OnAreaProgression(Area area, int gamesNumber)
        {
            if (!isPlayFabInitialized)
            {
                return;
            }

            if (area == currentAreaWidget.Area)
            {
                UpdateRewardsView();
            }
        }

        private (Area area, AreaProgressionEntry entry) GetAreaUnlockItem(Area area)
        {
            if (GameContentManager.Instance.allItemsInfo.ContainsKey(area.ToString()))
            {
                foreach (CatalogItem item in GameContentManager.Instance.allItemsInfo.Values)
                {
                    if (item.Bundle != null)
                    {
                        //check items in bundle
                        foreach (string bundleItem in item.Bundle.BundledItems)
                        {
                            if (area.ToString() == bundleItem)
                            {
                                AreaProgressionEntry result = InternalSettings.Current.TRAINING_GARDEN.progression.ToList().Find(_item => _item.id == item.ItemId);
                                if(result != null)
                                {
                                    return (Area.TRAINING_GARDEN, result);
                                }

                                result = InternalSettings.Current.ENCHANTED_FOREST.progression.ToList().Find(_item => _item.id == item.ItemId);
                                if (result != null)
                                {
                                    return (Area.ENCHANTED_FOREST, result);
                                }

                                result = InternalSettings.Current.SANDY_ISLAND.progression.ToList().Find(_item => _item.id == item.ItemId);
                                if (result != null)
                                {
                                    return (Area.SANDY_ISLAND, result);
                                }

                                result = InternalSettings.Current.ICE_PALACE.progression.ToList().Find(_item => _item.id == item.ItemId);
                                if (result != null)
                                {
                                    return (Area.ICE_PALACE, result);
                                }
                            }
                        }
                    }
                }
            }

            return (Area.NONE, null);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            foreach (AreasDataHolder.GameArea area in GameContentManager.Instance.areasDataHolder.areas)
            {
                PracticeScreenAreaSelectWidget widget = Instantiate(GameContentManager.GetPrefab<PracticeScreenAreaSelectWidget>("AREA_SELECT_WIDGET_SMALL"), areasHolder).SetData(area.areaID, keepEnabled: true);
                widget.transform.localScale = Vector3.one * .65f;
                widget.button.onTap += data => SetArea(widget, true);

                areasWidgets.Add(widget);
            }
        }
    }
}
