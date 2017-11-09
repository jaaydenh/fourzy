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
        public long coins;
        public Sprite profilePicture;
        public Text userNameLabel;
        public Image profilePictureImage;
        public Text coinsLabel;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);
            }

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);

            profilePicture = new Sprite();
        }

        public void UpdateInformation()
        {
            new AccountDetailsRequest().Send((response) =>
                {    
                UpdateGUI(response.DisplayName, response.UserId, response.ExternalIds.GetString("FB").ToString(), response.Currency1);
                });
        }

        public void UpdateGUI(string name, string uid, string fbId, long? coins)
        {
            userName = name;
            userNameLabel.text = userName;
            userId = uid;
            this.coins = coins.GetValueOrDefault(0);
            coinsLabel.text = this.coins.ToString();

            if (fbId != null) {
                StartCoroutine(UserManager.instance.GetFBPicture(fbId, (sprite) =>
                    {
                        if (sprite) {
                            profilePicture = sprite;
                        }
                    }));
            }
        }

        public IEnumerator GetFBPicture(string facebookId, Action<Sprite> callback)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://graph.facebook.com/" + facebookId + "/picture?width=210&height=210"))
            {
                yield return www.Send();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log("get_fb_picture_error: " + www.error);
                    AnalyticsManager.LogError("get_fb_picture_error", www.error);
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