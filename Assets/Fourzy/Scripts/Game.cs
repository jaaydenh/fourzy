using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //public string opponentFBId = "";
        public PlayerData playerData;
        public ChallengeState challengeState;
        public ChallengeType challengeType;
        public GameType gameType;

        //public string opponentName;
        public string winnerName;
        public string winnerUserId;
        public Sprite opponentProfilePictureSprite;

        public delegate void GameActive();
        public static event GameActive OnActiveGame;

        public Game(string challengeId, GameState gameState, bool isCurrentPlayer_PlayerOne, bool isExpired, bool didViewResult, PlayerData playerData, ChallengeState challengeState, ChallengeType challengeType, GameType gameType)
        {
            this.challengeId = challengeId;
            this.gameState = gameState;
            this.isCurrentPlayer_PlayerOne = isCurrentPlayer_PlayerOne;
            this.isExpired = isExpired;
            this.didViewResult = didViewResult;
            this.playerData = playerData;
            this.challengeState = challengeState;
            this.challengeType = challengeType;
            this.gameType = gameType;
            // Always visible until deleting games is implemented
            this.isVisible = true;
            InitGame();
        }

        public void InitGame() {

            //opponentName = playerData.opponentName;

            if (playerData.opponentFBId != "")
            {
                CoroutineHandler.StartStaticCoroutine(UserManager.instance.GetFBPicture(playerData.opponentFBId, (sprite) =>
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
            GameManager.instance.gameType = gameType;   
            // -------------------------------------------------------------------------------------------

            GameManager.instance.winner = winnerName;

            GameManager.instance.ResetGamePiecesAndTokens();
            GameManager.instance.ResetUI();

            //GameManager.instance.UpdatePlayerUI();
            GameManager.instance.SetupGame("", "");
            GameManager.instance.InitPlayerUI(playerData.opponentName, opponentProfilePictureSprite);

            // Triggers GameManager TransitionToGameScreen
            if (OnActiveGame != null)
                OnActiveGame();
        }
    }
}