//@vadym udod

using Fourzy._Updates.UI.Toasts;
using Fourzy._Updates.UI.Widgets;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class FriendsScreen : PromptScreen
    {
        public FriendWidget widgetPrefab;
        public RectTransform widgetsParent;

        private List<FriendWidget> friends = new List<FriendWidget>();
        private List<FriendInfo> friendsData = new List<FriendInfo>();

        private string lastAddFriendName;
        private int listUpdates;
        private bool keepUpdating;
        private bool isUpdating;

        public string[] friendsIDs => friendsData.Select(_friend => _friend.FriendPlayFabId).ToArray();

        public override void Open()
        {
            base.Open();

            //start playfab pull friends routine
            keepUpdating = true;
            StartRoutine("listUpdates", UpdateFriendsListRoutine(), null, null);

            Clear();
        }

        public override void Close(bool animate = true)
        {
            base.Close(animate);

            keepUpdating = false;
            isUpdating = false;
            StopRoutine("listUpdates", false);
        }

        public void AddFriend() => menuController.GetOrAddScreen<InputFieldPrompt>()._Prompt(OnAddFriend, "Add friend", "Input friends' name to add them", "Add", "Back");

        public void UpdateFriendsList()
        {
            if (isUpdating) return;

            isUpdating = true;

            PlayFabClientAPI.GetFriendsList(
                new GetFriendsListRequest() { ProfileConstraints = new PlayerProfileViewConstraints() { ShowDisplayName = true, ShowAvatarUrl = true} },
                OnGetFriendsListResult,
                OnGetFriendsError);
        }

        public void Clear()
        {
            foreach (FriendWidget widget in friends) Destroy(widget.gameObject);
            friends.Clear();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            FourzyPhotonManager.onFriendsUpdated += OnPhotonFriendsUpdated;
        }

        private void OnGetFriendsError(PlayFabError obj)
        {
            isUpdating = false;

            if (!isOpened) return;

            GamesToastsController.ShowTopToast($"Failed: {obj.ErrorMessage}");
        }

        private void OnGetFriendsListResult(GetFriendsListResult obj)
        {
            if (!isOpened) return;

            friendsData = obj.Friends;
            Debug.Log($"Playfab friends {obj.Friends.Count}");

            if (obj.Friends.Count > 0)
                //get photon friends list
                PhotonNetwork.FindFriends(friendsIDs);
            else
                isUpdating = false;
        }

        private void OnPhotonFriendsUpdated(List<Photon.Realtime.FriendInfo> _friends)
        {
            isUpdating = false;

            if (!isOpened) return;

            Clear();

            for (int friendIndex = 0; friendIndex < _friends.Count; friendIndex++)
                if (friendIndex < friendsData.Count)
                    friends.Add(Instantiate(widgetPrefab, widgetsParent)
                        .SetData(friendsData[friendIndex])
                        .SetOnFriendRemoved(OnFriendRemoved)
                        .SetPlayerIcon(string.IsNullOrEmpty(friendsData[friendIndex].Profile.AvatarUrl) ? Constants.DEFAULT_GAME_PIECE : friendsData[friendIndex].Profile.AvatarUrl)
                        .UpdateOnlineStatus(_friends[friendIndex].IsOnline));
        }

        private void OnFriendRemoved(FriendWidget widget)
        {
            if (widget)
            {
                Destroy(widget.gameObject);
                friends.Remove(widget);
            }
        }

        private void OnAddFriend(string displayName)
        {
            lastAddFriendName = displayName;
            PlayFabClientAPI.AddFriend(new PlayFab.ClientModels.AddFriendRequest() { FriendTitleDisplayName = displayName, }, OnAddFriendResult, OnAddFriendError);
        }

        private void OnAddFriendResult(AddFriendResult obj)
        {
            string message = null;

            if (obj.Created)
                message = $"Folliwing {lastAddFriendName}!";
            else
                message = $"Failed to follow {lastAddFriendName}";

            GamesToastsController.ShowTopToast(message);

            //lazy fix, just request new friends list
            UpdateFriendsList();
        }

        private void OnAddFriendError(PlayFabError obj)
        {
            GamesToastsController.ShowTopToast($"Failed: {obj.ErrorMessage}");
        }

        private float GetWaitTime(int iteration) => Mathf.Clamp(iteration, 0f, 10f) + .5f;

        private IEnumerator UpdateFriendsListRoutine()
        {
            listUpdates = 0;

            while (keepUpdating)
            {
                yield return new WaitForSeconds(GetWaitTime(listUpdates));
                listUpdates++;

                //update list
                UpdateFriendsList();
            }
        }
    }
}