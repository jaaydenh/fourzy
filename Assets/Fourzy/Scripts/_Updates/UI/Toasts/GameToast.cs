//@vadym udod

using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Widgets;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Toasts
{
    public class GameToast : WidgetBase
    {
        public GamesToastsIcon toastIcon;
        public TextMeshProUGUI tostText;
        public float displayTime = 2f;
        public int charactersLineLimit = 32;

        [Header("Movement values")]
        public Vector2 initialFromPosition;
        public Vector2 initialToPosition;
        public Vector2 toastStepDistance;

        [HideInInspector]
        public bool available;

        protected GamesToastsController owner;

        protected override void Awake()
        {
            base.Awake();

            available = true;
        }

        public override void Hide(float time)
        {
            if (IsRoutineActive("removeFromQueue"))
                return;

            base.Hide(time);

            CancelRoutine("displayCoroutine");
            StartRoutine("removeFromQueue", time, () =>
            {
                if (owner)
                {
                    if (owner.activeToasts.Contains(this))
                        owner.activeToasts.Dequeue();

                    owner.movableToast.Dequeue();
                }

                available = true;
            }, null);
        }

        public void SetData(GamesToastsController owner, string text, Sprite sprite)
        {
            this.owner = owner;

            if (toastIcon)
                toastIcon.SetIcon(sprite);

            SetToastText(text);
        }

        public void SetData(GamesToastsController owner, string text, ToastIconStyle style)
        {
            this.owner = owner;

            if (toastIcon)
                toastIcon.SetIcon(style);

            SetToastText(text);
        }

        public void SetData(GamesToastsController owner, string text)
        {
            this.owner = owner;

            if (toastIcon)
                toastIcon.SetIcon(ToastIconStyle.NONE);

            SetToastText(text);
        }

        public void ShowToast()
        {
            ShowToast(displayTime, initialFromPosition, initialToPosition);
        }

        public void ShowToast(float displayTime, Vector2 fromPosition, Vector2 toPosition)
        {
            alphaTween.OnReset();

            MoveTo(fromPosition, toPosition, .4f);
            Show(.4f);
            available = false;

            StartRoutine("displayCoroutine", displayTime, () => { Hide(.3f); }, null);
        }

        public void SetToastText(string text)
        {
            tostText.text = text.SplitText(charactersLineLimit);
        }
    }

    public enum ToastIconStyle
    {
        NONE,
        FAIL,
        SUCCESS,
    }
}