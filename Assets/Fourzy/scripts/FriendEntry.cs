﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Analytics.Experimental;
//using Fabric.Answers;

namespace Fourzy
{
    public class FriendEntry : MonoBehaviour 
    {
        public delegate void GameActive();
        public static event GameActive OnActiveGame;

        public string userName, id, facebookId;
        public bool isOnline;

        public Text nameLabel;
        public Image profilePicture;
        public Image onlineTexture;

        private GameObject UIScreen;

        void Start()
        {
            UIScreen = GameObject.Find("UI Screen");
            UpdateFriend();
        }

        public void UpdateFriend()
        {
            nameLabel.text = userName;
            onlineTexture.color = isOnline ? Color.green : Color.gray;
            if (facebookId != null) {
                StartCoroutine(UserManager.instance.GetFBPicture(facebookId, (sprite)=>
                    {
                        profilePicture.sprite = sprite;
                    }));
            }
        }

        //Open game gets called OnClick of the play button
        public void OpenNewFriendChallengeGame()
        {
            GameManager.instance.ResetGamePiecesAndTokens();
            GameManager.instance.PopulateMoveArrows();

            TokenBoard tokenBoard = TokenBoardLoader.instance.GetTokenBoard();
            //GameManager.instance.gameState.tokenBoard = tokenBoard;

            //If we initiated the challenge, we get to be player 1
            GameState gameState = new GameState(Constants.numRows, Constants.numColumns, true, true, tokenBoard, null, false);
            GameManager.instance.gameState = gameState;

            StartCoroutine(GameManager.instance.CreateTokenViews());

            GameManager.instance.isMultiplayer = true;
            GameManager.instance.isNewChallenge = true;
            GameManager.instance.challengedUserId = id;
            GameManager.instance.challengeInstanceId = null;
            GameManager.instance.opponentNameLabel.text = userName;
            GameManager.instance.opponentProfilePicture.sprite = profilePicture.sprite;

            GameManager.instance.UpdatePlayersStatusView();

            UIScreen.SetActive(false);

            if (OnActiveGame != null)
                OnActiveGame();

            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            customAttributes.Add("TokenBoardId", tokenBoard.id);
            customAttributes.Add("TokenBoardName", tokenBoard.name);
            AnalyticsEvent.Custom("OpenNewFriendChallenge", customAttributes);
            //Answers.LogCustom("OpenNewFriendChallenge", customAttributes);
        }
    }
}