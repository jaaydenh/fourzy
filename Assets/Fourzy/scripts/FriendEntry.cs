using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace ConnectFour
{
public class FriendEntry : MonoBehaviour 
{

	public string userName, id, facebookId;
	public bool isOnline;

	public Text nameLabel;
	public Image profilePicture;
	public Image onlineTexture;

	// Use this for initialization
	void Start()
	{
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

	//This is the function we call for our UIButton OnClick
	public void StartChallenge()
	{
		//CreateChallengeRequest takes a list of UserIds because you can challenge more than one user at a time
		List<string> gsId = new List<string>();
		//Add our friends UserId to the list
		gsId.Add(id);

		//Call the ChallengeUser function and pass on the list containing our friends UserId
		ChallengeManager.instance.ChallengeUser(gsId);
	}
}
}