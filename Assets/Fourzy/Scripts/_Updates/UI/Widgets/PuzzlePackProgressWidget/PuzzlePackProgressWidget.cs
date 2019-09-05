//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Helpers;
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
        private List<Image> icons = new List<Image>();
        private List<GameObject> containers = new List<GameObject>();

        public List<RewardsScreenWidget> widgets { get; private set; }
        public PuzzlePacksDataHolder.BasicPuzzlePack puzzlePack { get; private set; }

        private int lastIndex;

        public override void _Update()
        {
            base._Update();

            if (puzzlePack == null) return;
            
            //update slider
            slider.value = (float)puzzlePack.puzzlesComplete.Count / puzzlePack.enabledPuzzlesData.Count;

            //update stars
            for (int rewardPuzzleIndex = 0; rewardPuzzleIndex < puzzlePack.rewardPuzzles.Count; rewardPuzzleIndex++)
                icons[rewardPuzzleIndex].sprite =
                    puzzlePack.puzzlesComplete.Contains(puzzlePack.rewardPuzzles[rewardPuzzleIndex]) ?
                    puzzlePack.rewardPuzzles[rewardPuzzleIndex].progressionIconSet : puzzlePack.rewardPuzzles[rewardPuzzleIndex].progressionIconEmpty;
        }

        public PuzzlePackProgressWidget SetData(PuzzlePacksDataHolder.BasicPuzzlePack puzzlePack)
        {
            this.puzzlePack = puzzlePack;

            widgets = new List<RewardsScreenWidget>();
            CancelRoutine("animateRewards");

            Clear();

            int maxSize = 0;
            float rewardWidgetSampleHeight = 0f;
            //add new markers
            //add new icons
            foreach (ClientPuzzleData puzzleData in puzzlePack.rewardPuzzles)
            {
                lastIndex = puzzlePack.enabledPuzzlesData.IndexOf(puzzleData);
                float positionX = (float)(lastIndex + 1) / puzzlePack.enabledPuzzlesData.Count;

                TMP_Text markerInstance = Instantiate(markerPrefab, markerParent);
                markers.Add(markerInstance.gameObject);
                markerInstance.gameObject.SetActive(true);

                markerInstance.text = (lastIndex + 1) + "";
                markerInstance.rectTransform.anchorMin = markerInstance.rectTransform.anchorMax = new Vector2(positionX, .5f);

                Image iconInstance = Instantiate(iconPrefab, markerParent);
                icons.Add(iconInstance);
                iconInstance.gameObject.SetActive(true);

                iconInstance.preserveAspect = true;
                iconInstance.sprite = puzzlePack.puzzlesComplete.Contains(puzzleData) ? puzzleData.progressionIconSet : puzzleData.progressionIconEmpty;
                iconInstance.rectTransform.anchorMin = iconInstance.rectTransform.anchorMax = new Vector2(positionX, .45f);

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

                    //configure it
                    rewardWidget.SetData(puzzleData, reward);

                    if (rewardWidgetSampleHeight == 0f) rewardWidgetSampleHeight = rewardWidget.rectTransform.rect.height;

                    widgets.Add(rewardWidget);
                }
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
            foreach (Image icon in icons) Destroy(icon.gameObject);
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

        public PuzzlePackProgressWidget AnimateRewardsForIndex(int index, float delay = .5f)
        {
            StartRoutine("animateRewards", AnimateRewardsRoutine(index, delay));

            return this;
        }

        private IEnumerator AnimateRewardsRoutine(int index, float delay)
        {
            while (IsRoutineActive("spawnUIElements")) yield return null;

            List <RewardsScreenWidget> widgets = new List<RewardsScreenWidget>();

            if (puzzlePack.puzzlesComplete[index].rewards.Length > 0)
                widgets.AddRange(this.widgets.Where(_widget => puzzlePack.puzzlesComplete[index].rewards.Contains(_widget.reward)));

            if (widgets.Count == 0)
            {
                yield return null;
                yield break;
            }
            
            foreach (RewardsScreenWidget widget in widgets) widget.SetChecked(false);

            yield return new WaitForSeconds(delay);

            foreach (RewardsScreenWidget widget in widgets)
            {
                widget.ScaleTo(Vector3.one, Vector3.one * 1.2f, .35f);

                yield return new WaitForSeconds(.25f);

                widget.SetChecked(true);

                yield return new WaitForSeconds(.25f);
            }

            yield return null;
            yield break;
        }
    }
}