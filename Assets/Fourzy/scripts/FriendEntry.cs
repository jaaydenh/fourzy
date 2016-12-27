using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Fourzy
{
    public class FriendEntry : MonoBehaviour 
    {
        public delegate void GameActive(bool active);
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

            StartCoroutine(UserManager.instance.GetFBPicture(facebookId, (sprite)=>
                {
                    profilePicture.sprite = sprite;
                }));
    	}

        //Open game gets called OnClick of the play button
        public void OpenGame()
        {
            GameManager.instance.ResetGameBoard();

            GameManager.instance.isMultiplayer = true;
            //If we initiated the challenge, we get to be player 1
            GameManager.instance.isPlayerOneTurn = true;
            GameManager.instance.isCurrentPlayerTurn = true;
            GameManager.instance.isNewChallenge = true;
            GameManager.instance.challengedUserId = id;
            GameManager.instance.challengeInstanceId = null;

            GameManager.instance.UpdateGameStatusText();

            UIScreen.SetActive(false);

            if (OnActiveGame != null)
                OnActiveGame(true);
        }
    }
}