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

        public AlphaTween tapToContinue;

        private IClientFourzy game;
        private TurnBaseScreen turnBasedTab;
        private bool showButtonRow = false;

        private List<GamePieceView> gamePieces = new List<GamePieceView>();

        public void Open(IClientFourzy game)
        {
            menuController.OpenScreen(this);

            this.game = game;

            switch (game._Mode)
            {
                case GameMode.GAUNTLET:
                    if (game.draw)
                    {
                        stateLabel.text = $"<color=#{ColorUtility.ToHtmlStringRGB(loseColor)}>{LocalizationManager.Value("draw")}</color>";
                    }
                    else
                    {
                        if (game.IsWinner() && (game.myMembers.Count > 0 || game.puzzleData.pack.complete))
                            stateLabel.text = $"You<color=#{ColorUtility.ToHtmlStringRGB(winColor)}>{LocalizationManager.Value("won")}</color>";
                        else
                            stateLabel.text = $"You<color=#{ColorUtility.ToHtmlStringRGB(loseColor)}>{LocalizationManager.Value("lost")}</color>";
                    }

                    break;

                case GameMode.LOCAL_VERSUS:
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
            if (RewardsScreen.WillDisplayRewardsScreen(game) || (game.puzzleData && game.puzzleData.pack))
            {
                //display 'tap to continue'
                tapToContinue.PlayForward(true);
                SetButtonRowState(false);

                showButtonRow = true;
            }
            else
                SetButtonRowState(true);
            
            //gamepieces
            foreach (GamePieceView gamepiece in gamePieces) Destroy(gamepiece.gameObject);
            gamePieces.Clear();

            //get winning gamepieces
            List<GamePieceView> winningGamepieces = GamePlayManager.instance.board.GetWinningPieces();

            //move them to ui layer
            for (int index = 0; index < winningGamepieces.Count; index++)
            {
                GamePieceView gamepieceView = Instantiate(winningGamepieces[index], transform);
                
                gamepieceView.transform.position = winningGamepieces[index].transform.position;
                gamepieceView.transform.localScale = (GameManager.Instance.Landscape ? 89f : 75f) * Vector3.one;
                gamepieceView.PlayWinAnimation(index * .15f + .15f);

                gamePieces.Add(gamepieceView);
            }
        }

        public void CloseIfOpened()
        {
            if (isCurrent) menuController.CloseCurrentScreen(true);
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
            switch (game._Mode)
            {
                case GameMode.GAUNTLET:
                    menuController.CloseCurrentScreen();

                    if (game.IsWinner())
                    {
                        if (game.puzzleData.pack.complete)
                            menuController.GetOrAddScreen<GauntletWinPrompt>()._Prompt(game);
                        else
                        {
                            if (game.myMembers.Count == 0)
                                menuController.GetOrAddScreen<GauntletLostPrompt>()._Prompt(game);
                            else
                                menuController.GetOrAddScreen<VSGamePrompt>().Prompt(game.puzzleData.pack, () => GamePlayManager.instance.gameplayScreen.OnBack());
                        }
                    }
                    else
                    {
                        if (game.myMembers.Count == 0)
                            menuController.GetOrAddScreen<GauntletLostPrompt>()._Prompt(game);
                        else
                            Rematch();
                    }

                    break;

                case GameMode.AI_PACK:
                case GameMode.BOSS_AI_PACK:
                    if (!game.draw && game.IsWinner())
                    {
                        if (game.puzzleData.lastInPack)
                        {
                            //force update map
                            GameManager.Instance.currentMap.UpdateWidgets();

                            //open screen for next event
                            BasicPuzzlePack nextPack = GameManager.Instance.currentMap.GetNextPack(game.puzzleData.pack.packID);

                            if (nextPack == null)
                            {
                                GamePlayManager.instance.BackButtonOnClick();
                                return;
                            }
                            else
                                nextPack.StartNextUnsolvedPuzzle();

                            if (nextPack)
                            {
                                switch (nextPack.packType)
                                {
                                    case PackType.AI_PACK:
                                    case PackType.BOSS_AI_PACK:
                                        menuController.GetOrAddScreen<VSGamePrompt>().Prompt(
                                            nextPack,
                                            () => GamePlayManager.instance.BackButtonOnClick(),
                                            () => menuController.CloseCurrentScreen());

                                        break;

                                    case PackType.PUZZLE_PACK:
                                        menuController.GetOrAddScreen<PrePackPrompt>().Prompt(
                                            nextPack,
                                            () => GamePlayManager.instance.BackButtonOnClick(),
                                            () => menuController.CloseCurrentScreen());

                                        break;
                                }
                            }
                            else
                                GamePlayManager.instance.BackButtonOnClick();
                        }
                        else
                            menuController.GetOrAddScreen<VSGamePrompt>().Prompt(game.puzzleData.pack, () => GamePlayManager.instance.BackButtonOnClick());
                    }
                    else
                        Rematch();

                    break;

                default:
                    if (rematchButton.gameObject.activeInHierarchy) Rematch();
                    else if (nextGameButton.gameObject.activeInHierarchy) NextGame();
                    else
                    {
                        if (game.puzzleData && !game.puzzleData.lastInPack)
                            menuController.GetOrAddScreen<VSGamePrompt>().Prompt(game.puzzleData.pack, () => GamePlayManager.instance.gameplayScreen.OnBack());
                        else
                            GamePlayManager.instance.BackButtonOnClick();
                    }

                    break;
            }
        }

        private void SetButtonRowState(bool value)
        {
            buttonsRow.SetActive(value);

            if (value)
            {
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
            }
            else
            {
                nextGameButton.SetActive(false);
                rematchButton.SetActive(false);
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            turnBasedTab = menuController.GetOrAddScreen<TurnBaseScreen>();
        }
    }
}
