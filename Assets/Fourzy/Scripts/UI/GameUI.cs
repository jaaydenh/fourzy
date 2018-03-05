﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GameSparks.Core;
using System.Linq;

namespace Fourzy
{
    public class GameUI : MonoBehaviour
    {
        public delegate void GameActive();
        public static event GameActive OnActiveGame;

        public delegate void RemoveGame(string challengeInstanceId);
        public static event RemoveGame OnRemoveGame;

        //We store the challengeId, next player's userId and the userId who initiated the challenge.
        public string challengeId;
        //public string nextPlayerId;
        public string challengerId;
        public string winnerName;
        public string winnerId;
        public string challengeState;
        public string challengeShortCode;

        //We create a list for playerNames and Ids
        public List<string> playerNames = new List<string>();
        public List<string> playerIds = new List<string>();
        public List<string> playerFacebookIds = new List<string>();

        public GameState gameState;
        public Game game;

        [Header("Game UI")]
        public Text opponentNameLabel, statusLabel, moveTimeAgo;
        public Image opponentProfilePicture;
        public Texture2D defaultProfilePicture;
        public Image tournamentIcon;

        private GameObject deleteGameButton;
        private int opponentIndex;
        public bool isCurrentPlayer_PlayerOne;
        public bool isExpired;
        //private Sprite opponentProfilePictureSprite;
        private PlayerEnum currentplayer = PlayerEnum.NONE;

        void Start()
        {
            if (game == null) {
                Debug.Log("game is null");
            }

            if (game.challengeType == ChallengeType.TOURNAMENT) {
                tournamentIcon.enabled = true;
            } else {
                tournamentIcon.enabled = false;
            }
            if (game.opponent == null) {
                Debug.Log("game.opponent is null");
            }
            opponentNameLabel.text = game.opponent.opponentName;
            //Debug.Log("game.opponentProfilePictureSprite: " + game.opponentProfilePictureSprite);
            if (game.opponentProfilePictureSprite == null) {
                if (game.opponent.opponentFBId != null) {
                    //Debug.Log("game.playerData.opponentFBId: " + game.playerData.opponentFBId);    
                    StartCoroutine(UserManager.instance.GetFBPicture(game.opponent.opponentFBId, (sprite)=>
                        {
                            //opponentProfilePictureSprite = sprite;
                            opponentProfilePicture.sprite = sprite;
                        }));
                }
            } else {
                opponentProfilePicture.sprite = game.opponentProfilePictureSprite;
            }

            if (game.isCurrentPlayer_PlayerOne) {
                currentplayer = PlayerEnum.ONE;
            } else {
                currentplayer = PlayerEnum.TWO;
            }
                
            if (game.gameState.winner != PlayerEnum.EMPTY)
            {
                //Debug.Log("ActiveGame game.gameState.winner: " + game.gameState.winner);
                //Debug.Log("ActiveGame currentplayer: " + currentplayer);

                if (game.gameState.winner == currentplayer)
                {
                    statusLabel.text = LocalizationManager.instance.GetLocalizedValue("you_won_text");
                }
                else if (game.gameState.winner == PlayerEnum.NONE || game.gameState.winner == PlayerEnum.ALL)
                {
                    statusLabel.text = LocalizationManager.instance.GetLocalizedValue("draw_text");
                } else {
                    statusLabel.text = LocalizationManager.instance.GetLocalizedValue("they_won_text");
                }
            } else if (game.isExpired == true) {
                statusLabel.text = "Game Expired";
            } else {
                //We then check if the userId of the next player is equal to ours
                //if (nextPlayerId == UserManager.instance.userId)
                if (game.gameState.isCurrentPlayerTurn)
                {
                    statusLabel.text = LocalizationManager.instance.GetLocalizedValue("your_move_text");
                } else {
                    statusLabel.text = LocalizationManager.instance.GetLocalizedValue("their_move_text");
                }
            }

            if (game.gameState.moveList.LastOrDefault() == null) {
                Debug.Log("game.gameState.moveList == null");
            }

            GSData lastPlayerMove = game.gameState.moveList.LastOrDefault();
            long timestamp = 0;
            if (lastPlayerMove != null) {
               timestamp = lastPlayerMove.GetLong("timestamp").GetValueOrDefault();    
            }

            if (timestamp != 0) {
                System.DateTime timestampDateTime = new System.DateTime (1970, 1, 1, 0, 0, 0,System.DateTimeKind.Utc).AddMilliseconds(timestamp).ToLocalTime();
                moveTimeAgo.text = TimeAgo.DateTimeExtensions.TimeAgo(timestampDateTime, LocalizationManager.instance.cultureInfo);                
            }
        }

        //public void Initialize(int currentPlayerMove, bool isCurrentPlayerTurn, TokenBoard tokenBoard, int[] lastGameBoard, bool isGameOver, List<GSData> moveList) {
        //    bool isPlayerOneTurn = currentPlayerMove == (int)Piece.BLUE ? true : false;
        //    gameState = new GameState(Constants.numRows, Constants.numColumns, isPlayerOneTurn, isCurrentPlayerTurn, tokenBoard, lastGameBoard, isGameOver, moveList);
        //}

        private void OnEnable()
        {
            //GamesListManager.OnShowDeleteGame += ShowDeleteButton;

            //deleteGameButton = transform.Find("DeleteGameButton").gameObject;
            //deleteGameButton.SetActive(false);
        }

        //private void ShowDeleteButton(bool show) {
        //    if (challengeState == "COMPLETE" && deleteGameButton != null)
        //    {
        //        if (show)
        //        {
        //            deleteGameButton.SetActive(true);
        //        }
        //        else
        //        {
        //            deleteGameButton.SetActive(false);
        //        }
        //    }
        //}

        //public void DeleteGame() {
        //    if (OnRemoveGame != null)
        //    {
        //        OnRemoveGame(challengeId);
        //        gameObject.SetActive(false);
        //    }
        //}

        //Open game gets called OnClick of the play button which happens when selecting an active game in the games list
        public void OpenGame()
        {
            Debug.Log("Open GameUI: challengeInstanceId: " + challengeId);

            GameManager.instance.isLoading = true;
            GameManager.instance.gameState = game.gameState;

            // All these properties of the GameUI will remain the same for the entire lifecycle of the game
            GameManager.instance.challengeInstanceId = game.challengeId;
            //Debug.Log("Active game: isCurrentPlayer_PlayerOne: " + game.isCurrentPlayer_PlayerOne);
            GameManager.instance.isCurrentPlayer_PlayerOne = game.isCurrentPlayer_PlayerOne;
            GameManager.instance.isMultiplayer = true;
            GameManager.instance.isNewChallenge = false;
            GameManager.instance.isExpired = game.isExpired;
            GameManager.instance.didViewResult = game.didViewResult;
            GameManager.instance.gameType = game.gameType;
            GameManager.instance.challengerGamePieceId = game.challengerGamePieceId;
            GameManager.instance.challengedGamePieceId = game.challengedGamePieceId;
            // -------------------------------------------------------------------------------------------

            GameManager.instance.winner = winnerName;

            GameManager.instance.ResetGamePiecesAndTokens();
            GameManager.instance.ResetUIGameScreen();

            //GameManager.instance.UpdatePlayerUI();
            GameManager.instance.SetupGame("", "");
            GameManager.instance.InitPlayerUI(game.opponent.opponentName, game.opponentProfilePictureSprite);
            // Triggers GameManager TransitionToGameScreen
            if (OnActiveGame != null)
                OnActiveGame();
        }
    }
}