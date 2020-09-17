//@vadym udod

using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Menu.Screens;
using Fourzy._Updates.UI.Toasts;
using PlayFab;
using PlayFab.ClientModels;
using System;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class FriendWidget : WidgetBase
    {
        public Action<FriendWidget> onFriendRemoved;

        public TMP_Text nameLabel;
        public RectTransform gamepieceParent;
        public StringEventTrigger statusIcon;
        //public ButtonExtended playButton;

        private FriendInfo friendInfo;
        private bool removing;

        public string friendID => friendInfo.FriendPlayFabId;

        public FriendWidget SetData(FriendInfo friendInfo)
        {
            this.friendInfo = friendInfo;

            nameLabel.text = friendInfo.TitleDisplayName;

            return this;
        }

        public FriendWidget SetOnFriendRemoved(Action<FriendWidget> action)
        {
            onFriendRemoved = action;

            return this;
        }

        public FriendWidget UpdateOnlineStatus(bool state)
        {
            statusIcon.TryInvoke(state ? "on" : "off");

            return this;
        }

        public FriendWidget UpdateState(string state)
        {
            return this;
        }

        public void ShowUnfollowPrompt()
        {
            if (removing) return;

            menuScreen.menuController
                .GetOrAddScreen<PromptScreen>()
                .Prompt($"Unfollow {friendInfo.TitleDisplayName}", "", () => Unfollow(), null)
                .CloseOnAccept();
        }

        public FriendWidget Unfollow()
        {
            if (removing) return this;

            PlayFabClientAPI.RemoveFriend(new RemoveFriendRequest() { FriendPlayFabId = friendInfo.FriendPlayFabId }, OnRemoveFriend, OnRemoveFriendError);
            removing = true;

            return this;
        }

        private void OnRemoveFriendError(PlayFabError obj)
        {
            removing = false;

            GamesToastsController.ShowTopToast($"Failed: {obj.ErrorMessage}");
        }

        private void OnRemoveFriend(RemoveFriendResult obj)
        {
            removing = false;

            onFriendRemoved?.Invoke(this);

            GamesToastsController.ShowTopToast($"Unfollowed: {friendInfo.TitleDisplayName}");
        }
    }
}
