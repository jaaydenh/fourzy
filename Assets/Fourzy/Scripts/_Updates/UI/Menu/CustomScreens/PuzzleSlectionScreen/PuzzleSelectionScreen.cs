﻿//@vadym udod

using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Widgets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PuzzleSelectionScreen : MenuScreen
    {
        public AIPlayerUIWidget aiPlayerWidgetPrefab;
        public RectTransform aiPlayersParent;

        public PuzzlePackWidget puzzlePackPrefab;
        public GridLayoutGroup gridLayoutGroup;
        public TMP_Text completeText;

        public override void Open()
        {
            base.Open();

            LoadPuzzlePacks();
            LoadAIPlayerWidgets();
        }

        public void LoadPuzzlePacks()
        {
            //remove old one
            foreach (Transform child in gridLayoutGroup.transform)
                Destroy(child.gameObject);

            foreach (PuzzlePacksDataHolder.PuzzlePack puzzlePack in GameContentManager.Instance.puzzlePacks)
            {
                PuzzlePackWidget puzzlePackWidgetInstance = Instantiate(puzzlePackPrefab, gridLayoutGroup.transform);
                puzzlePackWidgetInstance.SetData(puzzlePack);
            }

            completeText.text = $"{GameContentManager.Instance.puzzlePacksDataHolder.totalPuzzlesCompleteCount} / {GameContentManager.Instance.puzzlePacksDataHolder.totalPuzzlesCount}";
        }

        public void LoadAIPlayerWidgets()
        {
            foreach (Transform child in aiPlayersParent)
                Destroy(child.gameObject);

            foreach (AIPlayersDataHolder.AIPlayerData aiPlayer in GameContentManager.Instance.aiPlayersDataHolder.enabledAIPlayers)
            {
                AIPlayerUIWidget widget = Instantiate(aiPlayerWidgetPrefab, aiPlayersParent);
                widget.transform.localScale = Vector3.one;

                widget.SetData(aiPlayer);
            }
        }
    }
}
