using UnityEngine;
using System;

namespace Fourzy
{
    public class Game
    {
        public string challengeId;
        public GameState gameState;
        public bool isCurrentPlayer_PlayerOne;
        public bool isExpired;
        public bool didViewResult;
        public bool isVisible;
        public Opponent opponent;
        public ChallengeState challengeState;
        public ChallengeType challengeType;
        // public GameType gameType;
        public int challengerGamePieceId;
        public int challengedGamePieceId;
        public string winnerName;
        public string winnerUserId;
        public int challengerRatingDelta;
        public int challengedRatingDelta;
        public Sprite opponentProfilePictureSprite;

        public delegate void GameActive();
        public static event GameActive OnActiveGame;

        public Game(string challengeId, GameState gameState, bool isCurrentPlayer_PlayerOne, bool isExpired, bool didViewResult, Opponent opponent, ChallengeState challengeState, ChallengeType challengeType, string challengerGamePieceId, string challengedGamePieceId)
        {
            //Debug.Log("challengeId: " + challengeId);
            this.challengeId = challengeId;
            this.gameState = gameState;
            this.isCurrentPlayer_PlayerOne = isCurrentPlayer_PlayerOne;
            this.isExpired = isExpired;
            this.didViewResult = didViewResult;
            this.opponent = opponent;
            this.challengeState = challengeState;
            this.challengeType = challengeType;
            // this.gameType = gameType;

            // Always visible until deleting games is implemented
            this.isVisible = true;
            this.challengerGamePieceId = 0;
            Int32.TryParse(challengerGamePieceId, out this.challengerGamePieceId);
            this.challengedGamePieceId = 0;
            Int32.TryParse(challengedGamePieceId, out this.challengedGamePieceId);

            InitGame();
        }

        public void InitGame() {

            //opponentName = playerData.opponentName;

            if (opponent.opponentFBId != "")
            {
                CoroutineHandler.StartStaticCoroutine(UserManager.instance.GetFBPicture(opponent.opponentFBId, (sprite) =>
                {
                    opponentProfilePictureSprite = sprite;
                }));
            }
        }

        public void OpenGame()
        {
            Debug.Log("Open Game: challengeInstanceId: " + challengeId);

            GameManager.instance.isLoading = true;
            GameManager.instance.gameState = gameState;

            // All these properties of the Game will remain the same for the entire lifecycle of the game
            GameManager.instance.challengeInstanceId = challengeId;
            GameManager.instance.isCurrentPlayer_PlayerOne = isCurrentPlayer_PlayerOne;
            GameManager.instance.isMultiplayer = true;
            GameManager.instance.isNewRandomChallenge = false;
            GameManager.instance.isNewChallenge = false;
            GameManager.instance.isExpired = isExpired;
            GameManager.instance.didViewResult = didViewResult;
            // GameManager.instance.gameType = gameType;
            GameManager.instance.challengerGamePieceId = challengerGamePieceId;
            GameManager.instance.challengedGamePieceId = challengedGamePieceId;
            GameManager.instance.opponentUserId = opponent.opponentId;
            GameManager.instance.activeGame = this;
            // -------------------------------------------------------------------------------------------

            GameManager.instance.winner = winnerName;

            GameManager.instance.ResetGamePiecesAndTokens();
            GameManager.instance.ResetUIGameScreen();
            GameManager.instance.SetupGame("", "");
            GameManager.instance.InitPlayerUI(opponent.opponentName, opponentProfilePictureSprite);

            // Triggers GameManager TransitionToGameScreen
            if (OnActiveGame != null)
                OnActiveGame();
        }
    }
}