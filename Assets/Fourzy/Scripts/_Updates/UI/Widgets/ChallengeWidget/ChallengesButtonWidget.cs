//@vadym udod

using Fourzy._Updates.ClientModel;
using System.Collections.Generic;

namespace Fourzy._Updates.UI.Widgets
{
    public class ChallengesButtonWidget : WidgetBase
    {
        protected override void Awake()
        {
            base.Awake();

            ChallengeManager.OnChallengesUpdate += OnChallengesUpdate;
            ChallengeManager.OnChallengeUpdate += OnChallengeUpdate;
            ChallengeManager.OnChallengeUpdateLocal += OnChallengeUpdate;
        }

        public override void _Update()
        {
            base._Update();

            button.GetBadge("games").badge.SetValue(ChallengeManager.Instance.NextChallenges.Count);
        }

        private void OnChallengesUpdate(List<ChallengeData> data) => button.GetBadge("games").badge.SetValue(ChallengeManager.Instance.NextChallenges.Count);

        private void OnChallengeUpdate(ChallengeData data) => button.GetBadge("games").badge.SetValue(ChallengeManager.Instance.NextChallenges.Count);
    }
}