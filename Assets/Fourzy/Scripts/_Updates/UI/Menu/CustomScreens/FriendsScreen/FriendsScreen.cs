//@vadym udod

using Fourzy._Updates.UI.Toasts;
using Fourzy._Updates.UI.Widgets;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class FriendsScreen : PromptScreen
    {
        public FriendWidget widgetPrefab;
        public RectTransform widgetsParent;

        private List<FriendWidget> friends = new List<FriendWidget>();

        private string lastAddFriendName;
        private int listUpdates;
        private bool keepUpdating;
        private bool isUpdating;

        public override void Open()
        {
            base.Open();

            //start playfab pull friends routine
            keepUpdating = true;
            StartRoutine("listUpdates", UpdateFriendsListRoutine(), null, null);
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
                new GetFriendsListRequest() /*{ ProfileConstraints = new PlayerProfileViewConstraints() { ShowDisplayName = true, } }*/,
                OnGetFriendsListResult, 
                OnGetFriendsError);
        }

        private void OnGetFriendsError(PlayFabError obj)
        {
            isUpdating = false;

            if (!isOpened) return;

            GamesToastsController.ShowTopToast($"Failed: {obj.ErrorMessage}");
        }

        private void OnGetFriendsListResult(GetFriendsListResult obj)
        {
            isUpdating = false;

            if (!isOpened) return;

            Debug.Log("Updating friends");
            //update list entries
            foreach (FriendWidget widget in friends) Destroy(widget.gameObject);
            friends.Clear();

            foreach (FriendInfo friend in obj.Friends)
                friends.Add(Instantiate(widgetPrefab, widgetsParent)
                    .SetData(friend)
                    .SetOnFriendRemoved(OnFriendRemoved));
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