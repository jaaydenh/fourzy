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

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class LobbyPromptScreen : PromptScreen
    {
        public PhotonRoomWidget widgetPrefab;
        public RectTransform widgetsParent;
        public ButtonExtended createGameButton;

        protected List<PhotonRoomWidget> rooms = new List<PhotonRoomWidget>();
        protected List<RoomInfo> roomsData = new List<RoomInfo>();

        private PromptScreen waitingPrompt;

        protected override void Awake()
        {
            base.Awake();

            FourzyPhotonManager.onRoomsListUpdated += OnRoomsUpdated;
            FourzyPhotonManager.onJoinedRoom += OnJoinedRoom;

            FourzyPhotonManager.onCreateRoomFailed += OnCreateRoomFailed;
            FourzyPhotonManager.onJoinRoomFailed += OnJoinRoomFailed;
            FourzyPhotonManager.onConnectionTimeOut += OnConnectionTimerOut;
        }

        protected void OnDestroy()
        {
            FourzyPhotonManager.onRoomsListUpdated -= OnRoomsUpdated;
            FourzyPhotonManager.onJoinRoomFailed -= OnJoinRoomFailed;
            FourzyPhotonManager.onJoinedRoom -= OnJoinedRoom;
        }

        public override void Open()
        {
            base.Open();

            DisplayRooms(FourzyPhotonManager.Instance.roomsInfo);
        }

        public void CreateRoom()
        {
            FourzyPhotonManager.CreateRoom(RoomType.LOBBY_ROOM);

            waitingPrompt = PersistantMenuController.instance.GetOrAddScreen<PromptScreen>().Prompt("Creating new room", "", null, null, null, null);
            waitingPrompt.BlockInput();
        }

        public void JoinRoom(string name)
        {
            FourzyPhotonManager.JoinRoom(name);

            waitingPrompt = PersistantMenuController.instance.GetOrAddScreen<PromptScreen>().Prompt("Joining room", name, null, null, null, null);
            waitingPrompt.BlockInput();
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

        private void RemoveRoom(string roomName)
        {
            PhotonRoomWidget toRemove = rooms.Find(_widget => _widget.roomName == roomName);

            if (toRemove)
            {
                Destroy(toRemove.gameObject);
                rooms.Remove(toRemove);
            }
        }

        private void OnRoomsUpdated(List<RoomInfo> _rooms) => DisplayRooms(roomsData = _rooms);

        private void OnJoinedRoom(string roomName)
        {
            if (!isOpened) return;

            if (waitingPrompt) waitingPrompt.CloseSelf();

            BlockInput();

            //play GAME_FOUND sfx
            AudioHolder.instance.PlaySelfSfxOneShotTracked(Serialized.AudioTypes.GAME_FOUND);
            GameManager.Vibrate(MoreMountains.NiceVibrations.HapticTypes.Success);

            GameManager.Instance.StartGame();
        }

        private void OnCreateRoomFailed(string error)
        {
            if (isOpened && waitingPrompt)
            {
                waitingPrompt.CloseSelf();

                GamesToastsController.ShowTopToast($"Failed: {error}");
            }
        }

        private void OnJoinRoomFailed(string error)
        {
            if (isOpened && waitingPrompt)
            {
                waitingPrompt.CloseSelf();

                GamesToastsController.ShowTopToast($"Failed: {error}");
            }
        }

        private void OnConnectionTimerOut()
        {
            if (isOpened && waitingPrompt)
                waitingPrompt.CloseSelf();
        }
    }
}