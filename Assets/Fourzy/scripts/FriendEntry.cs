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

    	// Use this for initialization
    	void Start()
    	{
            UIScreen = GameObject.Find("UI Screen");
            //When the object is instantiated, update the GUI variables
    		UpdateFriend();
    	}

    	public void UpdateFriend()
    	{
    		nameLabel.text = userName;
    		onlineTexture.color = isOnline ? Color.green : Color.gray;
    		//onlineTexture.texture.SetPixels(new Color[] {isOnline ? Color.green : Color.gray});
    		StartCoroutine(getFBPicture());
    	}

    	public IEnumerator getFBPicture()
    	{
    		//To get our facebook picture we use this address which we pass our facebookId into
    		var www = new WWW("http://graph.facebook.com/" + facebookId + "/picture?width=210&height=210");

    		yield return www;

    		Texture2D tempPic = new Texture2D(25, 25);

    		www.LoadImageIntoTexture(tempPic);
    		Sprite tempSprite = Sprite.Create(tempPic, new Rect(0,0,tempPic.width, tempPic.height), new Vector2(0.5f, 0.5f));
    		profilePicture.sprite = tempSprite;
    	}
            
    	public void StartChallenge()
    	{
    		ChallengeManager.instance.ChallengeUserOld(id);
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

            GameManager.instance.SetMultiplayerGameStatusText();

            UIScreen.SetActive(false);

            if (OnActiveGame != null)
                OnActiveGame(true);
        }
    }
}