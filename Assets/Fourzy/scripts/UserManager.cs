using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using GameSparks.Api.Requests;
using System;

namespace Fourzy
{
    public class UserManager : MonoBehaviour
    {

        public static UserManager instance;

        public string userName;
        public string userId;
        public Sprite profilePicture;
        public Text userNameLabel;
        public Image profilePictureImage;

        void Start() {
            profilePicture = new Sprite();
        }

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

            if (fbId != null) {
                StartCoroutine(UserManager.instance.GetFBPicture(fbId, (sprite) =>
                    {
                        if (sprite) {
                            profilePicture = sprite;
                            //profilePictureImage.sprite = sprite;
                        }
                    }));
            }
        }

        public IEnumerator GetFBPicture(string facebookId, Action<Sprite> callback)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://graph.facebook.com/" + facebookId + "/picture?width=210&height=210"))
            {
                yield return www.Send();

                if (www.isNetworkError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Texture2D tempPic = new Texture2D(25, 25);
                    tempPic = DownloadHandlerTexture.GetContent(www);
                    Sprite profilePictureSprite = Sprite.Create(tempPic, new Rect(0,0,tempPic.width, tempPic.height), new Vector2(0.5f, 0.5f));

                    callback(profilePictureSprite);
                }
            }
        }
    }
}