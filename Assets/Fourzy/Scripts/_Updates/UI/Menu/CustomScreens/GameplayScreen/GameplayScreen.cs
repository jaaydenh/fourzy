//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GameplayScreen : MenuScreen
    {
        public PlayerUIWidget player;
        public PlayerUIWidget opponent;

        public ButtonExtended createGameButton;
        public ButtonExtended retryPuzzleChallengeButton;
        public ButtonExtended backButton;
        public ButtonExtended rematchButton;
        public ButtonExtended resignButton;

        private Game game;

        //tabs
        private PuzzleUIScreen puzzleUI;

        protected override void Awake()
        {
            base.Awake();

            puzzleUI = GetComponentInChildren<PuzzleUIScreen>();
        }

        public void InitUI(Game game)
        {
            this.game = game;

            //set names
            player.SetupPlayerName(UserManager.Instance.userName);
            switch (game.gameState.GameType)
            {
                case GameType.REALTIME:
                case GameType.RANDOM:
                case GameType.FRIEND:
                case GameType.LEADERBOARD:
                case GameType.PASSANDPLAY:
                    string opponentName;
                    if (game.opponent != null && game.opponent.opponentName != null && game.opponent.opponentName != "")
                    {
                        Debug.Log("game.opponent.opponentName: " + game.opponent.opponentName);
                        opponentName = game.opponent.opponentName;
                    }
                    else
                    {
                        opponentName = LocalizationManager.Instance.GetLocalizedValue("waiting_opponent_text");
                    }

                    opponent.SetupPlayerName(opponentName);

                    //set icons
                    player.InitPlayerIcon(game.boardView.PlayerPiece);
                    opponent.InitPlayerIcon(game.boardView.OpponentPiece);

                    player.StopPlayerTurnAnimation();
                    opponent.StopPlayerTurnAnimation();
                    break;

                case GameType.PUZZLE:
                    opponent.SetActive(false);

                    player.InitPlayerIcon(game.boardView.PlayerPiece);
                    player.StopPlayerTurnAnimation();
                    break;
            }

            //extra
            switch (game.gameState.GameType)
            {
                case GameType.REALTIME:
                    backButton.SetActive(false);
                    resignButton.SetActive(true);
                    break;
                default:
                    resignButton.SetActive(false);
                    break;
            }

            puzzleUI.Open(game);
        }

        public void UpdatePlayerTurn()
        {
            switch (game.gameState.GameType)
            {
                case GameType.REALTIME:
                case GameType.RANDOM:
                case GameType.FRIEND:
                case GameType.LEADERBOARD:
                case GameType.PASSANDPLAY:
                    if (game.gameState.IsPlayerOneTurn == game.isCurrentPlayer_PlayerOne)
                    {
                        player.ShowPlayerTurnAnimation();
                        opponent.StopPlayerTurnAnimation();
                    }
                    else if (game.gameState.IsPlayerOneTurn != game.isCurrentPlayer_PlayerOne)
                    {
                        opponent.ShowPlayerTurnAnimation();
                        player.StopPlayerTurnAnimation();
                    }
                    break;
                case GameType.PUZZLE:
                    if (game.gameState.IsPlayerOneTurn == game.isCurrentPlayer_PlayerOne)
                        player.ShowPlayerTurnAnimation();
                    else if (game.gameState.IsPlayerOneTurn != game.isCurrentPlayer_PlayerOne)
                        player.StopPlayerTurnAnimation();
                    break;
            }
        }

        public void UpdateTabs()
        {
            switch (game.gameState.GameType)
            {
                case GameType.PUZZLE:
                    puzzleUI.UpdateWidgets();
                    break;
            }
        }

        public void ShowWinnerAnimation(bool isPlayerWinner)
        {
            float delay = 0.3f;
            for (int i = 0; i < game.gameState.GameBoard.player1WinningPositions.Count; i++)
            {
                Position position = game.gameState.GameBoard.player1WinningPositions[i];
                GamePiece gamePiece = game.boardView.GamePieceAt(position);
                gamePiece.gamePieceView.PlayWinAnimation(delay);
                delay += 0.12f;
            }

            delay = 0.3f;
            for (int i = 0; i < game.gameState.GameBoard.player2WinningPositions.Count; i++)
            {
                Position position = game.gameState.GameBoard.player2WinningPositions[i];
                GamePiece gamePiece = game.boardView.GamePieceAt(position);
                gamePiece.gamePieceView.PlayWinAnimation(delay);
                delay += 0.12f;
            }

            switch (game.gameState.GameType)
            {
                case GameType.PUZZLE:
                    if (isPlayerWinner)
                        player.StartWinJumps();
                    break;

                default:
                    if (isPlayerWinner)
                        player.StartWinJumps();
                    else
                        opponent.StartWinJumps();
                    break;
            }

            //show rewards screen
            //ShowRewardsScreen();
        }

        public void SetActionButton()
        {
            if (game.gameState.GameType == GameType.RANDOM || game.gameState.GameType == GameType.FRIEND || game.gameState.GameType == GameType.LEADERBOARD)
            {
                //bool hasNextGame = false;
                //var games = GameManager.Instance.Games;
                //for (int i = 0; i < games.Count; i++)
                //{
                //    if (games[i].gameState != null)
                //    {
                //        if ((games[i].gameState.isCurrentPlayerTurn == true || (games[i].didViewResult == false && games[i].gameState.IsGameOver == true && games[i].gameState.isCurrentPlayerTurn == false)) && games[i].challengeId != game.challengeId)
                //        {
                //            hasNextGame = true;
                //        }
                //    }
                //    else
                //    {
                //        AnalyticsManager.LogError("set_action_button_error", "GameState is null for challengeId: " + games[i].challengeId);
                //    }
                //}

                //if (hasNextGame)
                //{
                //    nextGameButton.gameObject.SetActive(true);
                //    createGameButton.gameObject.SetActive(false);
                //}
                //else
                //{
                //    createGameButton.gameObject.SetActive(true);
                //    nextGameButton.gameObject.SetActive(false);
                //}
            }
            else
            {
                if (game.gameState.IsGameOver)
                {
                    if (!GameManager.Instance.isOnboardingActive && game.gameState.GameType == GameType.PASSANDPLAY)
                    {
                        rematchButton.gameObject.SetActive(true);
                    }
                }
            }
        }

        public void ShowRewardsScreen()
        {
            //GameManager.Instance.VisitedGameResults(game);

            menuController.GetScreen<RewardScreen>().OpenScreen(new Reward[] {
                    new Reward()
                    {
                        Type = Reward.RewardType.Coins,
                        NumberOfCoins = 10
                    },
                    new Reward()
                    {
                        Type = Reward.RewardType.CollectedGamePiece,
                        CollectedGamePiece = new GamePieceData()
                    }
                });
        }

        public void OnBackButton()
        {
            GamePlayManager.Instance.BackButtonOnClick();
        }

        public void RealtimeResignButton()
        {
            GamePlayManager.Instance.RealtimeResignButtonOnClick();
        }

        public void RematchPassAndPlay()
        {
            GamePlayManager.Instance.RematchPassAndPlayGameButtonOnClick();
        }

        public void NextGameButton()
        {
            GamePlayManager.Instance.NextGameButtonOnClick();
        }

        public void CreateGameButton()
        {
            GamePlayManager.Instance.CreateGameButtonOnClick();
        }

        public void NextChallengeButton()
        {
            GameManager.Instance.SetNextActivePuzzleLevel();
            GamePlayManager.Instance.NextPuzzleChallengeButtonOnClick();
        }

        public void RetryPuzzleButton()
        {
            GamePlayManager.Instance.RetryPuzzleChallengeButtonOnClick();
        }
    }
}
