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
    [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene)]
    public class UserManager : UnitySingleton<UserManager>
    {
        public string userName;
        public string userId;
        public int ratingElo;
        public long coins;
        public int gamePieceId;
        public Sprite profilePicture;

        public static event Action OnUpdateUserInfo;
        public static event Action<int> OnUpdateUserGamePieceID;

        protected void Start()
        {
            ChallengeManager.Instance.GetPlayerGamePiece();
        }

        protected void OnEnable()
        {
            ChallengeManager.OnReceivedPlayerGamePiece += SetPlayerGamePiece;
            ChallengeManager.OnSetGamePieceSuccess += SetPlayerGamePiece;
        }

        protected void OnDisable()
        {
            ChallengeManager.OnReceivedPlayerGamePiece -= SetPlayerGamePiece;
            ChallengeManager.OnSetGamePieceSuccess -= SetPlayerGamePiece;
        }

        public void UpdateInformation()
        {
            new AccountDetailsRequest().Send((response) =>
            {
                string facebookId = response.ExternalIds.GetString("FB");
                int? rateElo = response.ScriptData.GetInt("ratingElo");
                Mixpanel.Identify(response.UserId);
                Mixpanel.people.Set("$name", response.DisplayName);

                // "$email": "jsmith@example.com",    
                // "$created": "2011-03-16 16:53:54",
                // "$last_login": new Date(),         

                UpdateUserInfo(response.DisplayName, response.UserId, facebookId, response.Currency1, rateElo);
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
                        this.userName = name;
                        if (OnUpdateUserInfo != null)
                        {
                            OnUpdateUserInfo();
                        }
                    }
                });
        }

        private void SetPlayerGamePiece(string id)
        {
            gamePieceId = int.Parse(id);
            if (gamePieceId > GameContentManager.Instance.piecesDataHolder.gamePieces.Length - 1) 
            {
                gamePieceId = 0;
            }

            if (OnUpdateUserGamePieceID != null)
            {
                OnUpdateUserGamePieceID(gamePieceId);
            }
        }

        public void UpdateUserInfo(string name, string uid, string fbId, long? coins, int? rating)
        {
            this.userName = name;
            this.userId = uid;
            this.coins = coins.GetValueOrDefault(0);
            this.ratingElo = rating.GetValueOrDefault(0);

            if (fbId != null)
            {
                StartCoroutine(GetFBPicture(fbId, (sprite) =>
                {
                    if (sprite)
                    {
                        this.profilePicture = sprite;
                    }
                }));
            }

            if (OnUpdateUserInfo != null)
            {
                OnUpdateUserInfo();
            }
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