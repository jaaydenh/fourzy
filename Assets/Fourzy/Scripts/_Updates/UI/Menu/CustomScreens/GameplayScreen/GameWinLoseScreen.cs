//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Helpers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GameWinLoseScreen : MenuScreen
    {
        public Color winColor;
        public Color loseColor;
        public TMP_Text stateLabel;

        public GameObject buttonsRow;
        public ButtonExtended nextGameButton;
        public ButtonExtended rematchButton;
        //public ButtonExtended continueButton;

        public AlphaTween tapToContinue;

        private IClientFourzy game;
        private TurnBaseScreen turnBasedTab;
        private bool showButtonRow = false;

        private List<GamePieceView> gamePieces = new List<GamePieceView>();

        public void Open(IClientFourzy game)
        {
            menuController.OpenScreen(this);

            this.game = game;

            switch (game._Type)
            {
                case GameType.PASSANDPLAY:
                case GameType.PRESENTATION:
                    if (game.draw)
                    {
                        stateLabel.text = $"<color=#{ColorUtility.ToHtmlStringRGB(loseColor)}>{LocalizationManager.Value("draw")}</color>";
                    }
                    else
                    {
                        if (game.IsWinner())
                            stateLabel.text = $"{LocalizationManager.Value("player_one")} <color=#{ColorUtility.ToHtmlStringRGB(winColor)}>{LocalizationManager.Value("won")}</color>";
                        else
                            stateLabel.text = $"{LocalizationManager.Value("player_two")} <color=#{ColorUtility.ToHtmlStringRGB(winColor)}>{LocalizationManager.Value("won")}</color>";
                    }

                    break;

                default:
                    if (game.draw)
                    {
                        stateLabel.text = $"<color=#{ColorUtility.ToHtmlStringRGB(loseColor)}>{LocalizationManager.Value("draw")}</color>";
                    }
                    else
                    {
                        if (game.IsWinner())
                            stateLabel.text = $"You<color=#{ColorUtility.ToHtmlStringRGB(winColor)}>{LocalizationManager.Value("won")}</color>";
                        else
                            stateLabel.text = $"You<color=#{ColorUtility.ToHtmlStringRGB(loseColor)}>{LocalizationManager.Value("lost")}</color>";
                    }

                    break;
            }

            tapToContinue.AtProgress(0f);
            if (RewardsScreen.WillDisplayRewardsScreen(game) || game.puzzleData)
            {
                //display 'tap to continue'
                tapToContinue.PlayForward(true);
                SetButtonRowState(false);

                showButtonRow = true;
            }
            else
                SetButtonRowState(true);
            
            //gamepieces
            //clear old ones
            foreach (GamePieceView gamepiece in gamePieces) Destroy(gamepiece.gameObject);
            gamePieces.Clear();

            //get winning gamepieces
            List<GamePieceView> winningGamepieces = GamePlayManager.instance.board.GetWinningPieces();

            //move them to ui layer
            for (int index = 0; index < winningGamepieces.Count; index++)
            {
                GamePieceView gamepieceView = Instantiate(winningGamepieces[index], transform);
                
                gamepieceView.transform.position = winningGamepieces[index].transform.position;
                gamepieceView.transform.localScale = 75f * Vector3.one;
                gamepieceView.PlayWinAnimation(index * .15f + .15f);

                gamePieces.Add(gamepieceView);
            }
        }

        public override void Open()
        {
            base.Open();

            if (showButtonRow)
            {
                tapToContinue.AtProgress(0f);
                SetButtonRowState(true);

                showButtonRow = false;
            }
        }

        public void Rematch()
        {
            GamePlayManager.instance.Rematch();

            if (isCurrent) menuController.CloseCurrentScreen(true);
        }

        public void NextGame()
        {
            switch (game._Type)
            {
                case GameType.TURN_BASED:
                    turnBasedTab.OpenNext();

                    break;

                default:
                    GamePlayManager.instance.LoadGame(game.Next());

                    break;
            }

            if (isCurrent) menuController.CloseCurrentScreen(true);
        }

        public void OnBGTap()
        {
            //if gauntlet, exit
            if (game.puzzleData && game.puzzleData.pack && game.puzzleData.pack.gauntletStatus != null)
            {
                if (game.puzzleData.pack.gauntletStatus.FourzyCount == 0)
                    GamePlayManager.instance.gameplayScreen.OnBack();
                else
                {
                    if (game.IsWinner())
                    {
                        if(game.puzzleData.pack.complete)
                            GamePlayManager.instance.gameplayScreen.OnBack();
                        else
                            NextGame();
                    }
                    else
                        Rematch();
                }
            }
            else
            {
                if (rematchButton.gameObject.activeInHierarchy) Rematch();
                else if (nextGameButton.gameObject.activeInHierarchy) NextGame();
                else
                {
                    if (game.puzzleData && !game.puzzleData.lastInPack)
                        menuController.GetScreen<VSGamePrompt>().Prompt(game.puzzleData.pack, () => GamePlayManager.instance.gameplayScreen.OnBack());
                    else
                        GamePlayManager.instance.gameplayScreen.OnBack();
                }
            }
        }

        private void SetButtonRowState(bool value)
        {
            buttonsRow.SetActive(value);

            if (value)
            {
                //if (game.puzzleData)
                //{
                //    continueButton.SetActive(true);
                //}
                //else
                //{
                    //next button
                    switch (game._Type)
                    {
                        case GameType.TURN_BASED:
                            nextGameButton.SetActive(turnBasedTab.nextChallenge != null);

                            break;

                        default:
                            nextGameButton.SetActive(false);

                            break;
                    }

                    //rematch button
                    switch (game._Type)
                    {
                        case GameType.REALTIME:
                        case GameType.PRESENTATION:
                            rematchButton.SetActive(false);

                            break;

                        default:
                            rematchButton.SetActive(true);

                            break;
                    }
                //}
            }
            else
            {
                nextGameButton.SetActive(false);
                rematchButton.SetActive(false);
                //continueButton.SetActive(false);
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            turnBasedTab = menuController.GetScreen<TurnBaseScreen>();
        }
    }
}
