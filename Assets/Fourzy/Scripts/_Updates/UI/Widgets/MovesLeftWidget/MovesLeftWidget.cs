//@vadym udod

using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Menu.Screens;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class MovesLeftWidget : WidgetBase
    {
        private SliderExtended slider;
        private LayoutElement sliderLayout;
        private PuzzleUIScreen puzzleScreen;

        public override void _Update()
        {
            if (puzzleScreen.game._Type != GameType.PUZZLE)
                return;

            sliderLayout.minWidth = 65f * puzzleScreen.game.asFourzyPuzzle.MoveLimit;
            slider.SetMaxValue(puzzleScreen.game.asFourzyPuzzle.MoveLimit, puzzleScreen.game.asFourzyPuzzle.MoveLimit - puzzleScreen.game._playerTurnRecord.Count);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            slider = GetComponentInChildren<SliderExtended>();
            sliderLayout = slider.GetComponent<LayoutElement>();
            puzzleScreen = GetComponentInParent<PuzzleUIScreen>();
        }
    }
}