//@vadym udod

using ByteSheep.Events;
using FourzyGameModel.Model;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class PracticeScreenAreaSelectWidget : WidgetBase
    {
        public Image icon;
        public AdvancedEvent onSelect;
        public AdvancedEvent onDeselect;

        public Area area { get; private set; }

        public PracticeScreenAreaSelectWidget SetData(Area area)
        {
            this.area = area;

            gameObject.SetActive(true);

            if (PlayerPrefsWrapper.GetThemeUnlocked((int)area))
            {
                icon.sprite = GameContentManager.Instance.miscGameDataHolder.GetIcon("AreaIconsTemp", area.ToString()).sprite;
                button.interactable = true;
            }
            else
            {
                icon.sprite = GameContentManager.Instance.miscGameDataHolder.GetIcon("AreaIconsTemp", area.ToString() + "_locked").sprite;
                button.interactable = false;
            }

            return this;
        }

        public void Select()
        {
            onSelect.Invoke();
        }

        public void Deselect()
        {
            onDeselect.Invoke();
        }
    }
}