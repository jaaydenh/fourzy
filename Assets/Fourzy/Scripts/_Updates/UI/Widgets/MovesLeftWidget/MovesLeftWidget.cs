//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.UI.Helpers;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class MovesLeftWidget : WidgetBase
    {
        private SliderExtended slider;
        private LayoutElement sliderLayout;

        public void UpdateMovesLeft(ClientFourzyPuzzle puzzle)
        {
            if (puzzle.MoveLimit < 1)
            {
                if (alphaTween._value > 0f) Hide(.3f);
                return;
            }

            if (alphaTween._value < 1f) Show(.3f);

            sliderLayout.minWidth = 65f * puzzle.MoveLimit;
            slider.SetMaxValue(puzzle.MoveLimit, puzzle.MoveLimit - puzzle._playerTurnRecord.Count);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            slider = GetComponentInChildren<SliderExtended>();
            sliderLayout = slider.GetComponent<LayoutElement>();
        }
    }
}