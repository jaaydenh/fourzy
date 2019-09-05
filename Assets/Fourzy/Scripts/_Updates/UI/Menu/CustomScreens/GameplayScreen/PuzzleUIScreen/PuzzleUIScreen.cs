//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using TMPro;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PuzzleUIScreen : MenuScreen
    {
        public MovesLeftWidget movesLeftWidget;
        public ButtonExtended rematchButton;
        public ButtonExtended nextButton;

        public TMP_Text packName;
        public TMP_Text rule;
        public TweenBase completeIcon;
        public AlphaTween packInfoTween;

        private bool rematchButtonState = false;

        public IClientFourzy game { get; private set; }

        public void Open(IClientFourzy game)
        {
            if (game == null || game.puzzleData == null) return;

            base.Open();

            this.game = game;
            
            if (GameManager.Instance.currentPuzzlePack.puzzlesEnabled.Count > 1)
            {
                if (nextButton.alphaTween._value < 1f)
                {
                    nextButton.Show(.3f);
                    nextButton.SetState(true);
                }
            }
            else
            {
                nextButton.Hide(0f);
                nextButton.SetState(false);
            }

            if (rematchButtonState)
            {
                rematchButton.scaleTween.PlayBackward(true);
                rematchButton.alphaTween.PlayBackward(true);

                rematchButton.interactable = false;
                rematchButtonState = false;
            }

            switch (this.game.puzzleData.packType)
            {
                case PuzzlePacksDataHolder.PackType.BOSS_AI_PACK:
                case PuzzlePacksDataHolder.PackType.AI_PACK:
                    packInfoTween.SetAlpha(0f);

                    break;

                case PuzzlePacksDataHolder.PackType.PUZZLE_PACK:
                    packInfoTween.SetAlpha(1f);

                    movesLeftWidget.UpdateMovesLeft(game.asFourzyPuzzle);

                    if (PlayerPrefsWrapper.IsPuzzleChallengeComplete(this.game.GameID))
                        completeIcon.PlayForward(true);
                    else
                        completeIcon.AtProgress(0f);

                    packName.text = GameManager.Instance.currentPuzzlePack.name;
                    rule.text = (this.game.puzzleData.PackLevel + 1) + ": " + this.game.puzzleData.Instructions;

                    break;
            }
        }

        public void UpdatePlayerTurn()
        {
            if (game == null || game.puzzleData == null) return;

            if (game._Type == GameType.PUZZLE) movesLeftWidget.UpdateMovesLeft(game.asFourzyPuzzle);

            if (game._allTurnRecord.Count == 1 && !game.isOver)
            {
                rematchButton.scaleTween.PlayForward(true);
                rematchButton.alphaTween.PlayForward(true);

                rematchButton.interactable = true;
                rematchButtonState = true;
            }
        }

        public void Next() => GamePlayManager.instance.LoadGame(game.Next());

        public void GameComplete()
        {
            if (game.IsWinner())
            {
                completeIcon.PlayForward(true);
            }

            if (movesLeftWidget.alphaTween._value > 0f)  movesLeftWidget.Hide(.3f);

            nextButton.SetState(false);
            nextButton.Hide(.3f);
        }
    }
}
