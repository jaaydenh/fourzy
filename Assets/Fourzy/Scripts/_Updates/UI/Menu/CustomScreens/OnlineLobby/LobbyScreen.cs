//@vadym udod

using Fourzy._Updates.Audio;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Toasts;
using Fourzy._Updates.UI.Widgets;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class LobbyScreen : MenuScreen
    {
        public PhotonRoomWidget widgetPrefab;
        public RectTransform widgetsParent;
        public ButtonExtended createGameButton;
        public Image checkmark;
        public Sprite checkmarkOn;
        public Sprite checkmarkOff;

        protected List<PhotonRoomWidget> rooms = new List<PhotonRoomWidget>();
        protected List<RoomInfo> roomsData = new List<RoomInfo>();

        private bool passwordEnabled;
        private LoadingPromptScreen _prompt;
        private LobbyState state;

        protected override void Awake()
        {
            base.Awake();

            FourzyPhotonManager.onRoomsListUpdated += OnRoomsUpdated;
            FourzyPhotonManager.onJoinedRoom += OnJoinedRoom;
            FourzyPhotonManager.onJoinedLobby += OnJoinedLobby;

            FourzyPhotonManager.onCreateRoomFailed += OnCreateRoomFailed;
            FourzyPhotonManager.onJoinRoomFailed += OnJoinRoomFailed;
            FourzyPhotonManager.onConnectionTimeOut += OnConnectionTimerOut;
            FourzyPhotonManager.onConnectedToMaster += OnConnectedToMaster;
        }

        protected void OnDestroy()
        {
            FourzyPhotonManager.onRoomsListUpdated -= OnRoomsUpdated;
            FourzyPhotonManager.onJoinedRoom -= OnJoinedRoom;
            FourzyPhotonManager.onJoinedLobby -= OnJoinedLobby;

            FourzyPhotonManager.onCreateRoomFailed -= OnCreateRoomFailed;
            FourzyPhotonManager.onJoinRoomFailed -= OnJoinRoomFailed;
            FourzyPhotonManager.onConnectionTimeOut -= OnConnectionTimerOut;
            FourzyPhotonManager.onConnectedToMaster -= OnConnectedToMaster;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _prompt = PersistantMenuController.instance
                .GetOrAddScreen<LoadingPromptScreen>()
                .SetType(LoadingPromptScreen.LoadingPromptType.BASIC);

            UpdateCheckmarkButton();
        }

        public override void OnBack()
        {
            base.OnBack();

            CloseSelf();
        }

        public override void Close(bool animate = true)
        {
            base.Close(animate);

            state = LobbyState.NONE;
        }

        public override void Open()
        {
            base.Open();

            OnRoomsUpdated(FourzyPhotonManager.Instance.roomsInfo);
        }

        public void ToggleCheckmark()
        {
            passwordEnabled = !passwordEnabled;
            UpdateCheckmarkButton();
        }

        public bool CheckLobby()
        {
            if (PhotonNetwork.CurrentRoom != null)
            {
                menuController
                    .GetOrAddScreen<PromptScreen>()
                    .Prompt("Leave room?", "To enter the lobby you have to leave the room you'r currently in.", () =>
                    {
                        state = LobbyState.LEAVING_ROOM;
                        FourzyPhotonManager.TryLeaveRoom();

                        _prompt
                            .Prompt("Leaving room...", "", null, "Back", null, () => state = LobbyState.NONE)
                            .CloseOnDecline();
                    }, null)
                    .CloseOnDecline()
                    .CloseOnAccept();

                return false;
            }
            else if (FourzyPhotonManager.ConnectedAndReady && FourzyPhotonManager.InDefaultLobby)
            {
                if (!isCurrent) menuController.OpenScreen(this);

                return true;
            }
            else
            {
                state = LobbyState.JOINING_LOBBY;

                _prompt
                    .Prompt("Joining lobby...", "", null, "Back", null, () => state = LobbyState.NONE)
                    .CloseOnDecline();

                FourzyPhotonManager.Instance.JoinLobby();

                return false;
            }
        }

        public void CreateRoom()
        {
            string password = "";

            if (passwordEnabled)
                password = Guid.NewGuid().ToString().Substring(0, Constants.REALTIME_ROOM_PASSWORD_LENGTH);

            FourzyPhotonManager.CreateRoom(RoomType.LOBBY_ROOM, password: password) ;

            _prompt
                .Prompt("Creating new room", "", null, null, null, null)
                .CloseOnDecline();
        }

        public void JoinRoom(string name)
        {
            FourzyPhotonManager.JoinRoom(name);

            _prompt
                .Prompt("Joining room", name, null, null, null, null)
                .CloseOnDecline();
        }

        private void UpdateCheckmarkButton()
        {
            checkmark.sprite = passwordEnabled ? checkmarkOn : checkmarkOff;
        }

        private void DisplayRooms(List<RoomInfo> data)
        {
            if (!isOpened) return;

            Clear();

            foreach (RoomInfo roomData in data)
                if (!roomData.RemovedFromList && 
                    roomData.CustomProperties.ContainsKey(Constants.REALTIME_ROOM_TYPE_KEY) && 
                    (RoomType)roomData.CustomProperties[Constants.REALTIME_ROOM_TYPE_KEY] == RoomType.LOBBY_ROOM)
                    rooms.Add(Instantiate(widgetPrefab, widgetsParent).SetData(roomData));
        }

        private void Clear()
        {
            foreach (PhotonRoomWidget widget in rooms) Destroy(widget.gameObject);
            rooms.Clear();
        }

        private void OnRoomsUpdated(List<RoomInfo> _rooms) => DisplayRooms(roomsData = _rooms);

        private void OnJoinedRoom(string roomName)
        {
            if (!isOpened) return;

            if (_prompt.isOpened) _prompt.Decline(true);
            if (isOpened) CloseSelf();

            //get opponent
            GameManager.Instance.opponentID = PhotonNetwork.PlayerListOthers[0].UserId;

            AudioHolder.instance.PlaySelfSfxOneShotTracked(Serialized.AudioTypes.GAME_FOUND);
            GameManager.Vibrate(MoreMountains.NiceVibrations.HapticTypes.Success);
        }

        private void OnCreateRoomFailed(string error)
        {
            if (isOpened) GamesToastsController.ShowTopToast($"Failed: {error}");
            if (_prompt.isOpened) _prompt.Decline(true);
        }

        private void OnJoinRoomFailed(string error)
        {
            if (isOpened) GamesToastsController.ShowTopToast($"Failed: {error}");
            if (_prompt.isOpened) _prompt.Decline(true);
        }

        private void OnConnectionTimerOut()
        {
            if (isOpened && state != LobbyState.NONE) CloseSelf();
            if (_prompt.isOpened) _prompt.Decline(true);
        }

        private void OnJoinedLobby(string lobbyName)
        {
            if (state != LobbyState.JOINING_LOBBY) return;

            if (_prompt.isOpened) _prompt.Decline(true);

            CheckLobby();
        }

        private void OnConnectedToMaster()
        {
            if (state != LobbyState.LEAVING_ROOM) return;

            //if (_prompt.isOpened) _prompt.Decline(true);

            CheckLobby();
        }

        private enum LobbyState
        {
            NONE,
            JOINING_LOBBY,
            LEAVING_ROOM,
        }
    }
}