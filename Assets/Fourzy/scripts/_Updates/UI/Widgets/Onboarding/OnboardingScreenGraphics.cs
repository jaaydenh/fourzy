//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class OnboardingScreenGraphics : WidgetBase
    {
        public GameObject wizard;

        protected override void Awake()
        {
            base.Awake();

            SetInteractable(false);
            BlockRaycast(false);
        }

        public void SetWizardState(bool state)
        {
            wizard.SetActive(state);
        }
    }
}