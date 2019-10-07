//@vadym udod

using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class PlayerUIMessageWidget : WidgetBase
    {
        public TMP_Text label;

        private float durationLeft;

        public bool available { get; set; } = true;
        public PlayerUIMessagesWidget owner { get; private set; }

        protected void Update()
        {
            if (durationLeft <= 0f) return;

            if (durationLeft - Time.deltaTime <= 0f)
            {
                CancelRoutine("dismissRoutine");
                StartRoutine("dismissRoutine", alphaTween.playbackTime, () => Dismiss());
                alphaTween.PlayBackward(true);
            }

            durationLeft -= Time.deltaTime;
        }

        public PlayerUIMessageWidget SetData(PlayerUIMessagesWidget owner, string data, float duration, bool dismissable)
        {
            SetActive(true);

            this.owner = owner;

            label.text = data;
            durationLeft = duration - alphaTween.playbackTime;

            alphaTween.PlayForward(true);

            return this;
        }

        public void OnTap()
        {

        }

        public void Dismiss()
        {
            available = true;
            SetActive(false);
        }
    }
}