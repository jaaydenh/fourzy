//@vadym udod

using ByteSheep.Events;
using Fourzy._Updates.Tween;
using System;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Board
{
    [RequireComponent(typeof(ColorTween))]
    [RequireComponent(typeof(AlphaTween))]
    public class HintBlock : MonoBehaviour
    {
        public static float HOLD_TIME = 1f;
        public static Action<HintBlock> onHold;

        public AdvancedEvent onShow;
        public AdvancedEvent onHide;
        public AdvancedEvent onSelected;
        public AdvancedEvent onDeselected;

        private ColorTween colorTween;
        private AlphaTween alphaTween;

        public bool shown { get; private set; }
        public bool selected { get; private set; }
        public float selectedTimer { get; private set; }

        public Move blockDirection { get; set; }

        protected void Awake()
        {
            colorTween = GetComponent<ColorTween>();
            alphaTween = GetComponent<AlphaTween>();

            shown = false;
            selected = false;
        }

        protected void Start()
        {
            alphaTween.AtProgress(0f);
        }

        protected void Update()
        {
            if (selected)
            {
                selectedTimer += Time.deltaTime;

                if (selectedTimer > HOLD_TIME)
                {
                    Deselect();

                    if (onHold != null)
                        onHold.Invoke(this);
                }
            }
        }

        public void Show()
        {
            alphaTween.PlayForward(true);
            shown = true;

            onShow.Invoke();
        }

        public void Hide()
        {
            if (!shown)
                return;

            if (selected)
                Deselect();

            shown = false;
            alphaTween.PlayBackward(true);

            onHide.Invoke();
        }

        public void Select()
        {
            if (selected)
                return;

            selected = true;
            colorTween.PlayForward(true);
            selectedTimer = 0f;

            onSelected.Invoke();
        }

        public void Deselect()
        {
            if (!selected)
                return;

            selected = false;
            colorTween.PlayBackward(true);
            selectedTimer = 0f;

            onDeselected.Invoke();
        }
    }
}