using UnityEngine;
using System;

namespace Fourzy
{
    public class GameArea
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
        public PuzzleChallengeLevel puzzleChallengeInfo;
        public Sprite opponentProfilePictureSprite;

        public GameArea(string challengeId, GameState gameState, bool isCurrentPlayer_PlayerOne, bool isExpired, bool didViewResult, Opponent opponent, ChallengeState challengeState, ChallengeType challengeType, string challengerGamePieceId, string challengedGamePieceId, PuzzleChallengeLevel puzzleChallengeInfo, string winnerName, string title, string subtitle, bool displayIntroUI)
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

            InitGameArea();
        }

        public void InitGameArea() {

            // if (opponent.opponentFBId != "")
            // {
            //     CoroutineHandler.StartStaticCoroutine(UserManager.Instance.GetFBPicture(opponent.opponentFBId, (sprite) =>
            //     {
            //         opponentProfilePictureSprite = sprite;
            //     }));
            // }
        }
    }
}