using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
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
        public void OpenGame()
        {
            GameManager.instance.ResetGameBoard();
            GameManager.instance.PopulateMoveArrows();

            TokenBoard tokenBoard = TokenBoardLoader.instance.GetTokenBoard();
            GameManager.instance.tokenBoard = tokenBoard;
            StartCoroutine(GameManager.instance.CreateTokens());
            //int[] tokenData = TokenBoardLoader.Instance.FindTokenBoardNoSticky();
            //StartCoroutine(GameManager.instance.SetTokenBoard(tokenData));

            GameManager.instance.isMultiplayer = true;
            //If we initiated the challenge, we get to be player 1
            GameManager.instance.isPlayerOneTurn = true;
            GameManager.instance.isCurrentPlayerTurn = true;
            GameManager.instance.isNewChallenge = true;
            GameManager.instance.challengedUserId = id;
            GameManager.instance.challengeInstanceId = null;
            GameManager.instance.opponentNameLabel.text = userName;
            GameManager.instance.opponentProfilePicture.sprite = profilePicture.sprite;

            GameManager.instance.UpdateGameStatusText();

            UIScreen.SetActive(false);

            if (OnActiveGame != null)
                OnActiveGame();

            Dictionary<String, object> customAttributes = new Dictionary<String, object>();
            customAttributes.Add("TokenBoardId", tokenBoard.id);
            customAttributes.Add("TokenBoardName", tokenBoard.name);
            //Answers.LogCustom("OpenNewFriendChallenge", customAttributes);
        }
    }
}