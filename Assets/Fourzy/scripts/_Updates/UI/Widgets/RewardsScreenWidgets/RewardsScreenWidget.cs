//@vadym udod

using Fourzy._Updates.Audio;
using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.Serialized;
using TMPro;

namespace Fourzy._Updates.UI.Widgets
{
    public class RewardsScreenWidget : WidgetBase
    {
        public AudioTypes onShow;

        public string format;
        public TMP_Text nameLabel;
        public TMP_Text valueLabel;

        public virtual void SetData(RewardsManager.Reward data)
        {
            if (nameLabel) nameLabel.text = data.name;
            if (valueLabel) valueLabel.text = string.Format(format, data.ToString());
        }

        public override void Show(float time)
        {
            AudioHolder.instance.PlaySelfSfxOneShotTracked(onShow);
            base.Show(time);
        }
    }
}