//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Menu.Screens;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class PuzzlePackProgressWidget : WidgetBase
    {
        public SliderExtended slider;
        public RectTransform markerParent;
        public RectTransform rewardsParent;
        public TMP_Text markerPrefab;
        public Image iconPrefab;
        public RectTransform rewardsContainerPrefab;

        private List<GameObject> markers = new List<GameObject>();
        private Dictionary<int, Image> icons = new Dictionary<int, Image>();
        private List<GameObject> containers = new List<GameObject>();

        public List<RewardsScreenWidget> widgets { get; private set; }
        public BasicPuzzlePack puzzlePack { get; private set; }

        //private int lastIndex;

        public override void _Update()
        {
            base._Update();

            if (puzzlePack == null) return;
            
            //update slider
            slider.value = (float)puzzlePack.puzzlesComplete.Count / puzzlePack.enabledPuzzlesData.Count;

            //update stars
            for (int puzzleIndex = 0; puzzleIndex < puzzlePack.enabledPuzzlesData.Count; puzzleIndex++)
                if (icons.ContainsKey(puzzleIndex))
                    icons[puzzleIndex].sprite = puzzlePack.enabledPuzzlesData[puzzleIndex].currentProgressionIcon;
        }

        public PuzzlePackProgressWidget SetData(BasicPuzzlePack puzzlePack)
        {
            this.puzzlePack = puzzlePack;

            widgets = new List<RewardsScreenWidget>();
            CancelRoutine("animateRewards");

            Clear();

            ClientPuzzleData lastPuzzleData = null;
            int maxSize = 0;
            int index;
            float rewardWidgetSampleHeight = 0f;
            float positionX;

            foreach (ClientPuzzleData puzzleData in puzzlePack.rewardPuzzles)
            {
                if (puzzleData.lastInPack) lastPuzzleData = puzzleData;

                index = puzzlePack.enabledPuzzlesData.IndexOf(puzzleData);
                positionX = (float)(index + 1) / puzzlePack.enabledPuzzlesData.Count;

                AddMarkerIcon(puzzleData.currentProgressionIcon, index, positionX);

                //create rewards container
                RectTransform containerInstance = Instantiate(rewardsContainerPrefab, rewardsParent);
                containers.Add(containerInstance.gameObject);
                containerInstance.gameObject.SetActive(true);
                containerInstance.anchorMin = containerInstance.anchorMax = new Vector2(positionX, 1f);

                if (puzzleData.rewards.Length > maxSize) maxSize = puzzleData.rewards.Length;
                //create rewards
                foreach (RewardsManager.Reward reward in puzzleData.rewards)
                {
                    RewardsScreenWidget rewardWidget =
                        Instantiate(GameContentManager.GetPrefab<RewardsScreenWidget>(reward.rewardType.AsPrefabType()), containerInstance);
                    rewardWidget.ResetAnchors();
                    //rewardWidget.ScaleTo(Vector3.one * .7f, 0f);

                    //configure it
                    rewardWidget.SetData(puzzleData, reward);

                    if (rewardWidgetSampleHeight == 0f) rewardWidgetSampleHeight = rewardWidget.rectTransform.rect.height;

                    widgets.Add(rewardWidget);
                }
            }

            if (!lastPuzzleData)
            {
                lastPuzzleData = puzzlePack.enabledPuzzlesData[puzzlePack.enabledPuzzlesData.Count - 1];
                AddMarkerIcon(lastPuzzleData.currentProgressionIcon, puzzlePack.enabledPuzzlesData.Count - 1, 1f);
            }

            rewardsParent.GetComponent<LayoutElement>().minHeight =
                (rewardWidgetSampleHeight * maxSize) + ((maxSize - 1) * rewardsContainerPrefab.GetComponent<VerticalLayoutGroup>().spacing);

            return this;
        }

        public PuzzlePackProgressWidget Clear()
        {
            //clear markers
            foreach (GameObject markerGO in markers) Destroy(markerGO);
            markers.Clear();
            //clear icons
            foreach (Image icon in icons.Values) Destroy(icon.gameObject);
            icons.Clear();
            //clear reward containers
            foreach (GameObject containerGO in containers) Destroy(containerGO);
            containers.Clear();
            widgets.Clear();

            return this;
        }

        public PuzzlePackProgressWidget CheckWidgets()
        {
            widgets.ForEach(widget => widget._Update());

            return this;
        }

        public void CancelWidgetsRewardAnimation()
        { 
            if (widgets == null) return;

            CancelRoutine("animateRewards");
            widgets.ForEach(widget => widget.CanceRewardlAnimation());
        }

        public PuzzlePackProgressWidget AnimateRewardsForIndex(int index, float delay = .5f)
        {
            StartRoutine("animateRewards", AnimateRewardsRoutine(index, delay));

            return this;
        }

        private void AddMarkerIcon(Sprite icon, int index, float positionX)
        {
            TMP_Text markerInstance = Instantiate(markerPrefab, markerParent);
            markers.Add(markerInstance.gameObject);
            markerInstance.gameObject.SetActive(true);

            markerInstance.text = (index + 1) + "";
            markerInstance.rectTransform.anchorMin = markerInstance.rectTransform.anchorMax = new Vector2(positionX, 0f);

            Image iconInstance = Instantiate(iconPrefab, markerParent);
            icons.Add(index, iconInstance);
            iconInstance.gameObject.SetActive(true);

            iconInstance.preserveAspect = true;
            iconInstance.sprite = icon;
            iconInstance.rectTransform.anchorMin = iconInstance.rectTransform.anchorMax = new Vector2(positionX, .35f);
        }

        private IEnumerator AnimateRewardsRoutine(int index, float delay)
        {
            while (IsRoutineActive("spawnUIElements")) yield return null;

            List <RewardsScreenWidget> widgets = new List<RewardsScreenWidget>();

            if (puzzlePack.puzzlesComplete[index].rewards.Length > 0)
                widgets.AddRange(this.widgets.Where(_widget => puzzlePack.puzzlesComplete[index].rewards.Contains(_widget.reward)));

            if (widgets.Count == 0) yield break;

            foreach (RewardsScreenWidget widget in widgets) widget.SetChecked(false);

            yield return new WaitForSeconds(delay);

            foreach (RewardsScreenWidget widget in widgets)
            {
                widget.ScaleTo(Vector3.one, Vector3.one * 1.2f, .35f);

                yield return new WaitForSeconds(.25f);

                //animate reward
                widget.AnimateReward();

                widget.SetChecked(true);

                yield return new WaitForSeconds(.25f);
            }
        }
    }
}