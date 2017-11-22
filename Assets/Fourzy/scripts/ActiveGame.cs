using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GameSparks.Core;
using System.Linq;

namespace Fourzy
{
    public class ActiveGame : MonoBehaviour
    {
        public delegate void GameActive();
        public static event GameActive OnActiveGame;

        public delegate void RemoveGame(string challengeInstanceId);
        public static event RemoveGame OnRemoveGame;

        //We store the challengeId, next player's userId and the userId who initiated the challenge.
        public string challengeId;
        public string nextPlayerId;
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

        [Header("Game UI")]
        public Text opponentNameLabel, statusLabel, moveTimeAgo;
        public Image opponentProfilePicture;
        public Texture2D defaultProfilePicture;
        public Image tournamentIcon;

        private GameObject deleteGameButton;
        private int opponentIndex;
        public bool isCurrentPlayer_PlayerOne;
        public bool viewedResult;
        public bool isExpired;
        private Sprite opponentProfilePictureSprite;

        void Start()
        {
            if (challengeShortCode == "tournamentChallenge") {
                tournamentIcon.enabled = true;
            } else {
                tournamentIcon.enabled = false;
            }

            if (playerIds.Count > 1)
            {
                if (playerIds[0] == UserManager.instance.userId)
                {
                    opponentIndex = 1;
                }
                else
                {
                    opponentIndex = 0;
                }

                opponentNameLabel.text = playerNames[opponentIndex];

                string fbID = playerFacebookIds[opponentIndex];
                if (fbID != null) {
                    StartCoroutine(UserManager.instance.GetFBPicture(playerFacebookIds[opponentIndex], (sprite)=>
                        {
                            opponentProfilePictureSprite = sprite;
                            opponentProfilePicture.sprite = sprite;
                        }));
                }
            }
            else
            {
                opponentNameLabel.text = LocalizationManager.instance.GetLocalizedValue("waiting_opponent_text");
                opponentProfilePicture.sprite = Sprite.Create(defaultProfilePicture, 
                    new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height), 
                    new Vector2(0.5f, 0.5f));
            }

            if (winnerId != null)
            {
                if (winnerId == UserManager.instance.userId)
                {
                    statusLabel.text = LocalizationManager.instance.GetLocalizedValue("you_won_text");
                }
                else
                {
                    statusLabel.text = LocalizationManager.instance.GetLocalizedValue("they_won_text");
                }
            } else if (isExpired == true) {
                statusLabel.text = "Game Expired";
            } else {
                //We then check if the userId of the next player is equal to ours
                if (nextPlayerId == UserManager.instance.userId)
                {
                    statusLabel.text = LocalizationManager.instance.GetLocalizedValue("your_move_text");
                } else {
                    statusLabel.text = LocalizationManager.instance.GetLocalizedValue("their_move_text");
                }
            }

            GSData lastPlayerMove = gameState.moveList.LastOrDefault();
            long timestamp = lastPlayerMove.GetLong("timestamp").GetValueOrDefault();

            Debug.Log("LocalizationManager.instance.cultureInfo: " + LocalizationManager.instance.cultureInfo);
            if (timestamp != 0) {
                System.DateTime timestampDateTime = new System.DateTime (1970, 1, 1, 0, 0, 0,System.DateTimeKind.Utc).AddMilliseconds(timestamp).ToLocalTime();
                moveTimeAgo.text = TimeAgo.DateTimeExtensions.TimeAgo(timestampDateTime, LocalizationManager.instance.cultureInfo);                
            }
        }

        public void Initialize(int currentPlayerMove, bool isCurrentPlayerTurn, TokenBoard tokenBoard, int[] lastGameBoard, bool isGameOver, List<GSData> moveList) {
            bool isPlayerOneTurn = currentPlayerMove == (int)Piece.BLUE ? true : false;
            gameState = new GameState(Constants.numRows, Constants.numColumns, isPlayerOneTurn, isCurrentPlayerTurn, tokenBoard, lastGameBoard, isGameOver, moveList);
        }

        private void OnEnable()
        {
            GamesListManager.OnShowDeleteGame += ShowDeleteButton;

            deleteGameButton = transform.Find("DeleteGameButton").gameObject;
            deleteGameButton.SetActive(false);
        }

        private void ShowDeleteButton(bool show) {
            if (challengeState == "COMPLETE" && deleteGameButton != null)
            {
                if (show)
                {
                    deleteGameButton.SetActive(true);
                }
                else
                {
                    deleteGameButton.SetActive(false);
                }
            }
        }

        public void DeleteGame() {
            if (OnRemoveGame != null)
            {
                OnRemoveGame(challengeId);
                gameObject.SetActive(false);
            }
        }

        //Open game gets called OnClick of the play button which happens when selecting an active game in the games list
        public void OpenGame()
        {
            Debug.Log("Open Active Game: challengeInstanceId: " + challengeId);

            GameManager.instance.isLoading = true;
            GameManager.instance.gameState = gameState;

            // All these properties of the ActiveGame will remain the same for the entire lifecycle of the game
            GameManager.instance.challengeInstanceId = challengeId;
            GameManager.instance.isCurrentPlayer_PlayerOne = isCurrentPlayer_PlayerOne;
            GameManager.instance.isMultiplayer = true;
            GameManager.instance.isNewChallenge = false;
            GameManager.instance.isExpired = isExpired;
            GameManager.instance.viewedResult = viewedResult;
            GameManager.instance.opponentProfilePicture.sprite = opponentProfilePictureSprite;
            GameManager.instance.opponentNameLabel.text = opponentNameLabel.text;
            GameManager.instance.playerNameLabel.text = UserManager.instance.userName;
            if (UserManager.instance.profilePicture) {
                GameManager.instance.playerProfilePicture.sprite = UserManager.instance.profilePicture;
            } else {
                GameManager.instance.playerProfilePicture.sprite = Sprite.Create(defaultProfilePicture,
                    new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height),
                    new Vector2(0.5f, 0.5f));
            }
            // -------------------------------------------------------------------------------------------

            GameManager.instance.winner = winnerName;

            GameManager.instance.ResetGamePiecesAndTokens();
            GameManager.instance.ResetUI();
            GameManager.instance.SetupGame("", "");

            //if (!viewedResult && challengeState == "COMPLETE") {
            //    ChallengeManager.instance.SetViewedCompletedGame(challengeId);
            //    viewedResult = true;
            //}

            // Triggers GameManager TransitionToGameScreen
            if (OnActiveGame != null)
                OnActiveGame();
        }
    }
}