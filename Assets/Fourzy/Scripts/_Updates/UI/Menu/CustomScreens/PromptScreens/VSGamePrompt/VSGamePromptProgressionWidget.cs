//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class VSGamePromptProgressionWidget : WidgetBase
    {
        public Image bg;
        public Image checkedImage;
        public RectTransform container;

        private BasicPuzzlePack puzzlePack;
        private ClientPuzzleData puzzleData;

        public override void _Update()
        {
            base._Update();

            if (puzzlePack != null && puzzleData != null)
                SetChecked(puzzlePack.puzzlesComplete.Contains(puzzleData) ? puzzleData.progressionIconSet : puzzleData.progressionIconEmpty);
        }

        public VSGamePromptProgressionWidget SetData(BasicPuzzlePack puzzlePack, ClientPuzzleData puzzleData)
        {
            this.puzzlePack = puzzlePack;
            this.puzzleData = puzzleData;

            SetChecked(puzzlePack.puzzlesComplete.Contains(puzzleData) ? puzzleData.progressionIconSet : puzzleData.progressionIconEmpty);

            return this;
        }

        public VSGamePromptProgressionWidget SetChecked(Sprite sprite)
        {
            if (sprite == null)
                checkedImage.color = Color.clear;
            else
            {
                checkedImage.color = Color.white;
                checkedImage.sprite = sprite;
            }

            return this;
        }

        public VSGamePromptProgressionWidget SetSprite(Sprite sprite)
        {
            bg.sprite = sprite;

            return this;
        }

        public VSGamePromptProgressionWidget SetReward(BasicPuzzlePack puzzlePack, ClientPuzzleData puzzleData, RewardsManager.Reward reward)
        {
            //spawn reward
            RewardsScreenWidget rewardWidget =
                Instantiate(GameContentManager.GetPrefab<RewardsScreenWidget>(reward.rewardType.AsPrefabType()), container);
            rewardWidget.ResetAnchors();

            //configure it
            rewardWidget
                .SetData(puzzleData, reward)
                .SetChecked(puzzlePack.puzzlesComplete.Contains(puzzleData));

            rewardWidget.transform.localScale = Vector3.one * .5f;

            return this;
        }
    }
}