//@vadym udod

using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GameplayScreen : MenuScreen
    {
        public PlayerUIWidget player;
        public PlayerUIWidget opponent;

        public ButtonExtended nextGameButton;
        public ButtonExtended createGameButton;
        public ButtonExtended retryPuzzleChallengeButton;
        public ButtonExtended nextPuzzleChallengeButton;
        public ButtonExtended backButton;
        public ButtonExtended rematchButton;
        public ButtonExtended resignButton;

        private Game game;

        public void InitUI(Game game)
        {
            this.game = game;
            
            //set names
            switch (game.gameState.GameType)
            {
                case GameType.REALTIME:
                case GameType.RANDOM:
                case GameType.FRIEND:
                case GameType.LEADERBOARD:
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

                    player.SetupPlayerName(UserManager.Instance.userName);
                    opponent.SetupPlayerName(opponentName);
                    break;

                default:
                    player.SetupPlayerName("Player 1");
                    opponent.SetupPlayerName("Player 2");
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

            //set icons
            player.InitPlayerIcon(game.boardView.PlayerPiece);
            opponent.InitPlayerIcon(game.boardView.OpponentPiece);

            player.StopPlayerTurnAnimation();
            opponent.StopPlayerTurnAnimation();
        }

        public void UpdatePlayerTurn()
        {
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
        }

        public void ShowWinnerAnimation(bool isPlayerWinner)
        {
            float delay = 0.3f;
            for (int i = 0; i < game.gameState.GameBoard.player1WinningPositions.Count; i++)
            {
                Position position = game.gameState.GameBoard.player1WinningPositions[i];
                GamePiece gamePiece = game.boardView.GamePieceAt(position);
                gamePiece.View.PlayWinAnimation(delay);
                delay += 0.12f;
            }

            delay = 0.3f;
            for (int i = 0; i < game.gameState.GameBoard.player2WinningPositions.Count; i++)
            {
                Position position = game.gameState.GameBoard.player2WinningPositions[i];
                GamePiece gamePiece = game.boardView.GamePieceAt(position);
                gamePiece.View.PlayWinAnimation(delay);
                delay += 0.12f;
            }

            if (isPlayerWinner)
                player.StartWinJumps();
            else
                opponent.StartWinJumps();
        }

        public void SetActionButton()
        {
            if (game.gameState.GameType == GameType.RANDOM || game.gameState.GameType == GameType.FRIEND || game.gameState.GameType == GameType.LEADERBOARD)
            {
                bool hasNextGame = false;
                var games = GameManager.Instance.Games;
                for (int i = 0; i < games.Count; i++)
                {
                    if (games[i].gameState != null)
                    {
                        if ((games[i].gameState.isCurrentPlayerTurn == true || (games[i].didViewResult == false && games[i].gameState.IsGameOver == true && games[i].gameState.isCurrentPlayerTurn == false)) && games[i].challengeId != game.challengeId)
                        {
                            hasNextGame = true;
                        }
                    }
                    else
                    {
                        AnalyticsManager.LogError("set_action_button_error", "GameState is null for challengeId: " + games[i].challengeId);
                    }
                }

                if (hasNextGame)
                {
                    nextGameButton.gameObject.SetActive(true);
                    createGameButton.gameObject.SetActive(false);
                }
                else
                {
                    createGameButton.gameObject.SetActive(true);
                    nextGameButton.gameObject.SetActive(false);
                }
            }
            else
            {
                if (game.gameState.IsGameOver)
                {
                    if (game.gameState.GameType == GameType.PUZZLE)
                    {
                        if (game.gameState.IsPuzzleChallengePassed)
                        {
                            nextPuzzleChallengeButton.gameObject.SetActive(true);
                        }
                        else
                        {
                            retryPuzzleChallengeButton.gameObject.SetActive(true);
                        }
                    }
                    else if (!GameManager.Instance.isOnboardingActive && game.gameState.GameType == GameType.PASSANDPLAY)
                    {
                        rematchButton.gameObject.SetActive(true);
                    }
                }
            }
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
            GamePlayManager.Instance.NextPuzzleChallengeButtonOnClick();
        }

        public void RetryPuzzleButton()
        {
            GamePlayManager.Instance.RetryPuzzleChallengeButtonOnClick();
        }
    }
}
