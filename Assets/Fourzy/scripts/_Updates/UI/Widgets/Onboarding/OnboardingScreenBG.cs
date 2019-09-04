//@vadym udod


namespace Fourzy._Updates.UI.Widgets
{
    public class OnboardingScreenBG : WidgetBase
    {
        public override void Hide(float time)
        {
            base.Hide(time);

            SetInteractable(false);
            BlockRaycast(false);
        }

        public override void Show(float time)
        {
            base.Show(time);

            SetInteractable(true);
            BlockRaycast(true);
        }
    }
}
