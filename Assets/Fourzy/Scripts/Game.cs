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
        public string opponentFBId;

        private string opponentName;
        private string winner;
        private string winnerUserId;
        private Sprite opponentProfilePictureSprite;

        public Game(string challengeId, GameState gameState, bool isCurrentPlayer_PlayerOne, bool isExpired, bool didViewResult, string opponentFBId)
        {
            this.challengeId = challengeId;
            this.gameState = gameState;
            this.isCurrentPlayer_PlayerOne = isCurrentPlayer_PlayerOne;
            this.isExpired = isExpired;
            this.didViewResult = didViewResult;
            this.opponentFBId = opponentFBId;

            InitGame();
        }

        private void InitGame() {
            
        }
    }
}