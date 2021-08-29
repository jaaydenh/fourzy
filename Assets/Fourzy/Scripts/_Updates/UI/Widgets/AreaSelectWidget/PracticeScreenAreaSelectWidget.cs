//@vadym udod

using ByteSheep.Events;
using Fourzy._Updates.Serialized;
using FourzyGameModel.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class PracticeScreenAreaSelectWidget : WidgetBase
    {
        [SerializeField]
        private Image icon;
        [SerializeField]
        private GameObject disabled;

        public AdvancedEvent onSelect;
        public AdvancedEvent onDeselect;

        public Area Area { get; private set; }
        public bool Disabled => disabled.activeInHierarchy;
        public AreasDataHolder.GameArea AreaData { get; private set; }

        public PracticeScreenAreaSelectWidget SetData(Area area, bool updateUnlocked = true, bool keepEnabled = false)
        {
            Area = area;

            AreaData = GameContentManager.Instance.areasDataHolder[area];
            icon.sprite = AreaData.square;

            CheckArea(updateUnlocked, keepEnabled);

            return this;
        }

        public void CheckArea(bool updateUnlocked = true, bool keepEnabled = false)
        {
            if (Area == Area.NONE)
            {
                return;
            }

            if (updateUnlocked)
            {
                SetState(AreaData.unlocked, keepEnabled);
            }
        }

        public void SetState(bool value, bool keepEnabled)
        {
            button.interactable = value || keepEnabled;
            disabled.SetActive(!value);
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