//@vadym udod

using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Widgets;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PuzzleSelectionScreen : MenuScreen
    {
        public AIPlayerUIWidget aiPlayerWidgetPrefab;
        public RectTransform aiPlayersParent;
        
        public GridLayoutGroup gridLayoutGroup;
        public TMP_Text completeText;

        private List<PuzzlePackWidget> puzzlePacksWidgets = new List<PuzzlePackWidget>();
        private PuzzlePackWidget puzzlePackPrefab;

        protected override void Awake()
        {
            base.Awake();

            puzzlePackPrefab = GameContentManager.GetPrefab<PuzzlePackWidget>(GameContentManager.PrefabType.PUZZLE_PACK_WIDGET);
        }

        public override void Open()
        {
            base.Open();

            OnInitialized();

            LoadAIPlayerWidgets();
            //LoadPuzzlePacks(GameContentManager.Instance.puzzlePacksDataHolder);
        }

        public override void OnBack()
        {
            base.OnBack();

            menuController.CloseCurrentScreen();
        }

        public void LoadPuzzlePacks(PuzzlePacksDataHolder puzzlePacksHolder)
        {
            puzzlePacksWidgets.Clear();

            //remove old one
            foreach (Transform child in gridLayoutGroup.transform) Destroy(child.gameObject);

            foreach (PuzzlePacksDataHolder.PuzzlePack puzzlePack in puzzlePacksHolder.puzzlePacks.list)
            {
                PuzzlePackWidget puzzlePackWidgetInstance = Instantiate(puzzlePackPrefab, gridLayoutGroup.transform);
                puzzlePackWidgetInstance.SetData(puzzlePack);

                puzzlePacksWidgets.Add(puzzlePackWidgetInstance);
            }

            completeText.text = $"{puzzlePacksHolder.totalPuzzlesCompleteCount} / {puzzlePacksHolder.totalPuzzlesCount}";
        }

        public void LoadAIPlayerWidgets()
        {
            foreach (Transform child in aiPlayersParent) Destroy(child.gameObject);

            foreach (AIPlayersDataHolder.AIPlayerData aiPlayer in GameContentManager.Instance.aiPlayersDataHolder.enabledAIPlayers)
            {
                AIPlayerUIWidget widget = Instantiate(aiPlayerWidgetPrefab, aiPlayersParent);
                widget.transform.localScale = Vector3.one;

                widget.SetData(aiPlayer);
            }
        }

        public override void ExecuteMenuEvent(MenuEvents menuEvent)
        {
            //respond to puzzle pack menu event
            if (menuEvent.data.ContainsKey("puzzlePack"))
            {
                if (menuController.currentScreen != this)
                {
                    //back to root
                    menuController.BackToRoot();
                    //open this screen
                    menuController.OpenScreen(this);
                }

                puzzlePacksWidgets.Find(widget => widget.puzzlePack.packID == (menuEvent["puzzlePack"] as PuzzlePacksDataHolder.PuzzlePack).packID).PlayCompleteAnimation();
            }
        }
    }
}
