//@vadym udod

using Fourzy._Updates.UI.Camera3D;
using Fourzy._Updates.UI.Menu.Screens;
using TMPro;

namespace Fourzy._Updates.UI.Widgets
{
    public class AdventureWidget : WidgetBase
    {
        public TMP_Text nameField;
        public TMP_Text completeField;

        public Camera3dItemProgressionMap map { get; private set; }

        public AdventureWidget SetData(Camera3dItemProgressionMap data)
        {
            map = data;

            nameField.text = data.name;
            completeField.text = "not definied yet..";

            return this;
        }

        public void OnTap()
        {
            //select
            menuScreen.menuController.GetScreen<ProgressionMapScreen>().Open(map);
        }
    }
}