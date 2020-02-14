//@vadym udod


namespace Fourzy._Updates.UI.Widgets
{
    public class OnboardingScreenBG : WidgetBase
    {
        public bool shown = false;

        public void _Hide()
        {
            if (alphaTween._value > 0f) Hide(.3f);

            SetInteractable(false);
            BlockRaycast(false);

            shown = false;
        }

        public void _Show(bool show)
        {
            if (show) Show(.3f);

            SetInteractable(true);
            BlockRaycast(true);

            shown = true;
        }
    }
}
