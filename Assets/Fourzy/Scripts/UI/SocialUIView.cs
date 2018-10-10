using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Lean.Pool;

namespace Fourzy
{
    public class SocialUIView : UIView
    {
        [SerializeField] Button btnFBLogin;
        [SerializeField] GameObject friendEntryPrefab;
        [SerializeField] GameObject friendsList;
        [SerializeField] GameObject noFriendsText;
        [SerializeField] List<GameObject> friendViews = new List<GameObject>();
        [SerializeField] Image onlineIndicator;

        public override void Awake()
        {
            base.Awake();

            this.UpdateFriendViews();
        }

        private void Start()
        {
            keepHistory = true;

            Button btn = btnFBLogin.GetComponent<Button>();
            btn.onClick.AddListener(ConnectFBOnClick);
        }

        private void OnEnable()
        {
            LoginManager.OnFBLoginComplete += LoginManager_OnFBLoginComplete;
            LoginManager.OnGetFriends += LoginManager_OnGetFriends;
        }

        private void OnDisable()
        {
            LoginManager.OnFBLoginComplete -= LoginManager_OnFBLoginComplete;
            LoginManager.OnGetFriends -= LoginManager_OnGetFriends;
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public override void ShowAnimated(AnimationDirection sourceDirection)
        {
            ViewController.instance.SetActiveView(TotalView.viewSocial);
            base.ShowAnimated(sourceDirection);
        }

        public override void HideAnimated(AnimationDirection getAwayDirection)
        {
            base.HideAnimated(getAwayDirection);
        }

        public void PlayButton()
        {
            ViewController.instance.ChangeView(ViewController.instance.viewGameboardSelection);
            ViewController.instance.HideTabView();
        }

        public void ConnectFBOnClick()
        {
            btnFBLogin.interactable = false;

            LoginManager.Instance.FacebookLogin();
        }

        void LoginManager_OnFBLoginComplete(bool isSuccessful)
        {
            btnFBLogin.interactable = true;

            if (isSuccessful)
            {
                btnFBLogin.gameObject.SetActive(false);
            }
            else
            {
                btnFBLogin.gameObject.SetActive(true);
            }
        }

        void LoginManager_OnGetFriends()
        {
            this.UpdateFriendViews();
        }

        private void UpdateFriendViews()
        {
            List<Friend> friends = LoginManager.Instance.Friends;
            if (friends == null)
            {
                return;
            }

            for (int i = 0; i < friendViews.Count; i++)
            {
                LeanPool.Despawn(friendViews[i]);
            }

            bool isOnline = false;

            foreach (Friend friend in friends)
            {
                GameObject go = LeanPool.Spawn(friendEntryPrefab) as GameObject;

                go.transform.SetParent(friendsList.transform);

                FriendEntry friendEntry = go.GetComponent<FriendEntry>();
                friendEntry.Reset();

                friendEntry.userName = friend.userName;
                friendEntry.id = friend.id;
                friendEntry.isOnline = friend.isOnline;
                friendEntry.facebookId = friend.facebookId;
                friendEntry.transform.localScale = Vector3.one;

                if (friend.isOnline)
                {
                    Debug.Log("online online online");
                    isOnline = true;
                }

                friendEntry.UpdateFriend();
                friendViews.Add(go);
            }

            if (friends.Count > 0)
            {
                noFriendsText.SetActive(false);
            }
            else
            {
                noFriendsText.SetActive(true);
            }

            onlineIndicator.color = isOnline ? Color.green : new Color(11.0f / 255.0f, 49.0f / 255.0f, 82.0f / 255.0f);
        }
    }
}
