//@vadym udod

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class FriendWidget : WidgetBase
    {
        public TMP_Text nameLabel;
        public Image profilePicture;
        public Texture2D defaultProfilePicture;
        public Image onlineTexture;

        private Friend friend;

        public void SetData(Friend friend)
        {
            this.friend = friend;

            nameLabel.text = friend.userName;
            onlineTexture.color = friend.isOnline ? Color.green : Color.gray;

            if (friend.facebookId != null)
            {
                StartCoroutine(UserManager.Instance.GetFBPicture(friend.facebookId, (sprite) =>
                {
                    profilePicture.sprite = sprite;
                }));
            }
            else
            {
                profilePicture.sprite = Sprite.Create(defaultProfilePicture,
                    new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height),
                    new Vector2(0.5f, 0.5f));
            }
        }

        public void OpenNewFriendChallengeGame()
        {
            //ViewController.instance.ChangeView(ViewController.instance.viewGameboardSelection);
            //ViewController.instance.HideTabView();
            //ViewGameBoardSelection.instance.TransitionToViewGameBoardSelection(GameType.FRIEND, id, userName, profilePicture);
        }
    }
}
