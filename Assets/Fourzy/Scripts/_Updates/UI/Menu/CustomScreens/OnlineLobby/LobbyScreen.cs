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

        protected List<PhotonRoomWidget> rooms = new List<PhotonRoomWidget>();

        private LoadingPromptScreen _prompt;
        private LobbyState state;

        protected override void Awake()
        {
            base.Awake();

            FourzyPhotonManager.onRoomsListUpdated += OnRoomsUpdated;
            FourzyPhotonManager.onJoinedRoom += OnJoinedRoom;
            FourzyPhotonManager.onJoinedLobby += OnJoinedLobby;

            FourzyPhotonManager.onJoinRoomFailed += OnJoinRoomFailed;
            FourzyPhotonManager.onConnectionTimeOut += OnConnectionTimerOut;
            FourzyPhotonManager.onConnectedToMaster += OnConnectedToMaster;
        }

        protected void OnDestroy()
        {
            FourzyPhotonManager.onRoomsListUpdated -= OnRoomsUpdated;
            FourzyPhotonManager.onJoinedRoom -= OnJoinedRoom;
            FourzyPhotonManager.onJoinedLobby -= OnJoinedLobby;

            FourzyPhotonManager.onJoinRoomFailed -= OnJoinRoomFailed;
            FourzyPhotonManager.onConnectionTimeOut -= OnConnectionTimerOut;
            FourzyPhotonManager.onConnectedToMaster -= OnConnectedToMaster;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _prompt = PersistantMenuController.Instance
                .GetOrAddScreen<LoadingPromptScreen>()
                .SetType(LoadingPromptScreen.LoadingPromptType.BASIC);
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

            if (HeaderScreen.Instance)
            {
                HeaderScreen.Instance.Close();
            }
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

        /// <summary>
        /// Invoke by Create Room button
        /// </summary>
        public void CreateRoom()
        {
            PersistantMenuController.Instance.GetOrAddScreen<CreateRealtimeGamePrompt>().Prompt();
        }

        public void JoinRoom(string name)
        {
            _prompt
                .Prompt("Retrieving player stats...", null, null, null, null, null)
                .CloseOnDecline();

            UserManager.GetPlayerRating(rating =>
            {
                FourzyPhotonManager.JoinRoom(name);

                if (_prompt.isOpened)
                {
                    _prompt.promptTitle.text = $"Joining room \n{name}";
                }
            }, () =>
            {
                if (_prompt.isOpened) _prompt.Decline(true);
            });
        }

        private void DisplayRooms(List<RoomInfo> data)
        {
            if (!isOpened) return;

            Clear();

            foreach (RoomInfo roomData in data)
            {
                if (!roomData.RemovedFromList && IsLobbyRoom(roomData))
                {
                    rooms.Add(Instantiate(widgetPrefab, widgetsParent).SetData(roomData));
                }
            }
        }

        private void Clear()
        {
            foreach (PhotonRoomWidget widget in rooms)
            {
                Destroy(widget.gameObject);
            }

            rooms.Clear();
        }

        private bool IsLobbyRoom(RoomInfo roomData)
        {
            return roomData.CustomProperties.ContainsKey(Constants.REALTIME_ROOM_TYPE_KEY) &&
                (RoomType)roomData.CustomProperties[Constants.REALTIME_ROOM_TYPE_KEY] == RoomType.LOBBY_ROOM;
        }

        private void OnRoomsUpdated(List<RoomInfo> _rooms)
        {
            foreach (RoomInfo roomData in _rooms)
            {
                if (IsLobbyRoom(roomData))
                {
                    //remove
                    if (roomData.RemovedFromList)
                    {
                        PhotonRoomWidget _roomWidget = rooms.Find(_widget => _widget.name == roomData.Name);

                        if (_roomWidget)
                        {
                            _roomWidget._Destroy();
                            rooms.Remove(_roomWidget);
                        }
                    }
                    //add
                    else
                    {
                        rooms.Add(Instantiate(widgetPrefab, widgetsParent).SetData(roomData));
                    }
                }
            }
            DisplayRooms(FourzyPhotonManager.Instance.roomsInfo);
        }

        private void OnJoinedRoom(string roomName)
        {
            if (isOpened)
            {
                CloseSelf();
            }

            if (_prompt.isOpened)
            {
                _prompt.Decline(true);
            }
        }

        private void OnJoinRoomFailed(string error)
        {
            if (isOpened)
            {
                GamesToastsController.ShowTopToast($"Failed: {error}");
            }

            if (_prompt.isOpened)
            {
                _prompt.Decline(true);
            }
        }

        private void OnConnectionTimerOut()
        {
            if (isOpened && state != LobbyState.NONE)
            {
                CloseSelf();
            }

            if (_prompt.isOpened)
            {
                _prompt.Decline(true);
            }
        }

        private void OnJoinedLobby(string lobbyName)
        {
            if (state != LobbyState.JOINING_LOBBY) return;

            if (_prompt.isOpened)
            {
                _prompt.Decline(true);
            }

            CheckLobby();
        }

        private void OnConnectedToMaster()
        {
            if (state != LobbyState.LEAVING_ROOM) return;

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