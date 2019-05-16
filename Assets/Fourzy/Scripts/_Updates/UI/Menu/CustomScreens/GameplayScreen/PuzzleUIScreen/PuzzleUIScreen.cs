//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using TMPro;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Mechanics.GameplayScene;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PuzzleUIScreen : MenuScreen
    {
        public MovesLeftWidget movesLeftWidget;
        public ButtonExtended hintButton;

        public TMP_Text packName;
        public TMP_Text rule;
        public TweenBase completeIcon;

        public IClientFourzy game { get; private set; }

        public void Open(IClientFourzy game)
        {
            this.game = game;

            switch (game._Type)
            {
                case GameType.PUZZLE:
                    Open();
                    break;
            }
        }

        public override void Open()
        {
            if (game == null || game._Type != GameType.PUZZLE) return;

            base.Open();

            if (PlayerPrefsWrapper.IsPuzzleChallengeComplete(game.asFourzyPuzzle.GameID))
                completeIcon.PlayForward(true);
            else
                completeIcon.AtProgress(0f);

            PuzzlePacksDataHolder.PuzzlePack activePack = game.asFourzyPuzzle.puzzlePack;

            packName.text = activePack.name;
            rule.text = (game.asFourzyPuzzle._data.PackLevel + 1) + ": " + game.asFourzyPuzzle._data.Instructions;
        }

        public void Complete()
        {
            completeIcon.PlayForward(true);

            PlayerPrefsWrapper.SetPuzzleChallengeComplete(game.GameID, true);
        }

        public void OpenNext()
        {
            if (game == null || game._Type != GameType.PUZZLE) return;

            GamePlayManager.instance.LoadGame(game.asFourzyPuzzle.Next());
        }
    }
}
