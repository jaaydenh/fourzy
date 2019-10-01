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
        public ButtonExtended backButton;

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
                        stateLabel.text = $"<color=#{ColorUtility.ToHtmlStringRGB(loseColor)}>Draw</color>";
                    }
                    else
                    {
                        if (game.IsWinner())
                            stateLabel.text = $"Player 1 <color=#{ColorUtility.ToHtmlStringRGB(winColor)}>{LocalizationManager.Instance.GetLocalizedValue("won_suffix")}</color>";
                        else
                            stateLabel.text = $"Player 2 <color=#{ColorUtility.ToHtmlStringRGB(winColor)}>{LocalizationManager.Instance.GetLocalizedValue("won_suffix")}</color>";
                    }

                    break;

                default:
                    if (game.draw)
                    {
                        stateLabel.text = $"<color=#{ColorUtility.ToHtmlStringRGB(loseColor)}>Draw</color>";
                    }
                    else
                    {
                        if (game.IsWinner())
                            stateLabel.text = $"You<color=#{ColorUtility.ToHtmlStringRGB(winColor)}>{LocalizationManager.Instance.GetLocalizedValue("won_suffix")}</color>";
                        else
                            stateLabel.text = $"You<color=#{ColorUtility.ToHtmlStringRGB(loseColor)}>Lost</color>";
                    }

                    break;
            }

            tapToContinue.AtProgress(0f);
            if (RewardsScreen.WillDisplayRewardsScreen(game))
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

                case GameType.PASSANDPLAY:
                    //load next passplay game
                    GamePlayManager.instance.LoadGame(game.Next());

                    break;
            }

            if (isCurrent) menuController.CloseCurrentScreen(true);
        }

        public void OnBGTap()
        {
            if (rematchButton.gameObject.activeInHierarchy) Rematch();
            else if (nextGameButton.gameObject.activeInHierarchy) NextGame();
            else GamePlayManager.instance.gameplayScreen.OnBack();
        }

        private void SetButtonRowState(bool value)
        {
            buttonsRow.SetActive(value);

            if (value)
            {
                //next button
                switch (game._Type)
                {
                    //no 'next' for realtime/ai games
                    case GameType.REALTIME:
                    case GameType.AI:
                    case GameType.PRESENTATION:
                        nextGameButton.SetActive(false);

                        break;

                    case GameType.TURN_BASED:
                        nextGameButton.SetActive(turnBasedTab.nextChallenge != null);

                        break;

                    case GameType.PASSANDPLAY:
                        //nextGameButton.SetActive(true);

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
            }
            else
            {
                nextGameButton.SetActive(false);
                rematchButton.SetActive(false);
            }

            //back button
            switch (game._Type)
            {
                case GameType.PRESENTATION:

                    break;

                default:
                    //set back button state
                    backButton.SetActive(!nextGameButton.gameObject.activeInHierarchy && !rematchButton.gameObject.activeInHierarchy);

                    break;
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            turnBasedTab = menuController.GetScreen<TurnBaseScreen>();
        }
    }
}
