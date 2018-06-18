﻿using UnityEngine;
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
        public bool displayIntroUI;
        public Opponent opponent;
        public ChallengeState challengeState;
        public ChallengeType challengeType;
        public int challengerGamePieceId;
        public int challengedGamePieceId;
        public string winnerName;
        public string winnerUserId;
        public int challengerRatingDelta;
        public int challengedRatingDelta;
        public string title;
        public string subtitle;
        public PuzzleChallengeInfo puzzleChallengeInfo;
        public Sprite opponentProfilePictureSprite;

        public delegate void GameActive();
        public static event GameActive OnActiveGame;

        public Game(string challengeId, GameState gameState, bool isCurrentPlayer_PlayerOne, bool isExpired, bool didViewResult, Opponent opponent, ChallengeState challengeState, ChallengeType challengeType, string challengerGamePieceId, string challengedGamePieceId, PuzzleChallengeInfo puzzleChallengeInfo, string winnerName, string title, string subtitle, bool displayIntroUI)
        {
            this.challengeId = challengeId;
            this.gameState = gameState;
            this.isCurrentPlayer_PlayerOne = isCurrentPlayer_PlayerOne;
            this.isExpired = isExpired;
            this.didViewResult = didViewResult;
            this.opponent = opponent;
            this.challengeState = challengeState;
            this.challengeType = challengeType;
            this.puzzleChallengeInfo = puzzleChallengeInfo;
            this.winnerName = winnerName;
            this.title = title;
            this.subtitle = subtitle;
            this.displayIntroUI = displayIntroUI;

            // Always visible until deleting games is implemented
            this.isVisible = true;
            this.challengerGamePieceId = 0;
            Int32.TryParse(challengerGamePieceId, out this.challengerGamePieceId);
            this.challengedGamePieceId = 0;
            Int32.TryParse(challengedGamePieceId, out this.challengedGamePieceId);

            InitGame();
        }

        public void InitGame() {

            // if (opponent.opponentFBId != "")
            // {
            //     CoroutineHandler.StartStaticCoroutine(UserManager.instance.GetFBPicture(opponent.opponentFBId, (sprite) =>
            //     {
            //         opponentProfilePictureSprite = sprite;
            //     }));
            // }
        }

        public void OpenGame()
        {
            Debug.Log("Open Game: challengeId: " + challengeId);

            GameManager.instance.activeGame = this;

            // Triggers GameManager TransitionToGameScreen
            if (OnActiveGame != null)
                OnActiveGame();
        }
    }
}