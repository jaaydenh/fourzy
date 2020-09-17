//@vadym udod

using Fourzy._Updates.Audio;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using Photon.Pun;
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

        protected override void Awake()
        {
            base.Awake();

            FourzyPhotonManager.onRoomsListUpdated += OnRoomsUpdated;
            FourzyPhotonManager.onJoinedRoom += OnJoinedRoom;
        }

        protected void OnDestroy()
        {
            FourzyPhotonManager.onRoomsListUpdated -= OnRoomsUpdated;
            FourzyPhotonManager.onJoinedRoom -= OnJoinedRoom;
        }

        public override void Open()
        {
            base.Open();

            DisplayRooms(FourzyPhotonManager.Instance.roomsInfo);
            createGameButton.SetState(true);
        }

        public void CreateRoom()
        {
            FourzyPhotonManager.CreateRoom(RoomType.LOBBY_ROOM);

            BlockInput(10f);
            createGameButton.SetState(false);
        }

        private void DisplayRooms(List<RoomInfo> data)
        {
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

            //play GAME_FOUND sfx
            AudioHolder.instance.PlaySelfSfxOneShotTracked(Serialized.AudioTypes.GAME_FOUND);
            GameManager.Vibrate(MoreMountains.NiceVibrations.HapticTypes.Success);

            GameManager.Instance.StartGame();
        }
    }
}