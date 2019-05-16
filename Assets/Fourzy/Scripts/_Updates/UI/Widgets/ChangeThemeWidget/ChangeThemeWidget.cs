//@vadym udod

using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class ChangeThemeWidget : WidgetBase
    {
        public bool updateOnStart = true;

        private Image image;

        protected void Start()
        {
            if (updateOnStart) _Update();
        }

        public override void _Update()
        {
            image.sprite = GameContentManager.Instance.currentTheme.preview;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            image = GetComponent<Image>();
        }
    }
}