//@vadym udod


using Fourzy._Updates.UI.Widgets;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class UpgradeGamePiecePromptScreen : PromptScreen
    {
        private GamePieceData data;
        private GamePieceWidgetSmall gamePieceWidget;

        protected override void Awake()
        {
            base.Awake();

            gamePieceWidget = GetComponentInChildren<GamePieceWidgetSmall>();
        }

        public void Prompt(GamePieceData data)
        {
            this.data = data;
            gamePieceWidget.SetData(data);

            Prompt(data.name, "", () =>
            {
                data.Upgrade();
                gamePieceWidget._Update();

                //can upgrade further?
                CheckUpdate();
            });

            CheckUpdate();
        }

        public void CheckUpdate()
        {
            if (data.CanUpgrade)
                UpdateAcceptButton("Upgrade");
            else
                UpdateAcceptButton("");
        }

        public override void Accept()
        {
            if (onAccept != null)
                onAccept.Invoke();
        }
    }
}