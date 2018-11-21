//@vadym udod

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

        [Header("Movement values")]
        public Vector2 initialFromPosition;
        public Vector2 initialToPosition;
        public Vector2 toastStepDistance;

        [HideInInspector]
        public bool available;

        private GamesToastsController owner;

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

            toastIcon.SetIcon(sprite);
            tostText.text = text;
        }

        public void SetData(GamesToastsController owner, string text, ToastIconStyle style)
        {
            this.owner = owner;

            toastIcon.SetIcon(style);
            tostText.text = text;
        }

        public void SetData(GamesToastsController owner, string text)
        {
            this.owner = owner;

            toastIcon.SetIcon(ToastIconStyle.NONE);
            tostText.text = text;
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
    }

    public enum ToastIconStyle
    {
        NONE,
        FAIL,
        SUCCESS,
    }
}