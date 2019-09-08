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

        protected override void Start()
        {
            base.Start();

            //load adventures
            LoadAdventures();
        }

        public void LoadAdventures()
        {
            //clear widgets
            foreach (WidgetBase widget in widgets) Destroy(widget.gameObject);

            //load new ones
            foreach (Camera3dItemProgressionMap map in GameContentManager.Instance.progressionMaps)
                widgets.Add(Instantiate(adventureWidgetPrefab, widgetsParent).SetData(map));
        }
    }
}