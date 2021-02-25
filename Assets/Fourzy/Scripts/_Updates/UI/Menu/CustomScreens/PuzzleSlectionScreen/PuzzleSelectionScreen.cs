//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Widgets;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PuzzleSelectionScreen : MenuScreen
    {
        public AIPlayerUIWidget aiPlayerWidgetPrefab;
        public RectTransform aiPlayersParent;
        public SelectPackWidget selectPackWidgetPrefab;
        public RectTransform puzzlePacks;

        protected override void Start()
        {
            base.Start();

            InitWidgets();
        }

        public override void OnBack()
        {
            base.OnBack();

            menuController.CloseCurrentScreen();
        }

        public void InitWidgets()
        {
            //ai widgets
            foreach (Transform child in aiPlayersParent) Destroy(child.gameObject);

            foreach (AIPlayersDataHolder.AIPlayerData aiPlayer in GameContentManager.Instance.aiPlayersDataHolder.enabledAIPlayers)
                Instantiate(aiPlayerWidgetPrefab, aiPlayersParent).SetData(aiPlayer);

            //pack widgets
            foreach (Transform child in puzzlePacks) Destroy(child.gameObject);
            foreach (BasicPuzzlePack pack in GameContentManager.Instance.externalPuzzlePacks.Values)
                Instantiate(selectPackWidgetPrefab, puzzlePacks).SetData(pack);
        }
    }
}
