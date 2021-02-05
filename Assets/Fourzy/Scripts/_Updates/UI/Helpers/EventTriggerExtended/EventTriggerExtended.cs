//@vadym udod

using ByteSheep.Events;
using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Menu;
using FourzyGameModel.Model;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Helpers
{
    public class EventTriggerExtended : EventTrigger
    {
        public AdvancedEvent onSelect;
        public AdvancedEvent onDeselect;

        public bool adjustSelectableOnSelect = true;
        public bool onlyActiveSelectables = true;

        [SerializeField]
        private Vector2 customSize = Vector2.zero;

        private Selectable selectable;
        private MenuScreen menuScreen;
        private RectTransform rectTransform;

        public Vector2 size
        {
            get
            {
                if (customSize == Vector2.zero)
                    return new Vector2(rectTransform.rect.width, rectTransform.rect.height);

                return new Vector2(customSize.x, customSize.y);
            }
        }

        public Vector2 center
        {
            get
            {
                if (!rectTransform) return transform.position;

                return new Vector2(transform.position.x, transform.position.y) + new Vector2(rectTransform.rect.center.x * transform.lossyScale.x, rectTransform.rect.center.y * transform.lossyScale.y);
            }
        }

        protected void Awake()
        {
            selectable = GetComponent<Selectable>();
            rectTransform = GetComponent<RectTransform>();

            menuScreen = GetComponentInParent<MenuScreen>();
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);

            onSelect.Invoke();

            if (adjustSelectableOnSelect) AdjustSelectable();
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);

            onDeselect.Invoke();
        }

        public void AdjustSelectable()
        {
            if (!selectable || !menuScreen) return;

            //get all selectable
            List<Selectable> selectables = new List<Selectable>(menuScreen.GetComponentsInChildren<Selectable>());

            //remove this
            selectables.Remove(selectable);

            //remove unactive
            if (onlyActiveSelectables)
                selectables.RemoveAll(_selectable => !_selectable.interactable || !_selectable.gameObject.activeInHierarchy);

            Selectable right = GetNeighbor(selectable, selectables, Direction.RIGHT, true);
            Selectable left = GetNeighbor(selectable, selectables, Direction.LEFT, true);
            Selectable up = GetNeighbor(selectable, selectables, Direction.UP, true);
            Selectable down = GetNeighbor(selectable, selectables, Direction.DOWN, true);

            Navigation navigation = selectable.navigation;
            navigation.mode = Navigation.Mode.Explicit;

            if (!navigation.selectOnRight || navigation.selectOnRight != right) navigation.selectOnRight = right;
            if (!navigation.selectOnLeft || navigation.selectOnLeft != left) navigation.selectOnLeft = left;
            if (!navigation.selectOnUp || navigation.selectOnUp != up) navigation.selectOnUp = up;
            if (!navigation.selectOnDown || navigation.selectOnDown != down) navigation.selectOnDown = down;

            selectable.navigation = navigation;
        }

        private Selectable GetNeighbor(Selectable target, List<Selectable> other, Direction direction, bool closest, bool warp = true)
        {
            List<Selectable> sorted = null;
            Vector2 towards = direction.GetVector();

            sorted = other
                .Where(_other =>
                {
                    float angle = towards.AngleTo(_other.transform.position - target.transform.position);

                    return angle <= 45f && angle >= -45f;
                }).OrderBy(_other => Vector2.Distance(_other.transform.position, target.transform.position)).ToList();

            if (sorted.Count > 0)
            {
                if (closest)
                    return sorted[0];
                else
                    return sorted[sorted.Count - 1];
            }
            else if (warp)
                return GetNeighbor(target, other, BoardLocation.Reverse(direction), false, false);

            return null;
        }
    }
}
