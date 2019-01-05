//@vadym udod

using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using mixpanel;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class SocialScreen : MenuScreen
    {
        public ButtonExtended fbLoginButton;
        public TMP_Text noFrinedOnlineLabel;
        public RectTransform friendsParent;
        public FriendWidget friendWidgetPrefab;

        private List<Friend> friends;

        public override void Open()
        {
            base.Open();

            fbLoginButton.SetActive(!LoginManager.Instance.IsFBLoggedIn);

            LoginManager.OnFBLoginComplete += LoginManager_OnFBLoginComplete;
            LoginManager.OnGetFriends += LoginManager_OnGetFriends;
        }

        public override void Close()
        {
            base.Close();

            LoginManager.OnFBLoginComplete -= LoginManager_OnFBLoginComplete;
            LoginManager.OnGetFriends -= LoginManager_OnGetFriends;
        }

        public void ConnectFB()
        {
            Mixpanel.Track("Facebook Connect Button Press");
            fbLoginButton.interactable = false;

            LoginManager.Instance.FacebookLogin();
        }

        private void LoginManager_OnFBLoginComplete(bool success)
        {
            fbLoginButton.interactable = true;
            fbLoginButton.SetActive(!success);
        }

        private void LoginManager_OnGetFriends()
        {
            UpdateFriendList();
        }

        private void UpdateFriendList()
        {
            //remove old ones
            foreach (Transform gamePiece in friendsParent)
                Destroy(gamePiece.gameObject);

            friends = LoginManager.Instance.Friends;

            if (friends == null)
                return;

            noFrinedOnlineLabel.gameObject.SetActive(friends.Count > 0);

            foreach (Friend friend in friends)
            {
                FriendWidget friendWidget = Instantiate(friendWidgetPrefab, friendsParent);
                friendWidget.transform.localScale = Vector3.one;

                friendWidget.SetData(friend);
            }
        }
    }
}
