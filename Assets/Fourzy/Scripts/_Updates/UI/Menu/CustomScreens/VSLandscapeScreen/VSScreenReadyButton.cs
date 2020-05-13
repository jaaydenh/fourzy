//@vadym udod

using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class VSScreenReadyButton : WidgetBase
    {
        public Sprite readyBG;
        public Sprite notReadyBG;
        public Color textReadyColor;
        public Color textNotReadyColor;

        private Image image;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            image = GetComponent<Image>();
        }

        public void SetState(bool state)
        {
            image.sprite = state ? readyBG : notReadyBG;

            button.SetState(state);
            button.GetLabel().label.color = state ? textReadyColor : textNotReadyColor;
        }
    }
}