//@vadym udod

using Fourzy._Updates.Tween;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class RefreshIndicatorWidget : WidgetBase
    {
        public bool affectScaleWithVisibility = true;

        public RotationTween rotationTween { get; private set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            rotationTween = GetComponent<RotationTween>();

            if (rotationTween)
                rotationTween.Initialize();
        }

        public override void SetVisibility(float value)
        {
            base.SetVisibility(value);

            if (affectScaleWithVisibility)
                transform.localScale = new Vector3(value, value, transform.localScale.z);
        }

        public void SetAnimationState(bool state)
        {
            if (state)
                rotationTween.PlayForward(true);
            else
                rotationTween.StopTween(false);
        }
    }
}