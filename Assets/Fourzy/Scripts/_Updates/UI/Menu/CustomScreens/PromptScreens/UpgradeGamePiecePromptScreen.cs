//@vadym udod


using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class UpgradeGamePiecePromptScreen : PromptScreen
    {
        public ButtonExtended selectButton;
        public ButtonExtended upgradeButton;

        private GamePieceData data;
        private GamePieceWidgetSmall gamePieceWidget;

        protected override void Awake()
        {
            base.Awake();

            gamePieceWidget = GetComponentInChildren<GamePieceWidgetSmall>();
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
                UserManager.Instance.gamePieceID != data.Id &&
                data.State == GamePieceState.FoundAndUnlocked);
        }
    }
}