//@vadym udod

using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Widgets;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PuzzleSelectionScreen : MenuScreen
    {
        public AIPlayerUIWidget aiPlayerWidgetPrefab;
        public RectTransform aiPlayersParent;
        
        public GridLayoutGroup gridLayoutGroup;

        public override void Open()
        {
            base.Open();

            OnInitialized();

            LoadAIPlayerWidgets();
        }

        public override void OnBack()
        {
            base.OnBack();

            menuController.CloseCurrentScreen();
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

        }
    }
}
