//@vadym udod


using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class UpgradeGamePiecePromptScreen : PromptScreen
    {
        [SerializeField]
        private ButtonExtended selectButton;
        [SerializeField]
        private Transform gamepieceParent;

        private GamePieceData data;
        private GamePieceWidgetSmall gamePieceWidget;

        protected override void Awake()
        {
            base.Awake();

            gamePieceWidget = GameContentManager.InstantiatePrefab<GamePieceWidgetSmall>(
                "GAME_PIECE_SMALL",
                gamepieceParent);
            gamePieceWidget.SetAnchors(Vector2.one * .5f);
            gamePieceWidget.SetLocalPosition(Vector3.zero);
        }

        public override void Open()
        {
            base.Open();

            CheckUpdate();
            CheckSelected();
        }

        public void Prompt(GamePieceData data)
        {
            this.data = data;
            gamePieceWidget.SetData(data);

            Prompt(data.name, "", () =>
            {
                //data.Upgrade();
                //gamePieceWidget._Update();

                CheckSelected();
            });
        }

        public void SelectGamepiece()
        {
            UserManager.Instance.UpdateSelectedGamePiece(data.Id);

            //close screen
            menuController.CloseCurrentScreen();
        }

        private void CheckUpdate()
        {
            UpdateAcceptButton("");
        }

        private void CheckSelected()
        {
            selectButton.SetActive(
                UserManager.Instance.gamePieceId != data.Id &&
                data.State == GamePieceState.FoundAndUnlocked);
        }
    }
}