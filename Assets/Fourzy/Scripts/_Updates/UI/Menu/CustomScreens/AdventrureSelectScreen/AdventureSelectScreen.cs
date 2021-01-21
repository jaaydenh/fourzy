//@vadym udod

using Fourzy._Updates.UI.Camera3D;
using Fourzy._Updates.UI.Widgets;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class AdventureSelectScreen : MenuScreen
    {
        public AdventureWidget adventureWidgetPrefab;
        public RectTransform widgetsParent;

        public void LoadAdventures()
        {
            //clear widgets
            foreach (WidgetBase widget in widgets) Destroy(widget.gameObject);

            //load new ones
            foreach (Camera3dItemProgressionMap map in GameContentManager.Instance.progressionMaps)
                widgets
                    .Add(Instantiate(adventureWidgetPrefab, widgetsParent)
                    .SetData(map));
        }

        public override void Open()
        {
            base.Open();

            HeaderScreen.instance.Close();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            //load adventures
            LoadAdventures();
        }
    }
}