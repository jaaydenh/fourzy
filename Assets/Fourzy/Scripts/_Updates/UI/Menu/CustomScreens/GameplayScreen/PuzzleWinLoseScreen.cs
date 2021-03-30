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
        public TMP_Text puzzleStateLine1Text;
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

        public void CloseIfOpened()
        {
            if (isCurrent) menuController.CloseCurrentScreen(true);

            CancelRoutine("open");
            puzzlePackProgressWidget.CancelWidgetsRewardAnimation();
        }

        public void ButtonTap()
        {
            if (!game.draw && game.IsWinner())
            {
                if (game.puzzleData.pack)
                {
                    if (game.puzzleData.lastInPack)
                    {
                        //force update map
                        GameManager.Instance.currentMap.UpdateWidgets();

                        //open screen for next event
                        BasicPuzzlePack nextPack = 
                            GameManager.Instance.currentMap.GetNextPack(game.puzzleData.pack.packId);
                        nextPack.StartNextUnsolvedPuzzle();

                        if (nextPack)
                        {
                            switch (nextPack.packType)
                            {
                                case PackType.AI_PACK:
                                case PackType.BOSS_AI_PACK:
                                    menuController.GetOrAddScreen<VSGamePrompt>().Prompt(
                                        nextPack,
                                        () => GamePlayManager.Instance.BackButtonOnClick(),
                                        () => menuController.CloseCurrentScreen());

                                    break;

                                case PackType.PUZZLE_PACK:
                                    menuController.GetOrAddScreen<PrePackPrompt>().Prompt(
                                        nextPack, 
                                        () => GamePlayManager.Instance.BackButtonOnClick(),
                                        () => menuController.CloseCurrentScreen());

                                    break;
                            }
                        }
                        else
                        {
                            GamePlayManager.Instance.BackButtonOnClick();
                        }
                    }
                    else
                    {
                        GamePlayManager.Instance.LoadGame(game.Next());
                    }
                }
                else
                {
                    GamePlayManager.Instance.LoadGame(game.Next());
                }
            }
            else
            {
                //puzzle failed, rematch
                GamePlayManager.Instance.Rematch();
            }

            if (isCurrent)
            {
                menuController.CloseCurrentScreen(true);
            }
        }

        private IEnumerator OpenRoutine()
        {
            puzzlePackProgressWidget.Hide(0f);

            if (game.IsWinner())
            {
                animator.SetTrigger("winTrigger");

                puzzleStateLine1Text.text = LocalizationManager.Value("puzzle");
                puzzleStateText.text = LocalizationManager.Value("complete");
                puzzleStateText.color = winColor;

                if (game.puzzleData.pack)
                {
                    if (!puzzlePackProgressWidget.puzzlePack ||
                        puzzlePackProgressWidget.puzzlePack.packId != game.puzzleData.pack.packId)
                    {
                        puzzlePackProgressWidget.SetData(game.puzzleData.pack);
                    }

                    yield return new WaitForSeconds(.95f);

                    puzzlePackProgressWidget
                        .CheckWidgets()
                        .Show(.5f);

                    //animate rewards
                    puzzlePackProgressWidget.AnimateRewardsForIndex(
                        game.puzzleData.pack.puzzlesComplete.IndexOf(game.puzzleData));
                }
            }
            else
            {
                animator.SetTrigger("loseTrigger");

                puzzleStateLine1Text.text = LocalizationManager.Value("failed1");
                puzzleStateText.text = LocalizationManager.Value("failed2");
                puzzleStateText.color = loseColor;
            }
        }
    }
}