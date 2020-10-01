//@vadym udod

using Fourzy._Updates.Audio;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Toasts;
using Fourzy._Updates.UI.Widgets;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class LobbyPromptScreen : PromptScreen
    {
        public PhotonRoomWidget widgetPrefab;
        public RectTransform widgetsParent;
        public ButtonExtended createGameButton;

        protected List<PhotonRoomWidget> rooms = new List<PhotonRoomWidget>();
        protected List<RoomInfo> roomsData = new List<RoomInfo>();

        private LoadingPromptScreen _prompt;
        private bool listenTo = false;

        protected override void Awake()
        {
            base.Awake();

            _prompt = PersistantMenuController.instance
                .GetOrAddScreen<LoadingPromptScreen>()
                .SetType(LoadingPromptScreen.LoadingPromptType.BASIC);

            FourzyPhotonManager.onRoomsListUpdated += OnRoomsUpdated;
            FourzyPhotonManager.onJoinedRoom += OnJoinedRoom;
            FourzyPhotonManager.onJoinedLobby += OnJoinedLobby;

            FourzyPhotonManager.onCreateRoomFailed += OnCreateRoomFailed;
            FourzyPhotonManager.onJoinRoomFailed += OnJoinRoomFailed;
            FourzyPhotonManager.onConnectionTimeOut += OnConnectionTimerOut;
        }

        protected void OnDestroy()
        {
            FourzyPhotonManager.onRoomsListUpdated -= OnRoomsUpdated;
            FourzyPhotonManager.onJoinedRoom -= OnJoinedRoom;
            FourzyPhotonManager.onJoinedLobby -= OnJoinedLobby;

            FourzyPhotonManager.onCreateRoomFailed -= OnCreateRoomFailed;
            FourzyPhotonManager.onJoinRoomFailed -= OnJoinRoomFailed;
            FourzyPhotonManager.onConnectionTimeOut -= OnConnectionTimerOut;
        }

        public override void Open()
        {
            base.Open();

            if (CheckLobby()) DisplayRooms(FourzyPhotonManager.Instance.roomsInfo);
        }

        public bool CheckLobby()
        {
            //includes join lobby logic
            if (FourzyPhotonManager.ConnectedAndReady && FourzyPhotonManager.IsDefaultLobby)
            {
                if (!isCurrent) menuController.GetOrAddScreen<LobbyPromptScreen>().Prompt();

                return true;
            }
            else
            {
                listenTo = true;

                _prompt
                    .Prompt("Connecting to server", "", null, "Back", null, () =>
                    {
                        listenTo = false;

                        if (isOpened) CloseSelf();
                    })
                    .CloseOnDecline();

                FourzyPhotonManager.Instance.JoinLobby();

                return false;
            }
        }

        public void CreateRoom()
        {
            FourzyPhotonManager.CreateRoom(RoomType.LOBBY_ROOM);

            _prompt
                .Prompt("Creating new room", "", null, null, null, null)
                .BlockInput();
        }

        public void JoinRoom(string name)
        {
            FourzyPhotonManager.JoinRoom(name);

            _prompt
                .Prompt("Joining room", name, null, null, null, null)
                .BlockInput();
        }

        private void DisplayRooms(List<RoomInfo> data)
        {
            if (!isOpened) return;

            Clear();

            foreach (RoomInfo roomData in data)
                if (!roomData.RemovedFromList)
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

            if (_prompt) _prompt.CloseSelf();
            if (isOpened) CloseSelf();

            //play GAME_FOUND sfx
            AudioHolder.instance.PlaySelfSfxOneShotTracked(Serialized.AudioTypes.GAME_FOUND);
            GameManager.Vibrate(MoreMountains.NiceVibrations.HapticTypes.Success);

            GameManager.Instance.StartGame();
        }

        private void OnCreateRoomFailed(string error)
        {
            if (isOpened && _prompt)
            {
                _prompt.CloseSelf();

                GamesToastsController.ShowTopToast($"Failed: {error}");
            }
        }

        private void OnJoinRoomFailed(string error)
        {
            if (isOpened && _prompt)
            {
                _prompt.CloseSelf();

                GamesToastsController.ShowTopToast($"Failed: {error}");
            }
        }

        private void OnConnectionTimerOut()
        {
            if (isOpened)
            {
                if (_prompt) _prompt.CloseSelf();
                if (listenTo) CloseSelf();
            }
        }

        private void OnJoinedLobby(string lobbyName)
        {
            if (!listenTo) return;

            _prompt.Decline();

            CheckLobby();
        }
    }
}