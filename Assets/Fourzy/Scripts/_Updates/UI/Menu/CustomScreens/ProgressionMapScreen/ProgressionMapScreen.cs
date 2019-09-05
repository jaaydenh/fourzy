//@vadym udod

using Fourzy._Updates.UI.Camera3D;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class ProgressionMapScreen : MenuScreen
    {
        public Camera3dItemToImageProgressionMap mapContent;

        public override void Open()
        {
            base.Open();

            mapContent.UpdateWidgets();
        }
    }
}