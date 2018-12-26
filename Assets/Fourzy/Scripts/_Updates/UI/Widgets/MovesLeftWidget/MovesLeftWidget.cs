//@vadym udod

using Fourzy._Updates.UI.Helpers;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class MovesLeftWidget : WidgetBase
    {
        private SliderExtended slider;
        private LayoutElement sliderLayout;

        protected override void Awake()
        {
            base.Awake();

            slider = GetComponentInChildren<SliderExtended>();
            sliderLayout = slider.GetComponent<LayoutElement>();
        }

        public void SetData(Game data)
        {
            if (data.gameState.GameType != GameType.PUZZLE)
                return;

            sliderLayout.minWidth = 65f * data.puzzleChallengeInfo.MoveGoal;
            slider.SetMaxValue(data.puzzleChallengeInfo.MoveGoal, data.puzzleChallengeInfo.MoveGoal - data.gameState.Player1MoveCount);
        }
    }
}