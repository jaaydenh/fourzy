//@vadym udod

using Fourzy._Updates.Tween;
using FourzyGameModel.Model;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class GameInfoWidget : WidgetBase
    {
        public TMP_Text label;
        public string NYTKey = "not_your_turn";

        public void NotYourTurn()
        {
            SetTextLocalized(NYTKey);
            Show(.3f);

            StartRoutine("hide", 2.5f, () => Hide(.3f), null);
        }

        public void PassTurn(float duration)
        {
            SetText("Pass..");
            Show(.3f);

            StartRoutine("hide", duration - .3f, () => Hide(.3f), null);
        }

        public void DisplayPower(BossPowerType type)
        {
            //type to string
            SetText(type.ToString());
            Show(.3f);

            StartRoutine("hide", 1f, () => Hide(.3f), null);
        }

        public override void Show(float time)
        {
            Hide(0f);

            _Reset();

            ScaleTo(Vector3.one, time);

            base.Show(time);
        }

        public override void Hide(float time)
        {
            _Reset();

            ScaleTo(Vector3.zero, time);

            base.Hide(time);
        }

        public GameInfoWidget SetTextLocalized(string key)
        {
            label.text = LocalizationManager.Instance.GetLocalizedValue(key);

            return this;
        }

        public GameInfoWidget SetText(string value)
        {
            label.text = value;

            return this;
        }

        /// <summary>
        /// Delay Show() by time value
        /// </summary>
        /// <param name="time"></param>
        public GameInfoWidget ShowDelayed(float time)
        {
            _Reset();

            StartRoutine("showDelayed", time, () => Show(.3f), null);

            return this;
        }

        public void _Reset()
        {
            CancelRoutine("hide");
            CancelRoutine("showDelayed");
        }
    }
}
