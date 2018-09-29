using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using GameSparks.Api.Requests;
using System;
using Facebook.Unity;
using mixpanel;

namespace Fourzy
{
    public class UserManager : Singleton<UserManager>
    {
        public string userName;
        public string userId;
        public int? ratingElo;
        public long coins;
        public int gamePieceId;

        public Sprite profilePicture;
        public Text gamePieceNameLabel;
        public Text ratingEloLabel;
        public Image profilePictureImage;

        [SerializeField]
        Text userNameLabel;
        [SerializeField]
        Image gamePieceImage;
        [SerializeField]
        Text coinsLabel;

        bool didLoadGamePieces;

        public delegate void UpdateName();
        public static event UpdateName OnUpdateName;

        new void Awake()
        {
            base.Awake();

			//profilePicture = new Sprite();
        }

        void Start()
        {
            ChallengeManager.instance.GetPlayerGamePiece();

            TokenBoardLoader.instance.LoadData();
            GameContentManager.Instance.UpdateContentData();
        }

        void OnEnable()
        {
            ChallengeManager.OnReceivedPlayerGamePiece += SetPlayerGamePiece;
            ChallengeManager.OnSetGamePieceSuccess += SetPlayerGamePiece;
        }

        void OnDisable()
        {
            ChallengeManager.OnReceivedPlayerGamePiece -= SetPlayerGamePiece;
            ChallengeManager.OnSetGamePieceSuccess -= SetPlayerGamePiece;
        }

        public void UpdateInformation()
        {
            
            new AccountDetailsRequest().Send((response) =>
                {
                    Debug.Log("COINS: " + response.Currency1);
                    string facebookId = response.ExternalIds.GetString("FB");
                    ratingElo = response.ScriptData.GetInt("ratingElo");
                    Mixpanel.Identify(response.UserId);
                    Mixpanel.people.Set("$name", response.DisplayName);

    // "$email": "jsmith@example.com",    
    // "$created": "2011-03-16 16:53:54",
    // "$last_login": new Date(),         

                    UpdateGUI(response.DisplayName, response.UserId, facebookId, response.Currency1, ratingElo);
                });
        }

        public void UpdatePlayerDisplayName(string name) 
        {
            new ChangeUserDetailsRequest()
                .SetDisplayName(name)
                .Send((response) => {
                    if (response.HasErrors)
                    {
                        Debug.Log("Error updating player display name: " + response.Errors.ToString());
                    }
                    else
                    {
                        Debug.Log("Successfully updated player display name");
                        this.ChangeName(name);    
                    }
                });
        }

        private void SetPlayerGamePiece(string id)
        {
            //Debug.Log("SetPlayerGamePiece: gamepieceid: " + id);
            gamePieceId = int.Parse(id);
            if (gamePieceId > GameContentManager.Instance.GetGamePieceCount() - 1) {
                gamePieceId = 0;
            }
            gamePieceImage.sprite = GameContentManager.Instance.GetGamePieceSprite(gamePieceId);
            gamePieceImage.gameObject.SetActive(true);

            if (!didLoadGamePieces)
            {
                didLoadGamePieces = true;
            }
            gamePieceNameLabel.text = GameContentManager.Instance.GetGamePieceName(gamePieceId);
        }

        public void UpdateGUI(string name, string uid, string fbId, long? coins, int? rating)
        {
            this.ChangeName(name);
            userId = uid;
            if (rating != null)
            {
                ratingEloLabel.text = rating.ToString();
            }
            else
            {
                ratingEloLabel.text = "0";
            }

            this.coins = coins.GetValueOrDefault(0);
            coinsLabel.text = this.coins.ToString();

            if (fbId != null) {

                // GetFBPicture(fbId, UpdateProfileImage);

                StartCoroutine(UserManager.instance.GetFBPicture(fbId, (sprite) =>
                    {
                        if (sprite) {
                            profilePicture = sprite;
                        }
                    }));
            }


        }

        private void ChangeName(string name)
        {
            userName = name;
            userNameLabel.text = userName;
            if (OnUpdateName != null)
                OnUpdateName();
        }

        // private void UpdateProfileImage(IGraphResult result) {
        //     if(result.Texture != null) {
        //         profilePicture = Sprite.Create(result.Texture, new Rect(0,0,result.Texture.width, result.Texture.height), new Vector2(0.5f, 0.5f));
        //     }
        // }

        // public IEnumerator GetFBPicture(string facebookId, FacebookDelegate<IGraphResult> callback = null) {
        //     FB.API("/" + facebookId + "/picture?type=square&height=210&width=210", HttpMethod.GET, callback);
        // }

        public IEnumerator GetFBPicture(string facebookId, Action<Sprite> callback)
        {
            // FB.API("/" + facebookId + "/picture?type=square&height=210&width=210", HttpMethod.GET, UpdateProfileImage);

            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture("https://graph.facebook.com/" + facebookId + "/picture?width=210&height=210"))
            {
                yield return uwr.SendWebRequest();

                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.Log("get_fb_picture_error: " + uwr.error);
                    AnalyticsManager.LogError("get_fb_picture_error", uwr.error);
                }
                else
                {
                    //Debug.Log("get_fb_picture_success: ");
                    Texture2D tempPic = new Texture2D(25, 25);
                    tempPic = DownloadHandlerTexture.GetContent(uwr);
                    Sprite profilePictureSprite = Sprite.Create(tempPic, new Rect(0,0,tempPic.width, tempPic.height), new Vector2(0.5f, 0.5f));

                    callback(profilePictureSprite);
                }
            }
        }
    }
}