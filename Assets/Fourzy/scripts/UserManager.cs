using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GameSparks.Api.Requests;

namespace Fourzy
{
    public class UserManager : MonoBehaviour
    {

    	public static UserManager instance;

    	public string userName;
    	public string userId;
    	private string facebookId;

    	public Text userNameLabel;
    	public Image profilePicture;

    	void Awake()
    	{
    		instance = this;
    	}

    	public void UpdateInformation()
    	{
    		new AccountDetailsRequest().Send((response) =>
    			{
    				UpdateGUI(response.DisplayName, response.UserId, response.ExternalIds.GetString("FB").ToString());
    			});
    	}

    	public void UpdateGUI(string name, string uid, string fbId)
    	{
    		userName = name;
    		userNameLabel.text = userName;
    		userId = uid;
    		facebookId = fbId;
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
    }
}