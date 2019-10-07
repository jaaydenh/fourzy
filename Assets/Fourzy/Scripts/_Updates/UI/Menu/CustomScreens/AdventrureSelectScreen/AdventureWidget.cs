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

        public override void _Update()
        {
            base._Update();

            bool unlocked = !map.showQuantity || PlayerPrefsWrapper.GetAdventureUnlocked(map.mapID);

            button.interactable = unlocked;

            if (unlocked)
            {
                if (PlayerPrefsWrapper.GetAdventureNew(map.mapID))
                    completeField.text = "new";
                else if (PlayerPrefsWrapper.GetAdventureComplete(map.mapID))
                    completeField.text = "complete";
                else
                    completeField.text = "not complete yet..";
            }
            else
            {
                completeField.text = "locked";
            }
        }

        public void OnTap()
        {
            //select
            menuScreen.menuController.GetScreen<ProgressionMapScreen>().Open(map);
        }
    }
}