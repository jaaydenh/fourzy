//@vady udod

using ByteSheep.Events;
using Fourzy._Updates.Audio;
using Fourzy._Updates.Serialized;
using TMPro;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Helpers
{
    public class ButtonExtended : Button
    {
        public AdvancedEvent events;
        public AudioTypes playOnClick = AudioTypes.BUTTON_CLICK;

        private TextMeshProUGUI label;

        protected override void Awake()
        {
            base.Awake();

            //find text
            label = transform.GetComponentInChildren<TextMeshProUGUI>();

            //add our events to onClick events
            onClick.AddListener(OnClick);
        }

        public void SetButtonText(string text)
        {
            if (label)
                label.text = text;
        }

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }

        private void OnClick()
        {
            events.Invoke();

            if (playOnClick != AudioTypes.NONE && playOnClick != AudioTypes.SIZE)
                AudioHolder.instance.PlaySelfSfxOneShotTracked(playOnClick);
        }
    }
}