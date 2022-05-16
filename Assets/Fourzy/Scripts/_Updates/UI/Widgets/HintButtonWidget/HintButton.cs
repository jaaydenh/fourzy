//@vadym udod

using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Helpers;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class HintButton : CurrencyWidget
    {
        [SerializeField]
        private Badge label;
        [SerializeField]
        private GameObject labelParent;

        private UIOutline outline;
        private OnRatio onRatio;

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

            if (!value)
            {
                SetMessage("");
                SetOutline(0f);
            }
        }

        public void SetMessage(string message) => label.SetValue(message);

        public void SetOutline(float value) => outline.intensity = value;

        public void OnPositionUpdated()
        {
            onRatio.CheckOrientation();
        }

        public void SetLabelParentState(bool state)
        {
            labelParent.SetActive(state);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            outline = GetComponentInChildren<UIOutline>();
            onRatio = GetComponent<OnRatio>();
        }
    }
}