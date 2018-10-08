using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GameSparks.Core;
using System.Linq;
using GameSparks.Api.Responses;

namespace Fourzy
{
    public class GameUI : MonoBehaviour
    {
        public string challengeId;
        public string challengerId;
        public string winnerName;
        public string winnerId;
        public string challengeState;
        public string challengeShortCode;

        public Game game;

        [Header("Game UI")]
        public Text opponentNameLabel, statusLabel, moveTimeAgo;
        public Image opponentProfilePicture;
        // public Texture2D defaultProfilePicture;
        public Image tournamentIcon;

        private GameObject deleteGameButton;
        private int opponentIndex;
        public bool isCurrentPlayer_PlayerOne;
        public bool isExpired;
        private PlayerEnum currentplayer = PlayerEnum.NONE;

        void Start()
        {
            if (game == null) {
                Debug.Log("game is null");
                return;
            }

            if (game.challengeType == ChallengeType.TOURNAMENT) {
                tournamentIcon.enabled = true;
            } else {
                tournamentIcon.enabled = false;
            }
            if (game.opponent == null) {
                Debug.Log("game.opponent is null");
                opponentNameLabel.text = LocalizationManager.Instance.GetLocalizedValue("waiting_opponent_text");
            } else {
                opponentNameLabel.text = game.opponent.opponentName;
            }

            // if (game.opponentProfilePictureSprite == null) {
            //     if (game.opponent.opponentFBId != null) {
            //         StartCoroutine(UserManager.Instance.GetFBPicture(game.opponent.opponentFBId, (sprite)=>
            //             {
            //                 opponentProfilePicture.sprite = sprite;
            //             }));
            //     }
            // } else {
            //     opponentProfilePicture.sprite = game.opponentProfilePictureSprite;
            // }

            if (game.opponentProfilePictureSprite != null) {
                opponentProfilePicture.sprite = game.opponentProfilePictureSprite;
            }

            if (game.isCurrentPlayer_PlayerOne) {
                currentplayer = PlayerEnum.ONE;
            } else {
                currentplayer = PlayerEnum.TWO;
            }
                
            if (game.gameState.Winner != PlayerEnum.EMPTY)
            {
                //Debug.Log("ActiveGame game.gameState.winner: " + game.gameState.winner);
                //Debug.Log("ActiveGame currentplayer: " + currentplayer);

                if (game.gameState.Winner == currentplayer)
                {
                    statusLabel.text = LocalizationManager.Instance.GetLocalizedValue("you_won_text");
                }
                else if (game.gameState.Winner == PlayerEnum.NONE || game.gameState.Winner == PlayerEnum.ALL)
                {
                    statusLabel.text = LocalizationManager.Instance.GetLocalizedValue("draw_text");
                } else {
                    statusLabel.text = LocalizationManager.Instance.GetLocalizedValue("they_won_text");
                }
            } else if (game.isExpired == true) {
                statusLabel.text = LocalizationManager.Instance.GetLocalizedValue("expired_text");
            } else {
                //We then check if the userId of the next player is equal to ours
                //if (nextPlayerId == UserManager.Instance.userId)
                if (game.gameState.isCurrentPlayerTurn)
                {
                    statusLabel.text = LocalizationManager.Instance.GetLocalizedValue("your_move_text");
                } else {
                    statusLabel.text = LocalizationManager.Instance.GetLocalizedValue("their_move_text");
                }
            }

            if (game.gameState.MoveList.LastOrDefault() == null) {
                Debug.Log("game.gameState.moveList == null");
            }

            Move lastPlayerMove = game.gameState.MoveList.LastOrDefault();
            long timestamp = 0;
            if (lastPlayerMove != null) {
                timestamp = lastPlayerMove.timeStamp;
            }

            if (timestamp != 0) {
                System.DateTime timestampDateTime = new System.DateTime (1970, 1, 1, 0, 0, 0,System.DateTimeKind.Utc).AddMilliseconds(timestamp).ToLocalTime();
                moveTimeAgo.text = TimeAgo.DateTimeExtensions.TimeAgo(timestampDateTime, LocalizationManager.Instance.cultureInfo);                
            }
        }

        public void UpdateGamesListItemUI() {
            ChallengeManager.Instance.GetGamePiece(game.opponent.opponentId, GetGamePieceIdSuccess, GetGamePieceIdFailure);
        }

        private void GetGamePieceIdSuccess(LogEventResponse response) {
            if (response.ScriptData != null) {
                var gamePieceIdString = response.ScriptData.GetString("gamePieceId");
                int gamePieceId = int.Parse(gamePieceIdString);
                game.opponentProfilePictureSprite = GameContentManager.Instance.GetGamePieceSprite(gamePieceId);
                if (opponentProfilePicture != null) {
                    opponentProfilePicture.sprite = game.opponentProfilePictureSprite;
                    opponentProfilePicture.enabled = true;
                }
            }
        }

        private void GetGamePieceIdFailure(LogEventResponse response) {
            Debug.Log("***** Error getting player gamepiece: " + response.Errors.JSON);
            AnalyticsManager.LogError("get_player_gamepiece_error", response.Errors.JSON);
            game.opponentProfilePictureSprite = GameContentManager.Instance.GetGamePieceSprite(0);
            opponentProfilePicture.sprite = game.opponentProfilePictureSprite;
            opponentProfilePicture.enabled = true;
        }

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

            ViewController.instance.view1.Hide();
            ViewController.instance.HideTabView();
            UITabManager.instance.ResetAllTabs();

            GameManager.Instance.OpenGame(game);
        }
    }
}