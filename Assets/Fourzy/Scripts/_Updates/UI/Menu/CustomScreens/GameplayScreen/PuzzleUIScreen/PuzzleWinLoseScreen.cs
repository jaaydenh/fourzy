//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.GameplayScene;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PuzzleWinLoseScreen : MenuScreen
    {
        public Color winColor;
        public Color loseColor;
        public TMP_Text puzzleStateText;

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
            menuController.OpenScreen(this);

            this.game = game;

            if (game.IsWinner())
            {
                animator.SetTrigger("winTrigger");

                puzzleStateText.text = "Completed!";
                puzzleStateText.color = winColor;
            }
            else
            {
                animator.SetTrigger("loseTrigger");

                puzzleStateText.text = "Failed";
                puzzleStateText.color = loseColor;
            }
        }

        public void ButtonTap()
        {
            if (!game.draw && game.IsWinner())
            {
                if (GameManager.Instance.currentPuzzlePack.justFinished)
                {
                    //add menu event
                    MenuController.AddMenuEvent(
                        Constants.MAIN_MENU_CANVAS_NAME, 
                        new KeyValuePair<string, object>("puzzlePack", GameManager.Instance.currentPuzzlePack));

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
    }
}