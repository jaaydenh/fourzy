using UnityEngine;
using UnityEngine.UI;

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
        public Texture2D defaultProfilePicture;
        public Image onlineTexture;

        public void Reset()
        {
            nameLabel.text = "";
            profilePicture.sprite = null;
            isOnline = false;
            userName = "";
            id = "";
            facebookId = "";
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
            } else {
                profilePicture.sprite = Sprite.Create(defaultProfilePicture, 
                    new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height), 
                    new Vector2(0.5f, 0.5f));
            }
        }

        //Open game gets called OnClick of the play button
        public void OpenNewFriendChallengeGame()
        {
            GameManager.instance.TransitionToGameOptionsScreen(GameType.FRIEND, id, userName, profilePicture);
            ViewController.instance.ChangeView(ViewController.instance.viewGameboardSelection);
            ViewController.instance.HideTabView();
        }
    }
}