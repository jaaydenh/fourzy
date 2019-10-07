//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.UI.Widgets;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PuzzleWinLoseScreen : MenuScreen
    {
        public Color winColor;
        public Color loseColor;
        public TMP_Text puzzleStateText;
        public PuzzlePackProgressWidget puzzlePackProgressWidget;

        private Animator animator;
        private IClientFourzy game;

        protected override void Awake()
        {
            base.Awake();

            animator = GetComponent<Animator>();
        }

        public override void OnBack()
        {
            base.OnBack();

            ButtonTap();
        }

        public void Open(IClientFourzy game)
        {
            this.game = game;

            CancelRoutine("open");
            StartRoutine("open", OpenRoutine());

            menuController.OpenScreen(this);
        }

        public void ButtonTap()
        {
            if (!game.draw && game.IsWinner())
            {
                if (game.puzzleData.pack && game.puzzleData.pack.justFinished)
                {
                    ////add menu event
                    //MenuController.AddMenuEvent(
                    //    Constants.MAIN_MENU_CANVAS_NAME, 
                    //    new KeyValuePair<string, object>("puzzlePack", GameManager.Instance.currentPuzzlePack));

                    //open main menu
                    GameManager.Instance.OpenMainMenu();

                    //consume
                    GameManager.Instance.currentPuzzlePack.justFinished = false;
                }
                else
                    GamePlayManager.instance.LoadGame(game.Next());
            }
            else
                //puzzle failed, rematch
                GamePlayManager.instance.Rematch();

            if (isCurrent) menuController.CloseCurrentScreen(true);
        }

        private IEnumerator OpenRoutine()
        {
            puzzlePackProgressWidget.Hide(0f);

            if (game.IsWinner())
            {
                animator.SetTrigger("winTrigger");

                puzzleStateText.text = "Completed!";
                puzzleStateText.color = winColor;

                if (game.puzzleData.pack)
                {
                    if (!puzzlePackProgressWidget.puzzlePack) puzzlePackProgressWidget.SetData(game.puzzleData.pack);

                    yield return new WaitForSeconds(.95f);

                    puzzlePackProgressWidget
                        .CheckWidgets()
                        .Show(.5f);

                    //animate rewards
                    puzzlePackProgressWidget.AnimateRewardsForIndex(game.puzzleData.pack.puzzlesComplete.IndexOf(game.puzzleData));
                }
            }
            else
            {
                animator.SetTrigger("loseTrigger");

                puzzleStateText.text = "Failed";
                puzzleStateText.color = loseColor;
            }
        }
    }
}