﻿//@vadym udod

using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Menu.Screens;
using StackableDecorator;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class CurrencyWidget : WidgetBase
    {
        public const int MAX_VALUE = 99999;

        public bool updateOnStart = true;
        public CurrencyType type;
        [StackableField]
        [ShowIf("#Check")]
        public TMP_Text levelLabel;
        public TMP_Text valueLabel;
        [StackableField]
        [ShowIf("#SliderCheck")]
        public SliderExtended slider;

        private int toValue;
        private int _value = 0;

        public override bool visible => gameObject.activeInHierarchy;

        protected void Start()
        {
            if (updateOnStart) SetTo(GameManager.ValueFromCurrencyType(type), false);

            UserManager.onCurrencyUpdate += OnCurrencyUpdate;
        }

        protected void OnDestroy()
        {
            UserManager.onCurrencyUpdate -= OnCurrencyUpdate;
        }

        public override void _Update()
        {
            if (GameManager.ValueFromCurrencyType(type) == _value || !gameObject.activeInHierarchy) return;

            CancelRoutine("animate");
            StartRoutine("animate", .25f, () => SetTo(GameManager.ValueFromCurrencyType(type), true));
        }

        public void SetTo(int value, bool animate)
        {
            toValue = value;
            CancelRoutine("labelAnimation");

            if (animate)
            {
                switch (type)
                {
                    case CurrencyType.COINS:
                    case CurrencyType.GEMS:
                    case CurrencyType.MAGIC:
                    case CurrencyType.TICKETS:
                    case CurrencyType.HINTS:
                        StartRoutine("labelAnimation", UpdateRoutine(1f));
                        break;

                    case CurrencyType.PORTAL_POINTS:
                        StartRoutine("labelAnimation", UpdateRoutine(Mathf.Clamp(value - _value, 0, int.MaxValue) / (float)Constants.PORTAL_POINTS));
                        break;

                    case CurrencyType.RARE_PORTAL_POINTS:
                        StartRoutine("labelAnimation", UpdateRoutine(Mathf.Clamp(value - _value, 0, int.MaxValue) / (float)Constants.RARE_PORTAL_POINTS));
                        break;

                    case CurrencyType.XP:
                        StartRoutine("labelAnimation", UpdateRoutine(UserManager.Instance.GetProgressionDifference(_value, toValue)));
                        break;
                }
            }
            else
                UpdateLabels(value);
        }

        public void OnTap()
        {
            switch (type)
            {
                case CurrencyType.HINTS:
                    menuScreen.menuController.GetOrAddScreen<StorePromptScreen>().Prompt(StorePromptScreen.StoreItemType.HINTS);

                    break;
            }
        }

        protected virtual void UpdateLabels(int value)
        {
            _value = value;

            switch (type)
            {
                case CurrencyType.COINS:
                case CurrencyType.GEMS:
                case CurrencyType.MAGIC:
                case CurrencyType.TICKETS:
                case CurrencyType.HINTS:
                    if (value > MAX_VALUE)
                        valueLabel.text = $"{MAX_VALUE / 1000},{MAX_VALUE % 1000}+";
                    else
                        valueLabel.text = string.Format("{0:N0}", value);
                    break;

                case CurrencyType.PORTAL_POINTS:
                    valueLabel.text = $"{value % Constants.PORTAL_POINTS}/{Constants.PORTAL_POINTS}";
                    slider.value = (value % Constants.PORTAL_POINTS) / (float)Constants.PORTAL_POINTS;
                    break;

                case CurrencyType.RARE_PORTAL_POINTS:
                    valueLabel.text = $"{value % Constants.RARE_PORTAL_POINTS}/{Constants.RARE_PORTAL_POINTS}";
                    slider.value = (value % Constants.RARE_PORTAL_POINTS) / (float)Constants.RARE_PORTAL_POINTS;
                    break;

                case CurrencyType.XP:
                    valueLabel.text = $"{UserManager.Instance.GetLevelXPLeft(value)}/{UserManager.Instance.GetLevelXP(UserManager.Instance.GetLevel(value))}";
                    levelLabel.text = UserManager.Instance.GetLevel(value) + "";
                    slider.value = UserManager.Instance.GetProgression(value);
                    break;
            }
        }

        private void OnCurrencyUpdate(CurrencyType type)
        {
            if (visible && this.type == type) _Update();
        }

        private bool Check() => type == CurrencyType.XP;

        private bool SliderCheck() => type == CurrencyType.PORTAL_POINTS || type == CurrencyType.RARE_PORTAL_POINTS || type == CurrencyType.XP;

        private IEnumerator UpdateRoutine(float duration)
        {
            float timer = 0f;
            int _fromValue = _value;

            while ((timer += Time.deltaTime) < duration)
            {
                UpdateLabels((int)Mathf.Lerp(_fromValue, toValue, timer / duration));

                yield return null;
            }

            UpdateLabels(toValue);
        }
    }
}