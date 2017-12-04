using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace Fourzy
{
    public class LeaderboardPlayer : MonoBehaviour {

        public delegate void GameActive();
        public static event GameActive OnActiveGame;
        public Text playerNameLabel, rankLabel, ratingLabel;
        public Image profilePicture;
        public Texture2D defaultProfilePicture;
        public Image onlineTexture;
        public bool isOnline;
        public string userName, id, facebookId;
        private GameObject UIScreen;

        void Start () {
            UIScreen = GameObject.Find("UI Screen");
            UpdatePlayer();
        }

        public void UpdatePlayer()
        {
            onlineTexture.color = isOnline ? Color.green : Color.gray;

            if (facebookId != null) {
                StartCoroutine(UserManager.instance.GetFBPicture(facebookId, (sprite)=>
                    {
                        profilePicture.sprite = sprite;
                    }));
            } else {
                profilePicture.sprite = Sprite.Create(defaultProfilePicture, 
                    new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height), 
                    new Vector2(0.5f, 0.5f));
            }
        }


        public void OpenNewLeaderboardChallengeGame()
        {
            Debug.Log("OpenNewLeaderboardChallengeGame userName: " + userName);
            GameManager.instance.TransitionToGameOptionsScreen(GameType.LEADERBOARD, id, userName, profilePicture);

            //GameManager.instance.ResetGamePiecesAndTokens();
            //TokenBoard tokenBoard = TokenBoardLoader.instance.GetTokenBoard();

            ////If we initiated the challenge, we get to be player 1
            //GameState gameState = new GameState(Constants.numRows, Constants.numColumns, true, true, tokenBoard, null, false, null);
            //GameManager.instance.gameState = gameState;

            //GameManager.instance.CreateTokenViews();

            //GameManager.instance.isMultiplayer = true;
            //GameManager.instance.isNewChallenge = true;
            //GameManager.instance.challengedUserId = id;
            //GameManager.instance.challengeInstanceId = null;
            //GameManager.instance.opponentNameLabel.text = userName;
            //GameManager.instance.opponentProfilePicture.sprite = profilePicture.sprite;

            //GameManager.instance.UpdatePlayerUI();
            //GameManager.instance.ResetUI();
            //GameManager.instance.DisplayIntroUI(tokenBoard.name, LocalizationManager.instance.GetLocalizedValue("leaderboard_challenge_text"), true);

            //UIScreen.SetActive(false);

            //if (OnActiveGame != null)
            //    OnActiveGame();

            ////GameManager.instance.EnableTokenAudio();

            //Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            //customAttributes.Add("TokenBoardId", tokenBoard.id);
            //customAttributes.Add("TokenBoardName", tokenBoard.name);
            //customAttributes.Add("Rank", rankLabel.text);
            //AnalyticsManager.LogCustom("open_new_leaderboard_challenge", customAttributes);
        }
    }
}
