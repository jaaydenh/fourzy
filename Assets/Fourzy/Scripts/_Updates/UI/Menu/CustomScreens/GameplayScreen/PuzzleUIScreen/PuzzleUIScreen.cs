//@vadym udod

using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using TMPro;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PuzzleUIScreen : MenuScreen
    {
        public MovesLeftWidget movesLeftWidget;
        public ButtonExtended hintButton;

        public TMP_Text packName;
        public TMP_Text rule;
        public TweenBase completeIcon;

        private Game game;

        public void Open(Game game)
        {
            this.game = game;

            switch (game.gameState.GameType)
            {
                case GameType.PUZZLE:
                    Open();
                    break;
            }
        }

        public override void Open()
        {
            base.Open();

            UpdateWidgets();

            PuzzlePack activePack = GameManager.Instance.ActivePuzzlePack;

            packName.text = game.puzzleChallengeInfo.Name;
            rule.text = GameManager.Instance.ActivePuzzlePack.ActiveLevel + ": " + game.subtitle;
        }

        public void UpdateWidgets()
        {
            movesLeftWidget.SetData(game);

            if (PlayerPrefsWrapper.IsPuzzleChallengeCompleted(game.puzzleChallengeInfo.ID))
                completeIcon.PlayForward(true);
        }
    }
}
