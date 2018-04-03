using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using GameSparks.Api.Requests;
using System;

namespace Fourzy
{
    public class UserManager : Singleton<UserManager>
    {
        public string userName;
        public string userId;
        public long coins;
        public int gamePieceId;

        public Sprite profilePicture;
        [SerializeField]
        Text userNameLabel;
        public Text gamePieceNameLabel;
        public Image profilePictureImage;
        [SerializeField]
        Image gamePieceImage;
        [SerializeField]
        Text coinsLabel;

        bool didLoadGamePieces;

        new void Awake()
        {
            base.Awake();

            profilePicture = new Sprite();
        }

        void Start()
        {
            ChallengeManager.instance.GetPlayerGamePiece();
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
                    UpdateGUI(response.DisplayName, response.UserId, facebookId, response.Currency1);
                });
        }

        private void SetPlayerGamePiece(string id)
        {
            //Debug.Log("SetPlayerGamePiece: gamepieceid: " + id);
            gamePieceId = int.Parse(id);
            if (gamePieceId > GamePieceSelectionManager.instance.gamePieces.Count - 1) {
                gamePieceId = 0;
            }
            gamePieceImage.sprite = GamePieceSelectionManager.instance.gamePieces[gamePieceId];
            gamePieceImage.gameObject.SetActive(true);

            //Debug.Log("didLoadGamePieces: "+ didLoadGamePieces);
            if (!didLoadGamePieces)
            {
                GamePieceSelectionManager.instance.LoadGamePieces(id);
                didLoadGamePieces = true;
            }
            gamePieceNameLabel.text = GamePieceSelectionManager.instance.GetGamePieceName(gamePieceId);
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