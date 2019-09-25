//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.UI.Helpers;
using System.Collections.Generic;

namespace Fourzy._Updates.UI.Widgets
{
    public class ChallengesButtonWidget : WidgetBase
    {
        protected ButtonExtended button;

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

        protected override void OnInitialized()
        {
            base.OnInitialized();

            button = GetComponent<ButtonExtended>();
        }

        private void OnChallengesUpdate(List<ChallengeData> data) => button.GetBadge("games").badge.SetValue(ChallengeManager.Instance.NextChallenges.Count);

        private void OnChallengeUpdate(ChallengeData data) => button.GetBadge("games").badge.SetValue(ChallengeManager.Instance.NextChallenges.Count);
    }
}