//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GameplayScreen : MenuScreen
    {
        public PlayerUIWidget playerWidget;
        public PlayerUIWidget opponentWidget;

        public ButtonExtended createGameButton;
        public ButtonExtended rematchButton;
        public ButtonExtended resignButton;

        private IClientFourzy game;
        private PuzzleUIScreen puzzleUI;
        private TurnBaseScreen turnbaseUI;
        private SpellsListUIWidget spellsListWidget;

        protected override void Awake()
        {
            base.Awake();

            puzzleUI = GetComponentInChildren<PuzzleUIScreen>();
            turnbaseUI = GetComponentInChildren<TurnBaseScreen>();
            spellsListWidget = GetComponentInChildren<SpellsListUIWidget>();
        }

        public override void OnBack()
        {
            base.OnBack();

            switch (game._Type)
            {
                case GameType.PASSANDPLAY:
                    if (game._State.WinningLocations == null)
                        menuController.GetScreen<PromptScreen>().Prompt("Leave Game?", "", "Yes", "No", () => { GamePlayManager.instance.BackButtonOnClick(); });
                    else
                        GamePlayManager.instance.BackButtonOnClick();

                    break;

                case GameType.PUZZLE:
                    GamePlayManager.instance.BackButtonOnClick();

                    break;

                case GameType.TURN_BASED:
                    GamePlayManager.instance.BackButtonOnClick();

                    break;

                case GameType.FRIEND:
                case GameType.LEADERBOARD:
                    if (game.isOver)
                        GamePlayManager.instance.BackButtonOnClick();
                    else
                        menuController.GetScreen<PromptScreen>().Prompt("Leave Game?", "", "Yes", "No", () => { GamePlayManager.instance.BackButtonOnClick(); });

                    break;

                default:
                    menuController.GetScreen<PromptScreen>().Prompt("Leave Game?", "", "Yes", "No", () => { GamePlayManager.instance.BackButtonOnClick(); });

                    break;
            }
        }

        public void InitUI(GamePlayManager gamePlayManager)
        {
            game = gamePlayManager.game;

            playerWidget.SetGame(game);
            opponentWidget.SetGame(game);

            DisableButtons();

            playerWidget.Initialize();
            opponentWidget.Initialize();

            playerWidget.SetPlayerName(game.me.DisplayName);
            opponentWidget.SetPlayerName(game.opponent.DisplayName);

            playerWidget.StopPlayerTurnAnimation();
            opponentWidget.StopPlayerTurnAnimation();

            playerWidget.SetPlayerIcon(game.me);
            opponentWidget.SetPlayerIcon(game.opponent);

            switch (game._Type)
            {
                case GameType.PUZZLE:
                    opponentWidget.alphaTween.SetAlpha(0f);

                    break;

                case GameType.ONBOARDING:
                    opponentWidget.alphaTween.SetAlpha(PersistantMenuController.instance.GetScreen<OnboardingScreen>().current.showPlayer2 ? 1f : 0f);

                    break;
            }

            puzzleUI.Open(game);
            turnbaseUI.Open(game);
            spellsListWidget.Open(game, gamePlayManager.board);
        }

        public void UpdatePlayerTurn()
        {
            switch (game._Type)
            {
                case GameType.PUZZLE:
                    puzzleUI.UpdateWidgets();

                    break;
            }

            if (game.isOver)
            {
                playerWidget.StopPlayerTurnAnimation();
                opponentWidget.StopPlayerTurnAnimation();

                return;
            }

            spellsListWidget.UpdateSpells(game._State.ActivePlayerId);

            switch (game._Type)
            {
                case GameType.REALTIME:
                case GameType.TURN_BASED:
                    if (game.isMyTurn)
                    {
                        playerWidget.ShowPlayerTurnAnimation();
                        opponentWidget.StopPlayerTurnAnimation();
                    }
                    else
                    {
                        opponentWidget.ShowPlayerTurnAnimation();
                        playerWidget.StopPlayerTurnAnimation();
                    }

                    break;

                case GameType.FRIEND:
                case GameType.LEADERBOARD:
                case GameType.PASSANDPLAY:
                case GameType.AI:
                    if (game.isMyTurn)
                    {
                        playerWidget.ShowPlayerTurnAnimation();
                        opponentWidget.StopPlayerTurnAnimation();
                    }
                    else
                    {
                        opponentWidget.ShowPlayerTurnAnimation();
                        playerWidget.StopPlayerTurnAnimation();
                    }

                    break;

                case GameType.ONBOARDING:
                case GameType.PUZZLE:
                    if (game.isMyTurn)
                        playerWidget.ShowPlayerTurnAnimation();
                    else
                        playerWidget.StopPlayerTurnAnimation();

                    break;
            }
        }

        public void OnGameFinished()
        {
            switch (game._Type)
            {
                case GameType.PUZZLE:
                    if (game.IsWinner())
                    {
                        playerWidget.StartWinJumps();

                        puzzleUI.Complete();
                    }
                    else
                        CheckButtons();
                    break;

                default:
                    if (game.IsWinner())
                        playerWidget.StartWinJumps();
                    else
                        opponentWidget.StartWinJumps();

                    CheckButtons();
                    break;
            }
        }

        public void DisableButtons()
        {
            createGameButton.SetActive(false);
            rematchButton.SetActive(false);
            resignButton.SetActive(false);
        }

        public void CheckButtons()
        {
            switch (game._Type)
            {
                //case GameType.REALTIME:
                case GameType.TURN_BASED:
                    rematchButton.GetComponentInChildren<LocalizedText>().UpdateLocale("rematch_button");
                    rematchButton.SetActive(true);

                    break;

                case GameType.AI:
                case GameType.PASSANDPLAY:
                case GameType.PUZZLE:
                    rematchButton.GetComponentInChildren<LocalizedText>().UpdateLocale("retry_challenge_button");
                    rematchButton.SetActive(true);

                    break;
            }
        }
    }
}
