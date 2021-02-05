//@vadym udod

using Fourzy._Updates.UI.Camera3D;
using TMPro;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class ProgressionMapScreen : MenuScreen
    {
        public Camera3dItemToImageProgressionMap mapContent;
        public TMP_Text completeText;

        public override void Open()
        {
            base.Open();

            mapContent.UpdateWidgets();

            completeText.text = $"{mapContent._item.totalGamesComplete} / {mapContent._item.totalGames}";

            HeaderScreen.Instance.Close();
        }

        public override void OnBack()
        {
            base.OnBack();

            mapContent._item.gameObject.SetActive(false);
        }

        public void Open(Camera3dItemProgressionMap map)
        {
            mapContent.LoadOther(map);

            menuController.OpenScreen(this);
        }
    }
}