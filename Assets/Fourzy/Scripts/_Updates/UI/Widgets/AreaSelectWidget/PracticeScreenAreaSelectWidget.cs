//@vadym udod

using ByteSheep.Events;
using FourzyGameModel.Model;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class PracticeScreenAreaSelectWidget : WidgetBase
    {
        public Image icon;
        public GameObject disabled;
        public AdvancedEvent onSelect;
        public AdvancedEvent onDeselect;

        public Area area { get; private set; }

        public PracticeScreenAreaSelectWidget SetData(Area area)
        {
            this.area = area;

            icon.sprite = GameContentManager.Instance.areasDataHolder[area].square;
            bool enabled = InternalSettings.Current.UNLOCKED_AREAS.Contains(area);

            button.interactable = enabled;
            disabled.SetActive(!enabled);

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