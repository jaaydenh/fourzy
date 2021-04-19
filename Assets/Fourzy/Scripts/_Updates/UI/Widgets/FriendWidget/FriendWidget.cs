//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
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
        public ButtonExtended playButton;

        private GamePieceView gamepiece;
        private FriendInfo friendInfo;
        private bool removing;

        public string friendID => friendInfo.FriendPlayFabId;

        public FriendWidget SetData(FriendInfo friendInfo)
        {
            this.friendInfo = friendInfo;

            nameLabel.text = friendInfo.TitleDisplayName;

            return this;
        }

        public FriendWidget SetPlayerIcon(string gamepieceID)
        {
            if (gamepiece) Destroy(gamepiece.gameObject);

            gamepiece = Instantiate(GameContentManager.Instance.piecesDataHolder.GetGamePieceData(gamepieceID).player1Prefab, gamepieceParent);

            gamepiece.transform.localPosition = Vector3.zero;
            gamepiece.StartBlinking();

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

            playButton.SetActive(state);

            return this;
        }

        public void Challenge()
        {
            menuScreen.menuController
                .GetOrAddScreen<PromptScreen>()
                .Prompt(
                    "Challenge Player?",
                    $"Challenge {friendInfo.TitleDisplayName} to play agains you?",
                    () => GameManager.Instance.ChallengePlayerRealtime(friendInfo.TitleDisplayName))
                .CloseOnAccept();
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

            PlayFabClientAPI.RemoveFriend(new RemoveFriendRequest()
            {
                FriendPlayFabId = friendInfo.FriendPlayFabId
            }, 
            OnRemoveFriend, 
            OnRemoveFriendError);
            removing = true;

            return this;
        }

        private void OnRemoveFriendError(PlayFabError error)
        {
            removing = false;

            GamesToastsController.ShowTopToast($"Failed: {error.ErrorMessage}");
            GameManager.Instance.ReportPlayFabError(error.ErrorMessage);
        }

        private void OnRemoveFriend(RemoveFriendResult obj)
        {
            removing = false;

            onFriendRemoved?.Invoke(this);

            GamesToastsController.ShowTopToast($"Unfollowed: {friendInfo.TitleDisplayName}");
        }
    }
}
