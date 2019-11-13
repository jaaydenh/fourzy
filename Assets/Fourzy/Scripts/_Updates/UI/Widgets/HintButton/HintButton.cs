//@vadym udod

using Fourzy._Updates.UI.Helpers;

namespace Fourzy._Updates.UI.Widgets
{
    public class HintButton : CurrencyWidget
    {
        public override void Show(float time = 0f)
        {
            SetActive(true);

            base.Show(time);
        }

        public override void Hide(float time = 0f)
        {
            SetActive(false);

            base.Hide(time);
        }

        public void SetState(bool value)
        {
            button.SetState(value);
        }
    }
}